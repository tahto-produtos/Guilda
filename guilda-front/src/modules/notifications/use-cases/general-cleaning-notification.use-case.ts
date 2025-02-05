import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface DeleteNotificationsUseCaseProps {
    ids: number[];
}

export class GeneralDeleteNotificationsUseCase {
    private client = guildaApiClient2;

    async handle(props: DeleteNotificationsUseCaseProps) {
        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/GeneralCleaningNotification`,
            props
        );

        return data;
    }
}
