import { ReactNode, useContext, useEffect, useState } from "react";
import { UserInfoContext } from "../user-context/user.context";
import { CountNotificationUseCase } from "src/modules/notifications/use-cases/count-notification.use-case";
import { ClimateContext } from "./climate.context";
import { ListClimateUseCase } from "src/modules/climate/use-case/list-climate.use-case";
import Cookies from "js-cookie";
import { Climate } from "src/typings/models/climate.model";
import { ModalClimate } from "src/modules/climate/fragments/modal-climate";
import { UserPersonaContext } from "../user-persona/user-persona.context";
interface IProviderProps {
  children: ReactNode;
}

export function ClimateProvider({ children }: IProviderProps) {
  const { myUser } = useContext(UserInfoContext);
  const { personaShowUser } = useContext(UserPersonaContext);
  const userToken = Cookies.get("jwtToken");
  const firstLogin = Cookies.get("firstLogin");
  const [response, setResponse] = useState<boolean>(false);
  const [climates, setClimates] = useState<Climate[]>([]);

  async function listClimate() {
    const admView = Cookies.get("admViewActive")?.toString();

    //só responde clima quando não é conta comercial (2)
    if(personaShowUser && personaShowUser.TIPO_CONTA == "1" && admView == "false") {

      await new ListClimateUseCase()
      .handle()
      .then((data) => {
        setResponse(data.response);
        setClimates(data.climates);
      })
      .catch(() => {})
      .finally(() => {});
    }
  }
  //console.log("PersonaShowUser: " + personaShowUser);
  useEffect(() => {
    userToken && firstLogin !== "true" && listClimate();
  }, [userToken, firstLogin, personaShowUser]);

  return (
    <ClimateContext.Provider value={{}}>
      {children}
      {response && <ModalClimate climates={climates} refresh={listClimate} />}
    </ClimateContext.Provider>
  );
}
