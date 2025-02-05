import { guildaApiClient } from "src/services";

export interface ListAllPermissionsProps {
    limit: number;
    offset: number;
}

export class ListAllPermissions {
    private client = guildaApiClient;

    async handle(props: ListAllPermissionsProps) {
        const { limit, offset } = props;

        const { data } = await this.client.get(
            `/permissions?limit=${limit}&offset=${offset}`
        );

        return data;
    }
}
