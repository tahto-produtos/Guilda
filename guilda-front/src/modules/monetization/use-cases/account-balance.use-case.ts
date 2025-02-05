import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";

export interface AccountBalanceUseCaseProps {
    userId: number;
}

export class AccountBalanceUseCase {
    private client = guildaApiClient;

    async handle(props: AccountBalanceUseCaseProps) {
        const { userId } = props;

        const { data } = await this.client.get(
            `/collaborators/${userId}/balance`
        );

        return data;
    }
}
