import { guildaApiClient2 } from "src/services";

export interface OperationalCampaignOther {
  TOTALCOST: number;
  PARTICIPATINGOPERATORS: number;
  GRIP: number;
}

export class ListOtherOperationalCampaignAvailableUseCase {
  private client = guildaApiClient2;

  async handle({ codCampanha }: { codCampanha: number }) {
    const { data } = await this.client.get<OperationalCampaignOther>(
      `/ListOtherOperationalCampaign?codCampanha=${codCampanha}`
    );

    return data;
  }
}
