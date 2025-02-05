import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { ListQuizResponseModel } from "src/typings";
import { MyQuizModel } from "src/typings/models/my-quiz.model";

interface LoadMyQuizUseCaseProps {
  DataInicial?: string;
  DataFinal?: string;
  DataInicialCriacaoQuiz?: string;
  DataFinalCriacaoQuiz?: string;
  DataInicialPublicacaoQuiz?: string;
  DataFinalPublicacaoQuiz?: string;
  DataInicialRespostaQuiz?: string;
  DataFinalRespostaQuiz?: string;
  Title?: string;
  Users?: number[];
  Hierachies?: number[];
  Cities?: number[];
  Sites?: number[];
  Clients?: number[];
  idRequest?: number;
  idCreated?: number;
}

export class ReportQuizUseCase {
  private client = guildaApiClient2;

  async handle(props: LoadMyQuizUseCaseProps) {
    const { data } = await this.client.post<unknown, AxiosResponse>(
      `/ReportQuiz`,
      props
    );

    return data;
  }
}
