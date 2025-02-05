import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export class ListProfileCollaboratorsUseCase {
    private client = guildaApiClient2;

    async handle(profileId: number) {
        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/ListProfileCollaborators?PROFILEID=${profileId}`
        );

        return data;
    }
}
