import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class DateValidationVoucherNotFoundException extends BaseException {
  constructor(
    keys: Record<string, string | number | boolean>,
  ) {
    super(
      {
        message: `Data de validade dos vouchers não informada.`,
        code: EXCEPTION_CODES.NOT_FOUND,
        keys: keys,
      },
      HttpStatus.NOT_FOUND,
    );
  }
}
