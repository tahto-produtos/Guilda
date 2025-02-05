import { addDays, format } from "date-fns";
import { guildaApiClient, guildaApiClient2 } from "src/services";

export interface ListFinancialSummaryUseCaseProps {
    startDate: string;
    endDate: string;
}

export class ListFinancialSummaryUseCase {
    private client = guildaApiClient2;

    async handle(props: ListFinancialSummaryUseCaseProps) {
        const { startDate, endDate } = props;

        const payload = {
            dataInicial: startDate,
            dataFinal: endDate,
        };

        const { data } = await this.client.post(`/FinancialSummary`, payload);

        return data;
    }
}
