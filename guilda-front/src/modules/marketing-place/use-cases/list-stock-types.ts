import { guildaApiClient } from "src/services";

export interface ListStockTypesProps {
    limit: number;
    offset: number;
    searchText: string;
}

export class ListStockTypes {
    private client = guildaApiClient;

    async handle(props: ListStockTypesProps) {
        const { limit, offset, searchText } = props;

        const { data } = await this.client.get(
            `/type?limit=${limit}&offset=${offset}&searchText=${searchText}`
        );

        return data;
    }
}
