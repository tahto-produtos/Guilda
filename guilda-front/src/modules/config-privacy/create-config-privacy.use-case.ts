import { guildaApiClient2 } from "src/services";

interface CreateConfigPrivacyUseCaseProps {
  idPublicPrivate: number;
  sector: number[];
  subsector: number[];
  period: number[];
  hierarchy: number[];
  group: number[];
  client: number[];
  homeOrFloor: number[];
  site: number[];
  userId: number[];
}

export class CreateConfigPrivacyUseCase {
  private client = guildaApiClient2;

  async handle(props: CreateConfigPrivacyUseCaseProps) {
    const { data } = await this.client.post(
      `/PersonaConfigPublicPrivate`,
      props
    );

    return data;
  }
}
