import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface ListMyNotificationUseCaseProps {
  cod: number;
  name: string;
  limit: number;
  page: number;
}

export class ListMyNotificationUseCase {
  private client = guildaApiClient2;

  async handle(props: ListMyNotificationUseCaseProps) {
    const { cod, name, limit, page } = props;

    const payload = {
      cod,
      name,
      limit,
      page,
    };

    const { data } = await this.client.post<unknown, AxiosResponse>(
      `/LoadMyNotification`,
      payload
    );

    return data;
  }
}
