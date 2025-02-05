import { guildaApiClient } from "src/services";

export class CreateTypes {
    private client = guildaApiClient;

    async handle(type: string) {
        const payload = {
            type,
        };

        const { data } = await this.client.post(`/types`, payload);

        return data;
    }
}
