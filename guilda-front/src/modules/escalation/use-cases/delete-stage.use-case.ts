import { guildaApiClient2 } from "src/services";
import { ActionDetails } from "src/typings/models/action-details.model";

interface Response {}

interface ShowActionDetailsUseCaseProps {
  IDGDA_ESCALATION_ACTION_STAGE: number;
  AUTOMATIC: number;
}

export class DeleteStageUseCase {
  private client = guildaApiClient2;

  async handle(props: ShowActionDetailsUseCaseProps) {
    const { data } = await this.client.delete<ActionDetails>(
      `/CreatedStageActionEscalation?IDGDA_ESCALATION_ACTION_STAGE=${props.IDGDA_ESCALATION_ACTION_STAGE}&AUTOMATIC=${props.AUTOMATIC}`
    );

    return data;
  }
}
