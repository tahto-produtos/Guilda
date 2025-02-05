import { guildaApiClient } from "src/services";

export interface DetailsStockUseCaseProps {
    id: string | string[];
}

export class DetailsStockUseCase {
    private client = guildaApiClient;

    async handle(props: DetailsStockUseCaseProps) {
        const { id } = props;

        const { data } = await this.client.get(`/stock/${id}`);

        return data;
    }
}
