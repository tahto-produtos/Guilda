import { guildaApiClient2 } from "src/services";
import { OperationalCampaign } from "src/typings/models/operational-campaign.model";

interface Response {
  totalpages: number;
  MyOperationalCampaign: OperationalCampaign[];
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

export class LoadMyOperationalCampaignUseCase {
  private client = guildaApiClient2;

  async handle(props: IProps) {
    const { data } = await this.client.post<Response>(
      `/LoadMyOperationalCampaign`,
      props
    );

    return data;
  }
}
