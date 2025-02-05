import { GdaStock } from '@prisma/client';

export class StockEntity implements GdaStock {
  id: number;
  description: string;
  city: string;
  client: string;
  campaign: string;
  type: string;
  createdAt: Date;
  deletedAt: Date | null;

  constructor(data: GdaStock) {
    this.id = data.id;
    this.description = data.description;
    this.client = data.client;
    this.campaign = data.campaign;
    this.type = data.type;
    this.createdAt = data.createdAt;
    this.deletedAt = data.deletedAt;
  }
}
