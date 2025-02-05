import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export class UpdateBasketIndicatorSector {
    private client = guildaApiClient2;

    async handle(
        codSector: string,
        codIndicator: string,
        weightIndicator: string
    ) {
        const payload = {
            codSector,
            codIndicator,
            weightIndicator,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/BasketIndicatorSector`,
            payload
        );

        return data;
    }
}
