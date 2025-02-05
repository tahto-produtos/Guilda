import { guildaApiClient, guildaApiClient2 } from "src/services";

export interface ExportMonetizationAdmReportUseCaseProps {
    sectors: { id: number }[];
    groups: { id: number }[];
    indicators: { id: number }[];
    hierarchies: { id: number }[];
    order: string;
    collaborators: { id: number }[];
    dataInicial: string;
    dataFinal: string;
}

export class ExportMonetizationAdmReportUseCase {
    private client = guildaApiClient2;

    async handle(props: ExportMonetizationAdmReportUseCaseProps) {
        const {
            dataFinal,
            dataInicial,
            groups,
            hierarchies,
            indicators,
            order,
            sectors,
            collaborators,
        } = props;

        const payload = {
            dataFinal,
            dataInicial,
            groups,
            hierarchies,
            indicators,
            order,
            sectors,
            collaborators,
        };

        const { data } = await this.client.post(`/ReportMonthAdm`, payload);

        return data;
    }
}
