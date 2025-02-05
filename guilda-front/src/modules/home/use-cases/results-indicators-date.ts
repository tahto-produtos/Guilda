import { addDays, format } from "date-fns";
import { guildaApiClient } from "src/services";

export interface ResultsIndicatorsDateProps {
    startDate: string;
    endDate: string;
    groupBy: string;
}

export class ResultsIndicatorsDate {
    private client = guildaApiClient;

    async handle(props: ResultsIndicatorsDateProps) {
        const { groupBy, startDate, endDate } = props;

        const { data } = await this.client.get(
            `/results/indicators/date?startDate=${startDate}&endDate=${endDate}&groupBy=${groupBy}`
        );

        return data;
    }
}
