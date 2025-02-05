import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";


  

export class ListProfilePermissionsUseCase {
  private client = guildaApiClient;

  async handle(id : number) {


    const { data } = await this.client.get<
      unknown,
      AxiosResponse
    >(`/profiles/${id}`);

    return data;
  }
}
