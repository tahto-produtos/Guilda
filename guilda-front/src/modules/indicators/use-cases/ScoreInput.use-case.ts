import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface ScoreInputUseCaseProps {
    Sectorid: number;
    Matricula: number;
    Indicators: { id: number; score: number }[];
}

export class ScoreInputUseCase {
    private client = guildaApiClient2;

    async handle(props: ScoreInputUseCaseProps) {
        const { Indicators, Matricula, Sectorid } = props;

        const payload = {
            Sectorid: Sectorid,
            Matricula: Matricula,
            Indicators: Indicators,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/ScoreInput`,
            payload
        );

        return data;
    }
}
