import { guildaApiClient2 } from "src/services";

export interface IndicatorPerformanceActionEscalation {
  STARTEDATFROM: string;
  STARTEDATTO: string;
  INDICATORSID: number;
  SECTORSID: number;
  SUBSECTORSID: number;
  IDHOMEBASED: number;
  IDGROUP: number;
  IDSITE: number;
  RESULT: number;
  PERCENT: number;
  GOAL: number;
  INDICATORNAME?: string;
}

interface IndicatorPerformanceActionEscalationUseCaseProps {
  STARTEDATFROM: string;
  STARTEDATTO: string;
  INDICATORSID?: number;
  SECTORSID?: number;
  SUBSECTORSID?: number;
  IDHOMEBASED?: number;
  IDGROUP?: number;
  IDSITE?: number;
}

export class IndicatorPerformanceActionEscalationUseCase {
  private client = guildaApiClient2;

  async handle(props: IndicatorPerformanceActionEscalationUseCaseProps) {
    const { data } = await this.client.post<
      IndicatorPerformanceActionEscalation | string
    >(`/IndicatorPerformanceActionEscalation`, props);

    return data;
  }
}
