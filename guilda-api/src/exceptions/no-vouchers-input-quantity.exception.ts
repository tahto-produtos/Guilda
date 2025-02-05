import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class NoVouchersInputQuantityException extends BaseException {
  constructor(
    keys: Record<string, string | number | boolean>,
  ) {
    super(
      {
        message: `Vouchers são obrigatórios para produtos digitais.`,
        code: EXCEPTION_CODES.NOT_FOUND,
        keys: keys,
      },
      HttpStatus.NOT_FOUND,
    );
  }
}
