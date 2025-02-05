import { Product } from "./product.model";

export class Order {
    id!: number;
    codOrder!: string;
    orderById!: number;
    deliveredById!: number | null;
    orderStatusId!: number;
    historyStockProductId!: 1;
    createdAt!: string;
    deletedAt!: null;
    orderStatus!: any;
    historyStockProduct: any;
    receivedBy?: string;
    whoReceived?: string;
    orderBy!: {
        email: string;
        genre: string;
        name: string;
        identification: string;
    };
    releasedAt!: string;
    deliveredAt!: string;
    historyProduct!: Array<{ product: Product; productId: number }>;
    GdaOrderProduct!: [
        {
            SupplierId: number;
            stockId: number;
            deliveryNote: string;
            observationDelivered: string;
            observationCanceled: string;
            orderProductStatus: string;
            observationReleased: string;
            amount: number;
            productId: number;
            product: {
                productImages: { upload: { url: string } }[];
                comercialName: string;
                description: string;
                price: number;
                type: string;
                validity: string;
                vouchers: { id: number; productId: number; stockId: number; voucher: string; voucherValidity: string; }[];
            };
            expireIn?: string;
        }
    ];
    lastUpdatedAt!: string;
    lastUpdatedBy!: {
        name: string;
        identification: string;
    };
    observationOrder?: string;
    expireIn?: string;
    totalPrice?: string;
    observationReleased!: string;
    observationDelivered!: string;
    observationCanceled!: string;
    deliveryNote!: string;
}
