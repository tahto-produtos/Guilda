import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";
import {
    CreateIndicatorRequestDto,
    CreateIndicatorResponseDto,
} from "../forms/create-indicator/dto";

interface ReportSectorIndicatorConnectUseCaseProps {
    limit: number;
    offset: number;
}

export class ReportSectorIndicatorConnectUseCase {
    private client = guildaApiClient;

    async handle(props: ReportSectorIndicatorConnectUseCaseProps) {
        const { data } = await this.client.post<unknown, AxiosResponse>(
            "/sectors/report",
            props
        );

        return data;
    }
}
