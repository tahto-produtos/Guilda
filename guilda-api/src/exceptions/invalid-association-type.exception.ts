import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class InvalidAssociationTypeException extends BaseException {
  constructor(stockType: string, productType: string) {
    super(
      {
        message: `stock type ${stockType} is not the same as product ${productType}`,
        code: EXCEPTION_CODES.INVALID_ENTRIES,
        keys: {
          stockType,
          productType,
        },
      },
      HttpStatus.BAD_REQUEST,
    );
  }
}
