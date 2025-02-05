export class Product {
    code!: string;
    collaboratorId!: number;
    createdAt!: string | null | Date;
    deletedAt!: string | null | Date;
    description!: string;
    id!: number;
    price?: number | string;
    productId?: number;
    productImages!: Array<{
        id: number;
        upload: {
            url: string;
        };
    }>;
    amount!: number;
    comercialName!: string;
    type!: string;
    totalAmount?: number;
    qrCode!: string;
}
