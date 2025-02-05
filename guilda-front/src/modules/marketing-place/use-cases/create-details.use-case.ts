import { guildaApiClient } from "src/services";

export class CreateDetails {
    private client = guildaApiClient;

    async handle(detail: string) {
        const payload = {
            detail,
        };

        const { data } = await this.client.post(`/Details`, payload);

        return data;
    }
}
