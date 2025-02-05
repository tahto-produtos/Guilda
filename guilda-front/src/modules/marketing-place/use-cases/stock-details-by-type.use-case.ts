import { guildaApiClient } from "src/services";

export interface StockDetailsByTypeUseCaseProps {
    limit: number;
    offset: number;
    type?: string | null;
}

export class StockDetailsByTypeUseCase {
    private client = guildaApiClient;

    async handle(props: StockDetailsByTypeUseCaseProps) {
        const { limit, offset, type } = props;

        const { data } = await this.client.get(
            `/stock/stock-details-by-type-new?limit=${limit}&offset=${offset}${
                type ? `&type=${type}` : ""
            }`
        );

        return data;
    }
}
