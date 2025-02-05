import { GridColDef } from "@mui/x-data-grid";
import { DateUtils } from "../../../../utils";

export const listGroupTableNewColumns: GridColDef[] = [
    {
        field: "CODIGO",
        headerName: "Código",
        flex: 1,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "CRIADO_POR",
        headerName: "Criado por",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "STATUS",
        headerName: "Status",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        valueFormatter({ value }) {
            let status = "";
            switch(value) {
                case "ORDERED":
                    status = "PEDIDO";
                    break;
                case "DELIVERED":
                    status = "ENTREGUE";
                    break;
                case "CANCELED":
                    status = "CANCELADO";
                    break;
                case "RELEASED":
                    status = "LIBERADO";
                    break;
                case "EXPIRED":
                    status = "EXPIRADO";
                    break;
            }
            return status;
        },
    },
    {
        field: "CRIADO_EM",
        headerName: "Criado em",
        flex: 2,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        valueFormatter({ value }) {
            return DateUtils.formatDatePtBRWithTime(value as Date);
        },
    },
    /* {
        field: "COLABORADOR",
        headerName: "Colaborador",
        flex: 1,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        //renderCell: (params) => <DetailsCard params={params} />,
    },
    {
        field: "CARGO",
        headerName: "Cargo",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "NOME_SUPERVISOR",
        headerName: "Nome Supervisor",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "NOME_COORDENADOR",
        headerName: "Nome Coordenador",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "NOME_GERENTE_II",
        headerName: "Nome Gerente 2",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "NOME_GERENTE_I",
        headerName: "Nome Gerente 1",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "NOME_DIRETOR",
        headerName: "Nome Diretor",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "NOME_CEO",
        headerName: "Nome CEO",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "GRUPO",
        headerName: "Grupo",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "UF",
        headerName: "UF",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "COD_GIP",
        headerName: "Cod. GIP",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "SETOR",
        headerName: "Setor",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "HOME",
        headerName: "Home",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    */
    {
        field: "TIPO_DE_PRODUTO",
        headerName: "Tipo de Produto",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        valueFormatter({ value }) {
            return value === 'PHYSICAL' ? "FÍSICO" : value;
        },
    },
    {
        field: "NOME_DO_PRODUTO",
        headerName: "Nome do Produto",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "QUANTIDADE",
        headerName: "Quantidade",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "VALOR_EM_MOEDAS",
        headerName: "Valor em Moedas",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "VALOR_EM_MOEDAS_TOTAL",
        headerName: "Valor em Moedas Total",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    /*{
        field: "ULTIMA_ATUALIZACAO",
        headerName: "Última Atualização",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        valueFormatter({ value }) {
            return DateUtils.formatDatePtBRWithTime(value as Date);
        },
    },
    {
        field: "QUEM_ATUALIZOU",
        headerName: "Quem atualizou",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "ENTREGUE_POR",
        headerName: "Entregue Por",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "BC_COLABORADOR",
        headerName: "BC Colaborador",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "QUEM_VAI_RETIRAR",
        headerName: "Quem Vai Retirar",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "QUEM_RETIROU",
        headerName: "Quem Retirou",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "ESTOQUE",
        headerName: "Estoque",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    }, */
];
