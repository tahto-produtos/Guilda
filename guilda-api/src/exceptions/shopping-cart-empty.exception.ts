import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class ShoppingCartEmptyException extends BaseException {
  constructor(keys: Record<string, string | number | boolean>) {
    super(
      {
        message: `shopping cart is empty`,
        code: EXCEPTION_CODES.OUT_OF_RANGE,
        keys: keys,
      },
      HttpStatus.NOT_FOUND,
    );
  }
}
