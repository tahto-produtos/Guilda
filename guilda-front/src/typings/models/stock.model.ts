import { Product } from "./product.model";

export class Stock {
    id!: number;
    description!: string;
    value!: string;
    typeId!: number;
    type?: string;
    campaign?: string;
    createdAt?: string | null;
    deletedAt?: string | null;
    GdaStockProduct!: Product[];
    totalAmount!: number;
    stock?: {
        [key: string]: string | number 
    };
    amount?: number;
}
