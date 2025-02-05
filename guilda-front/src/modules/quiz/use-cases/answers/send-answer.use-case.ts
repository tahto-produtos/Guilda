import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface SendAnswerUseCaseProps {
  IDGDA_QUIZ_USER: number;
  IDGDA_QUIZ_QUESTION: number;
  IDGDA_QUIZ_ANSWER: number[];
  NO_ANSWER: number;
  ANSWER: string;
}

export class SendAnswerUseCase {
  private client = guildaApiClient2;

  async handle(props: SendAnswerUseCaseProps) {
    const payload = props;
    const { data } = await this.client.post<unknown, AxiosResponse>(
      `/SendAnswerUser`,
      payload
    );

    return data;
  }
}
