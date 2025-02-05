import { guildaApiClient2 } from "src/services";
import {
  OperationalCampaign,
  OperationalCampaignDetails,
} from "src/typings/models/operational-campaign.model";

interface IProps {
  IDGDA_OPERATIONAL_CAMPAIGN: number,
  ISIMPORTANT: boolean | null,
}

export class DetailsOperationalCampaignUseCase {
  private client = guildaApiClient2;

  async handle(props: IProps) {
    const { data } = await this.client.post<OperationalCampaignDetails>(
      `/DetailsOperationalCampaign`,
      props
    );

    return data;
  }
}

export interface DetailsInformationOperationalCampaign {
  idCampaign: number;
  name: string;
  image: string;
  pontuation: string;
  max_pontuation: string;
  position: number;
  dtInicio: string;
  dtFim: string;
  showButtonPay: number;
  pay: number;
  missions: {
    mission_type: string;
    mission_indicator: string;
    mission_text: string;
  }[];
  rankings: {
    position: string;
    name: string;
    status: string;
    pontuation: string;
    award: string;
  }[];
}

export class DetailsInformationOperationalCampaignUseCase {
  private client = guildaApiClient2;

  async handle(props: IProps) {
    const { data } =
      await this.client.post<DetailsInformationOperationalCampaign>(
        `/DetailsInformationOperationalCampaign`,
        props
      );

    return data;
  }
}
