import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface DeleteNotificationsUseCaseProps {
    IDGDA_NOTIFICATION: number;
    VALIDADETED: boolean;
}

export class DeleteNotificationsUseCase {
    private client = guildaApiClient2;

    async handle(props: DeleteNotificationsUseCaseProps) {
        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/DeletedNotification`,
            props
        );

        return data;
    }
}
