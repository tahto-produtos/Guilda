import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface CreateNotificationUseCaseProps {
    NOTIFICATION_ID: number;
    TITLE: string;
    NOTIFICATION: string;
    ACTIVATED: 0 | 1;
    STARTED_AT: string;
    ENDED_AT: string;
}

export class EditNotificationUseCase {
    private client = guildaApiClient2;

    async handle(props: CreateNotificationUseCaseProps) {
        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/EditNotification`,
            props
        );

        return data;
    }
}
