import { guildaApiClient } from "src/services";

export class ListSupplierUseCase {
    private client = guildaApiClient;

    async handle() {
        const { data } = await this.client.get(`/Supplier`);

        return data;
    }
}
