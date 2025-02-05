import { Hierarchy } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';
export class HierarchyEntity implements HierarchyEntity {
  @ApiProperty()
  id: number;

  @ApiProperty()
  levelName: string;

  @ApiProperty()
  levelWeight: number;

  @ApiProperty()
  createdAt: Date;

  @ApiProperty({ nullable: true })
  deletedAt: Date | null;

  constructor(hierarchy: Hierarchy) {
    const { id, levelName, levelWeight, createdAt, deletedAt } = hierarchy;
    this.id = id;
    this.levelName = levelName;
    this.levelWeight = levelWeight;
    this.createdAt = createdAt;
    this.deletedAt = deletedAt;
  }
}
