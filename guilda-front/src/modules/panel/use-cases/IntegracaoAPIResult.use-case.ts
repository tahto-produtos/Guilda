import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface IntegracaoAPIResultUseCaseProps {
    dtInicial: string;
    dtFinal: string;
}

export class IntegracaoAPIResultUseCase {
    private client = guildaApiClient2;

    async handle(props: IntegracaoAPIResultUseCaseProps) {
        const { dtFinal, dtInicial } = props;

        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/IntegracaoAPIResult?dtInicial=${dtInicial}&dtFinal=${dtFinal}`
        );

        return data;
    }
}
