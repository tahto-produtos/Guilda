import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface MetricBySectorUseCaseProps {
    datainicial: string;
    datafinal: string;
}

export class MetricBySectorUseCase {
    private client = guildaApiClient2;

    async handle(
        props: MetricBySectorUseCaseProps,
        associatedIndicator?: boolean
    ) {
        const { datafinal, datainicial } = props;

        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/MetricBySector?datainicial=${datainicial}&datafinal=${datafinal}`
        );

        return data;
    }
}
