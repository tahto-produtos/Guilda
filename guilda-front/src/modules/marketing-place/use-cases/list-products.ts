import { guildaApiClient } from "src/services";

export interface ListProductsProps {
    limit: number;
    offset: number;
    searchText?: string;
    recents?: boolean;
    bestSeller?: boolean;
    priceMin?: number;
    priceMax?: number;
}

export class ListProducts {
    private client = guildaApiClient;

    async handle(props: ListProductsProps) {
        const {
            limit,
            offset,
            searchText,
            recents,
            bestSeller,
            priceMax,
            priceMin,
        } = props;

        const { data } = await this.client.get(
            `/product?limit=${limit}&offset=${offset}&searchText=${searchText}${
                recents ? `&recents=${recents}` : ""
            }${bestSeller ? `&bestSeller=${bestSeller}` : ""}${
                priceMin ? `&priceMin=${priceMin}` : ""
            }${priceMax ? `&priceMax=${priceMax}` : ""}`
        );

        return data;
    }
}
