import { guildaApiClient } from "src/services";

export class DeleteDetails {
    private client = guildaApiClient;

    async handle(id: number) {
        const { data } = await this.client.delete(`/Details/${id}`);

        return data;
    }
}
