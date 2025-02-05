import { Hobby } from "./hobby.model";

export interface PersonaShowUser {
    ID_PERSON_ACCOUNT: number;
    NOME: string;
    BC: string;
    IDADE: number;
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
    CIDADE: string;
    SITE: string;
    ISPUBLIC: number;
    FOLLOWERS: number;
    FOLLOWING: number;
    CARGO: string;
    FOLLOWED_BY_ME: boolean;
    SETOR: string;
    TIPO_CONTA: string;
    ISADM: string;
    PERSONAVISION: string;
}
