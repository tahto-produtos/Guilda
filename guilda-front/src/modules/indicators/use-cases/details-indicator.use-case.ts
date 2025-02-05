import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";

export class DetailsIndicatorProps {
  id!: number | number[] | string | string[];
}

export class DetailsIndicatorUseCase {
  private client = guildaApiClient;

  async handle(props: DetailsIndicatorProps) {
    const { id } = props;


    const { data } = await this.client.get<
      unknown,
      AxiosResponse
    >(`/indicators/${id}`);

    return data;
  }
}
