import { calculate, parse, tokenize } from 'calculy';
import { FactorEntity } from '~entities/factor.entity';

const variablesPattern = /#fator(\d+)/g;
export class Calc {
  static validate(expression: string): boolean {
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

  static calculate(
    expression: string,
    variables?: Record<string, string | number>,
  ): number {
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

  static parseVariables(expression: string, factors: FactorEntity[]) {
    const variablesNames = Calc.getVariables(expression);
    const variables: Record<string, number> = {};

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

  static getVariables(expression: string): string[] {
    return expression.match(variablesPattern);
  }

  static isEntriesValid(
    expression: string,
    variables?: Record<string, number>,
  ) {
    const expectedVariables = this.getVariables(expression).length;
    const providedVariables = Object.keys(variables).length;
    return providedVariables >= expectedVariables;
  }
}
