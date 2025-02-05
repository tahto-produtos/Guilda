import * as workerpool from 'workerpool';
import { PrismaClient } from '@prisma/client';
import { DATABASE_CONNECTION } from 'src/constants/environment-variable.constants';
const prisma = new PrismaClient({
  datasources: {
    db: {
      url: DATABASE_CONNECTION,
    },
  },
});
import { calculate, parse, tokenize } from 'calculy';
import { startOfDay } from 'date-fns';

async function calculateResults(transaction, resultsIds) {
  const { parentsIds } = await createAndStoreAgentConsolidatedResults(
    transaction,
    resultsIds,
  );

  return parentsIds;
}

async function createAndStoreAgentConsolidatedResults(transaction, resultsIds) {
  const parentsIds = [];

  const results = await prisma.result.findMany({
    where: {
      transactionId: transaction.id,
      id: {
        in: resultsIds,
      },
    },
    include: {
      collaborator: {
        include: {
          HistoryCollaboratorSector: {
            orderBy: {
              createdAt: 'desc',
            },
            take: 1,
          },
          historyHierarchyRelationship: {
            where: {
              deletedAt: null,
            },
            orderBy: {
              createdAt: 'desc',
            },
          },
        },
      },
      indicator: {
        include: {
          historyMathematicalExpressionIndicator: {
            orderBy: {
              createdAt: 'desc',
            },
            where: {
              deletedAt: null,
            },
            include: {
              mathematicalExpression: true,
            },
          },
          historyIndicatorSectors: {
            where: { deletedAt: null },
            orderBy: { createdAt: 'desc' },
          },
          historyIndicatorGroups: {
            include: {
              indicator: true,
              group: true,
            },
          },
        },
      },
    },
  });

  for (const result of results) {
    const consolidatedResults = await createConsolidatedResults(result);
    await storeConsolidatedResults(consolidatedResults);
    const parentId =
      result?.collaborator?.historyHierarchyRelationship[0]?.parentId;
    if (parentId) {
      parentsIds.push(parentId);
    }
  }

  return {
    parentsIds: Array.from(new Set(parentsIds)),
  };
}

async function createConsolidatedResults(result) {
  const results = [];
  const expression =
    result?.indicator?.historyMathematicalExpressionIndicator
      .mathematicalExpression?.[0];
  const rawResult = result.result;
  const historyIndicatorsSectors =
    result?.indicator?.historyIndicatorSectors || [];

  const collaboratorSectorId =
    result?.collaborator?.HistoryCollaboratorSector?.[0]?.sectorId;

  for (const indicatorSector of historyIndicatorsSectors) {
    if (collaboratorSectorId === indicatorSector.sectorId) {
      const goal = indicatorSector.goal;
      const indicatorId = indicatorSector.indicatorId;
      const sectorId = indicatorSector.sectorId;
      results.push({
        transaction: result.transactionId,
        value: expression ? await calculateResult(result) : rawResult,
        goal,
        indicatorId,
        sectorId,
        collaboratorId: result.collaboratorId,
        resultId: result.id,
        createdAt: result.createdAt,
      });
    }
  }

  return results;
}

async function storeConsolidatedResults(allResults) {
  for await (const results of paginateConsolidatedResults(allResults)) {
    await prisma.consolidatedResult.createMany({
      data: results.map((rawResult) => {
        return {
          transactionId: rawResult.transaction,
          resultId: rawResult.resultId,
          collaboratorId: rawResult.collaboratorId,
          sectorId: rawResult.sectorId,
          value: rawResult?.value || 0,
          goal: rawResult?.goal || 0,
          indicatorId: rawResult.indicatorId,
          createdAt: rawResult.createdAt,
        };
      }),
    });
  }
}

async function* paginateConsolidatedResults(rawResults) {
  const itemsPerBatch = 1000;
  let index = 0;
  while (index < rawResults.length) {
    yield rawResults.slice(index, index + itemsPerBatch);
    index += itemsPerBatch;
  }
}

