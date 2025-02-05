import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export interface StatePersonaResponse {
    id: number;
    name: string;
}

export class StatePersonaUseCase {
    private client = guildaApiClient2;

    async handle() {
        const { data } = await this.client.get<
            unknown,
            AxiosResponse<StatePersonaResponse[]>
        >(`/PersonaState`);

        return data;
    }
}
