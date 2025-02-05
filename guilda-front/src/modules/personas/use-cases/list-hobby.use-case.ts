import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface ListHobbyUseCaseResponse {
    IDGDA_PERSONA_USER_HOBBY: number;
    HOBBY: string;
}

interface ListHobbyUseCaseProps {
    hobby: string;
}

export class ListHobbyUseCase {
    private client = guildaApiClient2;

    async handle(props: ListHobbyUseCaseProps) {
        const { hobby } = props;

        const { data } = await this.client.get<
            unknown,
            AxiosResponse<ListHobbyUseCaseResponse[]>
        >(`/HobbyPersona?hobby=${hobby}`);

        return data;
    }
}
