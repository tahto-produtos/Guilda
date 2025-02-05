import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface SendNotificationUseCaseProps {
    NOTIFICATION_ID: number;
}

export class SendNotificationUseCase {
    private client = guildaApiClient2;

    async handle(props: SendNotificationUseCaseProps) {
        const { NOTIFICATION_ID } = props;

        const payload = {
            NOTIFICATION_ID,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/SendNotification`,
            payload
        );

        return data;
    }
}
