import { guildaApiClient } from "src/services";

export class DeleteProductGroup {
    private client = guildaApiClient;

    async handle(id: number) {
        const { data } = await this.client.delete(`/ProductGroup/${id}`);

        return data;
    }
}
