import { guildaApiClient, guildaApiClient2 } from "src/services";

export interface ExportMonetizationAdmReportUseCaseProps {
    sectors: { id: number }[];
    groups: { id: number }[];
    indicators: { id: number }[];
    hierarchies: { id: number }[];
    collaborators: { id: number }[];
    order: string;
    dataInicial: string;
    dataFinal: string;
}

export class ExportMonetizationAdmDayReportUseCase {
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

        const { data } = await this.client.post(`/ReportMonthDay`, payload);

        return data;
    }
}
