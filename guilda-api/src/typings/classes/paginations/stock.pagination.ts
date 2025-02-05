import { paginate } from '../../../utils';
import { StockEntity } from '~entities/stock.entity';

export class StockPagination extends paginate<StockEntity>(StockEntity) {}
