import { Box, CardMedia, LinearProgress, Stack } from "@mui/material";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { getLayout } from "src/utils";
import List from "@mui/icons-material/List";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { ListCatalogUseCase } from "src/modules/catalog/use-cases/list-catalog.use-case";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useLoadingState } from "src/hooks";

export interface CatalogItem {
    CODIGO_PRODUTO: string;
    PRODUTO: string;
    DESCRICAO: string;
    CODIGO_REF: string;
    ATIVO: string;
    INATIVO: string;
    ESTOQUE: string;
    IMAGEM: string;
}

export default function CatalogView() {
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [catalogData, setCatalogData] = useState<CatalogItem[]>([]);

    function listCatalog() {
        startLoading();
        new ListCatalogUseCase()
            .handle()
            .then((data) => {
                setCatalogData(data);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    useEffect(() => {
        listCatalog();
    }, []);

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader title={"Catálogo"} headerIcon={<List />} />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                <DataGrid
                    columns={columns}
                    rows={catalogData}
                    hideFooter
                    disableColumnFilter
                    disableRowSelectionOnClick
                    autoHeight
                    // rowCount={rowCountState}
                    paginationMode="server"
                    loading={isLoading}
                    slots={{
                        loadingOverlay: LinearProgress,
                    }}
                    getRowId={(row) =>
                        `${row.CodigoGip}-${row.Setor}-${row.Cargo}-${
                            row.NomeAgente
                        }-${Math.floor(Math.random() * (1000 - 1 + 1)) + 1}`
                    }
                />
            </Stack>
        </Card>
    );
}

CatalogView.getLayout = getLayout("private");

export const columns: GridColDef[] = [
    {
        field: "IMAGEM",
        headerName: "Imagem",
        flex: 2,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        renderCell: (params) => {
            return (
                <Box>
                    {params?.row?.IMAGEM && (
                        <CardMedia
                            component="img"
                            image={params.row.IMAGEM}
                            sx={{
                                width: "50px",
                                objectFit: "contain",
                            }}
                        />
                    )}
                </Box>
            );
        },
    },
    {
        field: "PRODUTO",
        headerName: "Produto",
        flex: 5,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "DESCRICAO",
        headerName: "Descrição",
        flex: 3,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "CODIGO_REF",
        headerName: "Código",
        flex: 3,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "ATIVO",
        headerName: "Qtd. Ativo",
        flex: 2,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "INATIVO",
        headerName: "Qtd. Inativo",
        flex: 2,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "ESTOQUE",
        headerName: "Estoque",
        flex: 3,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },

    // {
    //     field: "Reincidencia",
    //     headerName: "Reincidência",
    //     flex: 2,
    //     disableColumnMenu: true,
    //     filterable: false,
    //     sortable: false,
    //     // valueFormatter({ value }) {
    //     //     return value ? "Sim" : "Não";
    //     // },
    //     renderCell: (params) => {
    //         return (
    //             <Box
    //                 sx={{
    //                     backgroundColor: params.row.Reincidencia
    //                         ? lighten("#f00", 0.8)
    //                         : "none",
    //                 }}
    //             >
    //                 {params.row.Reincidencia === true ? "Sim" : "Não"}
    //             </Box>
    //         );
    //     },
    // },
    // {
    //     field: "Meta",
    //     headerName: "Meta",
    //     flex: 2,
    //     disableColumnMenu: true,
    //     filterable: false,
    //     sortable: false,
    //     renderCell: (params) => {
    //         return (
    //             <Box>
    //                 {params?.row?.Meta}
    //                 {params?.row?.TipoIndicador == "PERCENT" &&
    //                     params?.row?.Meta !== "-" &&
    //                     "%"}
    //             </Box>
    //         );
    //     },
    // },
    // {
    //     field: "Resultado",
    //     headerName: "Resultado",
    //     flex: 2,
    //     disableColumnMenu: true,
    //     filterable: false,
    //     sortable: false,
    //     renderCell: (params) => {
    //         return (
    //             <Box
    //                 sx={{
    //                     backgroundColor: params.row.value
    //                         ? lighten("#f00", 0.8)
    //                         : "none",
    //                 }}
    //             >
    //                 {params?.row?.TipoIndicador == "INTEGER"
    //                     ? params.row.Resultado.toFixed(0)
    //                     : params.row.Resultado.toFixed(2)}
    //                 {params?.row?.TipoIndicador == "PERCENT" && "%"}
    //             </Box>
    //         );
    //     },
    // },
];
