import { guildaApiClient, guildaApiClient2 } from "src/services";

export interface ReleaseProductUseCaseProps {
    orderId: number;
    productId: number;
    reason: string;
}

export class UnReleaseProductUseCase {
    private client = guildaApiClient;
    private client2 = guildaApiClient2;

    async handle(props: ReleaseProductUseCaseProps) {
        const { orderId, productId, reason } = props;

        const payload = { reason };

        await this.client2.post(`/RemoveItem`, {
            codOrder: orderId,
            codProduct: productId,
        });

        const { data } = await this.client.put(
            `/order/delete/product/${productId}/${orderId}`,
            payload
        );

        return data;
    }
}
