import { guildaApiClient2 } from "src/services";

interface ListConfigDayMonetizationProps {
    referer: any;
    filtertype: any;
}

export class ListConfigDayMonetizationUseCase {
    private client = guildaApiClient2;

    async handle(props: ListConfigDayMonetizationProps) {
        const { referer, filtertype } = props;

        const { data } = await this.client.get(
            `/ListMonetizationConfigDay?referer=${referer}&filtertype=${filtertype}`
        );

        return data;
    }
}
