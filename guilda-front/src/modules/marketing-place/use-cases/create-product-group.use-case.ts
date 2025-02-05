import { guildaApiClient } from "src/services";

export class CreateProductGroup {
    private client = guildaApiClient;

    async handle(groupName: string) {
        const payload = {
            groupName,
        };

        const { data } = await this.client.post(`/ProductGroup`, payload);

        return data;
    }
}
