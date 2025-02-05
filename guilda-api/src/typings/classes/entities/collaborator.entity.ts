import { Collaborator } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';

export class CollaboratorEntity implements Collaborator {
  @ApiProperty()
  id: number;

  @ApiProperty()
  name: string;

  @ApiProperty()
  identification: string;

  @ApiProperty()
  registry: string;

  @ApiProperty()
  genre: string;

  @ApiProperty()
  birthdate: Date;

  @ApiProperty()
  admissionDate: Date;

  @ApiProperty()
  maritalStatus: number;

  @ApiProperty()
  active: string | null;

  @ApiProperty()
  email: string | null;

  @ApiProperty()
  street: string | null;

  @ApiProperty()
  number: number | null;

  @ApiProperty()
  neighborhood: string | null;

  @ApiProperty()
  city: string | null;

  @ApiProperty()
  state: string | null;

  @ApiProperty()
  country: string | null;

  @ApiProperty()
  homeNumber: string | null;

  @ApiProperty()
  phoneNumber: string | null;

  @ApiProperty()
  schooling: number | null;

  @ApiProperty()
  contractorControlId: string | null;

  @ApiProperty()
  dependantNumber: string | null;

  @ApiProperty()
  updatedAt: Date | null;

  @ApiProperty()
  transactionId: bigint | null;

  @ApiProperty()
  profileCollaboratorAdministrationId: number | null;

  @ApiProperty()
  entryDate: Date | null;

  @ApiProperty()
  homeBased: boolean | null;

  @ApiProperty()
  firstLogin: boolean | null;

  @ApiProperty()
  entryTime: Date | null;

  @ApiProperty()
  createdAt: Date;

  @ApiProperty({ nullable: true })
  deletedAt: Date | null;

  constructor(collaborator: Collaborator) {
    this.id = collaborator.id;
    this.name = collaborator.name;
    this.identification = collaborator.identification;
    this.registry = collaborator.registry;
    this.genre = collaborator.genre;
    this.birthdate = collaborator.birthdate;
    this.firstLogin = collaborator.firstLogin;
    this.admissionDate = collaborator.admissionDate;
    this.maritalStatus = collaborator.maritalStatus;
    this.createdAt = collaborator.createdAt;
    this.deletedAt = collaborator.deletedAt;
  }
}
