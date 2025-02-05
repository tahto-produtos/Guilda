import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { PaginationModel } from "src/typings/models/pagination.model";

export class ListHierarchiesEscalationUseCase {
    private client = guildaApiClient2;

    async handle(props: PaginationModel) {
        const { limit, offset, searchText, startDate, endDate } = props;

        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/HierarchyEscalation?limit=${limit}&offset=${offset}&searchText=${
                searchText ? searchText : ""
            }${startDate ? `&startDate=${startDate}` : ""}${
                endDate ? `&endDate=${endDate}` : ""
            }`
        );

        return data;
    }
}
