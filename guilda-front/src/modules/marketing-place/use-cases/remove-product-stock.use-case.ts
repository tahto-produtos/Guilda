import { guildaApiClient } from "src/services";

export interface RemoveProductFromStockUseCaseProps {
    productId: number;
    stockId: number;
    supplierId: number;
    reason: string;
}

export class RemoveProductFromStockUseCase {
    private client = guildaApiClient;

    async handle(props: RemoveProductFromStockUseCaseProps) {
        const { productId, stockId, supplierId, reason } = props;

        const payload = {
            reason,
        };

        const { data } = await this.client.post(
            `/order/${productId}/${stockId}/${supplierId}/remove/product`,
            payload
        );

        return data;
    }
}
