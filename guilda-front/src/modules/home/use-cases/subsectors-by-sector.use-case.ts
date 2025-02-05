import { guildaApiClient2 } from "src/services";

export interface IProps {
  dtInicial: string;
  dtFinal: string;
  sector: number;
  sectorIds?: number[];
}

export class SubSectorsBySectorUseCase {
  private client = guildaApiClient2;

  async handle(props: IProps) {


    const {
      dtInicial,
      dtFinal,
      sector,
      sectorIds,
    } = props;

    const payload = {
      dtInicial,
      dtFinal,
      sector,
      sectorIds: sectorIds ? sectorIds : [],
    };

    const { data } = await this.client.post(`/SubSectorsBySector`, payload);

/*     const { dataFinal, dataInicial, sectorId, sectorIds } = props;

    const { data } = await this.client.get(
      `/SubSectorsBySector?sector=${sectorId}&dtInicial=${dataInicial}&dtFinal=${dataFinal}`
    ); */

    return data;
  }
}
