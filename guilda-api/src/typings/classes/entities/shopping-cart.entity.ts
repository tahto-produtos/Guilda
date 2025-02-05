import { ShoppingCart } from '@prisma/client';

export class ShoppingCartEntity implements ShoppingCart {
  id: number;
  orderedById: number;
  amount: number;
  stockProductId: number;

  constructor(data: ShoppingCart) {
    this.id = data.id;
    this.orderedById = data.orderedById;
    this.amount = data.amount;
    this.stockProductId = data.stockProductId;
  }
}
