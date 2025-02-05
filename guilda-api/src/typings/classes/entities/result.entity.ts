import { Result } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';

export class ResultEntity implements Result {
  @ApiProperty()
  id: number;

  @ApiProperty()
  indicatorId: number;

  @ApiProperty()
  transactionId: number | null;

  @ApiProperty()
  result: number;

  @ApiProperty()
  factorsList: string;

  @ApiProperty()
  createdAt: Date;

  @ApiProperty()
  deletedAt: Date;

  @ApiProperty()
  collaboratorId: number;
}
