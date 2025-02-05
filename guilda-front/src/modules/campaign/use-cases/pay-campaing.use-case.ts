import { guildaApiClient2 } from "src/services";
import {
  OperationalCampaign,
  OperationalCampaignDetails,
} from "src/typings/models/operational-campaign.model";

interface PayCampaignUseCaseProps {
    idCampaign: number;
}

export class PayCampaignUseCase {
  private client = guildaApiClient2;

  async handle(props: PayCampaignUseCaseProps) {
    const { data } = await this.client.post(
      `/DoPayOperationalCampaign`,
      props
    );

    return data;
  }
}
