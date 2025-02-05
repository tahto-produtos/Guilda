import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { BlackList } from "src/typings/models/blacklist.model";

interface ListBlacklistUseCaseProps {
    word: string;
}

export class ListBlacklistUseCase {
    private client = guildaApiClient2;

    async handle(props: ListBlacklistUseCaseProps) {
        const { word } = props;

        const { data } = await this.client.get<
            unknown,
            AxiosResponse<BlackList[]>
        >(`/BlackListPersona?word=${word}`);

        return data;
    }
}
