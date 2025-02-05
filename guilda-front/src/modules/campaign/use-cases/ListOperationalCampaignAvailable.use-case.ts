import { guildaApiClient2 } from "src/services";
import { OperationalCampaign } from "src/typings/models/operational-campaign.model";

interface Response {
  totalpages: number;
  OperationalCampaignAvailable: OperationalCampaign[];
}

interface IProps {
  STARTEDATFROM: string;
  STARTEDATTO: string;
  ENDEDATFROM: string;
  ENDEDATTO: string;
  NAME: string;
  limit: number;
  page: number;
}

export class ListOperationalCampaignAvailableUseCase {
  private client = guildaApiClient2;

  async handle(props: IProps) {
    const { data } = await this.client.post<Response>(
      `/ListOperationalCampaignAvailable`,
      props
    );

    return data;
  }
}
