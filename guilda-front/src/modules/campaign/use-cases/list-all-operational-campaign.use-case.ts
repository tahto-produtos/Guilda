import { guildaApiClient2 } from "src/services";

export interface OperationalCampaignAll {
  IDCAMPAIGN: number;
  NAME: string;
}

export class ListAllOperationalCampaignAvailableUseCase {
  private client = guildaApiClient2;

  async handle({ campaign }: { campaign: string }) {
    const { data } = await this.client.get<OperationalCampaignAll[]>(
      `/listAllOperationalCampaign?campaign=${campaign}`
    );

    return data;
  }
}
