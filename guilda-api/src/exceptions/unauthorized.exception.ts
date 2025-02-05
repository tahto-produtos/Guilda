import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class UnauthorizedException extends BaseException {
  constructor() {
    super(
      {
        message: `invalid or expired authentication token`,
        code: EXCEPTION_CODES.UNAUTHORIZED_USER,
      },
      HttpStatus.UNAUTHORIZED,
    );
  }
}
