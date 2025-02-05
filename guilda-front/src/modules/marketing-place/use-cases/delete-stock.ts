import { guildaApiClient } from "src/services";

export interface DeleteStockProps {
    id: string | undefined | string[];
}

export class DeleteStock {
    private client = guildaApiClient;

    async handle(props: DeleteStockProps) {
        const { id } = props;

        const { data } = await this.client.delete(`/stock/${id}`);

        return data;
    }
}
