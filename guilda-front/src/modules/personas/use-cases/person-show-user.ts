import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export class PersonaShowUserUseCase {
    private client = guildaApiClient2;

    async handle(props: { id?: string | number }) {
        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/PersonaShowUser${props.id ? `?specificUserId=${props.id}` : ""}`
        );

        return data;
    }
}
