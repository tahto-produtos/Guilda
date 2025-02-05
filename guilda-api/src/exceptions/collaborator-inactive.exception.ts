import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class CollaboratorInactiveException extends BaseException {
  constructor(id: number) {
    super(
      {
        message: `Collaborator is inactive`,
        code: EXCEPTION_CODES.COLLABORATOR_INACTIVE,
        keys: {
          id,
        },
      },
      HttpStatus.BAD_REQUEST,
    );
  }
}
