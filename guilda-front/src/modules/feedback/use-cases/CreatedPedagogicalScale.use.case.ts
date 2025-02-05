import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface IProps {
  NAME_INFRACTION: string;
  CODE: number;
  IDGDA_PEDAGOGICAL_SCALE_TYPE: number;
  IDGDA_PEDAGOGICAL_SCALE_GRAVITY: number;
  TIME_OFF: number;
  PEDAGOGICAL_ORDER: number;
}

export class CreatePedagogicalScaleUseCase {
  private client = guildaApiClient2;

  async handle(props: IProps) {
    const { data } = await this.client.post<unknown, AxiosResponse>(
      `/CreatedPedagogicalScale`,
      props
    );

    return data;
  }
}