async function calculateResult(result) {
  const [expression] = result?.indicator?.mathematicalExpressions;

  const variablesList = result?.factorsList
    .split?.(',')
    .reduce?.((acc, current, index) => {
      acc.push({ index: index, value: Number(current) });
      return acc;
    }, []);

  const variables = Calc.parseVariables(expression.expression, variablesList);

  const isVariablesValid = Calc.isEntriesValid(
    expression.expression,
    variables,
  );

  if (!isVariablesValid) {
    throw new Error('not enough factors');
  }

  return Calc.calculate(expression.expression, variables);
}

function calculateResultMonetization(result) {
  let dividend = 0;
  let divisor = 0;

  Calc.parseVariables('#fator0/#fator1', result.factors);
  if (result.factors.length >= 2) {
    dividend += result.factors[1].value;
    divisor += result.factors[0].value;
  }

  const [mathExpressionIndicator] =
    result?.indicator?.historyMathematicalExpressionIndicator;

  if (mathExpressionIndicator) {
    const expression =
      mathExpressionIndicator.mathematicalExpression.expression;

    const calculatedResult = Calc.calculate(expression, {
      '#fator0': divisor,
      '#fator1': dividend,
    });

    return calculatedResult;
  }

  const quotient = dividend / divisor;

  return quotient;
}

const variablesPattern = /#fator(\d+)/g;

class Calc {
  static validate(expression) {
    try {
      // Our variables sintaxy is invalid for 'calculy' lib
      // So we replace all variables for a valid
      // math token before validate
      expression = expression.replace(new RegExp(variablesPattern, 'g'), '0');
      const tokens = tokenize(expression);
      parse(tokens);
      return true;
    } catch {
      return false;
    }
  }

  static calculate(expression, variables) {
    if (variables) {
      for (const [variable, value] of Object.entries(variables)) {
        expression = expression.replace(
          new RegExp(variable, 'g'),
          String(value),
        );
      }
    }

    return calculate(expression);
  }

  static parseVariables(expression, factors) {
    const variablesNames = Calc.getVariables(expression);
    const variables = {};

    for (const variableName of variablesNames) {
      const index = Number(variableName.replace('#fator', ''));
      const value = factors[index]?.value;

      if (value === undefined || value === null) {
        continue;
      }

      variables[variableName] = value;
    }

    return variables;
  }

  static getVariables(expression) {
    return expression.match(variablesPattern);
  }

  static isEntriesValid(expression, variables) {
    const expectedVariables = this.getVariables(expression).length;
    const providedVariables = Object.keys(variables).length;
    return providedVariables >= expectedVariables;
  }
}

