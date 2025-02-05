import { guildaApiClient2 } from "src/services";

export interface CreateDisplayUseCaseProps {
    PRIORIDADE: string;
    NOMECONFIG: string;
    ITENS: {
        CODPRODUTO: number;
        POSICAO: string;
    }[];
    HIERARQUIA: {
        CODHIERARQUIA: number;
    }[];

    GRUPO:
        | {
              CODGRUPO: number;
          }[];

    ESTOQUE: {
        CODESTOQUE: number;
    }[];
}

export class CreateDisplayUseCase {
    private client = guildaApiClient2;

    async handle(props: CreateDisplayUseCaseProps) {
        const { ESTOQUE, GRUPO, HIERARQUIA, ITENS, NOMECONFIG, PRIORIDADE } =
            props;

        const payload = {
            PRIORIDADE: parseInt(PRIORIDADE),
            NOMECONFIG: NOMECONFIG,
            ITENS: ITENS,
            HIERARQUIA: HIERARQUIA,
            GRUPO: GRUPO,
            ESTOQUE: ESTOQUE,
        };

        const { data } = await this.client.post(`/DisplayConfig`, payload);

        return data;
    }
}
