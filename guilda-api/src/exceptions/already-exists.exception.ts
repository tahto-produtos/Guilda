import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class AlreadyExistsException extends BaseException {
  constructor(resource: string, keys: Record<string, string | number>) {
    super(
      {
        message: `${resource} já existe`,
        code: EXCEPTION_CODES.RESOURCE_ALREADY_EXISTS,
        keys,
      },
      HttpStatus.CONFLICT,
    );
  }
}
