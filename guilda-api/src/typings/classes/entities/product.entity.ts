import { GdaProduct } from '@prisma/client';

export class GdaProductEntity implements GdaProduct {
  id: number;
  code: string;
  quantity: number;
  comercialName: string;
  description: string;
  price: number;
  registeredBy: number;
  createdAt: Date;
  updatedAt: Date | null;
  deletedAt: Date | null;
  highlight: boolean;
  publicationDate: Date | null;
  expirationDate: Date | null;
  validity: Date | null;
  type: string;
  productStatusId: number;
  saleLimit: number;
  categoryId: number;
  sizeId: number;
  detailId: number;
  groupId: number;
  typeId: number;

  constructor(data: GdaProduct) {
    this.id = data.id;
    this.code = data.code;
    this.quantity = data.quantity;
    this.comercialName = data.comercialName;
    this.description = data.description;
    this.registeredBy = data.registeredBy;
    this.createdAt = data.createdAt;
    this.updatedAt = data.updatedAt;
    this.deletedAt = data.deletedAt;
    this.highlight = data.highlight;
    this.publicationDate = data.publicationDate;
    this.expirationDate = data.expirationDate;
    this.validity = data.validity;
    this.type = data.type;
    this.saleLimit = data.saleLimit;
    this.productStatusId = data.productStatusId;
    this.categoryId = data.categoryId;
    this.sizeId = data.sizeId;
    this.detailId = data.detailId;
    this.typeId = data.typeId;
  }
}
