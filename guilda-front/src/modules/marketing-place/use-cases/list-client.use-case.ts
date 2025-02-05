import { guildaApiClient } from "src/services";

export class ListClientUseCase {
    private client = guildaApiClient;

    async handle() {
        const { data } = await this.client.get(`/stock/list-client`);

        return data;
    }
}
