import { Seeder } from '../utils';
import { RemoveReason } from '../../../../../typings/enums/remove-reason.enum';

export class HistoryStockProductReasonRemovedSeeds extends Seeder<string[]> {
  async seed() {
    for (const reason of this.data) {
      const statusAlreadyRegistered =
        await this.prisma.gdaReasonRemoved.findFirst({
          where: {
            reason: reason,
          },
        });

      if (statusAlreadyRegistered) {
        continue;
      }

      await this.prisma.gdaReasonRemoved.create({
        data: { reason },
      });
    }
  }

  get data(): string[] {
    return [
      RemoveReason.SOLD,
      RemoveReason.EXPIRED,
      RemoveReason.OTHER,
      RemoveReason.INSERT_PRODUCT,
    ];
  }
}
