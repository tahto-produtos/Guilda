import { guildaApiClient2 } from "src/services";

export class ListGalleryUseCase {
    private client = guildaApiClient2;

    async handle() {
        const { data } = await this.client.get(`/Gallery`);

        return data;
    }
}
