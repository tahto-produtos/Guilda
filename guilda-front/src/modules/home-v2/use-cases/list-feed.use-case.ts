import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export interface ListFeedUseCaseProps {
  feed: boolean;
  feedTahto: boolean;
  specificUserId: number;
  isAdm: boolean;
  userConfigPost: {
    sector: number[];
    subsector: number[];
    period: number;
    hierarchy: number;
    group: number;
    client: [];
    homeOrFloor: number;
  };
  limit: number;
  page: number;
}
export class ListFeedUseCase {
  private client = guildaApiClient2;

  async handle(props: ListFeedUseCaseProps) {
    const { data } = await this.client.post<unknown, AxiosResponse>(
      `/PersonaListPost`,
      props
    );

    return data;
  }
}
