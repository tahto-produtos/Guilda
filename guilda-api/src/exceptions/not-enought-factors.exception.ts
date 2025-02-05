import { HttpStatus } from '@nestjs/common';
import { IndicatorEntity } from '~entities/indicator.entity';
import { MathematicalExpressionEntity } from '~entities/mathematical-expression.entity';
import { ResultEntity } from '~entities/result.entity';
import { FactorEntity } from '~entities/factor.entity';

import { Calc } from '../utils';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class NotEnoughtFactorsException extends BaseException {
  constructor(
    result: ResultEntity & {
      factors: FactorEntity[];
      indicator: IndicatorEntity & {
        mathematicalExpressions: MathematicalExpressionEntity[];
      };
    },
  ) {
    const [mathematicalExpression] = result.indicator.mathematicalExpressions;
    const expectedFactors = Calc.getVariables(
      mathematicalExpression.expression,
    ).length;

    const providedFactors = result.factors.length;

    super(
      {
        message: `indicator ${result.indicator.name} expect ${expectedFactors} factors but ${providedFactors} was provided`,
        code: EXCEPTION_CODES.OUT_OF_RANGE,
        keys: {
          expectedFactors,
          providedFactors,
          indicatorId: result.indicator.id,
        },
      },
      HttpStatus.BAD_REQUEST,
    );
  }
}
