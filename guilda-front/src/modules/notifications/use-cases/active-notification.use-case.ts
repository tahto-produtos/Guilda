import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface ActiveNotificationsUseCaseProps {
    NOTIFICATION_ID: number;
    ACTIVATED: number;
    STARTED_AT: string;
    ENDED_AT: string;
}

export class ActiveNotificationsUseCase {
    private client = guildaApiClient2;

    async handle(props: ActiveNotificationsUseCaseProps) {
        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/ActivatedNotification`,
            props
        );

        return data;
    }
}
