import { Type } from '@nestjs/common';
import { ApiProperty } from '@nestjs/swagger';

export interface PaginationMetadata<T> {
  items: Array<T>;
  limit: number;
  offset: number;
  totalItems: number;
  totalPages: number;
}

export function paginate<T>(type: Type) {
  class PaginatedEntity implements PaginationMetadata<T> {
    @ApiProperty({ type: [type] })
    items: Array<T>;

    @ApiProperty()
    offset: number;

    @ApiProperty()
    limit: number;

    @ApiProperty()
    totalItems: number;

    @ApiProperty()
    totalPages: number;

    constructor(metadata: Omit<PaginationMetadata<T>, 'totalPages'>) {
      const { items, limit, offset, totalItems } = metadata;
      this.items = items;
      this.limit = limit;
      this.offset = offset;
      this.totalItems = totalItems;
      this.totalPages = PaginatedEntity.getTotalPages(limit, totalItems);
    }

    static getTotalPages(limit: number, totalItems: number) {
      return Math.ceil(totalItems / limit);
    }
  }

  return PaginatedEntity;
}
