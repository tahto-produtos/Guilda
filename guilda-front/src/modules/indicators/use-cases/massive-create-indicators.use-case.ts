import { AxiosResponse } from "axios";
import FormData from "form-data";
import { guildaApiClient } from "src/services";

export class MassiveCreateIndicatorsUseCase {
    private client = guildaApiClient;

    async handle(file: File) {
        const form = new FormData();
        form.append("file", file);

        const { data } = await this.client.post<
            unknown,
            AxiosResponse,
            FormData
        >("/results/indicators/import", form);

        return data;
    }
}
