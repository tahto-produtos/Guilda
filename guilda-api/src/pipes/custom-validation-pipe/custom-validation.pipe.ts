import { ArgumentMetadata, Injectable, ValidationPipe } from '@nestjs/common';
import { REWRITE_METADATA_OPTIONS_KEY } from '../../constants';

@Injectable()
export class CustomValidationPipe extends ValidationPipe {
  async transform(value: any, metadata: ArgumentMetadata) {
    const options = Reflect.getMetadata(
      REWRITE_METADATA_OPTIONS_KEY,
      metadata.metatype,
    );
    let originOptions;
    if (options) {
      originOptions = Object.assign({}, this.validatorOptions);
      this.validatorOptions = Object.assign(this.validatorOptions, options);
    }
    try {
      const result = super.transform(value, metadata);
      if (originOptions) {
        this.validatorOptions = originOptions;
      }
      return result;
    } catch (error) {
      throw error;
    }
  }
}
