import { ConsolidatedResult } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';

import { ResultEntity } from '~entities/result.entity';

export class ConsolidatedResultEntity implements ConsolidatedResult {
  @ApiProperty()
  id: number;

  @ApiProperty()
  resultId: number;

  @ApiProperty()
  sectorId: number | null;

  @ApiProperty()
  value: number;

  @ApiProperty()
  collaboratorId: number | null;

  @ApiProperty()
  indicatorId: number | null;

  @ApiProperty()
  goal: number | null;

  @ApiProperty()
  transactionId: number | null;

  @ApiProperty()
  createdAt: Date;

  @ApiProperty({ type: ResultEntity })
  result?: ResultEntity;

  constructor(consolidatedResult: ConsolidatedResultEntity) {
    this.id = consolidatedResult.id;
    this.resultId = consolidatedResult.resultId;
    this.sectorId = consolidatedResult.sectorId;
    this.value = consolidatedResult.value;
    this.collaboratorId = consolidatedResult.collaboratorId;
    this.createdAt = consolidatedResult.createdAt;
    this.result = consolidatedResult.result;
    this.indicatorId = consolidatedResult.indicatorId;
    this.goal = consolidatedResult.goal;
  }
}
