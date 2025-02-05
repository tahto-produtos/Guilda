import { Permission } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';

export class PermissionEntity implements Permission {
  @ApiProperty()
  id: number;

  @ApiProperty()
  action: string;

  @ApiProperty()
  resource: string;

  @ApiProperty()
  createdAt: Date;

  @ApiProperty({ nullable: true })
  deletedAt: Date | null;

  constructor(permission: Permission) {
    this.id = permission.id;
    this.action = permission.action;
    this.resource = permission.resource;
    this.createdAt = permission.createdAt;
    this.deletedAt = permission.deletedAt;
  }
}
