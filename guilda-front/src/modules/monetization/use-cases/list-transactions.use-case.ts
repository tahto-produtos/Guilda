import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";

export interface ListTransactionsProps {
    userId: number;
    dateMin: string;
    dateMax: string;
    filter?: string | null;
    value?: string | null;
    limit: number;
    offset: number;
}

export class ListTransactions {
    private client = guildaApiClient;

    async handle(props: ListTransactionsProps) {
        const { userId, dateMin, dateMax, filter, value, limit, offset } =
            props;

        const { data } = await this.client.get(
            `/collaborators/${userId}/checking-account?dateMin=${dateMin}&dateMax=${dateMax}${
                filter && value ? `&filter=${filter}&value=${value}` : ""
            }&limit=${limit}&offset=${offset}`
        );

        return data;
    }
}
