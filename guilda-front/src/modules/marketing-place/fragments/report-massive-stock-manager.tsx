import FileOpen from "@mui/icons-material/FileOpen";
import {
    Autocomplete,
    Box,
    Button,
    LinearProgress,
    Stack,
    TextField,
} from "@mui/material";
import { Card, PageHeader } from "src/components";
import { SheetBuilder, getLayout } from "src/utils";
import { StockInputQuantityListUseCase } from "../use-cases/stock-input-quantity-list";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { useEffect, useState } from "react";
import { grey } from "@mui/material/colors";
import { Product } from "src/typings/models/product.model";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListProducts } from "../use-cases/list-products";
import { Stock } from "src/typings/models/stock.model";
import { ListStocks } from "../use-cases/list-stocks";
import { format } from "date-fns";
import { toast } from "react-toastify";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";

export default function MassiveStockManagerReport() {
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [startDatePicker, setStartDatePicker] = useState<
        dateFns | Date | null
    >(null);
    const [endDatePicker, setEndDatePicker] = useState<dateFns | Date | null>(
        null
    );

    const [product, setProduct] = useState<Product[]>([]);
    const [productsSearchValue, setProductsSearchValue] = useState<string>("");
    const [products, setProducts] = useState<Product[]>([]);
    const debouncedProductsSearchTerm: string = useDebounce<string>(
        productsSearchValue,
        400
    );

    const [stock, setStock] = useState<Stock[]>([]);
    const [stocksSearchValue, setStocksSearchValue] = useState<string>("");
    const [stocks, setStocks] = useState<Stock[]>([]);
    const debouncedStocksSearchTerm: string = useDebounce<string>(
        stocksSearchValue,
        400
    );

    const [rowCountState, setRowCountState] = useState(100);

    useEffect(() => {
        setRowCountState((prevRowCountState) => prevRowCountState);
    }, [setRowCountState]);

    const [report, setReport] = useState<any>(null);

    const getProducts = async () => {
        startLoading();

        const payload = {
            limit: 20,
            offset: 0,
            searchText: productsSearchValue,
        };

        new ListProducts()
            .handle(payload)
            .then((data) => {
                setProducts(data.items);
            })
            .catch(() => {})
            .finally(() => {
                finishLoading();
            });
    };

    const getStocks = async () => {
        startLoading();

        const payload = {
            limit: 20,
            offset: 0,
            searchText: stocksSearchValue,
        };

        new ListStocks()
            .handle(payload)
            .then((data) => {
                setStocks(data.items);
            })
            .catch(() => {})
            .finally(() => {
                finishLoading();
            });
    };

    useEffect(() => {
        getStocks();
    }, [debouncedStocksSearchTerm]);

    useEffect(() => {
        getProducts();
    }, [debouncedProductsSearchTerm]);

    async function handleConfirm() {
        if (!startDatePicker || !endDatePicker) {
            return toast.warning("Selecione as datas");
        }

        const payload = {
            Products: product.map((item) => {
                return { id: item.id };
            }),
            Stocks: stock.map((item) => {
                return { id: item.id };
            }),
            dataInicial: format(
                new Date(startDatePicker.toString()),
                "yyyy-MM-dd"
            ),
            dataFinal: format(new Date(endDatePicker.toString()), "yyyy-MM-dd"),
        };

        await new StockInputQuantityListUseCase()
            .handle(payload)
            .then((data) => {
                console.log("RESPONSE", data);
                setReport(data);
            })
            .catch(() => {})
            .finally(() => {
                finishLoading();
            });
    }

    function handleExtract() {
        if (report.length <= 0) {
            return toast.warning("Sem dados para exportar");
        }

        try {
            const docRows = report.map((item: any) => {
                return [
                    item.data,
                    item.CodProduto,
                    item.nomeProduto,
                    item.valorDoProduto,
                    item.fornecedor,
                    item.nomeEstoque,
                    item.quantidadeEntrada,
                ];
            });
            let indicatorSheetBuilder = new SheetBuilder();
            indicatorSheetBuilder
                .setHeader([
                    "Data",
                    "Código do Produto",
                    "Nome do Produto",
                    "Valor do Produto",
                    "Fornecedor",
                    "Nome Estoque",
                    "Quantidade de entrada",
                ])
                .append(docRows)
                .exportAs(
                    "Relatório de importação massiva de quantidade de produto"
                );
            toast.success("Relatório exportado com sucesso!");
        } catch {
            toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
        }
    }

    return (
        <Card
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"space-between"}
        >
            <PageHeader
                title={`Relatório de importação massiva de quantidade de produto`}
                headerIcon={<FileOpen />}
            />
            <Stack width={"100%"} gap={2}>
                <Box display={"flex"} flexDirection={"row"} gap={"10px"}>
                    <LocalizationProvider dateAdapter={AdapterDateFns}>
                        <DatePicker
                            label="De"
                            value={startDatePicker}
                            onChange={(newValue) =>
                                setStartDatePicker(newValue)
                            }
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
                            value={endDatePicker}
                            onChange={(newValue) => setEndDatePicker(newValue)}
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
                <Autocomplete
                    multiple
                    size={"small"}
                    options={products}
                    fullWidth
                    getOptionLabel={(option) => option.comercialName}
                    onChange={(event, value) => {
                        setProduct(value);
                    }}
                    onInputChange={(e, text) => setProductsSearchValue(text)}
                    filterOptions={(x) => x}
                    filterSelectedOptions
                    renderInput={(params) => (
                        <TextField
                            {...params}
                            variant="outlined"
                            label="Selecione um ou mais produtos"
                            placeholder="Buscar"
                        />
                    )}
                    renderOption={(props, option) => {
                        return (
                            <li {...props} key={option.id}>
                                {option.comercialName}
                            </li>
                        );
                    }}
                    isOptionEqualToValue={(option, value) =>
                        option.comercialName === value.comercialName
                    }
                    sx={{ p: 0, m: 0 }}
                />
                <Autocomplete
                    multiple
                    size={"small"}
                    options={stocks}
                    fullWidth
                    getOptionLabel={(option) => option.description}
                    onChange={(event, value) => {
                        setStock(value);
                    }}
                    onInputChange={(e, text) => setStocksSearchValue(text)}
                    filterOptions={(x) => x}
                    filterSelectedOptions
                    renderInput={(params) => (
                        <TextField
                            {...params}
                            variant="outlined"
                            label="Selecione um ou mais estoques"
                            placeholder="Buscar"
                        />
                    )}
                    renderOption={(props, option) => {
                        return (
                            <li {...props} key={option.id}>
                                {option.description}
                            </li>
                        );
                    }}
                    isOptionEqualToValue={(option, value) =>
                        option.description === value.description
                    }
                    sx={{ p: 0, m: 0 }}
                />
                <Button variant="contained" onClick={handleConfirm}>
                    Visualizar
                </Button>
                {report && (
                    <Button variant="contained" onClick={handleExtract}>
                        Exportar relatório
                    </Button>
                )}
                {report && (
                    <DataGrid
                        columns={columns}
                        rows={report}
                        hideFooter
                        disableColumnFilter
                        disableRowSelectionOnClick
                        autoHeight
                        rowCount={rowCountState}
                        paginationMode="server"
                        loading={isLoading}
                        localeText={{
                            noResultsOverlayLabel:
                                "Nenhum resultado encontrado",
                        }}
                        slots={{
                            loadingOverlay: LinearProgress,
                        }}
                        getRowId={(row) =>
                            `${row.CodProduto}-${
                                Math.floor(Math.random() * (1000 - 1 + 1)) + 1
                            }`
                        }
                    />
                )}
            </Stack>
        </Card>
    );
}

MassiveStockManagerReport.getLayout = getLayout("private");

export const columns: GridColDef[] = [
    {
        field: "data",
        headerName: "Data",
        flex: 2,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "CodProduto",
        headerName: "Código do produto",
        flex: 2,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "nomeProduto",
        headerName: "Nome do Produto",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "valorDoProduto",
        headerName: "Valor do Produto",
        flex: 2,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "fornecedor",
        headerName: "Fornecedor",
        flex: 2,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "nomeEstoque",
        headerName: "Nome Estoque",
        flex: 2,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "quantidadeEntrada",
        headerName: "Quantidade Entrada",
        flex: 1,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
];
