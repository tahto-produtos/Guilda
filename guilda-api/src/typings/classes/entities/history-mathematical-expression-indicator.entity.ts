import { HistoryMathematicalExpressionIndicator } from '@prisma/client';
export class HistoryMathematicalExpressionIndicatorEntity
  implements HistoryMathematicalExpressionIndicator
{
  id: number;
  mathematicalExpressionId: number;
  indicatorId: number;
  createdAt: Date;
  deletedAt: Date | null;
}
