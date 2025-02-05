import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export interface CreateBlackListProps {
    word: string;
}

export class CreateBlackList {
    private client = guildaApiClient2;

    async handle(props: CreateBlackListProps) {
        const payload = {
            WORD: props.word,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/CreatedBlackListPersona`,
            payload
        );

        return data;
    }
}
