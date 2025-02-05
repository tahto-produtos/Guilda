import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";


export class DeleteSectorUseCase {
  private client = guildaApiClient;

  async handle(id : number) {
    const { data } = await this.client.delete<
      unknown,
      AxiosResponse
    >(`/sectors/${id}`);

    return data;
  }
}
