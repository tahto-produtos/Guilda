import { paginate } from '../../../utils';
import { OrderEntity } from '~entities/order.entity';

export class OrderPagination extends paginate<OrderEntity>(OrderEntity) {}
