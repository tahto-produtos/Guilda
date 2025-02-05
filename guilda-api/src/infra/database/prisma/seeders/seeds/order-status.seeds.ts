import { Seeder } from '../utils';
import { OrderStatus } from '../../../../../typings/enums/order-status.enum';

export class OrderStatusSeeds extends Seeder<string[]> {
  async seed() {
    for (const status of this.data) {
      const statusAlreadyRegistered =
        await this.prisma.gdaOrderStatus.findFirst({
          where: {
            status: status,
          },
        });

      if (statusAlreadyRegistered) {
        continue;
      }

      await this.prisma.gdaOrderStatus.create({
        data: { status },
      });
    }
  }

  get data(): string[] {
    return [
      OrderStatus.ORDERED,
      OrderStatus.RELEASED,
      OrderStatus.DELIVERED,
      OrderStatus.CANCELED,
      OrderStatus.EXPIRED,
    ];
  }
}
