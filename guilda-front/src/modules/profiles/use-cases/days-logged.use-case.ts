import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import {DaysLogged} from "../../../typings";

export class DaysLoggedUseCase {
    private client = guildaApiClient2;

    async handle() {

        const { data } = await this.client.get<unknown, AxiosResponse<DaysLogged[]>>(
            `/DayLoggeds`
        );

        return data;
    }
}
