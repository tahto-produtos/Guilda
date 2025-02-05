import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export class FilterMyNotificationUseCase {
    private client = guildaApiClient2;

    async handle() {
        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/FilterMyNotification`
        );

        return data;
    }
}
