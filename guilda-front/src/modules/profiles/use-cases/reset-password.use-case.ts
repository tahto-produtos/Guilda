import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export class ResetPasswordUsecase {
    private client = guildaApiClient2;

    async handle(login: string) {
        const payload = {
            LOGIN: login,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            "/ResetPassword",
            payload
        );

        return data;
    }
}
