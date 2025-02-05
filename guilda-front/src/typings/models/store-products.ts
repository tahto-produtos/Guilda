export interface StoreProduct {
    GdaStockProduct: { id: number }[];
    code: string;
    comercialName: string;
    createdAt: string;
    description: string;
    id: number;
    price: number;
    productImages: Array<{
        id: number;
        upload: {
            url: string;
        };
    }>;
    quantity: number;
    saleLimit: number;
    type: string;
}