async function monetize(transaction) {
  try {
    const datesCalculationMonetization = await getDatesCalculateMonetization(
      transaction.id,
    );

    if (datesCalculationMonetization.length > 0) {
      for (let dateMonetization of datesCalculationMonetization) {
        let date = dateMonetization.createdAt;
        const dateResult = date.toISOString().split('T')[0];

        const resultsRaw = await prisma.$queryRaw`
          SELECT R.*, S.IDGDA_SECTOR
          FROM GDA_RESULT (NOLOCK) AS R
          INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS S ON CONVERT(DATE, S.CREATED_AT, 120) = CONVERT(DATE, R.CREATED_AT, 120) AND S.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS
          INNER JOIN GDA_INDICATOR AS I ON I.IDGDA_INDICATOR = R.INDICADORID
          LEFT JOIN (
              SELECT SECTOR_ID, INDICATOR_ID
              FROM GDA_HISTORY_INDICATOR_GROUP
              WHERE MONETIZATION > 0  
          ) TS ON TS.SECTOR_ID = S.IDGDA_SECTOR AND TS.INDICATOR_ID = R.INDICADORID
          WHERE R.TRANSACTIONID = ${transaction.id}
          AND CONVERT(DATE, R.CREATED_AT, 120) = ${dateResult} AND  TS.SECTOR_ID IS NOT NULL AND I.STATUS = 1
          GROUP BY IDGDA_RESULT, INDICADORID, R.TRANSACTIONID, RESULT, R.CREATED_AT, R.IDGDA_COLLABORATORS, FACTORS, R.DELETED_AT, S.IDGDA_SECTOR
        `;
        if (resultsRaw.length > 0) {
          let results = await chunkedResultsIds(resultsRaw);
          if (results.length > 0) {
            for (let result of results) {
              const agentResult = calculateResultMonetization(result);
              if (
                result.indicator.historyIndicatorSectors.length > 0 &&
                result.indicator.historyIndicatorSectors[0].sectorId
              ) {
                const [historyIndicatorGroups, checkingAccont] =
                  await Promise.all([
                    prisma.historyIndicatorGroup.findMany({
                      where: {
                        indicatorId: result.indicatorId,
                        sectorId: result.indicator.historyIndicatorSectors[0]
                          .sectorId
                          ? result.indicator.historyIndicatorSectors[0].sectorId
                          : undefined,
                        deletedAt: null,
                      },
                      orderBy: {
                        groupId: 'asc',
                      },
                      include: {
                        group: true,
                      },
                    }),

                    prisma.checkingAccount.findMany({
                      where: {
                        collaboratorId: result.collaboratorId,
                        indicatorId: result.indicator.id,
                        resultDate: new Date(date),
                      },
                      take: 1,
                      orderBy: {
                        createdAt: 'desc',
                      },
                    }),
                  ]);

                const resultMonetizationGroup =
                  (agentResult * 100) /
                  result.indicator.historyIndicatorSectors[0].goal;

                const historyIndicatorGroup = historyIndicatorGroups.find(
                  (historyIndicatorGroup) => {
                    const { metricMin } = historyIndicatorGroup;
                    return resultMonetizationGroup * 100 >= metricMin;
                  },
                );

                if (!historyIndicatorGroup?.group) {
                  console.log(
                    `group for result ${result.id} not found skipping`,
                  );
                  continue;
                }

                if (checkingAccont.length > 0) {
                  const totalInput = checkingAccont.reduce(
                    (accumulator, currentValue) => {
                      return accumulator + currentValue.input;
                    },
                    0,
                  );
                  const basketIndicator =
                    await prisma.basketIndicator.findFirst({
                      where: {
                        sectorId:
                          result.indicator.historyIndicatorSectors[0].sectorId,
                        indicatorId: result.indicatorId,
                        date: new Date(date),
                        weight: result.indicator.weight,
                        monetizationG1: historyIndicatorGroups[0].monetization,
                      },
                    });
                  if (!basketIndicator) {
                    await prisma.basketIndicator.create({
                      data: {
                        sectorId:
                          result.indicator.historyIndicatorSectors[0].sectorId,
                        indicatorId: result.indicatorId,
                        date: new Date(date),
                        weight: result.indicator.weight,
                        monetizationG1: historyIndicatorGroups[0].monetization,
                      },
                    });
                  }
                  if (historyIndicatorGroup.monetization > 0) {
                    if (totalInput < historyIndicatorGroup.monetization) {
                      const checkingAccountExists =
                        await prisma.checkingAccount.findFirst({
                          where: {
                            collaboratorId: result.collaboratorId,
                            indicatorId: result.indicatorId,
                            resultDate: new Date(date),
                          },
                        });
                      if (!checkingAccountExists) {
                        const checkingAccountLastBalance =
                          await prisma.checkingAccount.findFirst({
                            where: {
                              collaboratorId: result.collaboratorId,
                            },
                            orderBy: {
                              createdAt: 'desc',
                            },
                          });
                        const lastBalance = checkingAccountLastBalance
                          ? checkingAccountLastBalance.balance
                          : 0;

                        await prisma.checkingAccount.create({
                          data: {
                            collaboratorId: result.collaboratorId,
                            input:
                              historyIndicatorGroup.monetization - totalInput,
                            balance:
                              lastBalance +
                              (historyIndicatorGroup.monetization - totalInput),
                            indicatorId: result.indicatorId,
                            weight: result.indicator.weight,
                            GdaConsolidateCheckingAccount: {
                              create: {
                                hierarchyId:
                                  result.collaborator
                                    .historyHierarchyRelationship[0]
                                    ?.hierarchyId ?? null,
                                indicatorId: result.indicatorId,
                                sectorId:
                                  result.indicator.historyIndicatorSectors[0]
                                    .sectorId,
                                weight: result.indicator.weight,
                                monetization:
                                  historyIndicatorGroup.monetization,
                              },
                            },
                            resultDate: new Date(date),
                          },
                        });

                        await prisma.historyMonetizationRecalculation.create({
                          data: {
                            resultDate: result.createdAt,
                            monetizationDate: new Date(),
                            result: {
                              connect: {
                                id: result.id,
                              },
                            },
                            collaborator: {
                              connect: {
                                id: result.collaboratorId,
                              },
                            },
                            indicator: {
                              connect: {
                                id: result.indicator.id,
                              },
                            },
                            sector: {
                              connect: {
                                id: result.indicator.historyIndicatorSectors[0]
                                  .sectorId,
                              },
                            },
                            monetization:
                              historyIndicatorGroup.monetization - totalInput,
                            beforeMonetization: totalInput,
                          },
                        });
                      }
                    } else {
                      console.log(
                        `[monetization is not greater than portfolio pay]`,
                      );
                    }
                  }
                } else {
                  const basketIndicator =
                    await prisma.basketIndicator.findFirst({
                      where: {
                        sectorId:
                          result.indicator.historyIndicatorSectors[0].sectorId,
                        indicatorId: result.indicatorId,
                        date: new Date(date),
                        weight: result.indicator.weight,
                        monetizationG1: historyIndicatorGroups[0].monetization,
                      },
                    });
                  if (!basketIndicator) {
                    await prisma.basketIndicator.create({
                      data: {
                        sectorId:
                          result.indicator.historyIndicatorSectors[0].sectorId,
                        indicatorId: result.indicatorId,
                        date: new Date(date),
                        weight: result.indicator.weight,
                        monetizationG1: historyIndicatorGroups[0].monetization,
                      },
                    });
                  }
                  //tem que cadastrar uma linha na checking account
                  if (historyIndicatorGroup.monetization > 0) {
                    const checkingAccountExists =
                      await prisma.checkingAccount.findFirst({
                        where: {
                          collaboratorId: result.collaboratorId,
                          indicatorId: result.indicatorId,
                          resultDate: new Date(date),
                        },
                      });
                    if (!checkingAccountExists) {
                      const checkingAccountLastBalance =
                        await prisma.checkingAccount.findFirst({
                          where: {
                            collaboratorId: result.collaboratorId,
                          },
                          orderBy: {
                            createdAt: 'desc',
                          },
                        });
                      const lastBalance = checkingAccountLastBalance
                        ? checkingAccountLastBalance.balance
                        : 0;
                      await prisma.checkingAccount.create({
                        data: {
                          collaboratorId: result.collaboratorId,
                          input: historyIndicatorGroup.monetization,
                          balance:
                            lastBalance + historyIndicatorGroup.monetization,
                          indicatorId: result.indicatorId,
                          weight: result.indicator.weight,
                          GdaConsolidateCheckingAccount: {
                            create: {
                              hierarchyId:
                                result.collaborator
                                  .historyHierarchyRelationship[0]
                                  ?.hierarchyId ?? null,
                              indicatorId: result.indicatorId,
                              sectorId:
                                result.indicator.historyIndicatorSectors[0]
                                  .sectorId,
                              weight: result.indicator.weight,
                              monetization: historyIndicatorGroup.monetization,
                            },
                          },
                          resultDate: new Date(date),
                        },
                      });
                    }
                  }
                }

                //HIERARCHY
                const childrensCollaboratorResult =
                  await findChildrensIdsForDate(
                    result.collaboratorId,
                    dateResult,
                  );

                if (childrensCollaboratorResult.length > 0) {
                  for (let parent of childrensCollaboratorResult) {
                    const resultsChildrensRaw =
                      await findChildrensResultsForDate(
                        parent.ParentID,
                        dateResult,
                        result.indicator.id,
                        result.indicator.historyIndicatorSectors[0].sectorId,
                      );

                    if (resultsChildrensRaw.length > 0) {
                      let sumFATOR1 = 0;
                      let sumFATOR2 = 0;

                      for (const item of resultsChildrensRaw) {
                        sumFATOR1 += item.FATOR1;
                        sumFATOR2 += item.FATOR2;
                      }

                      const divisor = sumFATOR2 / sumFATOR1;
                      const resultMonetizationGroup =
                        (divisor * 100) /
                        result.indicator.historyIndicatorSectors[0].goal;

                      const historyIndicatorGroup = historyIndicatorGroups.find(
                        (historyIndicatorGroup) => {
                          const { metricMin } = historyIndicatorGroup;
                          return resultMonetizationGroup * 100 >= metricMin;
                        },
                      );

                      if (!historyIndicatorGroup?.group) {
                        continue;
                      }

                      const checkingAccountParent =
                        prisma.checkingAccount.findMany({
                          where: {
                            collaboratorId: parent.ParentID,
                            indicatorId: result.indicator.id,
                            resultDate: new Date(date),
                          },
                          take: 1,
                          orderBy: {
                            createdAt: 'desc',
                          },
                        });

                      //INÍCIO CHECKING ACCOUNT HIERARCHY
                      if (checkingAccountParent.length > 0) {
                        const totalInput = checkingAccountParent.reduce(
                          (accumulator, currentValue) => {
                            return accumulator + currentValue.input;
                          },
                          0,
                        );

                        const basketIndicator =
                          await prisma.basketIndicator.findFirst({
                            where: {
                              sectorId:
                                result.indicator.historyIndicatorSectors[0]
                                  .sectorId,
                              indicatorId: result.indicatorId,
                              date: new Date(date),
                              weight: result.indicator.weight,
                              monetizationG1:
                                historyIndicatorGroups[0].monetization,
                            },
                          });
                        if (!basketIndicator) {
                          await prisma.basketIndicator.create({
                            data: {
                              sectorId:
                                result.indicator.historyIndicatorSectors[0]
                                  .sectorId,
                              indicatorId: result.indicatorId,
                              date: new Date(date),
                              weight: result.indicator.weight,
                              monetizationG1:
                                historyIndicatorGroups[0].monetization,
                            },
                          });
                        }
                        if (historyIndicatorGroup.monetization > 0) {
                          if (totalInput < historyIndicatorGroup.monetization) {
                            const checkingAccountExists =
                              await prisma.checkingAccount.findFirst({
                                where: {
                                  collaboratorId: result.collaboratorId,
                                  indicatorId: result.indicatorId,
                                  resultDate: new Date(date),
                                },
                              });
                            if (!checkingAccountExists) {
                              const checkingAccounParentLastBalance =
                                await prisma.checkingAccount.findFirst({
                                  where: {
                                    collaboratorId: parent.ParentID,
                                  },
                                  orderBy: {
                                    createdAt: 'desc',
                                  },
                                });
                              const lastBalance =
                                checkingAccounParentLastBalance
                                  ? checkingAccounParentLastBalance.balance
                                  : 0;

                              await prisma.checkingAccount.create({
                                data: {
                                  collaboratorId: parent.ParentID,
                                  input:
                                    historyIndicatorGroup.monetization -
                                    totalInput,
                                  balance:
                                    lastBalance +
                                    (historyIndicatorGroup.monetization -
                                      totalInput),
                                  indicatorId: result.indicatorId,
                                  weight: result.indicator.weight,
                                  GdaConsolidateCheckingAccount: {
                                    create: {
                                      hierarchyId:
                                        result.collaborator
                                          .historyHierarchyRelationship[0]
                                          ?.hierarchyId ?? null,
                                      indicatorId: result.indicatorId,
                                      sectorId:
                                        result.indicator
                                          .historyIndicatorSectors[0].sectorId,
                                      weight: result.indicator.weight,
                                      monetization:
                                        historyIndicatorGroup.monetization,
                                    },
                                  },
                                  resultDate: new Date(date),
                                },
                              });

                              await prisma.historyMonetizationRecalculation.create(
                                {
                                  data: {
                                    resultDate: result.createdAt,
                                    monetizationDate: new Date(),
                                    result: {
                                      connect: {
                                        id: result.id,
                                      },
                                    },
                                    collaborator: {
                                      connect: {
                                        id: parent.ParentID,
                                      },
                                    },
                                    indicator: {
                                      connect: {
                                        id: result.indicator.id,
                                      },
                                    },
                                    sector: {
                                      connect: {
                                        id: result.indicator
                                          .historyIndicatorSectors[0].sectorId,
                                      },
                                    },
                                    monetization:
                                      historyIndicatorGroup.monetization -
                                      totalInput,
                                    beforeMonetization: totalInput,
                                  },
                                },
                              );
                            }
                          } else {
                            console.log(
                              `[monetization is not greater than portfolio pay]`,
                            );
                          }
                        }
                      } else {
                        const basketIndicator =
                          await prisma.basketIndicator.findFirst({
                            where: {
                              sectorId:
                                result.indicator.historyIndicatorSectors[0]
                                  .sectorId,
                              indicatorId: result.indicatorId,
                              date: new Date(date),
                              weight: result.indicator.weight,
                              monetizationG1:
                                historyIndicatorGroups[0].monetization,
                            },
                          });
                        if (!basketIndicator) {
                          await prisma.basketIndicator.create({
                            data: {
                              sectorId:
                                result.indicator.historyIndicatorSectors[0]
                                  .sectorId,
                              indicatorId: result.indicatorId,
                              date: new Date(date),
                              weight: result.indicator.weight,
                              monetizationG1:
                                historyIndicatorGroups[0].monetization,
                            },
                          });
                        }
                        //tem que cadastrar uma linha na checking account
                        if (historyIndicatorGroup.monetization > 0) {
                          const checkingAccountExists =
                            await prisma.checkingAccount.findFirst({
                              where: {
                                collaboratorId: result.collaboratorId,
                                indicatorId: result.indicatorId,
                                resultDate: new Date(date),
                              },
                            });
                          if (!checkingAccountExists) {
                            const checkingAccountParentLastBalance =
                              await prisma.checkingAccount.findFirst({
                                where: {
                                  collaboratorId: parent.ParentID,
                                },
                                orderBy: {
                                  createdAt: 'desc',
                                },
                              });
                            const lastBalance = checkingAccountParentLastBalance
                              ? checkingAccountParentLastBalance.balance
                              : 0;
                            await prisma.checkingAccount.create({
                              data: {
                                collaboratorId: parent.ParentID,
                                input: historyIndicatorGroup.monetization,
                                balance:
                                  lastBalance +
                                  historyIndicatorGroup.monetization,
                                indicatorId: result.indicatorId,
                                weight: result.indicator.weight,
                                GdaConsolidateCheckingAccount: {
                                  create: {
                                    hierarchyId:
                                      result.collaborator
                                        .historyHierarchyRelationship[0]
                                        ?.hierarchyId ?? null,
                                    indicatorId: result.indicatorId,
                                    sectorId:
                                      result.indicator
                                        .historyIndicatorSectors[0].sectorId,
                                    weight: result.indicator.weight,
                                    monetization:
                                      historyIndicatorGroup.monetization,
                                  },
                                },
                                resultDate: new Date(date),
                              },
                            });
                          }
                        }
                      }
                      //FIM CHECKING ACCOUNT HIERARCHY
                    } else {
                      console.log(
                        `[childrens not results for date ${dateResult}, for parent ${parent.ParentID}]`,
                      );
                    }
                  }
                }
              }
            }
            return transaction.id;
          } else {
            console.info(
              `[no results to calculation for monetization for date ${dateResult} #2]`,
            );
          }
        } else {
          console.info(
            `[no results to calculation for monetization for date ${dateResult}]`,
          );
        }
      }
      return transaction.id;
    } else {
      console.info('[no dates to calculation for results monetization]');
    }
  } catch (e) {
    console.log('[error monetization]', e.message);
  }
}

