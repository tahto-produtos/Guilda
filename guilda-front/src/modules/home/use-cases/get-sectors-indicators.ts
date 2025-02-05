import { addDays, format, startOfDay } from "date-fns";
import { guildaApiClient } from "src/services";

export interface ListIndicatorsResultsProps {
    collaboratorId: number;
    startDate: string;
    endDate: string;
    sectorId?: number;
}

export class SectorIndicators {
    private client = guildaApiClient;

    async handle(props: ListIndicatorsResultsProps) {
        const { collaboratorId, startDate, endDate } = props;

        const { data } = await this.client.get(
            `/results/sectors?${[collaboratorId]
                .map((n, index) => `collaboratorsIds[${index}]=${n}`)
                .join("&")}&startDate=${startDate}&endDate=${endDate}`
        );

        return data;
    }
}
