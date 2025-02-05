import { ProfilePermission } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';
import { PermissionEntity } from './permission.entity';

export class ProfilePermissionEntity implements ProfilePermission {
  @ApiProperty()
  id: number;

  @ApiProperty()
  profileCollaboratorAdministrationId: number | null;

  @ApiProperty()
  profileId: number;

  @ApiProperty()
  createdAt: Date;

  @ApiProperty({ type: PermissionEntity })
  Permission?: PermissionEntity[];

  @ApiProperty()
  permissionId: number | null;

  constructor(profilePermission: ProfilePermissionEntity) {
    this.id = profilePermission.id;
    this.profileId = profilePermission.profileId;
    this.Permission = profilePermission.Permission;
    this.permissionId = profilePermission.permissionId;
    this.profileCollaboratorAdministrationId =
      profilePermission.profileCollaboratorAdministrationId;
    this.createdAt = profilePermission.createdAt;
  }
}
