import { Seeder } from '../utils';

interface ProfileCollaboratorAdministration {
  name: string;
}

export class ProfileCollaboratorAdministrationSeeds extends Seeder<
  ProfileCollaboratorAdministration[]
> {
  async seed() {
    const profiles = this.data;
    for (const profile of profiles) {
      const profileCollaboratorAdministrationAlreadyAdded =
        await this.prisma.profileCollaboratorAdministration.findFirst({
          where: { name: profile.name },
        });
      let permissions;
      if (!profileCollaboratorAdministrationAlreadyAdded) {
        if (profile.name === 'administrador – CRUD DE USUARIO') {
          permissions = await this.prisma.permission.findMany({
            where: {
              resource: 'Collaborators',
            },
          });
        }
        if (profile.name === 'administrador') {
          permissions = await this.prisma.permission.findMany({
            where: {
              NOT: {
                resource: 'Collaborators',
              },
            },
          });
        }

        const profileCollaboratorAdministration =
          await this.prisma.profileCollaboratorAdministration.create({
            data: {
              name: profile.name,
            },
          });
        for await (const permission of permissions) {
          const profilePermissionAlreadyAdded =
            await this.prisma.profilePermission.findFirst({
              where: {
                permissionId: permission.id,
                profileCollaboratorAdministrationId:
                  profileCollaboratorAdministration.id,
              },
            });
          if (!profilePermissionAlreadyAdded) {
            await this.prisma.profilePermission.create({
              data: {
                permissionId: permission.id,
                profileCollaboratorAdministrationId:
                  profileCollaboratorAdministration.id,
              },
            });
          }
        }
      }
    }
  }

  get data(): ProfileCollaboratorAdministration[] {
    return [
      {
        name: 'administrador',
      },
      {
        name: 'administrador – CRUD DE USUARIO',
      },
    ];
  }
}
