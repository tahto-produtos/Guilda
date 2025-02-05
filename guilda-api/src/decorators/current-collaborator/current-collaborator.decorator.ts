import { createParamDecorator, ExecutionContext } from '@nestjs/common';

export const CurrentCollaborator = createParamDecorator(
  (data: unknown, ctx: ExecutionContext) => {
    const request = ctx.switchToHttp().getRequest();
    return request.collaborator;
  },
);
