import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";
import {
    CreateIndicatorRequestDto,
    CreateIndicatorResponseDto,
} from "../forms/create-indicator/dto";

export class CreateIndicatorUseCase {
    private client = guildaApiClient;

    async handle(createIndicatorDto: CreateIndicatorRequestDto) {
        const {
            name,
            description,
            id,
            weight,
            calculationType,
            expression,
            sectorsIds,
            status,
        } = createIndicatorDto;

        const payload = {
            name: name,
            description: description,
            id: id,
            weight: weight,
            calculationType: calculationType,
            expression: expression,
            sectorsIds: sectorsIds,
            status: status == "ativado" ? true : false,
        };

        const { data } = await this.client.post<
            unknown,
            AxiosResponse<CreateIndicatorResponseDto>
        >("/indicators", payload);

        return data;
    }
}
