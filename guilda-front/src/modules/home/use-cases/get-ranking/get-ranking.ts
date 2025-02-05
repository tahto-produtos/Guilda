import { guildaApiClient } from "src/services";

export interface GetRankingProps {
    collaboratorId: number;
    startDate: string;
    endDate: string;
    sectorId: number;
}

export class GetRanking {
    private client = guildaApiClient;

    async handle(props: GetRankingProps) {
        const { collaboratorId, startDate, endDate, sectorId } = props;

        const { data } = await this.client.get(
            `/results/indicators/sectors/ranking?collaboratorId=${collaboratorId}&startDate=${startDate}&endDate=${endDate}&sectorId=${sectorId}`
        );

        return data;
    }
}
