import { paginate } from '../../../utils';
import { HierarchyEntity } from '~entities/hierarchy.entity';

export class HierarchyPagination extends paginate<HierarchyEntity>(
  HierarchyEntity,
) {}
