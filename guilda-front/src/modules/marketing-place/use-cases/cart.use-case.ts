import { guildaApiClient } from "src/services";

interface ShoppingCartPutUseCase {
    products: Array<{ stockProductId: number; amount: number }>;
}

export class ShoppingCartUseCase {
    private client = guildaApiClient;

    async read() {
        const { data } = await this.client.get(`/shopping-cart`);
        return data;
    }

    async update(props: ShoppingCartPutUseCase) {
        const { data } = await this.client.put(`/shopping-cart`, props);
    }
}
