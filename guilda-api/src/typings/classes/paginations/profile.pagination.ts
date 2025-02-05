import { paginate } from '../../../utils';
import { ProfileEntity } from '~entities/profile.entity';

export class ProfilePagination extends paginate<ProfileEntity>(ProfileEntity) {}
