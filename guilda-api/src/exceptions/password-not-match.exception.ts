import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class PasswordNotMatchException extends BaseException {
  constructor(message: string = undefined) {
    super(
      {
        message: message || `password not match`,
        code: EXCEPTION_CODES.PASSWORD_NOT_MATCH,
      },
      HttpStatus.UNAUTHORIZED,
    );
  }
}
