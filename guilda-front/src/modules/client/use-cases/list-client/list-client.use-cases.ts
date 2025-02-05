import { guildaApiClient2 } from "../../../../services";
import { AxiosResponse } from "axios";
import { Client } from "../../../../typings";

interface ListClientUseCaseProps {
    codCollaborator?: number;
}

export class ListClientUseCase {
    private client = guildaApiClient2;

    async handle(props: ListClientUseCaseProps) {
        const { codCollaborator } = props;

        const { data } = await this.client.get<
            unknown,
            AxiosResponse<Client[]>
        >(`/Client`);

        return data;
    }
}
