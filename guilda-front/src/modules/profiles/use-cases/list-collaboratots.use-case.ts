import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";
import { PaginationModel } from "src/typings/models/pagination.model";

export class ListCollaboratorsUseCase {
    private client = guildaApiClient;

    async handle(props: PaginationModel) {
        const { limit, offset } = props;

        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/collaborators/list-collaborators?limit=${limit}&offset=${offset}`
        );

        return data;
    }
}
