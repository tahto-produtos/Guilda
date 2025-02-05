import { guildaApiClient2 } from "src/services";

export class ListPedagogicalEscaleUseCase {
  private client = guildaApiClient2;

  async handle() {
    const { data } = await this.client.get(`/ListPedagogicalEscale`);

    return data;
  }
}
