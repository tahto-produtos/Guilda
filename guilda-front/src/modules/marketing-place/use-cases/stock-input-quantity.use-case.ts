import { AxiosResponse } from "axios";
import FormData from "form-data";
import { guildaApiClient2 } from "src/services";

export class StockInputQuantityUseCase {
    private client = guildaApiClient2;

    async handle(file: File) {
        const form = new FormData();
        form.append("FILE", file);

        const { data } = await this.client.post<
            unknown,
            AxiosResponse,
            FormData
        >("/StockInputQuantity", form);

        return data;
    }
}
