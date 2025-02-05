import { guildaApiClient2 } from "src/services";

export class DetailsGalleryUseCase {
    private client = guildaApiClient2;

    async handle() {
        const payload = {
            images: [
                {
                    id: 298,
                },
            ],
        };

        const { data } = await this.client.post(
            `/VerifyFileInProducts`,
            payload
        );

        return data;
    }
}
