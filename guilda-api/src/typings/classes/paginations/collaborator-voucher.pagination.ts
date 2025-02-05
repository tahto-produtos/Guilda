import { paginate } from '../../../utils';
import { CollaboratorVoucherEntity } from '~entities/collaborator-voucher.entity';

export class CollaboratorVoucherPagination extends paginate<CollaboratorVoucherEntity>(
  CollaboratorVoucherEntity,
) {}
