import { guildaApiClient } from "src/services";

export class ListProductGroup {
    private client = guildaApiClient;

    async handle(searchText: string) {
        const { data } = await this.client.get(`/ProductGroup`);
        // const { data } = await this.client.get(
        //     `/ProductGroup?searchText=${searchText}`
        // );

        return data;
    }
}
