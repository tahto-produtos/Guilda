import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface CreateQuizUseCaseProps {
    IDGDA_COLLABORATOR_DEMANDANT?: number;
    IDGDA_COLLABORATOR_RESPONSIBLE?: number;
    TITLE?: string;
    ORIENTATION?: string;
    DESCRIPTION?: string;
    REQUIRED?: number;
    MONETIZATION?: number;
    PERCENT_MONETIZATION?: number;
    STARTED_AT?: string;
    ENDED_AT?: string;
    visibility: 
    {
        sector: number[];
        subSector: number[];
        period: number[];
        hierarchy: number[];
        group: number[];
        userId: number[];
        client: number[];
        homeOrFloor: number[];
        site: number[];
    };
}

export class CreateQuizUseCase {
  private client = guildaApiClient2;

  async handle(props: CreateQuizUseCaseProps) {
    const payload = props;

    const { data } = await this.client.post<unknown, AxiosResponse>(
      `/CreatedQuiz`,
      payload
    );

    return data;
  }
}
