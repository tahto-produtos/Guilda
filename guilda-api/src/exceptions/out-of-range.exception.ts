import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class OutOfRangeException extends BaseException {
  constructor(limit: number, offset: number, total: number) {
    super(
      {
        message: `cannot take ${limit} or skip ${offset} item(s) on total ${total} items`,
        code: EXCEPTION_CODES.OUT_OF_RANGE,
        keys: {
          limit,
          offset,
        },
      },
      HttpStatus.BAD_REQUEST,
    );
  }
}
