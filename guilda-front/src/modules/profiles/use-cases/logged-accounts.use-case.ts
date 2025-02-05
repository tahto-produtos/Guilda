import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export class LoggedAccountsUseCase {
    private client = guildaApiClient2;

    async handle() {
        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/LoggedAccounts?limit=${1000}&page=${1}`
        );

        return data;
    }
}
