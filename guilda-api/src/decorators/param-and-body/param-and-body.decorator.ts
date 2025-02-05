import {
  createParamDecorator,
  ExecutionContext,
  PipeTransform,
  Type,
} from '@nestjs/common';
import { plainToInstance } from 'class-transformer';
import { validateSync } from 'class-validator';

import { InvalidEntriesException } from '../../exceptions';

export const ParamAndBody = (dtoType?: Type, pipes?: PipeTransform[]) => {
  return createParamDecorator((data: unknown, ctx: ExecutionContext) => {
    const req = ctx.switchToHttp().getRequest();
    const paramsAndBody = { ...req.body, ...req.params };

    if (dtoType) {
      const dto = plainToInstance(dtoType, paramsAndBody);
      const errors = validateSync(dto);
      if (errors.length > 0) {
        throw new InvalidEntriesException(errors);
      }

      return dto;
    }

    return paramsAndBody;
  })(...(pipes || []));
};
