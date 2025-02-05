import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface ReportConsolidatedSectorUseCaseProps {
    sectors: { id: number }[];
    indicators: { id: number }[];
    order: "Melhor" | "Pior";
    consolidado: boolean;
    dataInicial: string;
    dataFinal: string;
}

export class ReportConsolidatedSectorUseCase {
    private client = guildaApiClient2;

    async handle(props: ReportConsolidatedSectorUseCaseProps) {
        const { data } = await this.client.post<unknown, AxiosResponse>(
            "/ReportConsolidatedSector",
            props
        );

        return data;
    }
}
