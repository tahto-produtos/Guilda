import { guildaApiClient } from "src/services";

export interface GenerateStockReportUseCaseProps {
    startDate: string;
    endDate: string;
    productId?: number;
    stock?: number;
    quantity?: number;
    bestSeller?: boolean;
    lessSold?: boolean;
    city?: string;
    sector?: number;
    type?: string;
    status?: string;
}

export class GenerateStockReportUseCase {
    private client = guildaApiClient;

    async handle(props: GenerateStockReportUseCaseProps) {
        const {
            startDate,
            endDate,
            productId,
            quantity,
            stock,
            bestSeller,
            city,
            lessSold,
            sector,
            type,
            status,
        } = props;

        const payload = {
            startDate,
            endDate,
            productId,
            quantity,
            stock,
            bestSeller,
            city: city ? city : undefined,
            sector,
            lessSold,
            type: type ? type : undefined,
            status: status ? status : undefined,
        };

        const { data } = await this.client.post("/stock/report", payload);

        return data;
    }
}
