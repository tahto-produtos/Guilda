import { AxiosResponse } from "axios";
import FormData from "form-data";
import { guildaApiClient2 } from "src/services";

export class MassiveCreditUseCase {
    private client = guildaApiClient2;

    async handle(file: File) {
        const form = new FormData();
        form.append("file", file);

        const { data } = await this.client.post<
            unknown,
            AxiosResponse,
            FormData
        >(`/InputMassiveMonetization`, form);

        return data;
    }
}
