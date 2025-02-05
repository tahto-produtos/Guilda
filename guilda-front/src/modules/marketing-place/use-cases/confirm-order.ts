import { guildaApiClient, guildaApiClient2 } from "src/services";

export interface ConfirmOrderUseCaseProps {
    orderId: number;
    observationText: string;
    receivedBy: string;
}
export class ConfirmOrderUseCase {
    private client2 = guildaApiClient2;

    async handle(props: ConfirmOrderUseCaseProps) {
        const { orderId, observationText, receivedBy } = props;

        const payload = {
            orderId: orderId,
            observationText: observationText,
            receivedBy: receivedBy,
        };

        const { data } = await this.client2.post(
            `/OrderConfirm`,
            payload
        );

        return data;
    }
}
/* export class ConfirmOrderUseCase {
    private client = guildaApiClient;

    async handle(props: ConfirmOrderUseCaseProps) {
        const { orderId, observation, observationText } = props;

        const payload = {
            observation: observationText,
            receivedBy: observation,
        };

        const { data } = await this.client.post(
            `/order/${orderId}/confirm`,
            payload
        );

        return data;
    }
} */
