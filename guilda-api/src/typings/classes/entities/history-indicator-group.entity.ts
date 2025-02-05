import { HistoryIndicatorGroup } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';

export class HistoryIndicatorGroupEntity implements HistoryIndicatorGroup {
  @ApiProperty()
  id: number;

  @ApiProperty()
  indicatorId: number;

  @ApiProperty()
  sectorId: number;

  @ApiProperty()
  monetization: number | null;

  @ApiProperty()
  metricMin: number | null;

  @ApiProperty()
  metricMinNight: number | null;

  @ApiProperty()
  metricMinLatenight: number | null;

  @ApiProperty()
  monetizationNight: number | null;

  @ApiProperty()
  monetizationLatenight: number | null;

  @ApiProperty()
  ended_at: Date | null;

  @ApiProperty()
  started_at: Date | null;

  @ApiProperty()
  groupId: number;

  @ApiProperty()
  createdAt: Date;

  @ApiProperty()
  deletedAt: Date | null;

  constructor(historyIndicatorGroup: HistoryIndicatorGroup) {
    this.id = historyIndicatorGroup.id;
    this.indicatorId = historyIndicatorGroup.indicatorId;
    this.sectorId = historyIndicatorGroup.sectorId;
    this.groupId = historyIndicatorGroup.groupId;
    this.metricMin = historyIndicatorGroup.metricMin;
    this.ended_at = historyIndicatorGroup.ended_at;
    this.started_at = historyIndicatorGroup.started_at;
    this.createdAt = historyIndicatorGroup.createdAt;
    this.deletedAt = historyIndicatorGroup.deletedAt;
    this.metricMinNight = historyIndicatorGroup.metricMinNight;
    this.metricMinLatenight = historyIndicatorGroup.metricMinLatenight;
    this.monetizationNight = historyIndicatorGroup.monetizationNight;
    this.monetizationLatenight = historyIndicatorGroup.monetizationLatenight;
  }
}
