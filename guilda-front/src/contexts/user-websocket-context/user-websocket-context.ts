import { createContext } from "react";
import { CartDataModel } from "src/typings/models/cart-data.model";
import { WebSocketStatusTypes } from "./user-websocket-provider";

export interface UserWebsocketContextData {
    status: WebSocketStatusTypes;
}

export const UserWebsocketContext = createContext<UserWebsocketContextData>(
    {} as UserWebsocketContextData
);
