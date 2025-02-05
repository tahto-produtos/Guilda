import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class NoValidQuantityVouchersImportException extends BaseException {
  constructor(
    keys: Record<string, string | number | boolean>,
  ) {
    super(
      {
        message: `A quantitdade de vouchers deve ser igual a quantidade de entrada.`,
        code: EXCEPTION_CODES.NOT_FOUND,
        keys: keys,
      },
      HttpStatus.NOT_FOUND,
    );
  }
}
