import { guildaApiClient } from "src/services";

export interface GenerateStockProductQrCodeUseCaseProps {
    productId: number;
    stockId: number;
    supplierId: number;
}

export class GenerateStockProductQrCodeUseCase {
    private client = guildaApiClient;

    async handle(props: GenerateStockProductQrCodeUseCaseProps) {
        const { productId, stockId, supplierId } = props;

        const { data } = await this.client.post(
            `/order/${productId}/${stockId}/${supplierId}/generate/qrcode`
        );

        return data;
    }
}
