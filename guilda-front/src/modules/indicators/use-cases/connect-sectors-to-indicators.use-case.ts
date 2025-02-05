import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";
import GoalsBySectorModel from "src/typings/models/goals-by-sector.model";

export class ConnectSectorsToIndicators {
  private client = guildaApiClient;

  async handle(props : {indicatorId: number, sectorsIds: number[], goalsBySector: GoalsBySectorModel[], period?: any}) {

    const {indicatorId, sectorsIds, goalsBySector, period} = props

    const payload = {
        sectorsIds: sectorsIds,
        goalsBySector : goalsBySector,
        period: period,
    }

    const { data } = await this.client.post<
      unknown,
      AxiosResponse
    >(`/indicators/${indicatorId}/sectors/v2`, payload);

    return data;
  }
}
