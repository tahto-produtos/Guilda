export interface StockReport {
    id: number;
    name: string;
    type: string;
    code: number;
    status: string;
    productPrice: number;
    totalCost: number;
    inputs: {
        stockName: string;
        amount: number;
    }[];
    outputsBySales: {
        stockName: string;
        amount: number;
    }[];
    generalOutputs: {
        stockName: string;
        amount: number;
    }[];
}
