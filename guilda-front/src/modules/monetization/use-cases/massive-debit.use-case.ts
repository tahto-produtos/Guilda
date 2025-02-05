import { AxiosResponse } from "axios";
import FormData from "form-data";
import { guildaApiClient } from "src/services";

export class MassiveDebitUseCase {
    private client = guildaApiClient;

    async handle(file: File, collaboratorId: string) {
        const form = new FormData();
        form.append("table", file);

        const { data } = await this.client.post<
            unknown,
            AxiosResponse,
            FormData
        >(`/collaborator/${collaboratorId}/add-debit`, form);

        return data;
    }
}
