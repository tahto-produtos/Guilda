import { guildaApiClient } from "src/services";

export interface CompleteStockReportProps {
    limit: number;
    offset: number;
    startDate: string;
    endDate: string;
    bestSeller?: boolean;
    city?: string;
    visibilityType?: string;
    visibilityValue?: string;
    productStatus?: string;
}

export class CompleteStockReport {
    private client = guildaApiClient;

    async handle(props: CompleteStockReportProps) {
        const {
            limit,
            offset,
            endDate,
            startDate,
            bestSeller,
            city,
            visibilityType,
            visibilityValue,
            productStatus,
        } = props;

        const { data } = await this.client.get(
            `/stock/report?limit=${limit}&offset=${offset}&startDate=${startDate}&endDate=${endDate}${
                bestSeller ? `&bestSeller=${bestSeller}` : ""
            }${city ? `&city=${city}` : ""}${
                visibilityType ? `&visibilityType=${visibilityType}` : ""
            }${visibilityValue ? `&visibilityValue=${visibilityValue}` : ""}${
                productStatus ? `&productStatus=${productStatus}` : ""
            }`
        );

        return data;
    }
}
