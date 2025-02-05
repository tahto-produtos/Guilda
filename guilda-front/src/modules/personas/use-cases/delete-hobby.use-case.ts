import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export interface DeleteHobbyUseCaseProps {
    id: number;
}

export class DeleteHobbyUseCase {
    private client = guildaApiClient2;

    async handle(props: DeleteHobbyUseCaseProps) {
        const payload = {
            IDGDA_PERSONA_HOBBY: props.id,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/DeletedHobbyPersona`,
            payload
        );

        return data;
    }
}
