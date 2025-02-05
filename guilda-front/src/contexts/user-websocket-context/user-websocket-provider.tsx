import { ReactNode, useContext, useEffect, useState } from "react";
import Cookies from "js-cookie";
import { UserWebsocketContext } from "./user-websocket-context";
import useWebSocket, { ReadyState, useEventSource } from "react-use-websocket";
import { NEXT_PUBLIC_GUILDA_WEBSOCKET } from "src/constants/environment-variable.constants";
import { toast } from "react-toastify";
import { ToastCustom } from "src/components/feedback/toast-custom/toast-custom";
import { NotificationsContext } from "../notifications-provider/notifications.context";

interface WSJsonMsgNotification {
  type: string;
  data: {
    idNotification: number;
    idNotificationUser: number;
    idUserReceive: number;
    idUserSend: number;
    message: string;
    nameUserSend: string;
  };
}
interface IProviderProps {
  children: ReactNode;
}

export type WebSocketStatusTypes =
  | "Connecting"
  | "Open"
  | "Closing"
  | "Closed"
  | "Uninstantiated";

export function UserWebsocketProvider({ children }: IProviderProps) {
  const token = Cookies.get("jwtToken") || "";
  const [status, setStatus] = useState<WebSocketStatusTypes>("Connecting");
  const { getCountNotifications } = useContext(NotificationsContext);

  const {
    sendMessage,
    lastMessage,
    readyState,
    getWebSocket,
    lastJsonMessage,
  } = useWebSocket(NEXT_PUBLIC_GUILDA_WEBSOCKET, {
    shouldReconnect: () => {
      return true;
    },
    queryParams: {
      token: token,
    },
    onOpen: () => {
      console.log("WebSocket connection established.");
    },
    onClose(event) {
      console.log("CLOSED", event);
    },
  });

  const connectionStatus = {
    [ReadyState.CONNECTING]: "Connecting",
    [ReadyState.OPEN]: "Open",
    [ReadyState.CLOSING]: "Closing",
    [ReadyState.CLOSED]: "Closed",
    [ReadyState.UNINSTANTIATED]: "Uninstantiated",
  }[readyState];

  useEffect(() => {
    console.log("websocket status: ", connectionStatus);
    setStatus(connectionStatus as WebSocketStatusTypes);
  }, [connectionStatus]);

  useEffect(() => {
    if (lastJsonMessage) {
      const msg = lastJsonMessage as WSJsonMsgNotification;

      ToastCustom({
        subtitle: msg?.data?.message || "",
        title: msg?.data?.nameUserSend || "",
      });

      getCountNotifications();
    }
  }, [lastJsonMessage]);

  return (
    <UserWebsocketContext.Provider value={{ status }}>
      {children}
    </UserWebsocketContext.Provider>
  );
}
