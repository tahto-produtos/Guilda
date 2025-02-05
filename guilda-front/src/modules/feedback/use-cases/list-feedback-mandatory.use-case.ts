import { guildaApiClient2 } from "src/services";

interface Response {
  ids: number;
}

export class ListFeedBackMandatoryUseCase {
  private client = guildaApiClient2;

  async handle() {
    const { data } = await this.client.get<Response[]>(
      `/ListMyFeedBackMandatory`
    );

    return data;
  }
}
