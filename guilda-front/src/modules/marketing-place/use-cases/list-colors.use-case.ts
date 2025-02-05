import { guildaApiClient } from "src/services";

export class ListColors {
    private client = guildaApiClient;

    async handle(searchText: string) {
        const { data } = await this.client.get(`/ProductColor`);

        return data;
    }
}
