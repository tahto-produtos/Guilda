import { guildaApiClient } from "src/services";

interface IProps {
    productId: number;
    orderId: number;
}

export class GenerateConfirmProductQrCodeUseCase {
    private client = guildaApiClient;

    async handle(props: IProps) {
        const { productId, orderId } = props;

        const { data } = await this.client.post(
            `/order/${productId}/${orderId}/generate/qrcode`
        );

        return data;
    }
}
