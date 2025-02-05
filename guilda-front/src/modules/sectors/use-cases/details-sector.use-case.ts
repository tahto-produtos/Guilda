import { AxiosResponse } from "axios";
import FormData from "form-data";
import { guildaApiClient } from "src/services";
import { CreateSectorResponseDto } from "../forms";
import { DetailsSectorRequestDto } from "./dto/details-sector.use-case";

export class DetailsSectorUseCase {
  private client = guildaApiClient;

  async handle(props: DetailsSectorRequestDto) {
    const { id } = props;


    const { data } = await this.client.get<
      unknown,
      AxiosResponse<CreateSectorResponseDto>
    >(`/sectors/${id}`);

    return data;
  }
}
