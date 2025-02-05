import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface PermissionUseCaseProps {
    codCollaborator?: number;
    codProfile?: number;
}

export class PermissionUseCase {
    private client = guildaApiClient2;

    async handle(props: PermissionUseCaseProps) {
        const { codCollaborator, codProfile } = props;

        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/Permission?codCollaborator=${
                codCollaborator ? codCollaborator : ""
            }&codProfile=${codProfile ? codProfile : ""}`
        );

        return data;
    }
}
