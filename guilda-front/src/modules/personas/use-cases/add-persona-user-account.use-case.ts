import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface CityPersonaUseCaseProps {
  IDPERSONAUSER: number;
  ids: number[];
}

export class AddPersonaUserAccountUseCase {
  private client = guildaApiClient2;

  async handle(props: CityPersonaUseCaseProps) {
    const { data } = await this.client.post<unknown, AxiosResponse>(
      `/AddPersonaUserAccount`,
      props
    );

    return data;
  }
}
