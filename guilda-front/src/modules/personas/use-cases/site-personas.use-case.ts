import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export interface SitePersonaResponse {
    id: number;
    name: string;
}

export class SitePersonaUseCase {
    private client = guildaApiClient2;

    async handle() {
        const { data } = await this.client.get<
            unknown,
            AxiosResponse<SitePersonaResponse[]>
        >(`/PersonaSite`);

        return data;
    }
}
