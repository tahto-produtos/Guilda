import { guildaApiClient } from "src/services";

export interface CreateStockTypeUseCaseProps {
    type: string;
}

export class CreateStockTypeUseCase {
    private client = guildaApiClient;

    async handle(props: CreateStockTypeUseCaseProps) {
        const { type } = props;

        const payload = {
            type: type,
        };

        const { data } = await this.client.post("/type", payload);

        return data;
    }
}
