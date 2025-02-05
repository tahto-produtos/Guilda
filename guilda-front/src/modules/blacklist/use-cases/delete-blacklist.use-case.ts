import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export interface DeleteBlackListProps {
    id: number;
}

export class DeleteBlackList {
    private client = guildaApiClient2;

    async handle(props: DeleteBlackListProps) {
        const payload = {
            IDGDA_BLACKLIST: props.id,
            Validated: "true",
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/DeletedBlackListPersona`,
            payload
        );

        return data;
    }
}
