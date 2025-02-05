import { guildaApiClient } from "src/services";

export interface ListVoucherCodesProps {
    voucherId: number;
}

export class ListVoucherCodes {
    private client = guildaApiClient;

    async handle(props: ListVoucherCodesProps) {
        const { voucherId } = props;

        const { data } = await this.client.get(
            `/collaborator-voucher/${voucherId}`
        );

        return data;
    }
}
