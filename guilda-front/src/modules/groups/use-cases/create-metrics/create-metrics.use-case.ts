import { CreateMetricsDto } from "./dto";
import { guildaApiClient, guildaApiClient2 } from "../../../../services";
import { AxiosResponse } from "axios";
import { HistoryIndicatorSector } from "../../../../typings";

export class CreateMetricsUseCase {
  private client = guildaApiClient2;

  async handle(createMetricsDto: CreateMetricsDto) {
    const { data } = await this.client.post<
      unknown,
      AxiosResponse<HistoryIndicatorSector[]>,
      CreateMetricsDto
    >("/MetricSettings", createMetricsDto);

    return data;
  }
}
