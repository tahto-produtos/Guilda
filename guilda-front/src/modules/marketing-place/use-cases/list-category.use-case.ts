import { guildaApiClient } from "src/services";

export class ListCategoryUseCase {
    private client = guildaApiClient;

    async handle() {
        const { data } = await this.client.get(`/Category`);

        return data;
    }
}
