import { guildaApiClient } from "src/services";

export class ListCityUseCase {
    private client = guildaApiClient;

    async handle() {
        const { data } = await this.client.get(`/stock/list-city`);

        return data;
    }
}
