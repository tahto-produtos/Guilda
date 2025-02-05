import { paginate } from '../../../utils';
import { ShoppingCartEntity } from '~entities/shopping-cart.entity';

export class ShoppingCartPagination extends paginate<ShoppingCartEntity>(
  ShoppingCartEntity,
) {}
