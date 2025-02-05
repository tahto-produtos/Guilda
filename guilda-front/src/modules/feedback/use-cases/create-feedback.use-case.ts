import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface CreateFeedBackUseCaseProps {
  IDGDA_PEDAGOGICAL_SCALE_GRAVITY: number;
  IDGDA_PEDAGOGICAL_SCALE: number;
  IDPERSONA_RECEIVED_BY: number;
  REASON: string;
  DETAILS?: string;
  FILES?: File[];
}

export class CreateFeedBackUseCase {
  private client = guildaApiClient2;

  async handle(props: CreateFeedBackUseCaseProps) {
    const form = new FormData();

    form.append("json", JSON.stringify(props));
    if (props.FILES && props.FILES.length > 0) {
      for (let i = 0; i < props.FILES.length; i++) {
          form.append(`FILES[${i}]`, props.FILES[i]);
      }
  }

    const { data } = await this.client.post<unknown, AxiosResponse, FormData>(
      `/CreatedFeedback`,
      form
    );

    return data;
  }
}
