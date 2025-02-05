import { CheckingAccount } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';

export class CheckingAccountEntity implements CheckingAccount {
  @ApiProperty()
  id: number;

  @ApiProperty()
  input: number;

  @ApiProperty()
  output: number;

  @ApiProperty()
  reason: string;

  @ApiProperty()
  observation: string;

  @ApiProperty()
  balance: number;

  @ApiProperty()
  collaboratorId: number;

  @ApiProperty()
  indicatorId: number;

  @ApiProperty()
  orderId: number;

  @ApiProperty()
  resultId: number;

  @ApiProperty()
  weight: number;

  @ApiProperty()
  createdByCollaboratorId: number;

  @ApiProperty()
  resultDate: Date;

  @ApiProperty()
  createdAt: Date;

  constructor(checkingAccount: CheckingAccount) {
    this.id = checkingAccount.id;
    this.input = checkingAccount.input;
    this.output = checkingAccount.output;
    this.reason = checkingAccount.reason;
    this.observation = checkingAccount.observation;
    this.balance = checkingAccount.balance;
    this.collaboratorId = checkingAccount.collaboratorId;
    this.indicatorId = checkingAccount.indicatorId;
    this.orderId = checkingAccount.orderId;
    this.resultId = checkingAccount.resultId;
    this.weight = checkingAccount.weight;
    this.resultDate = checkingAccount.resultDate;
    this.createdAt = checkingAccount.createdAt;
  }
}
