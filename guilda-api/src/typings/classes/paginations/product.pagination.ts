import { paginate } from '../../../utils';
import { GdaProductEntity } from '~entities/product.entity';

export class ProductPagination extends paginate<GdaProductEntity>(
  GdaProductEntity,
) {}
