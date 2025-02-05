import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class AssociationNotFoundException extends BaseException {
  constructor(resources: string[], keys: Record<string, string | number>) {
    super(
      {
        message: `association between ${resources.toString()} not found`,
        code: EXCEPTION_CODES.ASSOCIATION_NOT_FOUND,
        keys,
      },
      HttpStatus.NOT_FOUND,
    );
  }
}
