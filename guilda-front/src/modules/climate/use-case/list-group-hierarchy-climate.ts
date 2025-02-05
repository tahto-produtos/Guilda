import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface ListGroupReasonProps {
  STARTEDATFROM: string;
  STARTEDATTO: string;
}

export interface ListGroupReasonResponse {
  idClimate: number;
  climate: string;
  image: string;
  count: number;
  percent: number;
}
export class ListGroupReason {
  private client = guildaApiClient2;

  async handle(props: ListGroupReasonProps) {
    const { data } = await this.client.get<
      unknown,
      AxiosResponse<ListGroupReasonResponse[]>
    >(
      `/ListGroupClimate?STARTEDATFROM=${props.STARTEDATFROM}&STARTEDATTO=${props.STARTEDATTO}`
    );

    return data;
  }
}
