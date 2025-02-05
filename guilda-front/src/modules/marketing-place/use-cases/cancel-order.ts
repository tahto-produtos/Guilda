import { guildaApiClient, guildaApiClient2 } from "src/services";

export interface CancelOrderUseCaseProps {
    orderId: number;
    observation: string;
    reason: string;
}

export class CancelOrderUseCase {
    private client = guildaApiClient;
    private client2 = guildaApiClient2;

    async handle(props: CancelOrderUseCaseProps) {
        const { orderId, observation, reason } = props;

        await this.client2.post(`/RemoveItem`, {
            codOrder: orderId,
            codProduct: 0,
        });

        const payload = {
            observation,
            reason,
        };

        const { data } = await this.client.delete(`/order/${orderId}`, {
            data: payload,
        });

        return data;
    }
}
