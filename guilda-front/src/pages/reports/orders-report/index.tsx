import FileOpen from "@mui/icons-material/FileOpen";
import {
    Box,
    Button,
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    Stack,
    TextField,
    Autocomplete,
    CircularProgress,
    LinearProgress,
    Pagination,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format, isValid } from "date-fns";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import WithoutPermissionCard from "src/components/data-display/without-permission/without-permissions";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useLoadingState, useDebounce } from "src/hooks";
import { DateUtils, SheetBuilder, getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";
import { StockReport } from "src/typings/models/stock-report.model";
import { CompleteOrderReport } from "src/modules/marketing-place/use-cases/complete-order-report.use-case";
import { OrderReport } from "src/typings";
import { OrderReportTable } from "src/modules/marketing-place/tables/order-report";
import { ListSectorsUseCase } from "src/modules/sectors";
import { PaginationModel } from "src/typings/models/pagination.model";
import { Sector } from "src/typings";
import { ListCityUseCase } from "src/modules/marketing-place/use-cases/list-city.use-case";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import PaginationComponent from "src/components/navigation/pagination/pagination";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export default function OrderReportScreen() {
    const [searchText, setSearchText] = useState<string>("");
    const { myPermissions } = useContext(PermissionsContext);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [page, setPage] = useState<number>(1);
    const LIMIT_PER_PAGE = 20;
    const [startDate, setStartDate] = useState<dateFns | null>(null);
    const [endDate, setEndDate] = useState<dateFns | null>(null);
    const [stockReportData, setStockReportData] = useState<OrderReport[]>([]);
    const [totalItems, setTotalItems] = useState<number | null>(null);
    const [type, setType] = useState<"PHYSICAL" | "DIGITAL" | "">("");
    const [uf, setUf] = useState<string>("");
    const [sectorSearchValue, setSectorSearchValue] = useState<string>("");
    const debouncedSectorSearchTerm: string = useDebounce<string>(
        sectorSearchValue,
        400
    );
    const [productType, setProductType] = useState<"PHYSICAL" | "DIGITAL" | "">(
        "PHYSICAL"
    );
    const [status, setStatus] = useState<string>("");
    const [orderBy, setOrderBy] = useState<string>("");
    const [productName, setProductName] = useState<string>("");

    const [selectedSector, setSelectedSector] = useState<Sector | null>(null);
    const [sectors, setSectors] = useState<Sector[]>([]);
    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const debouncedSectorsSearchValue: string = useDebounce<string>(
        sectorsSearchValue,
        400
    );

    const [rowCountState, setRowCountState] = useState(100);

    const [cityList, setCityList] = useState<any[]>([]);
    const [city, setCity] = useState<string>("");
    const [quantity, setQuantity] = useState<string>("");

    const getSectorsList = async () => {
        const pagination = {
            limit: 20,
            offset: 0,
            searchText: sectorsSearchValue,
            deleted: false,
        };

        new ListSectorsUseCase()
            .handle(pagination)
            .then((data) => {
                setSectors(data.items);
            })
            .catch(() => {
                // toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {});
    };

    useEffect(() => {
        getSectorsList();
    }, [debouncedSectorsSearchValue]);

    async function getCities() {
        const set = new Set();

        await new ListCityUseCase()
            .handle()
            .then((data) => {
                data.cities.forEach((el: any, index: number) => {
                    if (el.VALUE !== "-") {
                        set.add(el.VALUE);
                    }
                    console.log(set, el.value, el.VALUE, el);
                });
                setCityList(Array.from(set));
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {});
    }

    useEffect(() => {
        getCities();
    }, []);

    useEffect(() => {
        setPage(1);
        setStockReportData([]);
        setTotalItems(0);
    }, [startDate, endDate]);

    const getStockReport = async () => {
        if (startDate && endDate && isValid(startDate) && isValid(endDate)) {
            startLoading();
            const formatedStartDate = format(
                new Date(startDate.toString()),
                "yyyy-MM-dd"
            );
            const formatedEndDate = format(
                new Date(endDate.toString()),
                "yyyy-MM-dd"
            );

            new CompleteOrderReport()
                .handle({
                    dataFim: formatedEndDate,
                    dataInicio: formatedStartDate,
                    nomeDoProduto: productName,
                    ordemVendas: orderBy,
                    status: status,
                    quantidade: quantity,
                    setor: selectedSector?.id,
                    tipo: productType,
                    uf: city,
                })
                .then((data) => {
                    setStockReportData(data);
                    setTotalItems(data.length);
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    finishLoading();
                });
        }
    };

    useEffect(() => {
        stockReportData.length > 0 && getStockReport();
    }, [page]);

    useEffect(() => {
        const pagination: PaginationModel = {
            limit: 1500,
            offset: 0,
            searchText: sectorSearchValue,
        };
        new ListSectorsUseCase()
            .handle(pagination)
            .then((data) => {
                setSectors(data.items);
            })
            .catch(() => {});
    }, [debouncedSectorSearchTerm]);

    function handleReportExport() {
        if (
            startDate &&
            endDate &&
            isValid(startDate) &&
            isValid(endDate) &&
            totalItems
        ) {
            startLoading();

            const formatedStartDate = format(
                new Date(startDate.toString()),
                "yyyy-MM-dd"
            );
            const formatedEndDate = format(
                new Date(endDate.toString()),
                "yyyy-MM-dd"
            );

            new CompleteOrderReport()
                .handle({
                    dataFim: formatedEndDate,
                    dataInicio: formatedStartDate,
                    nomeDoProduto: productName,
                    ordemVendas: orderBy,
                    status: status,
                    quantidade: quantity,
                    setor: selectedSector?.id,
                    tipo: productType,
                    uf: city,
                })
                .then((data) => {
                    const docRows = data.map((item: any) => {
                        let status = "";
                        switch (item.STATUS) {
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

                        return [
                            item.CRIADO_EM
                                ? DateUtils.formatDatePtBR(item.CRIADO_EM)
                                : "",
                            item.CODIGO,
                            item.CRIADO_POR,
                            item.COLABORADOR,
                            item.CARGO,
                            status,
                            item.GRUPO,
                            item.UF,
                            { v: item.COD_GIP, t: "n" },
                            
                            item.SETOR,
                            item.HOME,
                            item.TIPO_DE_PRODUTO == "PHYSICAL"
                                ? "FÍSICO"
                                : item.TIPO_DE_PRODUTO,
                            item.NOME_DO_PRODUTO,
                            item.QUANTIDADE,
                            item.VALOR_EM_MOEDAS,
                            item.VALOR_EM_MOEDAS_TOTAL,
                            item.ULTIMA_ATUALIZACAO
                                ? DateUtils.formatDatePtBR(
                                      item.ULTIMA_ATUALIZACAO
                                  )
                                : "",
                            item.QUEM_ATUALIZOU,
                            item.ENTREGUE_POR,
                            item.BC_COLABORADOR,
                            item.LIBERADO_POR,
                            item.OBSERVACAO_LIBERACAO,
                            item.CANCELADO_POR,
                            item.OBSERVACAO_CANCELAMENTO,
                            item.QUEM_VAI_RETIRAR,
                            item.QUEM_RETIROU,
                            item.OBSERVACAO_DE_ENTREGA,
                            item.ESTOQUE,
                            { v: item["MATRICULA_SUPERVISOR"], t: "n" },
                            
                            item["NOME_SUPERVISOR"],
                            { v: item["MATRICULA_COORDENADOR"], t: "n" },
                            
                            item["NOME_COORDENADOR"],
                            { v: item["MATRICULA_GERENTE_II"], t: "n" },
                            
                            item["NOME_GERENTE_II"],
                            { v: item["MATRICULA_GERENTE_I"], t: "n" },
                            
                            item["NOME_GERENTE_I"],
                            { v: item["MATRICULA_DIRETOR"], t: "n" },
                            
                            item["NOME_DIRETOR"],
                            { v: item["MATRICULA_CEO"], t: "n" },

                            item["NOME_CEO"],
                        ];
                    });
                    let sectorsSheetBuilder = new SheetBuilder();
                    sectorsSheetBuilder
                        .setHeader([
                            "Criado Em",
                            "Código do Produto",
                            "Criado Por",
                            "Nome do Colaborador",
                            "Cargo",
                            "Status",
                            "Grupo",
                            "UF",
                            "Código GIP",
                            "Setor",
                            "Home",
                            "Tipo Produto",
                            "Nome Produto",
                            "Quantidade",
                            "Valor em Moedas",
                            "Valor Em Moedas Total",
                            "Última Atualização",
                            "Quem Atualizou",
                            "Entregue Por",
                            "BC Colaborador",
                            "Liberado por",
                            "Observação de Liberação",
                            "Quem Cancelou",
                            "Observação de Cancelamento",
                            "Quem Vai Retirar",
                            "Quem Retirou",
                            "Observação de Entrega",
                            "Estoque",
                            "Matricula do supervisor",
                            "Nome Supervisor",
                            "Matricula do coordenador",
                            "Nome Coordenador",
                            "Matricula do gerente II",
                            "Nome Gerente II",
                            "Matricula do gerente I",
                            "Nome Gerente I",
                            "Matricula do diretor",
                            "Nome Diretor",
                            "Matricula do CEO",
                            "Nome CEO",
                        ])
                        .append(docRows)
                        .exportAs(`Relatório de Pedidos Completo`);
                    toast.success("Relatório exportado com sucesso!");
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    finishLoading();
                });
        } else {
            toast.warning("Visualize o relatório antes de exporta-lo");
        }
    }

    if (
        abilityFor(myPermissions).cannot("Relatorio de estoque", "Relatorios")
    ) {
        return (
            <Card width={"100%"} display={"flex"} flexDirection={"column"}>
                <WithoutPermissionCard />
            </Card>
        );
    }

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Relatório de pedidos"}
                headerIcon={<FileOpen />}
            />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                <Box display={"flex"} gap={2} width={"100%"}>
                    <LocalizationProvider dateAdapter={AdapterDateFns}>
                        <DatePicker
                            label="De"
                            value={startDate}
                            onChange={(newValue) => setStartDate(newValue)}
                            slotProps={{
                                textField: {
                                    size: "small",
                                    sx: {
                                        minWidth: "180px",
                                        svg: {
                                            color: grey[500],
                                        },
                                        width: "100%",
                                    },
                                },
                            }}
                        />
                    </LocalizationProvider>
                    <LocalizationProvider dateAdapter={AdapterDateFns}>
                        <DatePicker
                            label="Até"
                            value={endDate}
                            onChange={(newValue) => setEndDate(newValue)}
                            slotProps={{
                                textField: {
                                    size: "small",
                                    sx: {
                                        minWidth: "180px",
                                        svg: {
                                            color: grey[500],
                                        },
                                        width: "100%",
                                    },
                                },
                            }}
                        />
                    </LocalizationProvider>
                </Box>
                <Box
                    display={"flex"}
                    gap={1}
                    width={"100%"}
                    alignItems={"center"}
                >
                    <FormControl fullWidth>
                        <InputLabel
                            size="small"
                            sx={{ background: "#fff", px: "5px" }}
                        >
                            Status
                        </InputLabel>
                        <Select
                            size="small"
                            value={status}
                            onChange={(e) => setStatus(e.target.value)}
                        >
                            <MenuItem value={"CANCELED"}>Cancelados</MenuItem>
                            <MenuItem value={"EXPIRED"}>Expirados</MenuItem>
                            <MenuItem value={"RELEASED"}>Liberados</MenuItem>
                            <MenuItem value={"DELIVERED"}>Entregues</MenuItem>
                            <MenuItem value={"ORDERED"}>
                                Solicitados/Comprados
                            </MenuItem>
                            <MenuItem value={""}>Todos os pedidos</MenuItem>
                        </Select>
                    </FormControl>
                    <FormControl fullWidth>
                        <InputLabel
                            size="small"
                            sx={{ background: "#fff", px: "5px" }}
                        >
                            Ordenar por:
                        </InputLabel>
                        <Select
                            size="small"
                            value={orderBy}
                            onChange={(e) => setOrderBy(e.target.value)}
                        >
                            <MenuItem value={"1"}>Mais Vendidos</MenuItem>
                            <MenuItem value={"0"}>Menos Vendidos</MenuItem>
                            <MenuItem value={""}>Padrão</MenuItem>
                        </Select>
                    </FormControl>
                </Box>
                <Box
                    display={"flex"}
                    gap={1}
                    width={"100%"}
                    alignItems={"center"}
                >
                    <TextField
                        value={productName}
                        onChange={(e) => setProductName(e.target.value)}
                        size="small"
                        fullWidth
                        label="Nome do produto"
                    />
                    <FormControl fullWidth>
                        <InputLabel
                            size="small"
                            sx={{ background: "#fff", px: "5px" }}
                        >
                            Tipo de produto
                        </InputLabel>
                        <Select
                            value={productType}
                            size="small"
                            onChange={(e) =>
                                setProductType(
                                    e.target.value as "PHYSICAL" | "DIGITAL"
                                )
                            }
                        >
                            <MenuItem value={""}>Todos</MenuItem>
                            <MenuItem value={"PHYSICAL"}>
                                Produto físico
                            </MenuItem>
                            <MenuItem value={"DIGITAL"}>
                                Produto digital
                            </MenuItem>
                        </Select>
                    </FormControl>
                </Box>
                <Box
                    display={"flex"}
                    gap={1}
                    width={"100%"}
                    alignItems={"center"}
                >
                    <Autocomplete
                        value={selectedSector}
                        placeholder={"Setor"}
                        disableClearable={false}
                        onChange={(e, value) => setSelectedSector(value)}
                        onInputChange={(e, text) => setSectorsSearchValue(text)}
                        sx={{ mb: 0, width: "100%" }}
                        renderInput={(props) => (
                            <TextField
                                {...props}
                                size={"small"}
                                label={"Setor"}
                                InputProps={{
                                    ...props.InputProps,
                                    endAdornment: (
                                        <>
                                            {isLoading ? (
                                                <CircularProgress
                                                    color="primary"
                                                    size={20}
                                                />
                                            ) : (
                                                props.InputProps.endAdornment
                                            )}
                                        </>
                                    ),
                                }}
                            />
                        )}
                        isOptionEqualToValue={(option, value) =>
                            option.name === value.name
                        }
                        getOptionLabel={(option) =>
                            `${option.id} - ${option.name}`
                        }
                        options={sectors}
                    />
                </Box>
                <Box display={"flex"} gap={2} width={"100%"}>
                    <FormControl sx={{ width: "100%" }}>
                        <InputLabel
                            sx={{ backgroundColor: "#fff", px: 1 }}
                            size="small"
                        >
                            Cidade
                        </InputLabel>
                        <Select
                            onChange={(e) => setCity(e.target.value)}
                            value={city}
                            size="small"
                        >
                            {cityList.map((item, index) => {
                                return (
                                    <MenuItem value={item} key={index}>
                                        {item}
                                    </MenuItem>
                                );
                            })}
                            <MenuItem value={""}>Padrão</MenuItem>
                        </Select>
                    </FormControl>
                    <TextField
                        value={quantity}
                        onChange={(e) => setQuantity(e.target.value)}
                        label="Quantidade"
                        size="small"
                        fullWidth
                    />
                </Box>
                <Box display={"flex"} width={"100%"} gap={2}>
                    <Button
                        variant="contained"
                        fullWidth
                        disabled={!startDate || !endDate}
                        onClick={getStockReport}
                    >
                        Visualizar relatório
                    </Button>
                    <Button
                        variant="outlined"
                        fullWidth
                        disabled={!totalItems}
                        onClick={handleReportExport}
                    >
                        Exportar relatório completo
                    </Button>
                </Box>
                <Box display={"flex"} flexDirection={"column"} gap={1}>
                    {!isLoading ? (
                        // <OrderReportTable
                        //     tableData={{
                        //         items: stockReportData,
                        //         totalItems:
                        //             totalItems ?? stockReportData.length,
                        //     }}
                        //     isLoading={isLoading}
                        //     enableCodeSearch={false}
                        //     hideSearchInput={true}
                        //     hideDatePicker={true}
                        // />
                        <>
                            <DataGrid
                                columns={orderColumns}
                                rows={stockReportData}
                                hideFooter
                                disableColumnFilter
                                disableRowSelectionOnClick
                                autoHeight
                                rowCount={rowCountState}
                                loading={isLoading}
                                localeText={{
                                    noResultsOverlayLabel:
                                        "Nenhum resultado encontrado",
                                }}
                                slots={{
                                    loadingOverlay: LinearProgress,
                                }}
                                getRowId={(row) =>
                                    `${row.QTD_IMPORTACAO}-${
                                        Math.floor(
                                            Math.random() * (1000 - 1 + 1)
                                        ) + 1
                                    }`
                                }
                            />
                        </>
                    ) : (
                        <CircularProgress sx={{ mt: 2, ml: 2 }} />
                    )}
                </Box>
                <Box
                    display={"flex"}
                    justifyContent={"flex-end"}
                    alignItems={"center"}
                >
                    {/* {stockReportData.length > 0 && (
                        <Pagination
                            count={totalPages}
                            page={page}
                            onChange={handleChangePagination}
                            disabled={isLoading}
                        />
                    )} */}
                </Box>
            </Stack>
        </Card>
    );
}

OrderReportScreen.getLayout = getLayout("private");

export const orderColumns: GridColDef[] = [
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
            switch (value) {
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
            return value === "PHYSICAL" ? "FÍSICO" : value;
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
