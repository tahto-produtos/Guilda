import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface DoClimateUseCaseProps {
  idClimate: number;
  idClimateReason: number;
}

export class DoClimateUseCase {
  private client = guildaApiClient2;

  async handle(props: DoClimateUseCaseProps) {
    const { data } = await this.client.post<unknown, AxiosResponse>(
      `/DoClimate`,
      props
    );

    return data;
  }
}
