import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { OrderReport } from "src/typings";

export interface CompleteOrderReportProps {
    dataInicio?: string;
    dataFim?: string;
    status?: string;
    ordemVendas?: string;
    nomeDoProduto?: string;
    tipo?: string;
    setor?: number;
    uf?: string;
    quantidade?: string;
}

export class CompleteOrderReport {
    private client = guildaApiClient2;

    async handle(props: CompleteOrderReportProps) {
        const {
            dataInicio,
            dataFim,
            nomeDoProduto,
            ordemVendas,
            status,
            quantidade,
            setor,
            tipo,
            uf,
        } = props;

        var bodyFormData = new FormData();
        dataInicio && bodyFormData.append("DATAINICIO", dataInicio);
        dataFim && bodyFormData.append("DATAFIM", dataFim);
        status && bodyFormData.append("STATUS", status);
        ordemVendas && bodyFormData.append("ORDEM_VENDAS", ordemVendas);
        nomeDoProduto && bodyFormData.append("NOME_DO_PRODUTO", nomeDoProduto);

        quantidade && bodyFormData.append("QUANTIDADE", quantidade.toString());
        setor && bodyFormData.append("SETOR", setor.toString());
        tipo && bodyFormData.append("TIPO", tipo);
        uf && bodyFormData.append("UF", uf.toString());

        const { data } = await this.client.post<
            unknown,
            AxiosResponse<OrderReport[]>
        >(`/OrdersReport`, bodyFormData);

        return data;
    }
}
