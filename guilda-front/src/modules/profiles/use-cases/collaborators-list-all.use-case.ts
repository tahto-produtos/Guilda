import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";

export class CollaboratorsListAllUseCase {
    private client = guildaApiClient;

    async handle(props: { limit: number; offset: number; searchText: string }) {
        const { limit, offset, searchText } = props;

        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/collaborators/list/all?limit=${limit}&offset=${offset}&searchText=${searchText}`
        );

        return data;
    }
}
