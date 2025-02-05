import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class OutOfStockException extends BaseException {
  constructor(amount: number, totalInStock: number) {
    super(
      {
        message: `Não é possível comprar a quantidade de itens informado. Existe ${totalInStock} produtos no estoque.`,
        code: EXCEPTION_CODES.OUT_OF_STOCK,
        keys: {
          amount,
          totalInStock,
        },
      },
      HttpStatus.BAD_REQUEST,
    );
  }
}
