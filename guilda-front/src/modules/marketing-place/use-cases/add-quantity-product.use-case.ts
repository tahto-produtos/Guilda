import { guildaApiClient } from "src/services";

export interface AddQntProductUseCaseProps {
    productId: number;
    stockId: number;
    quantity: number;
    vouchers?: string[];
    validtyDate?: string | null;
}

export class AddQntProductUseCase {
    private client = guildaApiClient;

    async handle(props: AddQntProductUseCaseProps) {
        const { productId, quantity, stockId, vouchers, validtyDate } = props;

        const payload = {
            productId,
            quantity,
            stockId,
            ...((vouchers && vouchers.length > 0) && { vouchers }),
            ...(validtyDate && { validityDateVouchers: validtyDate }),
        };

        const { data } = await this.client.put(
            `/stock/add-quantity-product`,
            payload
        );

        return data;
    }
}
