import { guildaApiClient2 } from "src/services";
import { AxiosResponse } from "axios";

export class MaxValueProduct {
    private client = guildaApiClient2;

    async handle() {

        const { data } = await this.client.get<
        unknown,
        AxiosResponse<{ valueProduct: number }>
        >(`/ProductMaxValue`);
        
        return data;
    }
}
