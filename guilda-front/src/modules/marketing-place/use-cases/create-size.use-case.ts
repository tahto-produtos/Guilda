import { guildaApiClient } from "src/services";

export class CreateSizes {
    private client = guildaApiClient;

    async handle(size: string) {
        const payload = {
            size,
        };

        const { data } = await this.client.post(`/Sizes`, payload);

        return data;
    }
}
