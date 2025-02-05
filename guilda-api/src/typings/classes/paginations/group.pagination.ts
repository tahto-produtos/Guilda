import { paginate } from '../../../utils';
import { GroupEntity } from '~entities/group.entity';

export class GroupPagination extends paginate<GroupEntity>(GroupEntity) {}
