import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface DeleteNotificationsUseCaseProps {
  idNotification: number[];
}

export class GeneralDeleteUserNotificationsUseCase {
  private client = guildaApiClient2;

  async handle(props: DeleteNotificationsUseCaseProps) {
    const { data } = await this.client.post<unknown, AxiosResponse>(
      `/GeneralCleaningUserNotification`,
      props
    );

    return data;
  }
}
