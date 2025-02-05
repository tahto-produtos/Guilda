import { guildaApiClient } from "src/services";

export interface DeleteProductProps {
    id: string | number;
}

export class DeleteProduct {
    private client = guildaApiClient;

    async handle(props: DeleteProductProps) {
        const { id } = props;

        const { data } = await this.client.delete(`/product/${id}`);

        return data;
    }
}
