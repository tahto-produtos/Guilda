import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface ListIndicatorsByScoreUseCaseProps {
    SECTORID: number;
}

export class ListIndicatorsByScoreUseCase {
    private client = guildaApiClient2;

    async handle(props: ListIndicatorsByScoreUseCaseProps) {
        const { SECTORID } = props;

        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/ListIndicatorsByScore?SECTORID=${SECTORID}`
        );

        return data;
    }
}
