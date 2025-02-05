import { Seeder } from '../utils';

interface Group {
  name: string;
  order: number;
}

export class GroupSeeds extends Seeder<Group[]> {
  async seed() {
    for (const item of this.data) {
      const groupExists = await this.prisma.group.findFirst({
        where: {
          name: item.name,
          order: null,
        },
      });

      if (groupExists) {
        await this.prisma.group.update({
          where: {
            id: groupExists.id,
          },
          data: {
            order: item.order,
          },
        });
      }
    }
  }

  get data(): Group[] {
    return [
      {
        name: 'G1',
        order: 1,
      },
      {
        name: 'G2',
        order: 2,
      },
      {
        name: 'G3',
        order: 3,
      },
      {
        name: 'G4',
        order: 4,
      },
    ];
  }
}
