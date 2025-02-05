import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export class UpdateBasketMetricsUseCase {
    private client = guildaApiClient2;

    async handle(groupId: string, metricMin: string) {
        const payload = {
            groupId,
            metricMin,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/BasketIndicator`,
            payload
        );

        return data;
    }
}
