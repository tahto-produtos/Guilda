import { guildaApiClient } from "../../../../services";
import { AxiosResponse } from "axios";
import { ListIndicatorsResponseDto } from "./dto";
import { PaginationModel } from "src/typings/models/pagination.model";

export class ListIndicatorsUseCase {
    private client = guildaApiClient;

    async handle(props: PaginationModel, status?: boolean, sectorId?: number) {
        const { limit, offset, searchText, startDate, endDate, code } = props;

        const { data } = await this.client.get<
            unknown,
            AxiosResponse<ListIndicatorsResponseDto>
        >(
            `/indicators?limit=${limit}&offset=${offset}${
                startDate ? `&startDate=${startDate}` : ""
            }${endDate ? `&endDate=${endDate}` : ""}${
                searchText ? `&searchText=${searchText}` : ""
            }${code ? `&code=${code}` : ""}${
                status !== undefined ? `&status=${status}` : ""
            }${sectorId ? `&sectorId=${sectorId}` : ""}`
        );

        return data;
    }
}
