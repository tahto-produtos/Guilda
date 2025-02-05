import { guildaApiClient } from "src/services";

export interface GenerateVouchersSoldReportUseCaseProps {
    startDate: string;
    endDate: string;
    productId?: number;
    stock?: number;
    quantity?: number;
    bestSeller?: boolean;
    lessSold?: boolean;
    city?: string;
    sector?: number;
    type?: string;
    status?: string;
}

export class GenerateVouchersSoldReportUseCase {
    private client = guildaApiClient;

    async handle(props: GenerateVouchersSoldReportUseCaseProps) {
        const {
            startDate,
            endDate,
            productId,
            quantity,
            stock,
            bestSeller,
            city,
            lessSold,
            sector,
            type,
            status,
        } = props;

        const payload = {
            startDate,
            endDate,
            productId,
            quantity,
            stock,
            bestSeller,
            city: city ? city : undefined,
            sector,
            lessSold,
            type: type ? type : undefined,
            status: status ? status : undefined,
        };

        const { data } = await this.client.post(
            "/stock/report-vouchers-sold",
            payload
        );

        return data;
    }
}
