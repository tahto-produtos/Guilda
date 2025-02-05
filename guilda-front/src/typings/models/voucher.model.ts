export interface Voucher {
    id: number;
    collaboratorId: number;
    voucherId: number;
    createdAt: string;
    voucher: {
        id: number;
        productId: number;
        voucher: string;
        status: string;
        product: {
            comercialName: string;
            description: string;
            expirationDate: string;
            validity: string;
        };
        voucherValidity: string;
    };
    reasonPurchase: string;
}
