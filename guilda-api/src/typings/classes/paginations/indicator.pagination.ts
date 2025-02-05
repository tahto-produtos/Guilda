import { paginate } from '../../../utils';
import { IndicatorEntity } from '~entities/indicator.entity';

export class IndicatorPagination extends paginate<IndicatorEntity>(
  IndicatorEntity,
) {}
