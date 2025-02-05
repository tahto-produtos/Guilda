import { ReactNode, useContext, useEffect, useState } from "react";
import { UserPersonaContext } from "./user-persona.context";
import { ShowInfoUserUseCase } from "src/modules/personas/use-cases/show-inf-user.use-case";
import Cookies from "js-cookie";
import { PersonaInfo } from "src/typings/models/persona-info.model";
import { Persona } from "src/typings/models/persona.model";
import { ShowPersonaUseCase } from "src/modules/personas/use-cases/show-persona-user.use-case";
import { UserInfoContext } from "../user-context/user.context";
import { LoggedAccountsUseCase } from "src/modules/profiles/use-cases/logged-accounts.use-case";
import { PersonaShowUser } from "../../typings";
import { PersonaShowUserUseCase } from "../../modules/personas/use-cases/person-show-user";
import { LoggedAccount } from "src/typings/models/logged-account.model";

interface IProviderProps {
  children: ReactNode;
}

export function UserPersonaProvider({ children }: IProviderProps) {
  const userToken = Cookies.get("jwtToken");
  const [personaInfo, setPersonaInfo] = useState<PersonaInfo | null>(null);
  const [persona, setPersona] = useState<Persona | null>(null);
  const [personaShowUser, setPersonaShowUser] =
    useState<PersonaShowUser | null>(null);
  const [idPersonAccount, setIdPersonAccount] = useState<number | null>(null);
  const [loggedAccounts, setLoggedAccounts] = useState<LoggedAccount[]>([]);

  async function LoggedUserAccounts() {
    if (!userToken) return;
    console.log("LoggedUserAccounts");
    await new LoggedAccountsUseCase()
      .handle()
      .then((data) => {
        setLoggedAccounts(data.ACCOUNTS);

        setIdPersonAccount(data.PERSONAID);
      })
      .finally(() => {});
  }

  useEffect(() => {
    userToken && LoggedUserAccounts();
  }, [userToken]); 

  async function getPersonaShowUser() {
/*     console.log("id: " + idPersonAccount);
    if (!idPersonAccount) return; */

    await new PersonaShowUserUseCase()
      .handle({ id: "0" })
      .then((data) => {
        setPersonaShowUser({
          ...data[0],
          ID_PERSON_ACCOUNT: idPersonAccount,
        });
      })
      .catch(() => {});

      console.log("ids: " + personaShowUser);
  }

  useEffect(() => {
    userToken && idPersonAccount && getPersonaShowUser();
  }, [userToken, idPersonAccount]);

  async function getInfoUser() {
    new ShowInfoUserUseCase()
      .handle()
      .then((data) => {
        setPersonaInfo(data);
      })
      .catch(() => {});
  }

  useEffect(() => {
    userToken && getInfoUser();
  }, [userToken]);

  async function showPersonaUser() {
/*     if (!idPersonAccount) return; */

    await new ShowPersonaUseCase()
      .handle({ id: "0" })
      .then((data) => {
        setPersona(data[0]);
      });
  }

  useEffect(() => {
    idPersonAccount && showPersonaUser();
  }, [idPersonAccount]);

  function refreshPersona() {
    getPersonaShowUser();
    showPersonaUser();
  }

  return (
    <UserPersonaContext.Provider
      value={{
        personaInfo,
        persona,
        personaShowUser,
        loggedAccounts,
        idPersonAccount,
        LoggedUserAccounts,
        refreshPersona,
      }}
    >
      {children}
    </UserPersonaContext.Provider>
  );
}

// Exportar o hook useUserPersona para ser utilizado em outros componentes
export function useUserPersona() {
  const context = useContext(UserPersonaContext);
  console.log("Context: " + context);
  if (context === undefined) {
    throw new Error('useUserPersona deve ser usado dentro de um UserPersonaProvider');
  }
  return context;
}