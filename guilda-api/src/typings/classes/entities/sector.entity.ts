import { Sector } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';

export class SectorEntity implements Sector {
  @ApiProperty()
  id: number;

  @ApiProperty()
  name: string;

  @ApiProperty()
  level: number;

  @ApiProperty()
  sector: number;

  @ApiProperty()
  subSector: number;

  @ApiProperty()
  historyIndicatorSectors?: any;

  @ApiProperty()
  createdAt: Date;

  @ApiProperty({ nullable: true })
  deletedAt: Date | null;

  constructor(sectorEntity: Sector) {
    const { id, name, level, createdAt, deletedAt, sector, subSector } =
      sectorEntity;
    this.id = id;
    this.name = name;
    this.sector = sector;
    this.subSector = subSector;
    this.level = level;
    this.createdAt = createdAt;
    this.deletedAt = deletedAt;
  }
}
