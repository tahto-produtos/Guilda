import { paginate } from '../../../utils';
import { CollaboratorEntity } from '~entities/collaborator.entity';

export class CollaboratorPagination extends paginate<CollaboratorEntity>(
  CollaboratorEntity,
) {}
