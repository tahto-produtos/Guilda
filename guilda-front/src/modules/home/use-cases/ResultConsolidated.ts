import { guildaApiClient2 } from "src/services";

export interface ResultConsolidatedUseCaseProps {
  dataInicial: string;
  dataFinal: string;
  codCollaborator: number;
  Hierarchies?: number;
  sectors?: { id: number }[];
  sectorsGroups?: { name: string }[];
  periods?: { id: number }[];
  homeFloors?: { id: number }[];
  sites?: { id: number }[];
  basket?: boolean;
  subSectors: { id: number }[];
  sectorsIds?: { id: number }[];
}

export class ResultConsolidatedUseCase {
  private client = guildaApiClient2;

  async handle(props: ResultConsolidatedUseCaseProps) {
    const {
      Hierarchies,
      codCollaborator,
      dataFinal,
      dataInicial,
      periods,
      homeFloors,
      sites,
      sectors,
      sectorsGroups,
      basket,
      subSectors,
      sectorsIds,
    } = props;

    const payload = {
      dataFinal,
      dataInicial,
      codCollaborator,
      Hierarchies: Hierarchies || "",
      periods: periods ? periods : [],
      homeFloors: homeFloors? homeFloors : [],
      sites: sites? sites : [],
      sectors: sectors ? sectors : [],
      sectorsGroups: sectorsGroups ? sectorsGroups : [],
      Indicators: [],
      basket,
      subSectors,
      sectorsIds,
    };

    const { data } = await this.client.post(`/ResultConsolidated`, payload);

    return data;
  }
}
