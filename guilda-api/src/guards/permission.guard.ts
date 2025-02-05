import {
  CanActivate,
  ExecutionContext,
  Inject,
  Injectable,
} from '@nestjs/common';
import { Reflector } from '@nestjs/core';
import { CredentialsEntity } from '~entities/credential.entity';
import { PrismaService } from '~prisma/module';

export const RESOURCE_KEY = 'resource';
export const PERMISSIONS_KEY = 'permissions';

@Injectable()
export class PermissionGuard implements CanActivate {
  constructor(
    @Inject(PrismaService) private readonly prisma: PrismaService,
    private readonly reflector: Reflector,
  ) {}

  canActivate(context: ExecutionContext): boolean | Promise<boolean> {
    const resource = this.reflector.get<string>(
      RESOURCE_KEY,
      context.getClass(),
    );
    const permissions = this.reflector.get<string[]>(
      PERMISSIONS_KEY,
      context.getHandler(),
    );

    if (!resource || !permissions) {
      return true;
    }

    const credential = context.switchToHttp().getRequest().credential;

    return this.userHasPermissionsForResource(
      credential,
      resource,
      permissions,
    );
  }

  private async findCredential(credentialId: number) {
    return this.prisma.credentials.findFirst({
      where: {
        id: credentialId,
      },
      include: {
        collaborator: {
          include: {
            historyHierarchyRelationship: {
              where: {
                deletedAt: null,
              },
              orderBy: {
                createdAt: 'desc',
              },
              include: {
                hierarchy: {
                  include: {
                    Profile: {
                      select: {
                        id: true,
                        profile: true,
                        level: true,
                      },
                    },
                  },
                },
              },
            },
          },
        },
      },
    });
  }

  private async userHasPermissionsForResource(
    credentialEntity: CredentialsEntity,
    resource: string,
    permissions: string[],
  ) {
    let isAccept = false;
    const credential = await this.findCredential(credentialEntity.id);
    if (credential) {
      const action = await this.prisma.permission.findFirst({
        where: {
          action: {
            in: permissions,
          },
          resource: resource,
        },
      });
      const profileCollaboratorAdministrationId =
        credential?.collaborator?.profileCollaboratorAdministrationId;
      if (action && profileCollaboratorAdministrationId) {
        const havePermission = await this.prisma.profilePermission.findFirst({
          where: {
            profileCollaboratorAdministrationId,
            permissionId: action?.id,
          },
        });
        isAccept = havePermission ? true : false;
        if (isAccept === true) return isAccept;
      }
      if (
        credential.collaborator.historyHierarchyRelationship.length > 0 &&
        credential.collaborator.historyHierarchyRelationship[0].hierarchy &&
        credential.collaborator.historyHierarchyRelationship[0].hierarchy
          .Profile &&
        credential.collaborator.historyHierarchyRelationship[0].hierarchy
          .Profile.length > 0
      ) {
        const profile =
          credential.collaborator.historyHierarchyRelationship[0].hierarchy
            .Profile[0];

        if (action && !isAccept) {
          const havePermission = await this.prisma.profilePermission.findFirst({
            where: {
              profileId: profile.id,
              permissionId: action.id,
            },
          });
          isAccept = havePermission ? true : false;
        }
      }
    }

    return isAccept;
  }
}
