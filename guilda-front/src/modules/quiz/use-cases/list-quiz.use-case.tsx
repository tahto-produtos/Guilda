import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { ListQuizResponseModel } from "src/typings";

interface ListQuizUseCaseProps {
  CREATEDATFROM: string;
  CREATEDATTO: string;
  STARTEDATFROM: string;
  STARTEDATTO: string;
  ENDEDATFROM: string;
  ENDEDATTO: string;
  TITLE: string;
  SITES: number[];
  STATUS: string;
  WORD: string;
  IDCREATOR: number;
  DESCRIPTION: string;
  REQUIRED: number;
  LIMIT: number;
  PAGE: number;
  FLAGRELEVANCE: number;
  HIERARCHY: number[];
  QUEM_CRIOU: number[];
  QUEM_SOLICITOU: number[];
  CLIENT: number[];
}

export class ListQuizzesUseCase {
  private client = guildaApiClient2;

  async handle(props: ListQuizUseCaseProps) {
    const payload = props;

    const { data } = await this.client.post<
      unknown,
      AxiosResponse<ListQuizResponseModel>
    >(`/LoadLibraryQuiz`, payload);

    return data;
  }
}
