import { StockProduct } from "./stock-product.model";

export interface CartDataModel {
    amount: number;
    id: number;
    orderedById: number;
    stockProductId: number;
    stockProduct: StockProduct;
}
