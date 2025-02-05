import { Profile } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';

export class ProfileEntity implements Profile {
  @ApiProperty()
  id: number;

  @ApiProperty()
  profile: string;

  @ApiProperty()
  level: number;

  @ApiProperty()
  hierarchyId: number;

  @ApiProperty()
  createdAt: Date;

  @ApiProperty({ nullable: true })
  deletedAt: Date | null;

  constructor(profile: Profile) {
    this.id = profile.id;
    this.createdAt = profile.createdAt;
    this.deletedAt = profile.deletedAt;
  }
}
