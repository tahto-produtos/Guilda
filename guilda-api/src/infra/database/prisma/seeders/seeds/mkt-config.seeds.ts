import { Seeder } from '../utils';

interface MktConfig {
  type: string;
  value: number;
}

export class MktConfigSeeds extends Seeder<MktConfig[]> {
  async seed() {
    for (const item of this.data) {
      const statusAlreadyRegistered = await this.prisma.gdaMktConfig.findFirst({
        where: {
          type: item.type,
          value: item.value,
        },
      });

      if (statusAlreadyRegistered) {
        continue;
      }

      await this.prisma.gdaMktConfig.create({
        data: { type: item.type, value: item.value },
      });
    }
  }

  get data(): MktConfig[] {
    return [
      {
        type: 'VENCIMENTO_LIBERADO_HOME',
        value: 7,
      },
      {
        type: 'VENCIMENTO_LIBERADO_PRESENCIAL',
        value: 30,
      },
    ];
  }
}
