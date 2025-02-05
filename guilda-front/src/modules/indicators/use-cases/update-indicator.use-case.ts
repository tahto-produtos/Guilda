import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";

interface UpdateIndicatorUseCaseProps {
    id: number;
    name: string;
    description: string;
    expression?: string;
    calculationType?: string;
    weight?: number;
    sectorsIds: number[];
    code?: number;
    type: string;
}

export class UpdateIndicatorUseCase {
    private client = guildaApiClient;

    async handle(props: UpdateIndicatorUseCaseProps) {
        const {
            description,
            id,
            name,
            calculationType,
            expression,
            weight,
            sectorsIds,
            code,
            type,
        } = props;

        const payload = {
            id: id,
            name: name,
            description: description,
            expression: expression,
            calculationType: calculationType,
            weight: weight,
            sectorsIds: sectorsIds,
            type: type,
        };

        const { data } = await this.client.put<unknown, AxiosResponse>(
            `/indicators/${id}`,
            payload
        );

        return data;
    }
}
