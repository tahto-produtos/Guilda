import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class stockProductDontExistsException extends BaseException {
  constructor(
    resource: string,
    keys: Record<string, string | number>,
    code: any = EXCEPTION_CODES.CAN_NOT_REMOVE_ITEM,
  ) {
    super(
      {
        message: `${resource}`,
        code,
        keys,
      },
      HttpStatus.BAD_REQUEST,
    );
  }
}
