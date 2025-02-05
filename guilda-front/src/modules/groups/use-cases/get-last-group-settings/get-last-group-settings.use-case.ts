import { AxiosResponse } from "axios";
import { guildaApiClient, guildaApiClient2 } from "../../../../services";
import { HistoryIndicatorSector } from "../../../../typings";
import { GetLastGroupSettingsRequestDto } from "./dto";

class Response {
  goal?: number;
  groups!: HistoryIndicatorSector[];
  period?: string;
}

export class GetLastGroupSettingsUseCase {
/*   private client = guildaApiClient;

  async handle(getLastGroupSettingsRequestDto: GetLastGroupSettingsRequestDto) {
    const { data } = await this.client.get<Response>(
      "/metric-settings",
      {
        params: getLastGroupSettingsRequestDto,
      }
    );

    return data; */
    private client = guildaApiClient2;

    async handle(getLastGroupSettingsRequestDto: GetLastGroupSettingsRequestDto) {

      const { sectorId, indicatorId, period } = getLastGroupSettingsRequestDto;

      const { data } = await this.client.get<
      Response,
      AxiosResponse<Response>
      >(`/MetricSettings?sectorId=${sectorId}&indicatorId=${indicatorId}&period=${period}`);

      return data;
  }
}
