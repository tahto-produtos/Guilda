/**
 * USE CASE REPLICADO PORQUE NÃO SEI QUAL MOTIVO NA TELA DE QUIZ O PEGAR O RETORNO DO OUTRO USE CASE COMO ARRAY NÃO ESTAVA FUNCIONANDO, ENTÃO REPLIQUEI O MESMO SÓ QUE O RESPONSE NÃO É ARRAY
 */
import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { ListPersonasResponseModel } from "src/typings";

interface ListPersonasUseCaseProps {
    accountPersona: string;
    limit: number;
    page: number;
}

export class ListPersonasV2UseCase {
    private client = guildaApiClient2;

    async handle(props: ListPersonasUseCaseProps) {
        const { accountPersona, limit, page } = props;
        
        const { data } = await this.client.get<
            unknown,
            AxiosResponse<ListPersonasResponseModel>
        >(
            `/AccountsPersona?accountPersona=${accountPersona}&limit=${limit}&page=${page}`
        );

        return data;
    }
}
