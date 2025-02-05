import { createContext } from "react";
import { BasketData } from "src/typings/models/basket-data.model";
import { MyUser } from "src/typings/models/myuser.model";

export interface PermissionsContextData {
    myPermissions: number[];
    setMyPermissions: (input: number[]) => void;
}

export const PermissionsContext = createContext<PermissionsContextData>(
    {} as PermissionsContextData
);
