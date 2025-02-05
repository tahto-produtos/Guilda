import { ProfileCollaboratorAdministration } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';
import { ProfilePermissionEntity } from './profile-permission.entity';

export class ProfileCollaboratorAdministrationEntity
  implements ProfileCollaboratorAdministration
{
  @ApiProperty()
  id: number;

  @ApiProperty()
  name: string;

  @ApiProperty()
  createdAt: Date;

  @ApiProperty({ nullable: true })
  deletedAt: Date | null;

  @ApiProperty({ type: ProfilePermissionEntity })
  ProfilePermission?: ProfilePermissionEntity[];

  constructor(
    profileCollaboratorAdministrationEntity: ProfileCollaboratorAdministrationEntity,
  ) {
    this.id = profileCollaboratorAdministrationEntity.id;
    this.name = profileCollaboratorAdministrationEntity.name;
    this.createdAt = profileCollaboratorAdministrationEntity.createdAt;
    this.deletedAt = profileCollaboratorAdministrationEntity.deletedAt;
    this.ProfilePermission =
      profileCollaboratorAdministrationEntity.ProfilePermission;
  }
}
