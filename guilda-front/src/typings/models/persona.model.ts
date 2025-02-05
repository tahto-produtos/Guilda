import { Hobby } from "./hobby.model";

export interface Persona {
    NOME: string;
    BC: string;
    IDADE: 0 | 1;
    IDADE_CALCULADA: number;
    NOME_SOCIAL: string;
    FOTO: string;
    QUEM_E: string;
    MOTIVACOES: string;
    OBJETIVO: string;
    HOBBIES: Hobby[];
    EMAIL: string;
    TELEFONE: string;
    DATA_NASCIMENTO: string;
    UF: string;
    ID_UF: string;
    CIDADE: string;
    ID_CIDADE: string;
    SITE: string;
    ID_SITE: string;
    FLAGTAHTO: boolean;
}
