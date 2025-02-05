import { paginate } from '../../../utils';
import { ProfileCollaboratorAdministrationEntity } from '~entities/profiles-collaborators-administration.entity';

export class ProfileCollaboratorAdministrationPagination extends paginate<ProfileCollaboratorAdministrationEntity>(
  ProfileCollaboratorAdministrationEntity,
) {}
