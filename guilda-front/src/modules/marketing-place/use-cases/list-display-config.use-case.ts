import { guildaApiClient2 } from "src/services";

export class ListDisplayConfigUseCase {
    private client = guildaApiClient2;

    async handle(idConfig?: number | string) {
        const { data } = await this.client.get(
            `/ListDisplayConfig?idConfig=${idConfig ? idConfig : ""}`
        );

        return data;
    }
}
