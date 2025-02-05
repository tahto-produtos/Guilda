import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface SendFeedbackClimatekUseCaseProps {
  idClimateUser: number;
  idClimateApplyType: number;
}

export class SendFeedbackClimateUseCase {
  private client = guildaApiClient2;

  async handle(props: SendFeedbackClimatekUseCaseProps) {
    const { data } = await this.client.post<unknown, AxiosResponse>(
      `/FeedBackClimate`,
      props
    );

    return data;
  }
}
