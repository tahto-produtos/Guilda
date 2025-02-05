import { guildaApiClient } from "src/services";

export class ConfirmQrCodeUseCase {
    private client = guildaApiClient;

    async handle(orderId: string, collaboratorId: number) {
        const payload = {
            collaboratorId,
        };

        const { data } = await this.client.post(
            `/order/${orderId}/confirm/qrcode`,
            payload
        );

        return data;
    }
}
