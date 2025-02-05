import { AxiosResponse } from "axios";
import FormData from "form-data";
import { guildaApiClient2 } from "src/services";
import { CreateSectorRequestDto, CreateSectorResponseDto } from "../forms";

export class CreateSectorUseCase {
    private client = guildaApiClient2;

    async handle(createSectorDto: CreateSectorRequestDto) {
        const { name, indicatorsIds, code } = createSectorDto;

        const payload = {
            name: name,
            level: 0,
            indicatorsIds: indicatorsIds,
            COD_GIP: code,
        };

        const { data } = await this.client.post<
            unknown,
            AxiosResponse<CreateSectorResponseDto>
        >("/CreateSector", payload);

        return data;
    }
}
