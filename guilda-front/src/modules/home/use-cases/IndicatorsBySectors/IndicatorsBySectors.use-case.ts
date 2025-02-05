import { guildaApiClient, guildaApiClient2 } from "src/services";

export interface IndicatorsBySectorsUseCaseProps {
    sectors: { id: number }[];
    monetize?: boolean;
    dtInicial: string;
    dtfinal: string;
}

export class IndicatorsBySectorsUseCase {
    private client = guildaApiClient2;

    async handle(props: IndicatorsBySectorsUseCaseProps) {
        const { sectors, monetize, dtInicial, dtfinal } = props;

        const payload = {
            sectors,
            monetize: monetize ? monetize : false,
            dtInicial,
            dtfinal,
        };

        const { data } = await this.client.post(
            `/IndicatorsBySectors`,
            payload
        );

        return data;
    }
}
