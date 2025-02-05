import { guildaApiClient } from "src/services";

export interface ListProductStocksProps {
    id?: number;
}

export class ListProductStocks {
    private client = guildaApiClient;

    async handle(props: ListProductStocksProps) {
        const {
            id
        } = props;
        const { data } = await this.client.get(
            `/product/${id}/stocks/`
        );

        return data;
    }
}
