import { guildaApiClient2 } from "src/services";

interface Response {}

interface CreateAutoActionUseCaseProps {
  IDGDA_INDICATOR: number;
  IDGDA_SECTOR: number;
  IDGDA_SUBSECTOR?: number | null;
  GROUP: number;
  PERCENTAGE_DETOUR: number;
  TOLERANCE_RANGE: number;
  LIST_STAGES?: {
    IDGDA_HIERARCHY: number[];
    NUMBER_STAGE: number;
  }[];
}

export class CreateAutoActionUseCase {
  private client = guildaApiClient2;

  async handle(props: CreateAutoActionUseCaseProps) {
    const { data } = await this.client.post<Response>(
      `/CreatedAutomaticActionEscalation`,
      props
    );

    return data;
  }
}
