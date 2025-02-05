import { guildaApiClient } from "src/services";

export class DeleteTypes {
    private client = guildaApiClient;

    async handle(id: number) {
        const { data } = await this.client.delete(`/types/${id}`);

        return data;
    }
}
