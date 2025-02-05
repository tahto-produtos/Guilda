import { paginate } from '../../../utils';
import { PermissionEntity } from '~entities/permission.entity';

export class PermissionPagination extends paginate<PermissionEntity>(
  PermissionEntity,
) {}
