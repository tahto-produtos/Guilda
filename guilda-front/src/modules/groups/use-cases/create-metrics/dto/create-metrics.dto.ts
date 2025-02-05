export class CreateMetricsDto {
    sectorId?: number;
    indicatorsIds!: number[];
    metricSettings!: {
        groupId: number;
        metricMin: number;

        // metricMax: number;
    }[];
}
