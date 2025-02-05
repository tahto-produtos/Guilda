import * as crypto from 'node:crypto';
import { Seeder } from '../utils';

interface ResultData {
  transactionId: string;
  results: Result[];
}

interface Result {
  collaboratorId: number;
  indicatorId: number;

  // One of the following should be undefined
  result?: number;
  factors?: number[];
}

export class ResultsSeed extends Seeder<ResultData> {
  async seed() {
    const { results, transactionId } = this.data;
    const transaction = await this.prisma.transaction.create({
      data: {
        identification: transactionId,
        complete: true,
      },
    });
    for (const result of results) {
      await this.prisma.result.create({
        data: {
          result: result.result,
          indicator: {
            connect: {
              id: result.indicatorId,
            },
          },
          collaborator: {
            connect: {
              id: result.collaboratorId,
            },
          },
          factors: result.factors && {
            createMany: {
              data: result.factors.map((factor, index) => ({
                value: factor,
                index: index,
              })),
            },
          },
          transaction: {
            connect: { id: transaction.id },
          },
        },
      });
    }
  }

  get data(): ResultData {
    const randomData = crypto.randomBytes(16);
    const transactionHash = crypto
      .createHash('sha256')
      .update(randomData)
      .digest('hex');

    return {
      transactionId: transactionHash,
      results: [
        {
          result: 400,
          collaboratorId: 8,
          indicatorId: 1,
        },
        {
          result: 600,
          collaboratorId: 8,
          indicatorId: 2,
        },
        {
          result: 550,
          collaboratorId: 9,
          indicatorId: 1,
        },
        {
          result: 450,
          collaboratorId: 9,
          indicatorId: 2,
        },
        {
          result: 400,
          collaboratorId: 8,
          indicatorId: 2,
        },
        {
          result: 200,
          collaboratorId: 9,
          indicatorId: 1,
        },

        {
          result: 1000,
          collaboratorId: 11,
          indicatorId: 2,
        },
        {
          result: 500,
          collaboratorId: 11,
          indicatorId: 2,
        },
        {
          result: 200,
          collaboratorId: 11,
          indicatorId: 2,
        },
        {
          result: 60,
          collaboratorId: 12,
          indicatorId: 1,
        },
        {
          result: 58,
          collaboratorId: 12,
          indicatorId: 2,
        },
      ],
    };
  }
}
