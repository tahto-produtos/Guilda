import { guildaApiClient, guildaApiClient2 } from "src/services";
import {AxiosResponse} from "axios";
import {ReportDayLoggedsModel} from "../../../typings/models/report-day-loggeds.model";

export interface ExportDayLoggedsReportUseCaseProps {
    sectors: number[];
    hierarchies: number[];
    dataInicial?: string;
    dataFinal?: string;
    CollaboratorId: number;
}

export class ExportDayLoggedsReportUseCase {
    private client = guildaApiClient2;

    async handle(props: ExportDayLoggedsReportUseCaseProps) {
        const {
            dataFinal,
            dataInicial,
            hierarchies,
            sectors,
            CollaboratorId,
        } = props;

        const payload = {
            DataInicial: dataInicial,
            DataFinal: dataFinal,
            hierarchies,
            sectors,
            CollaboratorId,
        };

        const { data } = await this.client.post<unknown, AxiosResponse<ReportDayLoggedsModel[]>>(`/ReportAcessFee`, payload);

        return data;
    }
}
