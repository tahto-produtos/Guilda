import { paginate } from '../../../utils';
import { HistoryIndicatorSectorGroupEntity } from '~entities/history-indicator-sector-group.entity';

export class HistoryIndicatorSectorGroupPagination extends paginate<HistoryIndicatorSectorGroupEntity>(
  HistoryIndicatorSectorGroupEntity,
) {}
