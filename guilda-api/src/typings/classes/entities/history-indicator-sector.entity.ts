import { ApiProperty } from '@nestjs/swagger';
import { HistoryIndicatorSector } from '@prisma/client';

export class HistoryIndicatorSectorEntity implements HistoryIndicatorSector {
  @ApiProperty()
  id: number;

  @ApiProperty()
  indicatorId: number;

  @ApiProperty()
  sectorId: number;

  @ApiProperty()
  createdAt: Date;

  @ApiProperty()
  deletedAt: Date;

  goal: number | null;

  @ApiProperty()
  started_at: Date;

  @ApiProperty()
  ended_at: Date;

  @ApiProperty()
  active: boolean;

  @ApiProperty()
  alteredById: number;

  @ApiProperty()
  goalNight: number | null;

  @ApiProperty()
  goalLatenight: number | null;
}
