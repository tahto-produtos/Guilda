import { guildaApiClient } from "src/services";

export class DeleteColors {
    private client = guildaApiClient;

    async handle(id: number) {
        const { data } = await this.client.delete(`/ProductColor/${id}`);

        return data;
    }
}
