import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export interface CreateHobbyUseCaseProps {
    createdBy: number;
    name: string;
}

export class CreateHobbyUseCase {
    private client = guildaApiClient2;

    async handle(props: CreateHobbyUseCaseProps) {
        const payload = {
            CREATED_BY: props.createdBy,
            HOBBY: props.name,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/CreatedHobbyPersona`,
            payload
        );

        return data;
    }
}
