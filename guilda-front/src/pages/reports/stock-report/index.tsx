import FileOpen from "@mui/icons-material/FileOpen";
import {
    Autocomplete,
    Box,
    Button,
    Checkbox,
    CircularProgress,
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format, isValid } from "date-fns";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { AuthVerifyComponent } from "src/components/auth/AuthVerifyComponent/AuthVerifyComponent";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListSectorsUseCase } from "src/modules";
import { StockMovementListUseCase } from "src/modules/marketing-place/use-cases/StockMovementList.use-case";
import { ReportManageStockUseCase } from "src/modules/marketing-place/use-cases/generate-stock-manage-report.use-case";
import { GenerateStockReportUseCase } from "src/modules/marketing-place/use-cases/generate-stock-report.use-case";
import { GenerateVouchersSoldReportUseCase } from "src/modules/marketing-place/use-cases/generate-vouchers-sold-report.use-case";
import { GenerateVouchersStockReportUseCase } from "src/modules/marketing-place/use-cases/generate-vouchers-stock-report.use-case";
import { ListCityUseCase } from "src/modules/marketing-place/use-cases/list-city.use-case";
import { ListClientUseCase } from "src/modules/marketing-place/use-cases/list-client.use-case";
import { ListProducts } from "src/modules/marketing-place/use-cases/list-products";
import { ListStocks } from "src/modules/marketing-place/use-cases/list-stocks";
import { Sector } from "src/typings";
import { Product } from "src/typings/models/product.model";
import { StockReportItem } from "src/typings/models/stock-report-item";
import { Stock } from "src/typings/models/stock.model";
import { DateUtils, SheetBuilder, getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";

export default function StockReport() {
    const { myPermissions } = useContext(PermissionsContext);
    const { finishLoading, isLoading, startLoading } = useLoadingState();

    const [startDate, setStartDate] = useState<dateFns | null>(null);
    const [endDate, setEndDate] = useState<dateFns | null>(null);
    const [verifyStockVoucher, setVerifyStockVoucher] =
        useState<boolean>(false);
    const [verifyStockVoucher2, setVerifyStockVoucher2] =
        useState<boolean>(false);

    const [status, setStatus] = useState<"Ativo" | "Inativo" | "">("");
    const [product, setProduct] = useState<Product | null>(null);
    const [productsSearchValue, setProductsSearchValue] = useState<string>("");
    const [products, setProducts] = useState<Product[]>([]);
    const debouncedProductsSearchTerm: string = useDebounce<string>(
        productsSearchValue,
        400
    );

    const [productType, setProductType] = useState<"PHYSICAL" | "DIGITAL" | "">(
        "PHYSICAL"
    );
    const [stock, setStock] = useState<Stock | null>(null);
    const [stocksSearchValue, setStocksSearchValue] = useState<string>("");
    const [stocks, setStocks] = useState<Stock[]>([]);
    const debouncedStocksSearchTerm: string = useDebounce<string>(
        stocksSearchValue,
        400
    );

    const [cityList, setCityList] = useState<any[]>([]);
    const [city, setCity] = useState<string>("");
    const [quantity, setQuantity] = useState<string>("");
    const [bestSeller, setBestSeller] = useState<boolean>(false);
    const [lessSold, setLessSold] = useState<boolean>(false);

    const [selectedSector, setSelectedSector] = useState<Sector | null>(null);
    const [sectors, setSectors] = useState<Sector[]>([]);
    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const debouncedSectorsSearchValue: string = useDebounce<string>(
        sectorsSearchValue,
        400
    );

    const getSectorsList = async () => {
        startLoading();

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
            .finally(() => {
                finishLoading();
            });
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

    useEffect(() => {
        getProducts();
    }, [debouncedProductsSearchTerm]);

    function handleReportExport() {
        startLoading();

        if (
            startDate &&
            endDate &&
            isValid(startDate) &&
            isValid(endDate) &&
            endDate >= startDate
        ) {
            const formatedStartDate = format(
                new Date(startDate.toString()),
                "yyyy-MM-dd"
            );
            const formatedEndDate = format(
                new Date(endDate.toString()),
                "yyyy-MM-dd"
            );

            const payload = {
                startDate: formatedStartDate,
                endDate: formatedEndDate,
                productId: product ? product.id : undefined,
                quantity: quantity ? parseInt(quantity) : undefined,
                sector: selectedSector ? selectedSector.id : undefined,
                bestSeller: bestSeller,
                city: city,
                lessSold: lessSold,
                type: productType,
                status: status,
            };

            new GenerateStockReportUseCase()
                .handle(payload)
                .then((data) => {
                    if (data.length <= 0) {
                        return toast.warning("Sem dados para exportar");
                    }

                    const docRows = data.map((item: StockReportItem) => {
                        return [
                            item.product,
                            item.code,
                            item.quantity,
                            item.pending,
                            item.sold,
                            item.price,
                            "",
                            item.stock,
                            item.publicationDate,
                            item.expirationDate,
                            "",
                            item.expirationDateVoucher,
                            item.type == "PHYSICAL" ? "Físico" : item.type,
                            item.registeredBy,
                            item.createdAt,
                            item.status,
                            item.Visibility,
                            //item.startDate
                            //    ? DateUtils.formatDatePtBR(item.startDate)
                            //    : "",
                            //item.endDate
                            //    ? DateUtils.formatDatePtBR(item.endDate)
                            //    : "",
                        ];
                    });
                    let indicatorSheetBuilder = new SheetBuilder();
                    indicatorSheetBuilder
                        .setHeader([
                            "Produto",
                            "Código do Produto Fornecedor",
                            "Quantidade",
                            "Pendentes",
                            "Vendidos",
                            "Valor em moedas",
                            "Valor em R$",
                            "Em estoque",
                            "Data de publicação",
                            "Data de expiração",
                            "Data de garantia/vencimento",
                            "Vencimento do voucher",
                            "Tipo de Estoque",
                            "Quem Cadastrou o produto",
                            "Data de Cadastro",
                            "Status",
                            "Visibilidade",
                        ])
                        .append(docRows)
                        .exportAs(`Relatório de Estoque`);
                    toast.success("Relatório exportado com sucesso!");
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    finishLoading();
                });
        } else {
            toast.warning("Selecione datas válidas");
        }
    }

    function handleReportExport2() {
        startLoading();

        if (
            startDate &&
            endDate &&
            isValid(startDate) &&
            isValid(endDate) &&
            endDate >= startDate
        ) {
            const formatedStartDate = format(
                new Date(startDate.toString()),
                "yyyy-MM-dd"
            );
            const formatedEndDate = format(
                new Date(endDate.toString()),
                "yyyy-MM-dd"
            );

            const payload = {
                startDate: formatedStartDate,
                endDate: formatedEndDate,
                productId: product ? product.id : undefined,
                quantity: quantity ? parseInt(quantity) : undefined,
                sector: selectedSector ? selectedSector.id : undefined,
                bestSeller: bestSeller,
                city: city,
                lessSold: lessSold,
                type: productType,
                status: status,
            };

            new ReportManageStockUseCase()
                .handle(payload)
                .then((data) => {
                    if (data.length <= 0) {
                        return toast.warning("Sem dados para exportar");
                    }

                    const docRows = data.map((item: any) => {
                        return [
                            item.product || "",
                            item.code || "",
                            item.createdAt || "",
                            item.inicial_amount || 0,
                            item.removed || 0,
                            item.added || 0,
                            item.sold || 0,
                            item.released || 0,
                            item.pending || 0,
                            item.final_balance || 0,
                            item.canceled || 0,
                            item.expired || 0,
                            item.price || 0,
                            "",
                            "",
                            item.reason,
                            item?.stock?.description || "",
                            "",
                            //item.expirationDateVoucher || "",
                            item.type || "",
                            item.registeredBy || "",
                            item.createdAt || "",
                            item.status || "",
                            item.Visibility || "",
                        ];
                    });
                    let indicatorSheetBuilder = new SheetBuilder();
                    indicatorSheetBuilder
                        .setHeader([
                            "Produto",
                            "Código do Produto",
                            "Data de cadastro",
                            "Saldo inicial",
                            "Saída",
                            "Entrada",
                            "Foi retirado",
                            "Liberado para entrega",
                            "Pedido",
                            "Saldo Final",
                            "Cancelado",
                            "Expirado",
                            "Valor em moedas",
                            "Valor em R$",
                            "Valor em R$ do estoque",
                            "Motivo",
                            "Em estoque",
                            "Data de garantia/vencimento",
                            //"Vencimento do voucher",
                            "Tipo de estoque",
                            "Quem cadastrou o produto",
                            "Data de atualização",
                            "Status",
                            "Visibilidade",
                        ])
                        .append(docRows)
                        .exportAs(`Relatório de Estoque`);
                    toast.success("Relatório exportado com sucesso!");
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    finishLoading();
                });
        } else {
            toast.warning("Selecione datas válidas");
        }
    }

    function handleReportExport3() {
        startLoading();

        if (
            startDate &&
            endDate &&
            isValid(startDate) &&
            isValid(endDate) &&
            endDate >= startDate
        ) {
            const formatedStartDate = format(
                new Date(startDate.toString()),
                "yyyy-MM-dd"
            );
            const formatedEndDate = format(
                new Date(endDate.toString()),
                "yyyy-MM-dd"
            );

            const payload = {
                startDate: formatedStartDate,
                endDate: formatedEndDate,
                productId: product ? product.id : undefined,
                quantity: quantity ? parseInt(quantity) : undefined,
                sector: selectedSector ? selectedSector.id : undefined,
                bestSeller: bestSeller,
                city: city,
                lessSold: lessSold,
                type: productType,
                status: status,
            };

            new GenerateVouchersStockReportUseCase()
                .handle(payload)
                .then((data) => {
                    if (data.length <= 0) {
                        return toast.warning("Sem dados para exportar");
                    }

                    const docRows = data.map((item: any) => {
                        let pubDate, expDate, valDate;
                        if (item.publicationDate) {
                            pubDate = new Date(item.publicationDate);
                            pubDate.setMinutes(
                                pubDate.getMinutes() +
                                    pubDate.getTimezoneOffset()
                            );
                        }
                        if (item.voucherValidity) {
                            const validity = item.voucherValidity;
                            valDate = new Date(validity);
                            valDate.setMinutes(
                                valDate.getMinutes() +
                                    valDate.getTimezoneOffset()
                            );
                        } else if (item.validity) {
                            const validity = item.validity;
                            valDate = new Date(validity);
                            valDate.setMinutes(
                                valDate.getMinutes() +
                                    valDate.getTimezoneOffset()
                            );
                        }
                        if (item.expirationDate) {
                            expDate = new Date(item.expirationDate);
                            expDate.setMinutes(
                                expDate.getMinutes() +
                                    expDate.getTimezoneOffset()
                            );
                        }
                        return [
                            item.stockName,
                            item.voucher,
                            item.productCode,
                            item.productName,
                            item.price,
                            item.createdAt
                                ? DateUtils.formatDatePtBR(item.createdAt)
                                : "",
                            pubDate ? DateUtils.formatDatePtBR(pubDate) : "",
                            expDate ? DateUtils.formatDatePtBR(expDate) : "",
                            valDate ? DateUtils.formatDatePtBR(valDate) : "",
                        ];
                    });
                    let indicatorSheetBuilder = new SheetBuilder();
                    indicatorSheetBuilder
                        .setHeader([
                            "Estoque",
                            "Link",
                            "Código do Produto",
                            "Nome do Produto",
                            "Valor em moedas",
                            "Data do cadastro",
                            "Data da publicação",
                            "Data de expiração",
                            "Data de validade",
                        ])
                        .append(docRows)
                        .exportAs(`Relatório de Estoque Vouchers`);
                    toast.success("Relatório exportado com sucesso!");
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    finishLoading();
                });
        } else {
            toast.warning("Selecione datas válidas");
        }
    }

    function handleReportExport4() {
        startLoading();

        if (
            startDate &&
            endDate &&
            isValid(startDate) &&
            isValid(endDate) &&
            endDate >= startDate
        ) {
            const formatedStartDate = format(
                new Date(startDate.toString()),
                "yyyy-MM-dd"
            );
            const formatedEndDate = format(
                new Date(endDate.toString()),
                "yyyy-MM-dd"
            );

            const payload = {
                startDate: formatedStartDate,
                endDate: formatedEndDate,
                productId: product ? product.id : undefined,
                quantity: quantity ? parseInt(quantity) : undefined,
                sector: selectedSector ? selectedSector.id : undefined,
                bestSeller: bestSeller,
                city: city,
                lessSold: lessSold,
                type: productType,
                status: status,
            };

            new GenerateVouchersSoldReportUseCase()
                .handle(payload)
                .then((data) => {
                    if (data.length <= 0) {
                        return toast.warning("Sem dados para exportar");
                    }

                    const docRows = data.map((item: any) => {
                        return [
                            item.buyDate
                                ? DateUtils.formatDatePtBR(item.buyDate)
                                : "",
                            item.buyerId,
                            item.buyerName,
                            item.hierarchy,
                            item.codProduct,
                            item.productName,
                            item.linkVoucher,
                            item.deliverDate
                                ? DateUtils.formatDatePtBR(
                                      new Date(
                                          new Date(item.deliverDate).setHours(
                                              new Date(
                                                  item.deliverDate
                                              ).getHours() + 3
                                          )
                                      )
                                  )
                                : "",
                            item.deliveredById,
                            item.deliveredBy,
                            item.MATRICULA_SUPERVISOR,
                            item.NOME_SUPERVISOR,
                            item.MATRICULA_COORDENADOR,
                            item.NOME_COORDENADOR,
                            item.MATRICULA_GERENTE_II,
                            item.NOME_GERENTE_II,
                            item.MATRICULA_GERENTE_I,
                            item.NOME_GERENTE_I,
                            item.MATRICULA_DIRETOR,
                            item.NOME_DIRETOR,
                            item.MATRICULA_CEO,
                            item.NOME_CEO,
                            item.SECTOR_ID,
                            item.SECTOR_NAME,
                            item.VOUCHER_VALIDITY
                                ? DateUtils.formatDatePtBR(
                                      new Date(
                                          new Date(
                                              item.VOUCHER_VALIDITY
                                          ).setHours(
                                              new Date(
                                                  item.VOUCHER_VALIDITY
                                              ).getHours() + 3
                                          )
                                      )
                                  )
                                : "",
                        ];
                    });
                    let indicatorSheetBuilder = new SheetBuilder();
                    indicatorSheetBuilder
                        .setHeader([
                            "Data da compra",
                            "Matricula Comprador",
                            "Comprador",
                            "Cargo",
                            "Código do produto",
                            "Nome do produto",
                            "Link Voucher",
                            "Data entrega",
                            "Matrícula do entregador",
                            "Entregue por",
                            "MATRICULA DO SUPERVISOR",
                            "SUPERVISOR",
                            "MATRICULA DO COORDENADOR",
                            "NOME DO COORDENADOR",
                            "MATRICULA DO GERENTE 2",
                            "NOME DO GERENTE 2",
                            "MATRICULA DO GERENTE 1",
                            "NOME DO GERENTE 1	",
                            "MATRICULA DO DIRETOR",
                            "NOME DO DIRETOR",
                            "MATRICULA DO CEO",
                            "NOME DO CEO",
                            "CODIGO GIP",
                            "SETOR",
                            "VALIDATE DO VOUCHER",
                        ])
                        .append(docRows)
                        .exportAs(`Relatório de Estoque Vouchers`);
                    toast.success("Relatório exportado com sucesso!");
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    finishLoading();
                });
        } else {
            toast.warning("Selecione datas válidas");
        }
    }

    function handleReportExport5() {
        startLoading();

        if (
            startDate &&
            endDate &&
            isValid(startDate) &&
            isValid(endDate) &&
            endDate >= startDate
        ) {
            const formatedStartDate = format(
                new Date(startDate.toString()),
                "yyyy-MM-dd"
            );
            const formatedEndDate = format(
                new Date(endDate.toString()),
                "yyyy-MM-dd"
            );

            const payload = {
                startDate: formatedStartDate,
                endDate: formatedEndDate,
            };

            new StockMovementListUseCase()
                .handle(payload)
                .then((data) => {
                    const docRows = data.map((item: any) => {
                        return [
                            item.data,
                            item.Produto,
                            item.EstoqueDeOrigem,
                            item.EstoqueFinal,
                            item.Quantidade,
                            item.Motivo,
                        ];
                    });
                    let indicatorSheetBuilder = new SheetBuilder();
                    indicatorSheetBuilder
                        .setHeader([
                            "Data",
                            "Produto",
                            "Estoque de origem",
                            "Estoque final",
                            "Quantidade",
                            "Motivo",
                        ])
                        .append(docRows)
                        .exportAs(
                            `Relatório de movimentação de produtos entre estoques`
                        );
                    toast.success("Relatório exportado com sucesso!");
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    finishLoading();
                });
        } else {
            toast.warning("Selecione datas válidas");
        }
    }

    return (
        <Box display="flex" flexDirection={"column"} width={"100%"}>
            <Card width={"100%"} display={"flex"} flexDirection={"column"}>
                <PageHeader
                    title={"Relatórios de estoque"}
                    headerIcon={<FileOpen />}
                />
                <Stack px={2} py={4} width={"100%"} gap={2}>
                    <Box
                        width={"100%"}
                        gap={2}
                        display={"flex"}
                        flexDirection={"column"}
                    >
                        <Box display={"flex"} gap={2} width={"100%"}>
                            <LocalizationProvider dateAdapter={AdapterDateFns}>
                                <DatePicker
                                    label="De"
                                    value={startDate}
                                    onChange={(newValue) =>
                                        setStartDate(newValue)
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
                                    value={endDate}
                                    onChange={(newValue) =>
                                        setEndDate(newValue)
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
                        </Box>
                        <Box display={"flex"} gap={2} width={"100%"}>
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
                                            e.target.value as
                                                | "PHYSICAL"
                                                | "DIGITAL"
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
                            <FormControl fullWidth>
                                <InputLabel
                                    size="small"
                                    sx={{ background: "#fff", px: "5px" }}
                                >
                                    Status
                                </InputLabel>
                                <Select
                                    value={status}
                                    size="small"
                                    onChange={(e) =>
                                        setStatus(
                                            e.target.value as
                                                | "Ativo"
                                                | "Inativo"
                                        )
                                    }
                                >
                                    <MenuItem value={""}>Todos</MenuItem>
                                    <MenuItem value={"Ativo"}>Ativo</MenuItem>
                                    <MenuItem value={"Inativo"}>
                                        Inativo
                                    </MenuItem>
                                </Select>
                            </FormControl>
                        </Box>
                        <Box display={"flex"} gap={2} width={"100%"}>
                            <Autocomplete
                                size={"small"}
                                options={products}
                                fullWidth
                                getOptionLabel={(option) =>
                                    option.comercialName
                                }
                                onChange={(event, value) => {
                                    setProduct(value);
                                }}
                                onInputChange={(e, text) =>
                                    setProductsSearchValue(text)
                                }
                                filterOptions={(x) => x}
                                filterSelectedOptions
                                renderInput={(params) => (
                                    <TextField
                                        {...params}
                                        variant="outlined"
                                        label="Selecione um produto"
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
                                value={selectedSector}
                                placeholder={"Setor"}
                                disableClearable={false}
                                onChange={(e, value) =>
                                    setSelectedSector(value)
                                }
                                onInputChange={(e, text) =>
                                    setSectorsSearchValue(text)
                                }
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
                                                        props.InputProps
                                                            .endAdornment
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
                        <Box display={"flex"} gap={2} width={"100%"}>
                            <Box
                                sx={{
                                    display: "flex",
                                    alignItem: "center",
                                }}
                            >
                                <Checkbox
                                    checked={bestSeller}
                                    onChange={(e) =>
                                        setBestSeller(e.target.checked)
                                    }
                                />
                                <Typography
                                    sx={{
                                        display: "flex",
                                        alignItems: "center",
                                    }}
                                >
                                    Mais vendidos?
                                </Typography>
                            </Box>
                            <Box
                                sx={{
                                    display: "flex",
                                    alignItem: "center",
                                }}
                            >
                                <Checkbox
                                    checked={lessSold}
                                    onChange={(e) =>
                                        setLessSold(e.target.checked)
                                    }
                                />
                                <Typography
                                    sx={{
                                        display: "flex",
                                        alignItems: "center",
                                    }}
                                >
                                    Menos vendidos?
                                </Typography>
                            </Box>
                        </Box>
                        <Box
                            display={"flex"}
                            width={"100%"}
                            flexDirection={"column"}
                        >
                            {abilityFor(myPermissions).can(
                                "Relatorios de estoque - padrão",
                                "Relatorios"
                            ) && (
                            <Button
                                variant="contained"
                                onClick={handleReportExport}
                                fullWidth
                                sx={{ mt: 2 }}
                            >
                                Exportar relatório - Padrão
                            </Button>
                            )}
                             {abilityFor(myPermissions).can(
                                "Relatorios de estoque - gerenciar estoque",
                                "Relatorios"
                            ) && (
                            <Button
                                variant="outlined"
                                onClick={handleReportExport2}
                                fullWidth
                                sx={{ mt: 2 }}
                            >
                                Exportar relatório - Gerenciar estoque
                            </Button>
                            )}
                            {abilityFor(myPermissions).can(
                                "Relatorios de estoque - entrada voucher",
                                "Relatorios"
                            ) && (
                                <Button
                                    variant="outlined"
                                    onClick={() => setVerifyStockVoucher(true)}
                                    fullWidth
                                    sx={{ mt: 2 }}
                                >
                                    Exportar relatório - Estoque vouchers
                                </Button>
                            )}
                            {abilityFor(myPermissions).can(
                                "Relatorios de estoque - saida voucher",
                                "Relatorios"
                            ) && (
                                <Button
                                    variant="outlined"
                                    onClick={() => setVerifyStockVoucher2(true)}
                                    fullWidth
                                    sx={{ mt: 2 }}
                                >
                                    Exportar relatório - Saída Vouchers
                                </Button>
                            )}
                            {abilityFor(myPermissions).can(
                                "Relatorios de estoque - movimentação",
                                "Relatorios"
                            ) && (
                            <Button
                                variant="outlined"
                                onClick={handleReportExport5}
                                fullWidth
                                sx={{ mt: 2 }}
                            >
                                Exportar relatório - Movimentação de produtos
                                entre estoques
                            </Button>
                            )}
                        </Box>
                    </Box>
                </Stack>
            </Card>
            <AuthVerifyComponent
                isOpen={verifyStockVoucher}
                onClose={() => setVerifyStockVoucher(false)}
                onVerified={handleReportExport3}
            />
            <AuthVerifyComponent
                isOpen={verifyStockVoucher2}
                onClose={() => setVerifyStockVoucher2(false)}
                onVerified={handleReportExport4}
            />
        </Box>
    );
}

StockReport.getLayout = getLayout("private");
