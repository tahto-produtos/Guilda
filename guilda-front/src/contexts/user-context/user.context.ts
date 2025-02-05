import { createContext } from "react";
import { BasketData } from "src/typings/models/basket-data.model";
import { MyUser } from "src/typings/models/myuser.model";
import {DaysLogged} from "../../typings";

export interface UserInfoContextData {
    myUser: MyUser | null;
    setMyUser: (input: MyUser | null) => void;
    userGroup: number | null;
    userGroupName: string | null;
    basketData: BasketData | null;
    daysLogged: DaysLogged[] | null;
}

export const UserInfoContext = createContext<UserInfoContextData>(
    {} as UserInfoContextData
);
