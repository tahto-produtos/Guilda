import { guildaApiClient2 } from "src/services";

export class ListGravityPedagogicalEscale {
  private client = guildaApiClient2;

  async handle() {
    const { data } = await this.client.get(`/ListGravityPedagogicalEscale`);

    return data;
  }
}
