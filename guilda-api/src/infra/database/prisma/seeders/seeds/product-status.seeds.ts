import { Seeder } from '../utils';

export class ProductStatusSeeds extends Seeder<string[]> {
  async seed() {
    for (const status of this.data) {
      const statusAlreadyRegistered = await this.prisma.productStatus.findFirst(
        {
          where: {
            status: status,
          },
        },
      );

      if (statusAlreadyRegistered) {
        continue;
      }

      await this.prisma.productStatus.create({
        data: { status },
      });
    }
  }

  get data(): string[] {
    return ['Ativo', 'Inativo'];
  }
}
