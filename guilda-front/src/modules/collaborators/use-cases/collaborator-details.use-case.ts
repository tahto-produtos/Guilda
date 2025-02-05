import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";

export class CollaboratorDetailUseCase {
    private client = guildaApiClient;

    async handle(id: number) {
        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/collaborators/${id}`
        );

        return data;
    }
}
