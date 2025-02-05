import { guildaApiClient, guildaApiClient2 } from "src/services";

export interface ExportResultsReportUseCaseProps {
    sectors: { id: number }[];
    groups: { id: number }[];
    indicators: { id: number }[];
    hierarchies: { id: number }[];
    order: string;
    dataInicial?: string;
    dataFinal?: string;
    CollaboratorId: number;
}

export class ExportResultsReportUseCase {
    private client = guildaApiClient2;

    async handle(props: ExportResultsReportUseCaseProps) {
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

        const { data } = await this.client.post(`/ReportHomeResult`, payload);

        return data;
    }
}
