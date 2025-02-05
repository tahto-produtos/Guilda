import { guildaApiClient, guildaApiClient2 } from "src/services";

export interface ConfirmOrderProductUseCaseProps {
    orderId: number;
    observationText: string;
    productId: number;
    receivedBy: string;
}
export class ConfirmOrderProductUseCase {
    private client = guildaApiClient2;

    async handle(props: ConfirmOrderProductUseCaseProps) {
        const { orderId, observationText, productId, receivedBy } = props;

        const payload = {
            orderId,
            productId,
            observationText,
            receivedBy,
        };

        const { data } = await this.client.post(
            `/OrderProductConfirm`,
            payload
        );

        return data;
    }
}
/* export class ConfirmOrderProductUseCase {
    private client = guildaApiClient;

    async handle(props: ConfirmOrderProductUseCaseProps) {
        const { orderId, observation, productId, receivedBy } = props;

        const payload = {
            observation,
            receivedBy,
        };

        const { data } = await this.client.put(
            `/order/confirm/product/${productId}/${orderId}`,
            payload
        );

        return data;
    }
} */
