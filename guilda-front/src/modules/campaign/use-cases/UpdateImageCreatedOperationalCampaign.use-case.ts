import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import {
  OperationalCampaign,
  OperationalCampaignDetails,
} from "src/typings/models/operational-campaign.model";

interface IProps {
  file: File;
  ID_OPERATIONAL_CAMPAIGN: number;
}

export class UpdateImageCreatedOperationalCampaignUseCase {
  private client = guildaApiClient2;

  async handle(props: IProps) {
    const form = new FormData();
    form.append("FILES", props.file);
    form.append(
      "ID_OPERATIONAL_CAMPAIGN",
      props.ID_OPERATIONAL_CAMPAIGN.toString()
    );

    const { data } = await this.client.post<unknown, AxiosResponse, FormData>(
      `/UpdateImageCreatedOperationalCampaign`,
      form
    );

    return data;
  }
}
