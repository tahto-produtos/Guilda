import { Factor } from '@prisma/client';

export class FactorEntity implements Factor {
  id: number;
  index: number;
  value: number;
  resultId: number;
}
