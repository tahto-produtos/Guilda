import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";

export class UpdateAdministrationProfilePermissionsUseCase {
    private client = guildaApiClient;

    async handle(props: { profileName: string; permissionsId: Array<number> }) {
        const { profileName, permissionsId } = props;

        const payload = {
            profileName,
            permissionsId,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/profiles/collaborators-administration`,
            payload
        );

        return data;
    }
}
