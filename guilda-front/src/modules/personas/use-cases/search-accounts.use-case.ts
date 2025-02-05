import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export class SearchAccountsUseCase {
  private client = guildaApiClient2;

  async handle(props: { Collaborator: string; limit: number; page: number }) {
    const { data } = await this.client.get<unknown, AxiosResponse>(
      `/SearchAccounts?Collaborator=${props.Collaborator}&limit=${props.limit}&page=${props.page}`
    );

    return data;
  }
}