async function chunkedResultsIds(resultsRaw) {
  const results = [];
  const mappedResultsIds = resultsRaw.map((result) => result?.IDGDA_RESULT);
  const itemsPerBatch = 3500;
  const chunkedIds = [];
  for (let i = 0; i < mappedResultsIds.length; i += itemsPerBatch) {
    chunkedIds.push(mappedResultsIds.slice(i, i + itemsPerBatch));
  }

  for (const chunk of chunkedIds) {
    const lines = await prisma.result.findMany({
      where: {
        id: {
          in: chunk,
        },
        deletedAt: null,
      },
      include: {
        collaborator: {
          include: {
            historyHierarchyRelationship: {
              take: 1,
            },
          },
        },
        factors: {
          orderBy: {
            index: 'asc',
          },
        },
        indicator: {
          include: {
            historyIndicatorGroups: {
              where: {
                deletedAt: null,
              },
              orderBy: {
                createdAt: 'desc',
              },
              distinct: ['indicatorId', 'groupId', 'sectorId'],
            },
            historyIndicatorSectors: {
              where: {
                deletedAt: null,
              },
              orderBy: {
                createdAt: 'desc',
              },
              take: 1,
            },
            historyMathematicalExpressionIndicator: {
              include: {
                mathematicalExpression: true,
              },
            },
          },
        },
      },
    });

    results.push(...lines);
  }

  return results;
}

