import { guildaApiClient } from "../../../../services";
import { AxiosResponse } from "axios";
import { ListIndicatorsResponseDto } from "./dto";
import { PaginationModel } from "src/typings/models/pagination.model";

export class ListIndicatorsAllDetailsUseCase {
    private client = guildaApiClient;

    async handle(props: PaginationModel, status?: boolean) {
        const { limit, offset, searchText, startDate, endDate, code } = props;

        const { data } = await this.client.get<
            unknown,
            AxiosResponse<ListIndicatorsResponseDto>
        >(
            `/indicators/all/details?limit=${limit}&offset=${offset}${
                startDate ? `&startDate=${startDate}` : ""
            }${endDate ? `&endDate=${endDate}` : ""}${
                searchText ? `&searchText=${searchText}` : ""
            }${code ? `&code=${code}` : ""}${
                status !== undefined ? `&status=${status}` : ""
            }`
        );

        return data;
    }
}
