import { guildaApiClient2 } from "src/services";


export class ListTypeConfigMonetizationUseCase {
    private client = guildaApiClient2;

    async handle() {

        const { data } = await this.client.get(
            `/ListMonetizationConfigType`
        );

        return data;
    }
}
