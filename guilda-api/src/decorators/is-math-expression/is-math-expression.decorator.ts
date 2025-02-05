import {
  registerDecorator,
  ValidationArguments,
  ValidationOptions,
  ValidatorConstraint,
  ValidatorConstraintInterface,
} from 'class-validator';
import { Calc } from '../../utils';

@ValidatorConstraint({ name: 'custom', async: false })
class IsMathExpressionValidatorConstraint
  implements ValidatorConstraintInterface
{
  validate(expression: string, args: ValidationArguments): boolean {
    return Calc.validate(expression);
  }

  defaultMessage(validationArguments?: ValidationArguments): string {
    return `${validationArguments.property} should be a valid math expression`;
  }
}

export function IsMathExpression(validationOptions?: ValidationOptions) {
  return function (object: unknown, propertyName: string) {
    registerDecorator({
      name: 'isMathExpression',
      target: object.constructor,
      propertyName: propertyName,
      constraints: [],
      options: validationOptions,
      validator: IsMathExpressionValidatorConstraint,
    });
  };
}
