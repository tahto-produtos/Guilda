import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";
import { PaginationModel } from "src/typings/models/pagination.model";
import { PermissionModel } from "src/typings/models/permission.model";
import { ProfileModel } from "src/typings/models/profile.model";

export class ListPermissionsResponseDto {
    items!: PermissionModel[];
  }
  

export class ListPermissionsUseCase {
  private client = guildaApiClient;

  async handle(props : PaginationModel) {

    const {limit, offset} = props

    const { data } = await this.client.get<
      unknown,
      AxiosResponse<ListPermissionsResponseDto>
    >(`/permissions?limit=${limit}&offset=${offset}`);

    return data;
  }
}
