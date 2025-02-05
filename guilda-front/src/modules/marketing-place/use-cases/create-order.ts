import { guildaApiClient, guildaApiClient2 } from "src/services";
import { Order } from "src/typings/models/order.model";

export interface CreateOrderUseCaseProps {
    whoReceived?: string | null;
    supplierId?: number;
    userGroup?: number;
}

export class CreateOrderUseCase {
    private client = guildaApiClient;
    private client2 = guildaApiClient2;

    async handle(props: CreateOrderUseCaseProps) {
        const { whoReceived: observationOrder, supplierId, userGroup } = props;

        const payload = {
            observationOrder,
            supplierId: 0,
            userGroup,
        };

        const { data } = await this.client2.post<Order>(`/buyItem`, payload);

        //const { data } = await this.client.post<FormData>("/order", payload);

        return data;
    }
}
