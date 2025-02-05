import { guildaApiClient } from "src/services";

export interface ListStocksProps {
    limit: number;
    offset: number;
    searchText: string;
    highlight?: boolean;
    bestSeller?: boolean;
    lessSold?: boolean;
    oldStock?: boolean;
    releasedStock?: boolean;
    showAllProducts?: boolean;
}

export class ListStockStore {
    private client = guildaApiClient;

    async handle(props: ListStocksProps) {
        const {
            limit,
            offset,
            searchText,
            bestSeller,
            highlight,
            lessSold,
            oldStock,
            releasedStock,
            showAllProducts,
        } = props;
        console.log(props);
        const { data } = await this.client.get(
            `/stock/list-stock-store?limit=${limit}&offset=${offset}&searchText=${searchText}${
                bestSeller ? `&bestSeller=${bestSeller}` : ""
            }${highlight ? `&highlight=${highlight}` : ""}${
                lessSold ? `&lessSold=${lessSold}` : ""
            }${oldStock ? `&oldStock=${oldStock}` : ""}${
                releasedStock ? `&releasedStock=${releasedStock}` : ""
            }${showAllProducts ? `&showAllProducts=${showAllProducts}` : ""}`
        );

        return data;
    }
}
