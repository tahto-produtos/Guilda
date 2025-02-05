import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";

export class CreateCollaboratorUseCase {
    private client = guildaApiClient;

    async handle(props: {
        name: string;
        registration: string;
        profile: string;
        sector: string;
    }) {
        const { name, profile, registration, sector } = props;

        const payload = {
            name,
            profile,
            registration,
            sector,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/collaborators/create-collaborator`,
            payload
        );

        return data;
    }
}
