import { Seeder } from '../utils';

interface Hierarchy {
  levelName: string;
  levelWeight: number;
}

export class HierarchiesSeeds extends Seeder<Hierarchy[]> {
  async seed() {
    const hierarchies = this.data;
    for (const hierarchy of hierarchies) {
      const hierarchyAlreadyAdded = await this.prisma.hierarchy.findFirst({
        where: { levelName: hierarchy.levelName },
      });

      if (!hierarchyAlreadyAdded) {
        await this.prisma.hierarchy.create({
          data: {
            levelName: hierarchy.levelName,
            levelWeight: hierarchy.levelWeight,
            Profile: {
              create: {
                profile: hierarchy.levelName,
                level: hierarchy.levelWeight,
              },
            },
          },
        });
      }
    }
  }

  get data(): Hierarchy[] {
    return [
      {
        levelName: 'AGENTE',
        levelWeight: 1,
      },
      {
        levelName: 'SUPERVISOR',
        levelWeight: 2,
      },
      {
        levelName: 'COORDENADOR',
        levelWeight: 3,
      },
      {
        levelName: 'GERENTE II',
        levelWeight: 4,
      },
      {
        levelName: 'GERENTE I',
        levelWeight: 5,
      },
      {
        levelName: 'DIRETOR',
        levelWeight: 6,
      },
      {
        levelName: 'CEO',
        levelWeight: 7,
      },
      {
        levelName: 'ADMINISTRADOR',
        levelWeight: 1000,
      },
    ];
  }
}
