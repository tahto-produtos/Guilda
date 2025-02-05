import { Indicator } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';

export class IndicatorEntity implements Indicator {
  @ApiProperty()
  id: number;

  @ApiProperty()
  name: string;

  @ApiProperty()
  description: string;

  @ApiProperty()
  type: string;

  @ApiProperty()
  status: boolean;

  @ApiProperty()
  weight: number;

  @ApiProperty()
  calculationType: string;

  @ApiProperty()
  createdAt: Date;

  @ApiProperty({ nullable: true })
  deletedAt: Date | null;

  constructor(data: Indicator) {
    const {
      id,
      name,
      description,
      createdAt,
      deletedAt,
      type,
      status,
      weight,
      calculationType,
    } = data;
    this.id = id;
    this.name = name;
    this.description = description;
    this.type = type;
    this.status = status;
    this.weight = weight;
    this.calculationType = calculationType;
    this.createdAt = createdAt;
    this.deletedAt = deletedAt;
  }
}
