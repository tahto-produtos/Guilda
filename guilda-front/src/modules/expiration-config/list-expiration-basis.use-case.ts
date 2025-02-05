import { guildaApiClient2 } from "src/services";

export class ListExpirationBasisUseCase {
    private client = guildaApiClient2;

    async handle() {
        const { data } = await this.client.get(`/ExpirationBasis`);

        return data;
    }
}
