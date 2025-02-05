import { guildaApiClient } from "src/services";

export class ListDetails {
    private client = guildaApiClient;

    async handle(searchText: string) {
        const { data } = await this.client.get(`/Details`);

        return data;
    }
}
