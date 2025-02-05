import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export interface ListedPersona {
    IDGDA_PERSONA_USER: number;
    NOME: string;
    FOTO: string;
    TIPO: string;
    FOLLOWED_BY_ME: boolean;
}

interface ListPersonasResponse {
    TOTALPAGES: number;
    ACCOUNTS: ListedPersona[];
}

interface ListPersonasUseCaseProps {
    accountPersona: string;
    limit: number;
    page: number;
}

export class ListPersonasUseCase {
    private client = guildaApiClient2;

    async handle(props: ListPersonasUseCaseProps) {
        const { accountPersona, limit, page } = props;

        const { data } = await this.client.get<
            unknown,
            AxiosResponse<ListPersonasResponse>
        >(
            `/AccountsPersona?accountPersona=${accountPersona}&limit=${limit}&page=${page}`
        );

        return data;
    }
}
