import { AxiosResponse } from "axios";
import FormData from "form-data";
import { guildaApiClient } from "src/services";

export class MassiveCreateProductUseCase {
    private client = guildaApiClient;

    async handle(file: File) {
        const form = new FormData();
        form.append("file", file);

        const { data } = await this.client.post<
            unknown,
            AxiosResponse,
            FormData
        >("/product/import/v2", form);

        return data;
    }
}
