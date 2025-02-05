import { guildaApiClient } from "src/services";

export interface DetailsProductUseCaseProps {
    id: string | string[];
}

export class DetailsProductUseCase {
    private client = guildaApiClient;

    async handle(props: DetailsProductUseCaseProps) {
        const { id } = props;

        const { data } = await this.client.get(`/product/${id}`);

        return data;
    }
}
