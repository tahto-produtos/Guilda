export interface OperationalCampaign {
  IDGDA_OPERATIONAL_CAMPAIGN: number;
  NAME: string;
  IMAGE: string;
}

export interface OperationalCampaignDetails {
  idCampaign: number;
  name: string;
  image: string;
  punctuation: string;
  position: number;
  dtInicio: string;
  dtFim: string;
  mission_Concluded: string;
  mission_Punctuation: number;
  mission_Status: string;
  mission_Percent: number;
  value_win: number;
}
