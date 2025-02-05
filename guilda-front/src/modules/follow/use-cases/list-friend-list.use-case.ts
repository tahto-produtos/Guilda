import { addDays, format, startOfDay } from "date-fns";
import { guildaApiClient2 } from "src/services";

export interface ListFollowUseCaseProps {
  friend: string;
  tierList: boolean;
  personaId: number;
}

export class ListFriendUseCase {
  private client = guildaApiClient2;

  async handle(props: ListFollowUseCaseProps) {
    const { data } = await this.client.get(
      `/PersonaFriendList?friend=${props.friend}&tierList=${props.tierList}&personaId=${props.personaId}`
    );

    return data;
  }
}
