import { guildaApiClient2 } from "src/services";

export interface StockInputQuantityListUseCaseProps {
    Products: { id: number }[];
    Stocks: { id: number }[];
    dataInicial: string;
    dataFinal: string;
}

export class StockInputQuantityListUseCase {
    private client = guildaApiClient2;

    async handle(props: StockInputQuantityListUseCaseProps) {
        const { data } = await this.client.post(
            `/StockInputQuantityList`,
            props
        );

        return data;
    }
}
