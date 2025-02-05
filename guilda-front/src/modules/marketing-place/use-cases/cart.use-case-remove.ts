import { guildaApiClient, guildaApiClient2 } from "src/services";
import { AxiosResponse } from "axios";

export interface removeCartUseCase {
    products: Array<{ stockProductId: number; amount: number }>;
}

export class ShoppingCartRemoveUseCase {
    private client = guildaApiClient2;

    async handle(props: removeCartUseCase) {

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/RemoveShoppingCart`,
            props
        );

        return data;
    }
}