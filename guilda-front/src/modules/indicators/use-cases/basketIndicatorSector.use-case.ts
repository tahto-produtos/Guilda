import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export class BasketIndicatorSector {
    private client = guildaApiClient2;

    async handle(sectorId: number) {
        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/BasketIndicatorSector?codSetor=${sectorId}`
        );

        return data;
    }
}
