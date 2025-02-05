import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";
import { PaginationModel } from "src/typings/models/pagination.model";
import { ListSectorsResponseDto } from "./dto";

interface ListSectorsUseCaseProps extends PaginationModel {
    deleted?: boolean;
}

export class ListSectorsUseCase {
    private client = guildaApiClient;

    async handle(
        props: ListSectorsUseCaseProps,
        associatedIndicator?: boolean
    ) {
        const { limit, offset, searchText, startDate, endDate, deleted } =
            props;

        const { data } = await this.client.get<
            unknown,
            AxiosResponse<ListSectorsResponseDto>
        >(
            `/sectors?limit=${limit}&offset=${offset}&searchText=${
                searchText ? searchText : ""
            }${startDate ? `&startDate=${startDate}` : ""}${
                endDate ? `&endDate=${endDate}` : ""
            }${
                deleted !== undefined ? `&deleted=${deleted}` : ""
            }&associatedIndicator=${associatedIndicator || ""}`
        );

        return data;
    }
}
