import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export interface CityPersonaResponse {
    id: number;
    name: string;
}

interface CityPersonaUseCaseProps {
    city: string;
    state: number;
}

export class CityPersonaUseCase {
    private client = guildaApiClient2;

    async handle(props: CityPersonaUseCaseProps) {
        const { city, state } = props;

        const { data } = await this.client.get<
            unknown,
            AxiosResponse<CityPersonaResponse[]>
        >(`/PersonaCity?state=${state}&city=${city}`);

        return data;
    }
}
