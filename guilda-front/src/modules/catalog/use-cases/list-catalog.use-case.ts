import { guildaApiClient2 } from "src/services";

export class ListCatalogUseCase {
    private client = guildaApiClient2;

    async handle() {
        const { data } = await this.client.get(`/ListCatalog`);

        return data;
    }
}
