import { guildaApiClient2 } from "src/services";

interface ListFeedBackUseCaseProps {
  Hierarchy: number[];
  Site: number[];
  Sector: number[];
  SubSector: number[];
  Userid: number[];
  Groups: number[];
  NameBc: string;
  limit: number;
  page: number;
}

export class ListFeedBackUseCase {
  private client = guildaApiClient2;

  async handle(props: ListFeedBackUseCaseProps) {
    const { data } = await this.client.post(`/ListFeedBack`, props);

    return data;
  }
}
