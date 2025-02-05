import { guildaApiClient, guildaApiClient2 } from "src/services";

export class ListHolidayUseCase {
    private client = guildaApiClient;

    async handle() {
        const { data } = await this.client.get(`/Holidays`);

        return data;
    }
}
