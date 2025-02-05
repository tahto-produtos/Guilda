import { ApiProperty } from '@nestjs/swagger';

export class HistoryIndicatorSectorGroupEntity {
  @ApiProperty()
  indicator: string;

  @ApiProperty()
  startedAt: Date;

  @ApiProperty()
  endedAt: Date;

  @ApiProperty()
  goal: number;

  @ApiProperty()
  sector: string;

  @ApiProperty()
  active: boolean;

  @ApiProperty()
  groupOne: number;

  @ApiProperty()
  groupTwo: number;

  @ApiProperty()
  groupThree: number;

  @ApiProperty()
  groupFour: number;
}
