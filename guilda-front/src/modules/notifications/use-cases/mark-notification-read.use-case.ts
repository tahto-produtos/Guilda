import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface DeleteNotificationsUseCaseProps {
    ids: number[];
}

export class MarkNotificationReadUseCase {
    private client = guildaApiClient2;

    async handle(props: DeleteNotificationsUseCaseProps) {
        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/MarkNotificationRead`,
            props
        );

        return data;
    }
}
