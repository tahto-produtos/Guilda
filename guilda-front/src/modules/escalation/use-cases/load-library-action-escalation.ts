import { guildaApiClient2 } from "src/services";

export interface LoadActionEscalation {
  IDGDA_ESCALATION_ACTION: number;
  NOME: string;
  INDICADOR: string;
  STATUS: string;
  startedAt: string;
  endedAt: string;
}

interface Response {
  totalpages: number;
  LoadActionEscalation: LoadActionEscalation[];
}

interface LoadLibraryActionEscalationUseCaseProps {
  CREATEDATFROM: string;
  CREATEDATTO: string;
  STARTEDATFROM: string;
  STARTEDATTO: string;
  ENDEDATFROM: string;
  ENDEDATTO: string;
  NAME: string;
  DESCRIPTION: string;
  INDICATOR: number[];
  AUTOMATIC: 0 | 1;
  LIMIT: number;
  PAGE: number;
}

export class LoadLibraryActionEscalationUseCase {
  private client = guildaApiClient2;

  async handle(props: LoadLibraryActionEscalationUseCaseProps) {
    const { data } = await this.client.post<Response>(
      `/LoadLibraryActionEscalation`,
      props
    );

    return data;
  }
}
