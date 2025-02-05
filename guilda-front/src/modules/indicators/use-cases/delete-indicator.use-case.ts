import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";
import { CreateIndicatorRequestDto, CreateIndicatorResponseDto } from "../forms/create-indicator/dto";
import { DeleteIndicatorRequestDto } from "../forms/create-indicator/dto/delete-indicator.request-dto";

export class DeleteIndicatorUseCase {
  private client = guildaApiClient;

  async handle(id : number) {
    const { data } = await this.client.delete<
      unknown,
      AxiosResponse<CreateIndicatorResponseDto>
    >(`/indicators?indicatorsIds=${id}`);

    return data;
  }
}
