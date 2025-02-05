import { format, addDays } from "date-fns";
import { guildaApiClient } from "src/services";

export interface ListIndicatorsResultsProps {
    collaboratorId: number;
    startDate: string;
    endDate: string;
}

export class ListIndicatorsResults {
    private client = guildaApiClient;

    async handle(props: ListIndicatorsResultsProps) {
        const { collaboratorId, startDate, endDate } = props;

        const endDateFormated = format(
            addDays(new Date(endDate), 1),
            "yyyy-MM-dd"
        );
        const startDateFormated = format(new Date(startDate), "yyyy-MM-dd");

        const { data } = await this.client.get(
            `/results/indicators?collaboratorId=${collaboratorId}&startDate=${startDateFormated}&endDate=${endDateFormated}`
        );

        return data;
    }
}
