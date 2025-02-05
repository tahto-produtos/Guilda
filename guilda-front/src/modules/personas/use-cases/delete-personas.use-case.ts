import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export interface DeletePersonasUseCaseProps {
    IDPERSONAUSER: string;
    VALIDADETED: boolean;
}

export class DeletePersonasUseCase {
    private client = guildaApiClient2;

    async handle(props: DeletePersonasUseCaseProps) {
        const payload = {
            IDPERSONAUSER: props.IDPERSONAUSER,
            VALIDADETED: props.VALIDADETED,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/DeletedAccountPersona`,
            payload
        );

        return data;
    }
}
