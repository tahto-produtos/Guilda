import { guildaApiClient2 } from "src/services";

export class DeleteGalleryUseCase {
    private client = guildaApiClient2;

    async handle(imageId: number) {
        const payload = {
            images: [
                {
                    id: imageId,
                },
            ],
        };

        const { data } = await this.client.delete(`/Gallery`, {
            headers: {
                "Content-Type": "application/json",
            },
            data: payload,
        });

        return data;
    }
}
