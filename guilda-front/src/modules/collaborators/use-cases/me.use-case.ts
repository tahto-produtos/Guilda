import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";
import Cookies from "js-cookie";

export class MeUseCase {
    private client = guildaApiClient;

    async handle() {
        const userToken = Cookies.get("jwtToken");
        if(userToken) {
            this.client.defaults.headers.common.Authorization = `Bearer ${userToken}`;
        }
        const { data } = await this.client.get<unknown, AxiosResponse>(`/me`);

        return data;
    }
}
