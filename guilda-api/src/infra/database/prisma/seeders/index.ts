import { PrismaClient } from '@prisma/client';
import { SeederRunner } from './utils';
import {
  CollaboratorSeeds,
  HierarchiesSeeds,
  HistoryStockProductReasonRemovedSeeds,
  MktConfigSeeds,
  OrderStatusSeeds,
  PermissionsSeeds,
  ProfilePermissionsSeeds,
  ProductStatusSeeds,
  GroupSeeds,
  ProfileCollaboratorAdministrationSeeds,
} from './seeds';
import { DATABASE_CONNECTION } from 'src/constants/environment-variable.constants';

const prisma = new PrismaClient({
  datasources: {
    db: {
      url: DATABASE_CONNECTION,
    },
  },
});

async function main() {
  const seederRunner = new SeederRunner(prisma);
  await seederRunner.run(
    HierarchiesSeeds,
    CollaboratorSeeds,
    PermissionsSeeds,
    ProfilePermissionsSeeds,
    OrderStatusSeeds,
    HistoryStockProductReasonRemovedSeeds,
    MktConfigSeeds,
    ProductStatusSeeds,
    GroupSeeds,
    ProfileCollaboratorAdministrationSeeds,
  );
}

main().then(async () => {
  await prisma.$disconnect();
});
