import Cookies from "js-cookie";
import { jwtTokenKey } from "../../../../constants";
import { guildaApiClient } from "../../../../services";

export class LogoutUseCase {
  private client = guildaApiClient;

  async handle() {
    Cookies.remove(jwtTokenKey);
    this.client.defaults.headers.common.Authorization = undefined;
  }
}
