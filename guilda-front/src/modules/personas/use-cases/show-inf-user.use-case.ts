import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { PersonaInfo } from "src/typings/models/persona-info.model";

export class ShowInfoUserUseCase {
    private client = guildaApiClient2;

    async handle() {
        const { data } = await this.client.get<
            unknown,
            AxiosResponse<PersonaInfo>
        >(`/PersonaInfUser`);

        return data;
    }
}
