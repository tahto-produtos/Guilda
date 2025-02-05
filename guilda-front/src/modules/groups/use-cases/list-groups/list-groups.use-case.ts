import { guildaApiClient } from "../../../../services";
import { AxiosResponse } from "axios";
import { PaginationModel } from "src/typings/models/pagination.model";

export class ListGroupsUseCase {
  private client = guildaApiClient;

  async handle(props : PaginationModel) {

    const {limit, offset, searchText, startDate, endDate} = props

    const { data } = await this.client.get<
      unknown,
      AxiosResponse
    >(`/groups?limit=${limit}&offset=${offset}&searchText=${searchText ? searchText : ""}${startDate ? `&startDate=${startDate}` : ""}${endDate ? `&endDate=${endDate}` : ""}`);

    return data;
  }
}
