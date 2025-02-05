import { guildaApiClient } from "src/services";

export interface CreateStockUseCaseProps {
    description: string;
    city: string;
    client: string;
    campaign: string;
}

export class CreateStockUseCase {
    private client = guildaApiClient;

    async handle(props: CreateStockUseCaseProps) {
        const { data } = await this.client.post("/stock", props);

        return data;
    }
}
