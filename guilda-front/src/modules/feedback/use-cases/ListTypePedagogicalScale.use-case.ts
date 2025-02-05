import { guildaApiClient2 } from "src/services";

export class ListTypePedagogicalScaleUseCase {
  private client = guildaApiClient2;

  async handle() {
    const { data } = await this.client.get(`/ListTypePedagogicalEscale`);

    return data;
  }
}
