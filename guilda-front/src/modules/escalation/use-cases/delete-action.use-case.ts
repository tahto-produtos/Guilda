import { guildaApiClient2 } from "src/services";
import { ActionDetails } from "src/typings/models/action-details.model";

interface Response {}

interface ShowActionDetailsUseCaseProps {
  idAction: number | string;
  automatic: number;
}

export class DeleteActionUseCase {
  private client = guildaApiClient2;

  async handle(props: ShowActionDetailsUseCaseProps) {
    const { data } = await this.client.delete<ActionDetails>(
      `/CreatedActionEscalation?idAction=${props.idAction}&automatic=${props.automatic}`
    );

    return data;
  }
}
