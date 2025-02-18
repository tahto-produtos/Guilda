export interface OrderReport {
    CODIGO: string;
    CRIADO_POR: number;
    STATUS: string;
    CRIADO_EM: string;
    COLABORADOR: number;
    CARGO: string;
    NOME_SUPERVISOR: string;
    NOME_COORDENADOR: string;
    NOME_GERENTE_II: string;
    NOME_GERENTE_I: string;
    NOME_DIRETOR: string;
    NOME_CEO: string;
    GRUPO: string;
    UF: string;
    COD_GIP: number;
    SETOR: string;
    HOME: string;
    TIPO_DE_PRODUTO: string;
    NOME_DO_PRODUTO: string;
    QUANTIDADE: number;
    VALOR_EM_MOEDAS: number;
    VALOR_EM_MOEDAS_TOTAL: number;
    ULTIMA_ATUALIZACAO: string;
    QUEM_ATUALIZOU: string;
    ENTREGUE_POR: string;
    BC_COLABORADOR: string;
    QUEM_VAI_RETIRAR: string;
    QUEM_RETIROU: string;
    ESTOQUE: number;
}
