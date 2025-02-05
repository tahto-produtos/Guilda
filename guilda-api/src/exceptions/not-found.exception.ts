import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class NotFoundException extends BaseException {
  constructor(
    resource: string,
    keys: Record<string, string | number | boolean>,
  ) {
    super(
      {
        message: `${resource} not found`,
        code: EXCEPTION_CODES.NOT_FOUND,
        keys: keys,
      },
      HttpStatus.NOT_FOUND,
    );
  }
}
