import { CollaboratorVoucher } from '@prisma/client';

export class CollaboratorVoucherEntity implements CollaboratorVoucher {
  id: number;
  collaboratorId: number;
  voucherId: number;
  orderId: number;
  createdAt: Date;

  constructor(data: CollaboratorVoucher) {
    this.id = data.id;
    this.collaboratorId = data.collaboratorId;
    this.voucherId = data.voucherId;
    this.orderId = data.orderId;
    this.createdAt = data.createdAt;
  }
}
