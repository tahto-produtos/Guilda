import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";

export interface AddCreditUseCaseProps {
    amount: number;
    collaborator: string;
    reason: string;
}

export class AddCreditUseCase {
    private client = guildaApiClient;

    async handle(props: AddCreditUseCaseProps) {
        const { amount, collaborator, reason } = props;

        const payload = {
            amount: amount,
            reason: reason,
        };

        const { data } = await this.client.post(
            `/collaborators/${collaborator}/add-credit`,
            payload
        );

        return data;
    }
}
