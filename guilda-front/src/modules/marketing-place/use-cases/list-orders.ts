import { guildaApiClient } from "src/services";

export interface ListOrdersProps {
    limit: number;
    offset: number;
    searchText?: string;
    isAll?: boolean;
    status?: string;
    type?: string;
    collaboratorNameOrIdentification?: string;
    uf?: string;
    sector?: string;
    quantity?: string;
    orderBySell?: string;
    startDate?: Date;
    endDate?: Date;
}

export class ListOrders {
    private client = guildaApiClient;

    async handle(props: ListOrdersProps) {
        const {
            limit,
            offset,
            searchText,
            isAll,
            status,
            type,
            collaboratorNameOrIdentification,
            uf,
            sector,
            quantity,
            orderBySell,
            startDate,
            endDate,
        } = props;

        const { data } = await this.client.get(
            `${
                isAll ? "/order/all" : "/order"
            }?limit=${limit}&offset=${offset}${
                status ? `&status=${status}` : ""
            }${type ? `&type=${type}` : ""}${
                collaboratorNameOrIdentification
                    ? `&collaboratorNameOrIdentification=${collaboratorNameOrIdentification}`
                    : ""
            }${searchText ? `&searchText=${searchText}` : ""}${
                uf ? `&uf=${uf}` : ""
            }${sector ? `&sector=${sector}` : ""}${
                quantity ? `&quantity=${quantity}` : ""
            }${
                orderBySell == "bestSeller"
                    ? `&bestSeller=true`
                    : orderBySell == "lessSold"
                    ? `&lessSold=true`
                    : ""
            }${
                startDate && endDate && status == "RELEASED"
                    ? `&startDateReleased=${startDate
                          .toISOString()
                          .substring(0, 10)}&endDateReleased=${endDate
                          .toISOString()
                          .substring(0, 10)}`
                    : startDate && endDate
                    ? `&startDate=${startDate
                          .toISOString()
                          .substring(0, 10)}&endDate=${endDate
                          .toISOString()
                          .substring(0, 10)}`
                    : ""
            }`
        );

        return data;
    }
}
