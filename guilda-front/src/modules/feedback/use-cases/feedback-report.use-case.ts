import { guildaApiClient2 } from "src/services";

interface FeedbackReportUseCaseProps {
  DATE_START: string;
  DATE_END: string;
}

export class FeedbackReportUseCase {
  private client = guildaApiClient2;

  async handle(props: FeedbackReportUseCaseProps) {
    const { data } = await this.client.post(`/ReportFeedBack`, props);

    return data;
  }
}
