import { PersonaAccountModel } from "../persona-account.model";

export interface ListPersonasResponseModel {
    PERSONAID: number;
    TOTALPAGES: number;
    ACCOUNTS: PersonaAccountModel[];
}