import { guildaApiClient } from "src/services";

export class ListSizes {
    private client = guildaApiClient;

    async handle(searchText: string) {
        const { data } = await this.client.get(`/Sizes`);

        return data;
    }
}
