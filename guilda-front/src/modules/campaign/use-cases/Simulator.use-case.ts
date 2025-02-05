import { guildaApiClient2 } from "src/services";

interface Response {
  costCampaign: number;
  indices: SimulatorIndice[];
}

export interface SimulatorIndice {
  indice: string;
  hc: number;
  coinsMonth: number;
  fullPotentialCoins: number;
  fullPotentialTotal: number;
  total60: number;
  evol: number;
  payMonth: number;
  range: number;
}

export class SimulatorUseCase {
  private client = guildaApiClient2;

  async handle({ sector }: { sector: number }) {
    const { data } = await this.client.get<Response>(
      `/ListSimulatorOperationalCampaign?sector=${sector}`
    );

    return data;
  }
}
