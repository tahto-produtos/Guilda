import { guildaApiClient2 } from "src/services";

export class DeleteDisplayConfig {
    private client = guildaApiClient2;

    async handle(displayId: number) {
        const { data } = await this.client.delete(
            `/DisplayConfig?displayId=${displayId}`
        );

        return data;
    }
}
