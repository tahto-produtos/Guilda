import { Group } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';

export class GroupEntity implements Group {
  @ApiProperty()
  id: number;

  @ApiProperty()
  name: string;

  @ApiProperty()
  alias: string;

  @ApiProperty()
  uploadId: number;

  @ApiProperty()
  description: string;

  @ApiProperty()
  order: number;

  @ApiProperty()
  createdAt: Date;

  constructor(group: GroupEntity) {
    this.id = group.id;
    this.name = group.name;
    this.alias = group.alias;
    this.uploadId = group.uploadId;
    this.description = group.description;
    this.order = group.order;
    this.createdAt = group.createdAt;
  }
}
