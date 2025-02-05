import { Seeder } from '../utils';

interface ProfilePermission {
  profile: string;
}

export class ProfilePermissionsSeeds extends Seeder<ProfilePermission> {
  async seed() {
    const profile = this.data;
    const profileAlready = await this.prisma.profile.findFirst({
      where: {
        profile: profile.profile,
      },
    });

    if (profileAlready) {
      const permissions = await this.prisma.permission.findMany();

      for (const permission of permissions) {
        const profilePermissionAlready =
          await this.prisma.profilePermission.findFirst({
            where: {
              profileId: profileAlready.id,
              permissionId: permission.id,
            },
          });

        if (!profilePermissionAlready) {
          await this.prisma.profilePermission.create({
            data: {
              profileId: profileAlready.id,
              permissionId: permission.id,
            },
          });
        }
      }
    }
  }

  get data(): ProfilePermission {
    return { profile: 'ADMINISTRADOR' };
  }
}
