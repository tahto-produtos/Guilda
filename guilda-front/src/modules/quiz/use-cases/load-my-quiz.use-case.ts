import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { ListQuizResponseModel } from "src/typings";
import { MyQuizModel } from "src/typings/models/my-quiz.model";

export class LoadMyQuizUseCase {
  private client = guildaApiClient2;

  async handle() {
    const { data } = await this.client.get<
      unknown,
      AxiosResponse<MyQuizModel[]>
    >(`/LoadMyQuiz`);

    return data;
  }
}
