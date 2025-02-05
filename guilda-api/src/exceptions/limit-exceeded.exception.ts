import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class limitExceededException extends BaseException {
  constructor(quantity: number, product?: string, bought?: number) {
    super(
      {
        message: `Limit for purchasing is ${quantity}`,
        code: EXCEPTION_CODES.LIMIT_EXCEEDED,
        keys: {
          quantity,
          product,
          bought,
        },
      },
      HttpStatus.BAD_REQUEST,
    );
  }
}
