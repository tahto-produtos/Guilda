import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export class ListClimateReasonsUseCase {
  private client = guildaApiClient2;

  async handle({ id }: { id: number }) {
    const { data } = await this.client.get<unknown, AxiosResponse>(
      `/ListClimateReason?idClimate=${id}`
    );

    return data;
  }
}
