import { Transaction } from '@prisma/client';

export class TransactionEntity implements Transaction {
  id: number;
  identification: string;
  createdAt: Date;
  updatedAt: Date | null;
  complete: boolean;
  status: string;
}
