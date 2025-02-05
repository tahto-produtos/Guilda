import Settings from "@mui/icons-material/Settings";
import { Autocomplete, Box, Button, Chip, Stack, TextField } from "@mui/material";
import { grey } from "@mui/material/colors";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format } from "date-fns";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { AddQntProductUseCase } from "src/modules/marketing-place/use-cases/add-quantity-product.use-case";
import { ListProducts } from "src/modules/marketing-place/use-cases/list-products";
import { ListStocks } from "src/modules/marketing-place/use-cases/list-stocks";
import { ListSupplierUseCase } from "src/modules/marketing-place/use-cases/list-suppliers.use-case";
import { Product } from "src/typings/models/product.model";
import { Stock } from "src/typings/models/stock.model";
import { getLayout } from "src/utils";

export interface Supplier {
    supplierName: string;
    createdAt: string;
    createdByCollaboratorId: number;
    deletedAt: string;
    id: number;
}

export default function ProductQuantity() {
    const [searchText, setSearchText] = useState<string>("");
    const [searchTextSupplier, setSearchTextSupplier] = useState<string>("");
    const [searchTextStock, setSearchTextStock] = useState<string>("");
    const [products, setProducts] = useState<Product[]>([]);
    const [stocks, setStocks] = useState<Stock[]>([]);
    const [filteredStocks, setFilteredStocks] = useState<Stock[]>([]);
    const [qnt, setQnt] = useState<number>(0);
    const [selectedStock, setSelectedStock] = useState<Stock | null>(null);
    const [selectedProducts, setSelectedProducts] = useState<Product | null>(
        null
    );
    const [voucherTextValue, setVoucherTextValue] = useState<string>("");
    const [vouchers, setVouchers] = useState<string[]>([]);
    const [validityDate, setValidityDate] = useState<dateFns | Date | null>(
        null
    );

    const getStocks = async () => {
        const payload = {
            limit: 20,
            offset: 0,
            searchText: searchTextStock,
        };

        new ListStocks()
            .handle(payload)
            .then((data) => {
                setStocks(data.items);
                setFilteredStocks(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {});
    };

    useEffect(() => {
        getStocks();
    }, [searchTextStock]);

    const getProducts = async () => {
        const payload = {
            limit: 20,
            offset: 0,
            searchText: searchText,
            recents: true,
            bestSeller: false,
        };

        new ListProducts()
            .handle(payload)
            .then((data) => {
                setProducts(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {});
    };

    useEffect(() => {
        getProducts();
    }, [searchText]);

    useEffect(() => {
            setFilteredStocks(stocks);
            setSelectedStock(null);
            setFilteredStocks([]);
    }, [selectedProducts]);

    useEffect(() => {
        const filteredStocksShow = stocks.filter(stock => stock.type === selectedProducts?.type);
        setFilteredStocks(filteredStocksShow);
    }, [filteredStocks]);

    function handleConfirm() {
        if (!selectedProducts || !qnt || !selectedStock) {
            return;
        }

        if(selectedProducts.type === 'DIGITAL' && !validityDate) {
            return;
        }

        const formatedValidtDateDate = validityDate ? format(new Date(validityDate.toString()), "yyyy-MM-dd") : "";

        new AddQntProductUseCase()
            .handle({
                productId: selectedProducts?.id,
                quantity: qnt,
                stockId: selectedStock?.id,
                vouchers: vouchers,
                validtyDate: formatedValidtDateDate,
            })
            .then((data) => {
                toast.success("Quantidade adicionada com sucesso!");
            })
            .catch((data) => {
                if(data?.response?.data?.message) {
                    return toast.warning(data.response.data.message);
                }
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Entrada de produtos"}
                headerIcon={<Settings />}
            />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                <Autocomplete
                    value={selectedProducts}
                    placeholder={"Produtos"}
                    filterOptions={(x) => x}
                    disableClearable={false}
                    onChange={(_, item) => setSelectedProducts(item)}
                    onInputChange={(e, text) => setSearchText(text)}
                    isOptionEqualToValue={(option, value) =>
                        option.comercialName === value.comercialName
                    }
                    renderInput={(props) => (
                        <TextField
                            {...props}
                            size={"small"}
                            label={"Produto"}
                        />
                    )}
                    renderTags={() => null}
                    getOptionLabel={(option) => option.comercialName}
                    options={products}
                    sx={{ mb: 0 }}
                />
                <Autocomplete
                    value={selectedStock}
                    placeholder={"Estoques"}
                    filterOptions={(x) => x}
                    disableClearable={false}
                    onChange={(_, item) => setSelectedStock(item)}
                    onInputChange={(e, text) => setSearchTextStock(text)}
                    isOptionEqualToValue={(option, value) =>
                        option.description === value.description
                    }
                    renderInput={(props) => (
                        <TextField
                            {...props}
                            size={"small"}
                            label={"Estoque"}
                        />
                    )}
                    renderTags={() => null}
                    getOptionLabel={(option) => option.description}
                    options={filteredStocks}
                    sx={{ mb: 0 }}
                />
                <TextField
                    type="number"
                    label="Quantidade"
                    value={qnt}
                    onChange={(e) => setQnt(parseInt(e.target.value))}
                />
                {(selectedProducts && selectedProducts.type === 'DIGITAL') && (
                    <>
                        <Box
                            display={"flex"}
                            alignItems={"center"}
                            gap={1}
                            width={"100%"}
                        >
                            <LocalizationProvider dateAdapter={AdapterDateFns}>
                                <DatePicker
                                    label="Validade do Voucher"
                                    value={validityDate}
                                    onChange={(newValue) =>
                                        setValidityDate(newValue)
                                    }
                                    slotProps={{
                                        textField: {
                                            size: "small",
                                            sx: {
                                                minWidth: "180px",
                                                svg: {
                                                    color: grey[500],
                                                },
                                                width: "30%",
                                            },
                                        },
                                    }}
                                />
                            </LocalizationProvider>
                            <TextField
                                label="Adicionar voucher"
                                size="small"
                                value={voucherTextValue}
                                sx={{ width: "100%" }}
                                onChange={(e) => {
                                    setVoucherTextValue(e.target.value);
                                }}
                            />
                            <Button
                                variant="contained"
                                onClick={() => {
                                    if (
                                        vouchers.find(
                                            (item) => item === voucherTextValue
                                        )
                                    ) {
                                        return toast.warning(
                                            "Esse código de voucher já foi adicionado"
                                        );
                                    }
                                    setVouchers((vouchers) => [
                                        ...vouchers,
                                        voucherTextValue,
                                    ]);
                                }}
                            >
                                Adicionar
                            </Button>
                        </Box>
                        <Box
                            display={"flex"}
                            alignItems={"center"}
                            gap={1}
                            width={"100%"}
                            flexWrap={"wrap"}
                        >
                            {vouchers.map((item, index) => (
                                <Chip
                                    label={item}
                                    key={index}
                                    onDelete={() => {
                                        setVouchers((current) =>
                                            current.filter(
                                                (voucher) => voucher !== item
                                            )
                                        );
                                    }}
                                />
                            ))}
                        </Box>
                    </>
                )}
                <Button variant="contained" onClick={handleConfirm}>
                    Adicionar quantidade
                </Button>
            </Stack>
        </Card>
    );
}

ProductQuantity.getLayout = getLayout("private");
