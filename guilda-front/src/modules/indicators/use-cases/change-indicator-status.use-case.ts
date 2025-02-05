import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";

export class ChangeIndicatorStatusUseCase {
    private client = guildaApiClient;

    async handle(props: { indicatorId: number; status: boolean }) {
        const { status, indicatorId } = props;

        const payload = {
            status: status,
        };

        const { data } = await this.client.put<unknown, AxiosResponse>(
            `/indicators/${indicatorId}/status`,
            payload
        );

        return data;
    }
}
