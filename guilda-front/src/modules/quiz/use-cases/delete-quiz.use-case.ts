import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface DeleteQuizUseCaseProps {
  IDGDA_QUIZ: number[];
  VALIDADETED: boolean;
}

export class DeleteQuizUseCase {
  private client = guildaApiClient2;

  async handle(props: DeleteQuizUseCaseProps) {
    const payload = props;
    console.log(payload);
    const { data } = await this.client.post<unknown, AxiosResponse>(
      `/DeletedQuiz`,
      payload
    );

    return data;
  }
}
