import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface UpdatePermissionUseCaseProps {
    profileId: number;
    permit: { permissionId: number; active: boolean }[];
}

export class UpdatePermissionUseCase {
    private client = guildaApiClient2;

    async handle(props: UpdatePermissionUseCaseProps) {
        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/Permission`,
            props
        );

        return data;
    }
}
