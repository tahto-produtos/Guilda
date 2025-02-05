import { guildaApiClient } from "src/services";

export interface ListStocksProps {
    limit: number;
    offset: number;
    searchText: string;
    highlight?: boolean;
    oldStock?: boolean;
    releasedStock?: boolean;
    userGroup: string;
}

export class ListMyStoreProducts {
    private client = guildaApiClient;

    async handle(props: ListStocksProps) {
        const {
            limit,
            offset,
            searchText,
            highlight,
            oldStock,
            releasedStock,
            userGroup,
        } = props;

        const { data } = await this.client.get(
            `/product/list/store?limit=${limit}&offset=${offset}&userGroup=${userGroup}&searchText=${searchText}${
                highlight ? `&highlight=${highlight}` : ""
            }${oldStock ? `&oldStock=${oldStock}` : ""}${
                releasedStock ? `&releasedStock=${releasedStock}` : ""
            }`
        );

        return data;
    }
}
