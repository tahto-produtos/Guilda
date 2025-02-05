import { ApiProperty } from '@nestjs/swagger';
import { Exclude, Expose } from 'class-transformer';
import { IsInt } from 'class-validator';

import { TransformInteger } from '../../decorators/transform-integer';

@Exclude()
export abstract class PaginationDto {
  @Expose()
  @ApiProperty()
  @IsInt()
  @TransformInteger()
  limit: number;

  @Expose()
  @ApiProperty()
  @IsInt()
  @TransformInteger()
  offset: number;
}
