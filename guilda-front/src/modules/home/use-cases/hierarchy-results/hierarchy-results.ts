import { guildaApiClient } from "src/services";

export interface HierarchyResultsProps {
    startDate: string;
    endDate: string;
}

export class HierarchyResults {
    private client = guildaApiClient;

    async handle(props: HierarchyResultsProps) {
        const { startDate, endDate } = props;

        const { data } = await this.client.get(
            `/results/hierarchy?startDate=${startDate}&endDate=${endDate}`
        );

        return data;
    }
}
