import { SetMetadata } from '@nestjs/common';
import { ValidatorOptions } from 'class-validator';

import { REWRITE_METADATA_OPTIONS_KEY } from '../../constants';

export function RewriteValidationOptions(options: ValidatorOptions) {
  return SetMetadata(REWRITE_METADATA_OPTIONS_KEY, options);
}
