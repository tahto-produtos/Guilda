import { guildaApiClient2 } from "src/services";

interface ListFeedBackUseCaseProps {
  id: number;
}

export class DeleteIngractionUseCase {
  private client = guildaApiClient2;

  async handle(props: ListFeedBackUseCaseProps) {
    const { data } = await this.client.delete(
      `/CreatedPedagogicalScale?idPedagogicalScale=${props.id}`
    );

    return data;
  }
}
