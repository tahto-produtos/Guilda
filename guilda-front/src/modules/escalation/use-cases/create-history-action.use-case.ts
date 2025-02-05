import { guildaApiClient2 } from "src/services";

interface Response {}

interface CreateHistoryActionUseCaseProps {
  IDGDA_ESCALATION_ACTION: number;
  ACTION_REALIZE: string;
}

export class CreateHistoryActionUseCase {
  private client = guildaApiClient2;

  async handle(props: CreateHistoryActionUseCaseProps) {
    const { data } = await this.client.post<Response>(
      `/CreatedHistoryActionEscalation`,
      props
    );

    return data;
  }
}
