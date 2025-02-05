import { HttpException, HttpStatus } from '@nestjs/common';
import { Exception } from '../interfaces';

export class BaseException extends HttpException {
  constructor(params: Exception, httpStatusCode: HttpStatus) {
    super(params, httpStatusCode);
  }
}
