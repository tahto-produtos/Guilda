import { guildaApiClient } from "src/services";

export interface ListMonetizationHierarchyUseCaseProps {
    startDate: string;
    endDate: string;
    searchText?: string;
}

export class ListMonetizationHierarchyUseCase {
    private client = guildaApiClient;

    async handle(props: ListMonetizationHierarchyUseCaseProps) {
        const { startDate, endDate, searchText } = props;

        const payload = { startDate, endDate, searchText };

        const { data } = await this.client.get(
            `/monetizations/hierarchy?searchText=${
                searchText ? searchText : ""
            }&startDate=${startDate}&endDate=${endDate}`
        );

        return data;
    }
}
