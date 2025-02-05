import { ReactNode, useEffect, useState } from "react";
import { UserInfoContext } from "./user.context";
import Cookies from "js-cookie";
import { BasketGeneralUser } from "src/modules/indicators/use-cases/basket-general-user.use-case";
import { MeUseCase } from "src/modules/collaborators/use-cases/me.use-case";
import { toast } from "react-toastify";
import { MyUser } from "src/typings/models/myuser.model";
import { BasketData } from "src/typings/models/basket-data.model";
import {DaysLogged} from "../../typings";
import {DaysLoggedUseCase} from "../../modules/profiles/use-cases/days-logged.use-case";

interface IProviderProps {
    children: ReactNode;
}

export function UserInfoProvider({ children }: IProviderProps) {
    const userToken = Cookies.get("jwtToken");
    const [myUser, setMyUser] = useState<MyUser | null>(null);
    const [userGroup, setUserGroup] = useState<number | null>(null);
    const [userGroupName, setUserGroupName] = useState<string | null>(null);
    const [basketData, setBasketData] = useState<BasketData | null>(null);
    const [daysLogged, setDaysLogged] = useState<DaysLogged[] | null>(null);

    async function getBasketGeneralUser() {
        if (!myUser) {
            return;
        }

        await new BasketGeneralUser()
            .handle({ codCollaborator: myUser.id })
            .then((data) => {
                setBasketData(data);
                setUserGroup(data?.idGroup || 0);
                setUserGroupName(data?.groupAlias || "");
            })
            .catch(() => {});
    }

    useEffect(() => {
        myUser && getBasketGeneralUser();
    }, [myUser]);

    useEffect(() => {
        if (userToken) {
            new MeUseCase()
                .handle()
                .then((data) => {
                    setMyUser(data);
                })
                .catch(() => {
                    toast.error("Erro ao carregar suas permissões");
                })
                .finally(() => {});
        }
    }, [userToken]);

    useEffect(() => {
        if (userToken) {
            new DaysLoggedUseCase()
                .handle()
                .then((data) => {
                    setDaysLogged(data);
                })
                .catch(() => {
                    toast.error("Erro ao carregar seus dias logados");
                })
                .finally(() => {});
        }
    }, [userToken]);

    return (
        <UserInfoContext.Provider
            value={{
                myUser,
                userGroup,
                userGroupName,
                basketData,
                setMyUser,
                daysLogged,
            }}
        >
            {children}
        </UserInfoContext.Provider>
    );
}
