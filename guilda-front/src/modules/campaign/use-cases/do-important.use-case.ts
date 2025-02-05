import { guildaApiClient2 } from "src/services";
import { ActionDetails } from "src/typings/models/action-details.model";

interface Response {}

interface AlertImportantUseCaseProps {
  idCampaign: number;
}

export class AlertImportantUseCase {
  private client = guildaApiClient2;

  async handle(props: AlertImportantUseCaseProps) {
    const { data } = await this.client.post<Response>(
      `/DoImportantOperationalCampaign`,
      props
    );

    return data;
  }
}
