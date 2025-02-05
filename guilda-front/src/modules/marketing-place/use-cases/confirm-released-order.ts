import { guildaApiClient, guildaApiClient2 } from "src/services";
import { AxiosResponse } from "axios";

export interface ConfirmReleasedOrderUseCaseProps {
    orderId: number;
    observation: string;
}

export class ConfirmReleasedOrderUseCase {
    private client = guildaApiClient;
    private client2 = guildaApiClient2;
    async handle(props: ConfirmReleasedOrderUseCaseProps) {
        const { orderId, observation } = props;

        const payload = { observation };
       
        const { data } = await this.client.post(
            `/order/${orderId}/released/confirm`,
            payload
        );
        const productId = 0;
        const payload2 = { orderId, productId};

        await this.client2.post<unknown, AxiosResponse>(
            `/ReleasedItem`,
            payload2
        );

        return data;
    }
}
