import { guildaApiClient, guildaApiClient2 } from "src/services";

export interface ExportMonetizationAdmConsolidadoReportUseCaseProps {
    sectors: { id: number }[];
    hierarchies: { id: number }[];
    order: string;
    dataInicial: string;
    dataFinal: string;
    collaborators: { id: number }[];
}

export class ExportMonetizationAdmConsolidadoReportUseCase {
    private client = guildaApiClient2;

    async handle(props: ExportMonetizationAdmConsolidadoReportUseCaseProps) {
        const {
            dataFinal,
            dataInicial,
            hierarchies,
            order,
            sectors,
            collaborators,
        } = props;

        const payload = {
            dataFinal,
            dataInicial,
            hierarchies,
            order,
            sectors,
            collaborators,
        };

        const { data } = await this.client.post(
            `/ReportMonthConsolidated`,
            payload
        );

        return data;
    }
}
