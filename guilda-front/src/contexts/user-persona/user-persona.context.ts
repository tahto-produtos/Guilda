import { createContext } from "react";
import { PersonaInfo } from "src/typings/models/persona-info.model";
import { Persona } from "src/typings/models/persona.model";
import { PersonaShowUser } from "../../typings";
import { LoggedAccount } from "src/typings/models/logged-account.model";

export interface UserPersonaContextData {
  personaInfo: PersonaInfo | null;
  persona: Persona | null;
  personaShowUser: PersonaShowUser | null;
  loggedAccounts: LoggedAccount[];
  idPersonAccount: number | null;
  LoggedUserAccounts: () => void;
  refreshPersona: () => void;
}

export const UserPersonaContext = createContext<UserPersonaContextData>(
  {} as UserPersonaContextData
);
