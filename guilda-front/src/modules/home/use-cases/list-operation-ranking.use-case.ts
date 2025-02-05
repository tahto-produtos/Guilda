import { guildaApiClient2 } from "src/services";

export interface ListOperationRankingUseCaseProps {
    sectors: { id: number }[];
    groups: { id: number }[];
    indicators: { id: number }[];
    hierarchies: { id: number }[];
    order: string;
    dataInicial?: string;
    dataFinal?: string;
    CollaboratorId: number;
}

export class ListOperationRankingUseCase {
    private client = guildaApiClient2;

    async handle(props: ListOperationRankingUseCaseProps) {
        const {
            dataFinal,
            dataInicial,
            groups,
            hierarchies,
            indicators,
            order,
            sectors,
            CollaboratorId,
        } = props;

        const payload = {
            dataFinal,
            dataInicial,
            groups,
            hierarchies,
            indicators,
            order,
            sectors,
            CollaboratorId,
        };

        const { data } = await this.client.post(`/RankingOperacao`, payload);

        return data;
    }
}
