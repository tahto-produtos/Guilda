import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export class CreateGalleryUseCase {
    private client = guildaApiClient2;

    async handle(file: File[]) {
        const form = new FormData();
        // form.append(`FILES`, file);
        for (let i = 0; i < file?.length; i++) {
            form.append(`FILES[${i}]`, file[i]);
        }

        const { data } = await this.client.post<
            unknown,
            AxiosResponse,
            FormData
        >(`/Gallery`, form);

        return data;
    }
}
