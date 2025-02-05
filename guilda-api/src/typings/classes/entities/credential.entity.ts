import { Credentials } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';

export class CredentialsEntity implements Credentials {
  @ApiProperty()
  id: number;

  @ApiProperty()
  username: string;

  @ApiProperty()
  password: string;

  @ApiProperty()
  collaboratorId: number;

  constructor(consolidatedResult: Credentials) {
    this.id = consolidatedResult.id;
    this.username = consolidatedResult.username;
    this.password = consolidatedResult.password;
    this.collaboratorId = consolidatedResult.collaboratorId;
  }
}
