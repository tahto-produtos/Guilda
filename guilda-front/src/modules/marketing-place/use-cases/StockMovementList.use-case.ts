import { guildaApiClient2 } from "src/services";

export interface StockMovementListUseCaseProps {
    startDate: string;
    endDate: string;
}

export class StockMovementListUseCase {
    private client = guildaApiClient2;

    async handle(props: StockMovementListUseCaseProps) {
        const { startDate, endDate } = props;

        const payload = {
            Products: [],
            StocksFinals: [],
            StocksOrigins: [],
            dataInicial: startDate,
            dataFinal: endDate,
        };

        const { data } = await this.client.post("/StockMovementList", payload);

        return data;
    }
}
