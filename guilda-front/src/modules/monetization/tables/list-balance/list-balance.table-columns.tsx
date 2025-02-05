import CompareArrows from "@mui/icons-material/CompareArrows";
import East from "@mui/icons-material/East";
import West from "@mui/icons-material/West";
import { Box } from "@mui/system";
import { GridColDef } from "@mui/x-data-grid";
import { DateUtils } from "../../../../utils";
import { formatCurrency } from "src/utils/format-currency";

const Bar = (params: any) => {
    if (params.params.row.output > 0) {
        return <West color="error" />;
    } else {
        return <East color="success" />;
    }
};

const CustomHeader = () => {
    return <CompareArrows />;
};

export const listBalanceTableColumns: GridColDef[] = [
    {
        field: "id",
        headerName: "",
        flex: 0.5,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        renderCell: (params) => <Bar params={params} />,
        renderHeader: (params) => <CustomHeader />,
    },
    {
        field: "input",
        headerName: "Entrada",
        flex: 1,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        renderCell: (params) => (
            <p
                style={{
                    fontWeight: params.row.input > 0 ? "600" : "400",
                    color: params.row.input > 0 ? "#35A11A" : "#ccc",
                }}
            >
                {formatCurrency(params.row.input)}
            </p>
        ),
    },
    {
        field: "output",
        headerName: "Saída",
        flex: 1,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        renderCell: (params) => (
            <p
                style={{
                    fontWeight: params.row.output > 0 ? "600" : "400",
                    color: params.row.output > 0 ? "#E93030" : "#ccc",
                }}
            >
                {formatCurrency(params.row.output)}
            </p>
        ),
    },
    {
        field: "balance",
        headerName: "Saldo final",
        flex: 3,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        valueFormatter({ value }) {
            return `${formatCurrency(value)}`;
        },
    },
    {
        field: "",
        headerName: "Descrição",
        flex: 5,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        renderCell: (params) => (
            <p
                style={{
                    color: "#000",
                }}
            >
                {params.row.reason && `Motivo: ${params.row.reason}`}
                {(() => {
                    if (params.row.observation) return params.row.observation;
                    const teste = params?.row?.order?.GdaOrderProduct[1]
                        ?.product?.comercialName
                        ? `COD: ${params.row.order.id}`
                        : params?.row?.order?.GdaOrderProduct[0]?.product
                              ?.comercialName;
                    return params.row.indicator
                        ? `Indicador: ${params.row.indicator.name}`
                        : params.row.order
                        ? `Compra na loja - ${teste}`
                        : "";
                })()}
            </p>
        ),
    },
    {
        field: "dueAt",
        headerName: "Data de Expiração",
        flex: 3,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        align: "right",
        headerAlign: "right",
        valueFormatter({ value }) {
            if (value) {
                const date = value.split("T")[0];
                const dateSplited = date.split("-");
                return `${dateSplited[2]}/${dateSplited[1]}/${dateSplited[0]}`;
            } else {
                return null;
            }
        },
    },
    {
        field: "resultDate",
        headerName: "Data referência",
        flex: 3,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        align: "right",
        headerAlign: "right",
        valueFormatter({ value }) {
            if (value) {
                const date = value.split("T")[0];
                const dateSplited = date.split("-");
                return `${dateSplited[2]}/${dateSplited[1]}/${dateSplited[0]}`;
            } else {
                return null;
            }
        },
    },
    {
        field: "createdAt",
        headerName: "Criado em",
        flex: 3,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        align: "right",
        headerAlign: "right",
        valueFormatter({ value }) {
            return DateUtils.formatDatePtBRWithTime(value as Date);
        },
    },
];
