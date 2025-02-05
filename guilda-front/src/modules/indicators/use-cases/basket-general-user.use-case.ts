import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface BasketGeneralUserProps {
    dtinicial?: string;
    dtfinal?: string;
    codCollaborator: number;
}

export class BasketGeneralUser {
    private client = guildaApiClient2;

    async handle(props: BasketGeneralUserProps) {
        const { codCollaborator, dtfinal, dtinicial } = props;

        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/BasketGeneralUser?codCollaborator=${codCollaborator}&dtinicial=${
                dtinicial ? dtinicial : ""
            }&dtfinal=${dtfinal ? dtfinal : ""}`
        );

        return data;
    }
}
