import { AxiosResponse } from "axios";
import FormData from "form-data";
import { guildaApiClient2 } from "src/services";

export class ExportMassiveExpirationDateUseCase {
    private client = guildaApiClient2;

    async handle() {

        const { data } = await this.client.get<
        unknown, AxiosResponse
        >(`/ExportFileMonetizationExpiredDate`);

        return data;
    }
}