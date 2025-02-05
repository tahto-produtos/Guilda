import { addDays, format, startOfDay } from "date-fns";
import { guildaApiClient2 } from "src/services";

export interface ListIndicatorsResultsProps {
    collaboratorId: number;
    startDate: string;
    endDate: string;
    sectorId?: number;
}

export class SectorGroupsIndicatorsV2 {
    private client = guildaApiClient2;

    async handle(props: ListIndicatorsResultsProps) {
        const { collaboratorId, startDate, endDate } = props;

        const { data } = await this.client.get(
            `/SectorsGroupsDate?startDate=${startDate}&endDate=${endDate}`
        );

        return data;
    }
}
