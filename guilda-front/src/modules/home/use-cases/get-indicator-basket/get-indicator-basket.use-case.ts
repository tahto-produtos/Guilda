import { format, addDays } from "date-fns";
import { guildaApiClient } from "src/services";

export interface ListIndicatorBasketUseCaseProps {
    startDate: string;
    endDate: string;
}

export class ListIndicatorBasketUseCase {
    private client = guildaApiClient;

    async handle(props: ListIndicatorBasketUseCaseProps) {
        const { startDate, endDate } = props;

        const { data } = await this.client.get(
            `/monetizations/indicator-basket?startDate=${startDate}&endDate=${endDate}`
        );

        return data;
    }
}
