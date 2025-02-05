import { Seeder } from '../utils';
import { Hashing } from '../../../../../utils';
import { Hierarchies } from '../../../../../typings/enums/hierarchies.enum';
import { CollaboratorEntity } from '../../../../../typings/classes/entities/collaborator.entity';

interface Collaborator {
  name: string;
  identification: string;
  registry: string;
  genre: string;
  birthdate: Date;
  admissionDate: Date;
  maritalStatus: number;
  hierarchy: {
    contractorControlId: number;
    hierarchy: Hierarchies;
    parentIdentification?: string;
  };
  credentials: {
    username: string;
    password: string;
  };
}

export class CollaboratorSeeds extends Seeder<Collaborator[]> {
  async seed() {
    const collaborators = this.data;

    for (const collaborator of collaborators) {
      const alreadyRegisteredCollaborator =
        await this.prisma.collaborator.findFirst({
          where: {
            OR: [
              {
                identification: collaborator.identification,
              },
              {
                registry: collaborator.registry,
              },
            ],
          },
        });

      if (!alreadyRegisteredCollaborator) {
        const hierarchy = await this.prisma.hierarchy.findFirst({
          where: {
            levelName: collaborator.hierarchy.hierarchy,
          },
        });

        let parent: CollaboratorEntity;
        if (collaborator.hierarchy.parentIdentification) {
          parent = await this.prisma.collaborator.findFirst({
            where: {
              identification: collaborator.hierarchy.parentIdentification,
            },
          });
        }

        await this.prisma.collaborator.create({
          data: {
            name: collaborator.name,
            identification: collaborator.identification,
            registry: collaborator.registry,
            genre: collaborator.genre,
            birthdate: collaborator.birthdate,
            admissionDate: collaborator.admissionDate,
            maritalStatus: collaborator.maritalStatus,
            Credentials: {
              create: {
                username: collaborator.credentials.username,
                password: await Hashing.hash(collaborator.credentials.password),
              },
            },
            historyHierarchyRelationship: {
              create: {
                contractorControlId: collaborator.hierarchy.contractorControlId,
                parentId: parent?.id,
                hierarchyId: hierarchy.id,
              },
            },
          },
        });
      }
    }
  }

  get data(): Collaborator[] {
    return [
      {
        name: 'admin',
        identification: 'd9729feb',
        registry: '92cc3482',
        genre: 'M',
        birthdate: new Date('01/01/2000'),
        admissionDate: new Date('01/01/2020'),
        maritalStatus: 1,
        hierarchy: {
          contractorControlId: 1,
          hierarchy: Hierarchies.ADMINISTRADOR,
        },
        credentials: {
          username: 'admin',
          password: 'sBf4zoYl',
        },
      },
    ];
  }
}
