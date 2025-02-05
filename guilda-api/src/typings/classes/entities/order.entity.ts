import { GdaOrder } from '@prisma/client';

export class OrderEntity implements GdaOrder {
  id: number;
  orderById: number;
  releasedById: number | null;
  deliveredById: number | null;
  orderAt: Date | null;
  releasedAt: Date | null;
  deliveredAt: Date | null;
  orderStatusId: number;
  dueAt: Date | null;
  codOrder: string;
  observationOrder: string | null;
  observationDelivered: string | null;
  reasonPurchase: string | null;
  lastUpdatedId: number | null;
  lastUpdatedAt: Date | null;
  createdAt: Date;
  deletedAt: Date | null;
  totalPrice: number;
  idGroup: number | null;
  canceledById: number | null;
  canceledAt: Date | null;

  constructor(data: GdaOrder) {
    this.id = data.id;
    this.totalPrice = data.totalPrice;
    this.orderById = data.orderById;
    this.releasedById = data.releasedById;
    this.deliveredById = data.deliveredById;
    this.orderAt = data.orderAt;
    this.releasedAt = data.releasedAt;
    this.deliveredAt = data.deliveredAt;
    this.orderStatusId = data.orderStatusId;
    this.dueAt = data.dueAt;
    this.codOrder = data.codOrder;
    this.observationOrder = data.observationOrder;
    this.observationDelivered = data.observationDelivered;
    this.reasonPurchase = data.reasonPurchase;
    this.lastUpdatedId = data.lastUpdatedId;
    this.lastUpdatedAt = data.lastUpdatedAt;
    this.createdAt = data.createdAt;
    this.deletedAt = data.deletedAt;
    this.totalPrice = data.totalPrice;
    this.idGroup = data.idGroup;
    this.canceledById = data.canceledById;
    this.canceledAt = data.canceledAt;
  }
}
