import {
  CampaignEliminationCriteria,
  CampaignPontuationCriteria,
  CampaignRankingItem,
} from "src/pages/campaigns/create-campaign";
import { guildaApiClient2 } from "src/services";

interface IProps {
  NAME_CAMPAIGN: string;
  DESCRIPTION: string;
  STARTED_AT: string;
  ENDED_AT: string;
  VISIBILITY: {
    SECTOR: number[];
    SUBSECTOR: number[];
    HIERARCHY: number[];
    HOMEORFLOOR: number[];
    GROUP: number[];
    VETERANONOVADO: number[];
  };
  PONTUATION: CampaignPontuationCriteria[];
  RANKING: {
    //Caso seja Moedas virtuais, preencher as informações de Value_coins
    //Caso seja Itens especificos preencher o id_product, QUANTITY_PRODUCT
    //Caso seja primeiras colocações preencher o POSITION
    //Caso seja Pontuação minima preencher o MIN_PONTUATION
    ID_RANKING_AWARD_PUBLIC: number; // Sendo 1 = Primeiras colocações || 2 = Pontuação minima
    ID_RANKING_AWARD_TYPE: number; // Sendo 1 = Moedas Virtuais || 2 = Itens especificos
    ID_RANKING_PAY_OPTION: number; // Sendi 1 = Automatico || 2 = Manual
    RANKING_ITENS: CampaignRankingItem[];
  };
  ELIMINATION: CampaignEliminationCriteria[];
}
export class CreateOperationalCampaignUseCase {
  private client = guildaApiClient2;

  async handle(props: IProps) {
    const { data } = await this.client.post<{
      ID_OPERATIONAL_CAMPAIGN: number;
    }>(`/CreatedOperationalCampaign`, props);

    return data;
  }
}
