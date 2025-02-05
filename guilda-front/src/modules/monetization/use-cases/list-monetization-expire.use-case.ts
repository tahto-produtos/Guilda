import { guildaApiClient2 } from "src/services";


export class ListMonetizationExpireUseCase {
    private client = guildaApiClient2;

    async handle() {

        const payload = {
            limit: 1000,
            page: 1
        };

        const { data } = await this.client.post(
            `/ListMonetizationExpired`,
            payload
        );

        return data;
    }
}
