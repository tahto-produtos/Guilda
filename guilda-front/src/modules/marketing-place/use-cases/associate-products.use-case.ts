import { guildaApiClient } from "src/services";

export interface AssociateProductsProps {
    products: number;
    stock: string | string[] | undefined;
    amount: number;
    stockToRemove: number | null | undefined;
    reason: string | undefined;
}

export class AssociateProducts {
    private client = guildaApiClient;

    async handle(props: AssociateProductsProps) {
        const { products, stock, amount, stockToRemove, reason } = props;

        const payload = {
            products: [
                {
                    id: products,
                    amount: amount,
                    autosale: false,
                    stockToRemove: stockToRemove,
                    reason: reason
                },
            ],
        };

        const { data } = await this.client.post(
            `/stock/${stock}/associate-product`,
            payload
        );

        return data;
    }
}
