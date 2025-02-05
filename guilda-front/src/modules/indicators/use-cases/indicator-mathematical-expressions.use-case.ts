import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export class IndicatorMathematicalExpressionsUseCase {
    private client = guildaApiClient2;

    async handle() {
        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/IndicatorMathematicalExpressions`
        );

        return data;
    }
}