async function getDatesCalculateMonetization(transactionId) {
  return prisma.result.findMany({
    where: {
      transactionId: transactionId,
    },
    distinct: ['createdAt'],
    select: {
      createdAt: true,
    },
  });
}

async function findChildrensIdsForDate(collaboratorId, date) {
  return prisma.$queryRaw`
    DECLARE @InputID INT; SET @InputID = ${collaboratorId};
    DECLARE @DateEnv DATE; SET @DateEnv = ${date};

    WITH HierarchyCTE AS (
        SELECT 
            IDGDA_HISTORY_HIERARCHY_RELATIONSHIP,
            CONTRACTORCONTROLID,
            PARENTIDENTIFICATION,
            IDGDA_COLLABORATORS,
            IDGDA_HIERARCHY,
            CREATED_AT,
            DELETED_AT,
            TRANSACTIONID,
            LEVELWEIGHT,
            DATE,
            LEVELNAME
        FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP
        WHERE IDGDA_COLLABORATORS = @InputID
        AND CONVERT(DATE, [DATE]) = @DateEnv

        UNION ALL

        SELECT 
            h.IDGDA_HISTORY_HIERARCHY_RELATIONSHIP,
            h.CONTRACTORCONTROLID,
            h.PARENTIDENTIFICATION,
            h.IDGDA_COLLABORATORS,
            h.IDGDA_HIERARCHY,
            h.CREATED_AT,
            h.DELETED_AT,
            h.TRANSACTIONID,
            h.LEVELWEIGHT,
            h.DATE,
            h.LEVELNAME
        FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP h
        INNER JOIN HierarchyCTE cte ON h.IDGDA_COLLABORATORS = cte.PARENTIDENTIFICATION
        WHERE CONVERT(DATE, cte.[DATE]) = @DateEnv
  )

  , RecursiveCTE AS (
      SELECT
          cte.IDGDA_COLLABORATORS AS ParentID,
          cte.LEVELNAME,
          [Level] = 1
      FROM HierarchyCTE cte

      UNION ALL

      SELECT
          h.IDGDA_COLLABORATORS,
          h.LEVELNAME,
          [Level] = rc.[Level] + 1
      FROM RecursiveCTE rc
      JOIN HierarchyCTE h ON rc.ParentID = h.PARENTIDENTIFICATION
      WHERE CONVERT(DATE, h.[DATE]) = @DateEnv
  )

  SELECT DISTINCT ParentID, LEVELNAME, max([Level]) AS LEVEL
  FROM RecursiveCTE
  WHERE ParentID != @InputID
  group by ParentID, LEVELNAME

  `;
}

