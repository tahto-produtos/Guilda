import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";
import { PaginationModel } from "src/typings/models/pagination.model";

export class ListAdministrationProfilesUseCase {
    private client = guildaApiClient;

    async handle(props: PaginationModel) {
        const { limit, offset } = props;

        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/profiles/collaborators-administration?limit=${limit}&offset=${offset}`
        );

        return data;
    }
}
