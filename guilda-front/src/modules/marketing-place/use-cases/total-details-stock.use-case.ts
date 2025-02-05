import { guildaApiClient } from "src/services";

export interface TotalDetailsStockUseCaseProps {
    limit: number;
    offset: number;
}

export class TotalDetailsStockUseCase {
    private client = guildaApiClient;

    async handle(props: TotalDetailsStockUseCaseProps) {
        const { limit, offset } = props;

        const { data } = await this.client.get(
            `/stock/total-stock-details?limit=${limit}&offset=${offset}`
        );

        return data;
    }
}
