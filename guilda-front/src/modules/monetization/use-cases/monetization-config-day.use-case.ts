import { guildaApiClient2 } from "src/services";

interface MonetizationConfigDayProps {
    monetizatioConfigType: any;
    ids: any[];
    days: any;
}

export class MonetizationConfigDayUseCase {
    private client = guildaApiClient2;

    async handle(props: MonetizationConfigDayProps) {
        const { monetizatioConfigType, ids, days } = props;
        const date = new Date().toISOString();
        const payload = {
            DAYS: days,
            STARTED_AT: `${date.split('T')[0]} ${date.split('T')[1].split('.')[0]}`,
            visibility: {
                setor: monetizatioConfigType && monetizatioConfigType == 1 ? ids : [],
                site: monetizatioConfigType && monetizatioConfigType == 2 ? ids : [],
            }
        };

        const { data } = await this.client.post(
            `/MonetizationConfigDay`,
            payload
        );

        return data;
    }
}
