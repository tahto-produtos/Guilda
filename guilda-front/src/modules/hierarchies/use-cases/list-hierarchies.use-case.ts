import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";
import { PaginationModel } from "src/typings/models/pagination.model";

export class ListHierarchiesUseCase {
    private client = guildaApiClient;

    async handle(props: PaginationModel) {
        const { limit, offset, searchText, startDate, endDate } = props;

        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/hierarchies?limit=${limit}&offset=${offset}&searchText=${
                searchText ? searchText : ""
            }${startDate ? `&startDate=${startDate}` : ""}${
                endDate ? `&endDate=${endDate}` : ""
            }`
        );

        return data;
    }
}
