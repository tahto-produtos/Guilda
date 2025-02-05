import { guildaApiClient2 } from "src/services";

interface Response {}

interface CreateStageActionUseCaseProps {
  IDGDA_ESCALATION_ACTION: number;
  AUTOMATIC: number;
  IDGDA_HIERARCHY: number[];
  NUMBER_STAGE: number;
}

export class CreateStageActionUseCase {
  private client = guildaApiClient2;

  async handle(props: CreateStageActionUseCaseProps) {
    const { data } = await this.client.post<Response>(
      `/CreatedStageActionEscalation`,
      props
    );

    return data;
  }
}
