import { AxiosResponse } from "axios";
import Cookies from "js-cookie";
import { jwtTokenKey } from "src/constants";
import { guildaApiClient2 } from "src/services";

export class ChangePersonaUseCase {
    private client = guildaApiClient2;

    async handle(props: { idPersona: number }) {
        const { idPersona } = props;

        const { data } = await this.client.get<
            unknown,
            AxiosResponse<{ token: string }>
        >(`/ChangeAccountPersona?idPersona=${idPersona}`);

        const { token } = data;

        Cookies.set(jwtTokenKey, token);
        this.client.defaults.headers.common.Authorization = `Bearer ${token}`;

        return data;
    }
}
