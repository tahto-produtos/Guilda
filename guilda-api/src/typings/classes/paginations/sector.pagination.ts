import { paginate } from '../../../utils';
import { SectorEntity } from '~entities/sector.entity';

export class SectorPagination extends paginate<SectorEntity>(SectorEntity) {}
