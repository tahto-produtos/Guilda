import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface ListQuizUseCaseProps {
  idQuiz: number;
  idQuizUser: number;
}

export class ListQuizUseCase {
  private client = guildaApiClient2;

  async handle(props: ListQuizUseCaseProps) {
    const { idQuiz, idQuizUser } = props;

    const { data } = await this.client.get<unknown, AxiosResponse>(
      `/ListQuiz?quiz=${idQuiz}&idQuizUser=${idQuizUser}`
    );

    return data;
  }
}
