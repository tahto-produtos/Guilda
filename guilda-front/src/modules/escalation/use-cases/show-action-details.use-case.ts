import { guildaApiClient2 } from "src/services";
import { ActionDetails } from "src/typings/models/action-details.model";

interface Response {}

interface ShowActionDetailsUseCaseProps {
  idAction: number | string;
  automatic: 0 | 1;
}

export class ShowActionDetailsUseCase {
  private client = guildaApiClient2;

  async handle(props: ShowActionDetailsUseCaseProps) {
    const { data } = await this.client.get<ActionDetails>(
      `/LoadActionEscalationDetails?idAction=${props.idAction}&automatic=${props.automatic}`
    );

    return data;
  }
}
