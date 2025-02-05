import { guildaApiClient } from "src/services";

export class DeleteSizes {
    private client = guildaApiClient;

    async handle(id: number) {
        const { data } = await this.client.delete(`/Sizes/${id}`);

        return data;
    }
}
