export class StockProduct {
    id!: number;
    product!: {
        comercialName: string | number | undefined;
        code: string;
        collaboratorId: number;
        createdAt: string | null | Date;
        deletedAt: string | null | Date;
        description: string;
        id: number;
        price: number | string;
        productId: number;
        productImages: Array<{
            id: number;
            upload: {
                url: string;
            };
        }>;
        productsStatus: {
            id: number;
            status: string;
        };
        saleLimit: number;
    };
    amount!: number;
}
