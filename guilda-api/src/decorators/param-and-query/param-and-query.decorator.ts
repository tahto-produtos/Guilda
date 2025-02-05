import {
  createParamDecorator,
  ExecutionContext,
  PipeTransform,
  Type,
} from '@nestjs/common';
import { plainToInstance } from 'class-transformer';
import { validateSync } from 'class-validator';

import { InvalidEntriesException } from '../../exceptions';

export const ParamAndQuery = (dtoType?: Type, pipes?: PipeTransform[]) => {
  return createParamDecorator((data: unknown, ctx: ExecutionContext) => {
    const req = ctx.switchToHttp().getRequest();
    const paramsAndQuery = { ...req.query, ...req.params };

    if (dtoType) {
      const dto = plainToInstance(dtoType, paramsAndQuery);
      const errors = validateSync(dto);
      if (errors.length > 0) {
        throw new InvalidEntriesException(errors);
      }

      return dto;
    }

    return paramsAndQuery;
  })(...(pipes || []));
};