async function findChildrensResultsForDate(
  collaboratorId,
  date,
  indicatorId,
  sectorId,
) {
  return prisma.$queryRaw`
    DECLARE @InputID INT; SET @InputID = ${collaboratorId};
    DECLARE @DateEnv date; SET @DateEnv = ${date};

    WITH HierarchyCTE AS (
        SELECT 
            IDGDA_HISTORY_HIERARCHY_RELATIONSHIP,
            CONTRACTORCONTROLID,
            PARENTIDENTIFICATION,
            IDGDA_COLLABORATORS,
            IDGDA_HIERARCHY,
            CREATED_AT,
            DELETED_AT,
            TRANSACTIONID,
            LEVELWEIGHT,
            DATE,
            LEVELNAME
        FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP
        WHERE IDGDA_COLLABORATORS = @InputID  -- Substitua @InputID pelo ID fornecido
      and CONVERT(DATE, [DATE]) = @DateEnv

        UNION ALL

        SELECT 
            h.IDGDA_HISTORY_HIERARCHY_RELATIONSHIP,
            h.CONTRACTORCONTROLID,
            h.PARENTIDENTIFICATION,
            h.IDGDA_COLLABORATORS,
            h.IDGDA_HIERARCHY,
            h.CREATED_AT,
            h.DELETED_AT,
            h.TRANSACTIONID,
            h.LEVELWEIGHT,
            h.DATE,
            h.LEVELNAME
        FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP h
        INNER JOIN HierarchyCTE cte ON h.PARENTIDENTIFICATION = cte.IDGDA_COLLABORATORS
        WHERE cte.LEVELNAME <> 'AGENTE'
      and CONVERT(DATE, cte.[DATE]) = @DateEnv
    )


    SELECT R.*, F1.FACTOR AS FATOR1, F2.FACTOR AS FATOR2, S.IDGDA_SECTOR FROM GDA_RESULT (NOLOCK) AS R
    INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS S ON CONVERT(DATE, S.CREATED_AT, 120) = CONVERT(DATE, R.CREATED_AT, 120) AND S.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS
    INNER JOIN GDA_FACTOR (NOLOCK) AS F1 ON F1.IDGDA_RESULT = R.IDGDA_RESULT AND F1.[INDEX] = 1
    INNER JOIN GDA_FACTOR (NOLOCK) AS F2 ON F2.IDGDA_RESULT = R.IDGDA_RESULT AND F2.[INDEX] = 2
    WHERE R.IDGDA_COLLABORATORS IN (
      SELECT distinct(IDGDA_COLLABORATORS)
      FROM HierarchyCTE
      WHERE LEVELNAME = 'AGENTE'
      and CONVERT(DATE, DATE) = @DateEnv
    )
    AND R.CREATED_AT = @DateEnv
    AND R.DELETED_AT IS NULL
    AND INDICADORID = ${indicatorId}
    AND S.IDGDA_SECTOR = ${sectorId}
  `;
}

workerpool.worker({
  calculateResults,
  monetize,
});
