import { guildaApiClient2 } from "src/services";

interface ReportActionEscalationUseCaseProps {
  STARTEDATFROM: string;
  STARTEDATTO: string;
  ENDEDATFROM: string;
  ENDEDATTO: string;
  INDICATORSID: number[];
  SECTORSID: number[];
  SUBSECTORSID: number[];
}

export class ReportActionEscalationUseCase {
  private client = guildaApiClient2;

  async handle(props: ReportActionEscalationUseCaseProps) {
    const { data } = await this.client.post(`/ReportActionEscalation`, props);

    return data;
  }
}
