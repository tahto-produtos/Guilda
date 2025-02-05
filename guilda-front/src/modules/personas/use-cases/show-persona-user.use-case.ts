import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export class ShowPersonaUseCase {
    private client = guildaApiClient2;

    async handle(props: { id?: string | number }) {
        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/PersonaUser${props.id ? `?personaUser=${props.id}` : ""}`
        );

        return data;
    }
}
