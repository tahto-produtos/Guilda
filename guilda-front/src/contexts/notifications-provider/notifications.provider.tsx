import { ReactNode, useContext, useEffect, useState } from "react";
import { UserInfoContext } from "../user-context/user.context";
import { CountNotificationUseCase } from "src/modules/notifications/use-cases/count-notification.use-case";
import { NotificationsContext } from "./notifications.context";

interface IProviderProps {
  children: ReactNode;
}

export function NotificationsProvider({ children }: IProviderProps) {
  const { myUser } = useContext(UserInfoContext);

  const [count, setCount] = useState<number>(0);

  async function getCountNotifications() {
    await new CountNotificationUseCase()
      .handle()
      .then((data) => {
        setCount(data.quantity);
      })
      .catch(() => {})
      .finally(() => {});
  }

  useEffect(() => {
    getCountNotifications();

    const intervalId = setInterval(getCountNotifications, 10 * 60 * 1000);
    return () => clearInterval(intervalId);
  }, []);

  return (
    <NotificationsContext.Provider value={{ count, getCountNotifications }}>
      {children}
    </NotificationsContext.Provider>
  );
}
