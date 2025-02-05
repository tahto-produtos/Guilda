import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";

export class DeleteProfileUseCase {
    private client = guildaApiClient;

    async handle(id: number) {
        const { data } = await this.client.delete<unknown, AxiosResponse>(
            `/profiles/collaborators-administration/${id}`
        );

        return data;
    }
}
