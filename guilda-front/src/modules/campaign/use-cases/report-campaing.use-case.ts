import { guildaApiClient2 } from "src/services";

interface ReportCampaignUseCaseProps {
    STARTEDATFROM: string;
    STARTEDATTO: string;
    ENDEDATFROM: string;
    ENDEDATTO: string;
    INDICATORSID: number[];
}

export class ReportCampaignUseCase {
  private client = guildaApiClient2;

  async handle(props: ReportCampaignUseCaseProps) {
    const { data } = await this.client.post(
      `/ReportOperationalCampaign`,
      props
    );

    return data;
  }
}
