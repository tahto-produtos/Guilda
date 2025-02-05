import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";
import GoalsBySectorModel from "src/typings/models/goals-by-sector.model";

export class ConnectIndicatorstoSector {
    private client = guildaApiClient;

    async handle(props: { sectorId: number; indicatorsIds: number[] }) {
        const { sectorId, indicatorsIds } = props;

        const payload = {
            indicatorsIds: indicatorsIds,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/sectors/${sectorId}/indicator/`,
            payload
        );

        return data;
    }
}
