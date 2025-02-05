import { guildaApiClient2 } from "../../../../services";
import { AxiosResponse } from "axios";
import { ListNewIndicatorsResponseDto } from "./dto";
import { PaginationModel } from "src/typings/models/pagination.model";

export class ListNewIndicatorsUseCase {
    private client = guildaApiClient2;

    async handle(status?: boolean) {
        //const { limit, offset, searchText, startDate, endDate, code } = props;

        const { data } = await this.client.get<
            unknown,
            AxiosResponse<ListNewIndicatorsResponseDto[]>
        >(
            `/ListNewIndicators`
        );
        
        return data;
    }
}
