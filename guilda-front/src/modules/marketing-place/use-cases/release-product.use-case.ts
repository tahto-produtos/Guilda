import { guildaApiClient, guildaApiClient2 } from "src/services";
import { AxiosResponse } from "axios";

export interface ReleaseProductUseCaseProps {
    orderId: number;
    productId: number;
    reason: string;
}

export class ReleaseProductUseCase {
    private client = guildaApiClient;
    private client2 = guildaApiClient2;

    async handle(props: ReleaseProductUseCaseProps) {
        const { orderId, productId, reason } = props;

        const payload = { reason };

        const { data } = await this.client.put(
            `/order/release/product/${productId}/${orderId}`,
            payload
        );

        const payload2 = { orderId, productId};

        await this.client2.post<unknown, AxiosResponse>(
            `/ReleasedItem`,
            payload2
        );

        return data;
    }
}
