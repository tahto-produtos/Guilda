import { guildaApiClient2 } from "src/services";
import { OperationalCampaign } from "src/typings/models/operational-campaign.model";

export class ProductGeneralUseCase {
  private client = guildaApiClient2;

  async handle({ product }: { product: string }) {
    const { data } = await this.client.get(
      `/ProductGeneral?product=${product}`
    );

    return data;
  }
}
