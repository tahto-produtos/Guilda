import { Seeder } from '../utils';
import { Resources, Actions } from 'src/typings';

interface Permission {
  action: string;
  resource: string;
}

export class PermissionsSeeds extends Seeder<Permission[]> {
  async seed() {
    const permissions = this.data;
    for (const permission of permissions) {
      const permissionAlreadyAdded = await this.prisma.permission.findFirst({
        where: {
          action: permission.action,
          resource: permission.resource,
        },
      });

      if (!permissionAlreadyAdded) {
        await this.prisma.permission.create({
          data: {
            action: permission.action,
            resource: permission.resource,
          },
        });
      }
    }
  }

  get data(): Permission[] {
    return [
      {
        action: Actions.Read,
        resource: Resources.Health,
      },
      {
        action: Actions.Create,
        resource: Resources.Authentication,
      },
      {
        action: Actions.FindMany,
        resource: Resources.Groups,
      },
      {
        action: Actions.Create,
        resource: Resources.Groups,
      },
      {
        action: Actions.Update,
        resource: Resources.Groups,
      },
      {
        action: Actions.Delete,
        resource: Resources.Groups,
      },
      {
        action: Actions.Create,
        resource: Resources.Sectors,
      },
      {
        action: Actions.FindMany,
        resource: Resources.Sectors,
      },
      {
        action: Actions.DeleteMany,
        resource: Resources.Sectors,
      },
      {
        action: Actions.FindOne,
        resource: Resources.Sectors,
      },
      {
        action: Actions.DeleteOne,
        resource: Resources.Sectors,
      },
      {
        action: Actions.AssociateIndicatorsToSectors,
        resource: Resources.Sectors,
      },
      {
        action: Actions.ReportSectorsIndicatorsGroups,
        resource: Resources.Sectors,
      },
      {
        action: Actions.InputSectorsIndicatorsGroups,
        resource: Resources.Sectors,
      },
      {
        action: Actions.Create,
        resource: Resources.MetricSettings,
      },
      {
        action: Actions.FindMany,
        resource: Resources.MetricSettings,
      },
      {
        action: Actions.Create,
        resource: Resources.Indicators,
      },
      {
        action: Actions.DeleteMany,
        resource: Resources.Indicators,
      },
      {
        action: Actions.FindOne,
        resource: Resources.Indicators,
      },
      {
        action: Actions.FindMany,
        resource: Resources.Indicators,
      },
      {
        action: Actions.DeleteOne,
        resource: Resources.Indicators,
      },
      {
        action: Actions.Update,
        resource: Resources.Indicators,
      },
      {
        action: Actions.RemoveAssociateSectorsToIndicator,
        resource: Resources.Indicators,
      },
      {
        action: Actions.Create,
        resource: Resources.Hierarchies,
      },
      {
        action: Actions.FindMany,
        resource: Resources.Hierarchies,
      },
      {
        action: Actions.FindOne,
        resource: Resources.Hierarchies,
      },
      {
        action: Actions.DeleteOne,
        resource: Resources.Hierarchies,
      },
      {
        action: Actions.FindOne,
        resource: Resources.Collaborators,
      },
      {
        action: Actions.FindMany,
        resource: Resources.Collaborators,
      },
      {
        action: Actions.Update,
        resource: Resources.Collaborators,
      },
      {
        action: Actions.Create,
        resource: Resources.Collaborators,
      },
      {
        action: Actions.FindCheckingAccountCollaborator,
        resource: Resources.Collaborators,
      },
      {
        action: Actions.AddCreditToCollaboratorCheckingAccount,
        resource: Resources.Collaborators,
      },
      {
        action: Actions.AddDebtToCollaboratorCheckingAccount,
        resource: Resources.Collaborators,
      },
      {
        action: Actions.ResetPassword,
        resource: Resources.Collaborators,
      },
      {
        action: Actions.FindMany,
        resource: Resources.Profiles,
      },
      {
        action: Actions.Update,
        resource: Resources.Profiles,
      },
      {
        action: Actions.FindMany,
        resource: Resources.Permissions,
      },
      {
        action: Actions.UpdatePermission,
        resource: Resources.Profiles,
      },
      {
        action: Actions.FindOne,
        resource: Resources.Me,
      },
      {
        action: Actions.Create,
        resource: Resources.Result,
      },
      {
        action: Actions.FindMany,
        resource: Resources.Result,
      },
      {
        action: Actions.FindOne,
        resource: Resources.Result,
      },
      {
        action: Actions.Update,
        resource: Resources.Result,
      },
      {
        action: Actions.Delete,
        resource: Resources.Result,
      },
      {
        action: Actions.Create,
        resource: Resources.Product,
      },
      {
        action: Actions.FindOne,
        resource: Resources.Product,
      },
      {
        action: Actions.FindMany,
        resource: Resources.Product,
      },
      {
        action: Actions.Update,
        resource: Resources.Product,
      },
      {
        action: Actions.Delete,
        resource: Resources.Product,
      },
      {
        action: Actions.Create,
        resource: Resources.Stock,
      },
      {
        action: Actions.FindMany,
        resource: Resources.Stock,
      },
      {
        action: Actions.FindOne,
        resource: Resources.Stock,
      },
      {
        action: Actions.Update,
        resource: Resources.Stock,
      },
      {
        action: Actions.Delete,
        resource: Resources.Stock,
      },
      {
        action: Actions.AssociateProductToStock,
        resource: Resources.Stock,
      },
      {
        action: Actions.ReporStock,
        resource: Resources.Stock,
      },
      {
        action: Actions.Create,
        resource: Resources.Type,
      },
      {
        action: Actions.FindOne,
        resource: Resources.Type,
      },
      {
        action: Actions.FindMany,
        resource: Resources.Type,
      },
      {
        action: Actions.Update,
        resource: Resources.Type,
      },
      {
        action: Actions.Delete,
        resource: Resources.Type,
      },
      {
        action: Actions.Create,
        resource: Resources.Order,
      },
      {
        action: Actions.Update,
        resource: Resources.Order,
      },
      {
        action: Actions.FindOne,
        resource: Resources.Order,
      },
      {
        action: Actions.FindMany,
        resource: Resources.Order,
      },
      {
        action: Actions.FindOwn,
        resource: Resources.Order,
      },
      {
        action: Actions.Delete,
        resource: Resources.Order,
      },
      {
        action: Actions.ConfirmReleasedOrder,
        resource: Resources.Order,
      },
      {
        action: Actions.GenerateQrCodeOrder,
        resource: Resources.Order,
      },
      {
        action: Actions.FindOwn,
        resource: Resources.CollaboratorVoucher,
      },
      {
        action: Actions.FindOne,
        resource: Resources.CollaboratorVoucher,
      },
      {
        action: Actions.ReportMonetizationsByHierarchy,
        resource: Resources.Monetizations,
      },
      {
        action: Actions.IndicatorBasketMonetization,
        resource: Resources.Monetizations,
      },
      {
        action: Actions.FindMany,
        resource: Resources.ShoppingCart,
      },
      {
        action: Actions.Update,
        resource: Resources.ShoppingCart,
      },
    ];
  }
}
