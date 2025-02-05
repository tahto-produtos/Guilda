import { createContext } from "react";

export interface NotificationsContextData {
    count: number;
    getCountNotifications: () => void;
}

export const NotificationsContext = createContext<NotificationsContextData>(
    {} as NotificationsContextData
);
