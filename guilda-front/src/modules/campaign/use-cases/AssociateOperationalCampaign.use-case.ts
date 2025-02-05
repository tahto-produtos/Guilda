import { guildaApiClient2 } from "src/services";
import {
  OperationalCampaign,
  OperationalCampaignDetails,
} from "src/typings/models/operational-campaign.model";

interface IProps {
  IDGDA_OPERATIONAL_CAMPAIGN: number;
}

export class AssociateOperationalCampaignUseCase {
  private client = guildaApiClient2;

  async handle(props: IProps) {
    const { data } = await this.client.post<OperationalCampaignDetails>(
      `/AssociateOperationalCampaign`,
      props
    );

    return data;
  }
}
