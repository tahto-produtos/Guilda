import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";

export class UpdateProfilePermissionsUseCase {
  private client = guildaApiClient;

  async handle(props : {profileId: number, permissionsId: Array<number>,}) {

    const {profileId, permissionsId} = props

    const payload = {
        permissionsId: permissionsId,
    }

    const { data } = await this.client.put<
      unknown,
      AxiosResponse
    >(`/profiles/${profileId}/permissions`, payload);

    return data;
  }
}

