import { guildaApiClient } from "src/services";

export interface ListMyVouchersProps {
    limit: number;
    offset: number;
}

export class ListMyVouchers {
    private client = guildaApiClient;

    async handle(props: ListMyVouchersProps) {
        const { limit, offset } = props;

        const { data } = await this.client.get(
            `/collaborator-voucher?limit=${limit}&offset=${offset}`
        );

        return data;
    }
}
