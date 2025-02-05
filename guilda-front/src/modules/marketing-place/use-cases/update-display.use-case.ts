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

    GRUPO: {
        CODGRUPO: number;
    }[];

    ESTOQUE: {
        CODESTOQUE: number;
    }[];
}

export class UpdateDisplayUseCase {
    private client = guildaApiClient2;

    async handle(
        props: CreateDisplayUseCaseProps,
        displayId: number,
        status: string
    ) {
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

        const { data } = await this.client.put(
            `/DisplayConfig?displayId=${displayId}&activated=${status}`,
            payload
        );

        return data;
    }
}
