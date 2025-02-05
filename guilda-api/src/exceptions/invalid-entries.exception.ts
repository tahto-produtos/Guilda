import { HttpStatus, ValidationError } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class InvalidEntriesException extends BaseException {
  constructor(errors: ValidationError[]) {
    super(
      {
        message: `invalid entries`,
        code: EXCEPTION_CODES.INVALID_ENTRIES,
        keys: InvalidEntriesException.parseErrorKeys(errors),
      },
      HttpStatus.BAD_REQUEST,
    );
  }

  // todo - use recursion to parse nested error
  private static parseErrorKeys(errors: ValidationError[]) {
    const keys = {};
    for (const error of errors) {
      // Get First Error of array if one of them is invalid
      if (error?.children?.length > 0) {
        const [firstError] = error.children;
        const [firstErrorItem] = firstError.children;
        const [firstConstraintError] = Object.values(
          firstErrorItem?.constraints || firstError.constraints,
        );

        const errorIndex = firstError.property;
        const property = firstErrorItem?.property || firstError?.property;

        keys[`${property}[${errorIndex}]`] = firstConstraintError;
      } else {
        // Get First Error of object
        const [firstConstraintError] = Object.values(error.constraints);
        keys[error.property] = firstConstraintError;
      }
    }
    return keys;
  }
}
