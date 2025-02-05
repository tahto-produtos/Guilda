import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface ReportNotificationUseCaseProps {
    DataInicial: string;
    DataFinal: string;
    Sectors: number[];
    Destination: number[];
    Site: number[];
    Client: number[];
    Hierarchies: number[];
}

export class ReportNotificationUseCase {
    private client = guildaApiClient2;

    async handle(props: ReportNotificationUseCaseProps) {
        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/ReportNotification`,
            props
        );

        return data;
    }
}
