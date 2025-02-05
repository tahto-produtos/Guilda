import { MathematicalExpression } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';

export class MathematicalExpressionEntity implements MathematicalExpression {
  @ApiProperty()
  id: number;

  @ApiProperty()
  expression: string;

  @ApiProperty()
  createdAt: Date;

  @ApiProperty()
  deletedAt: Date | null;
}
