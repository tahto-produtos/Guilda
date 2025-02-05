import { addDays, format, startOfDay } from "date-fns";
import { guildaApiClient } from "src/services";

export interface ListIndicatorsResultsProps {
    collaboratorId: number;
    startDate: string;
    endDate: string;
    sectorId: number;
}

export class ResultsIndicatorsSectors {
    private client = guildaApiClient;

    async handle(props: ListIndicatorsResultsProps) {
        const { collaboratorId, startDate, endDate, sectorId } = props;

        const { data } = await this.client.get(
            `/results/indicators/sectors/new?${[collaboratorId]
                .map((n, index) => `collaboratorsIds[${index}]=${n}`)
                .join(
                    "&"
                )}&startDate=${startDate}&endDate=${endDate}&sectorId=${sectorId}`
        );

        return data;
    }
}
