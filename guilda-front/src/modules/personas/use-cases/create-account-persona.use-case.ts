import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { Hobby } from "src/typings/models/hobby.model";

export interface EditPersonasUseCaseProps {
  NOME?: string;
  TELEFONE?: string;
  UF?: number;
  SITE?: number;
  CIDADE?: number;
  WHO_IS?: string;
  NOME_SOCIAL?: string;
  MOTIVACOES?: string;
  OBJETIVO?: string;
  HOBBIES?: Hobby[];
  TYPE: number;
  VISIBILITY: number;
  PERSONATAHTO: 0 | 1;
}

export class CreateAccountPersonasUseCase {
  private client = guildaApiClient2;

  async handle(props: EditPersonasUseCaseProps) {
    const payload = props;

    if (payload.HOBBIES && payload.HOBBIES?.length > 0) {
      const newHobbiesArray = payload.HOBBIES.map((hobby) => ({
        HOBBY: hobby.HOBBY,
        IDGDA_HOBBIES: hobby.IDGDA_PERSONA_USER_HOBBY
          ? hobby.IDGDA_PERSONA_USER_HOBBY
          : hobby.IDGDA_HOBBIES,
      }));
      payload.HOBBIES = newHobbiesArray;
    }

    const form = new FormData();
    form.append("json", JSON.stringify(payload));

    const { data } = await this.client.post<unknown, AxiosResponse, FormData>(
      `/CreatedAccountPersona`,
      form
    );

    return data;
  }
}
