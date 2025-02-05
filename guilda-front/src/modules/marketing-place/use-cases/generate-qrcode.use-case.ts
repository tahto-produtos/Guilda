import { guildaApiClient } from "src/services";

export class GenerateQrCodeUseCase {
    private client = guildaApiClient;

    async handle(orderId: number) {
        const { data } = await this.client.post(
            `/order/${orderId}/generate/qrcode`
        );

        return data;
    }
}
