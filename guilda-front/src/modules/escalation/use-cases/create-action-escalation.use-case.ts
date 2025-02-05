import { guildaApiClient2 } from "src/services";

interface Response {}

interface CreateAutoActionUseCaseProps {
  IDGDA_INDICATOR: number;
  IDGDA_PERSONA_RESPONSIBLE_CREATION: number;
  IDGDA_PERSONA_RESPONSIBLE_ACTION: number;
  IDGDA_SECTOR: number;
  IDGDA_SUBSECTOR?: number | null;
  NAME: string;
  ACTION_REALIZED?: string;
  DESCRIPTION: string;
  STARTED_AT: string;
  ENDED_AT: string;
  LIST_STAGES?: {
    IDGDA_HIERARCHY: number[];
    NUMBER_STAGE: number;
  }[];
}

export class CreateActionEscalationUseCase {
  private client = guildaApiClient2;

  async handle(props: CreateAutoActionUseCaseProps) {
    const { data } = await this.client.post<Response>(
      `/CreatedActionEscalation`,
      props
    );

    return data;
  }
}
