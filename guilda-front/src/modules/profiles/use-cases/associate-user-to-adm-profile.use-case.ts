import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";

export class AssociateUserToAdmProfileUseCase {
    private client = guildaApiClient;

    async handle(props: { profileName: string; collaboratorId: number }) {
        const { profileName, collaboratorId } = props;

        const payload = {
            profileName,
            collaboratorId,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/profiles/associate-permissions-profiles-collaborators-administration`,
            payload
        );

        return data;
    }
}
