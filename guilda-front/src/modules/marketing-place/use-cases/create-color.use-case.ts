import { guildaApiClient } from "src/services";

export class CreateColor {
    private client = guildaApiClient;

    async handle(color: string) {
        const payload = {
            color,
        };

        const { data } = await this.client.post(`/ProductColor`, payload);

        return data;
    }
}
