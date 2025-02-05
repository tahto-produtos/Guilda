import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class NotEnoughDebtException extends BaseException {
  constructor(currentBalance: number, totalPrice: number) {
    super(
      {
        message: `you have only ${currentBalance} coins you can not debt ${totalPrice} coins`,
        code: EXCEPTION_CODES.OUT_OF_RANGE,
        keys: {
          currentBalance,
          totalPrice,
        },
      },
      HttpStatus.BAD_REQUEST,
    );
  }
}
