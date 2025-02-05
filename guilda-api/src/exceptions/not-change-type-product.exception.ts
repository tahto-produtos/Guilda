import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class NotChangeTypeProduct extends BaseException {
  constructor(product?: string, id?: number) {
    super(
      {
        message: `It is not allowed to update the product type ${product}`,
        code: EXCEPTION_CODES.NOT_CHANGE_TYPE_PRODUCT,
        keys: {
          id,
        },
      },
      HttpStatus.BAD_REQUEST,
    );
  }
}
