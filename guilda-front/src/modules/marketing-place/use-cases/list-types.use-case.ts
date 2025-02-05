import { guildaApiClient } from "src/services";

export class ListTypes {
    private client = guildaApiClient;

    async handle(searchText: string) {
        const { data } = await this.client.get(`/types`);

        return data;
    }
}
