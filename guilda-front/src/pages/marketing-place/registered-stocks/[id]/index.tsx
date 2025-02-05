import CalendarMonth from "@mui/icons-material/CalendarMonth";
import Category from "@mui/icons-material/Category";
import DriveFileRenameOutline from "@mui/icons-material/DriveFileRenameOutline";
import Inventory from "@mui/icons-material/Inventory";
import Link from "@mui/icons-material/Link";
import LocationOn from "@mui/icons-material/LocationOn";
import CampaignIcon from "@mui/icons-material/Campaign";
import {
    Autocomplete,
    Box,
    Button,
    CardMedia,
    Divider,
    List,
    ListItem,
    ListItemIcon,
    ListItemText,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useLoadingState } from "src/hooks";
import { AssociateProducts } from "src/modules/marketing-place/use-cases/associate-products.use-case";
import { DeleteStock } from "src/modules/marketing-place/use-cases/delete-stock";
import { DetailsStockUseCase } from "src/modules/marketing-place/use-cases/details-stock";
import { ListProducts } from "src/modules/marketing-place/use-cases/list-products";
import { ListStocks } from "src/modules/marketing-place/use-cases/list-stocks";
import { EXCEPTION_CODES } from "src/typings";
import { Product } from "src/typings/models/product.model";
import { Stock } from "src/typings/models/stock.model";
import { DateUtils, getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";
import { ListProductStocks } from "src/modules/marketing-place/use-cases/list-product-stocks";
import { GenerateStockProductQrCodeUseCase } from "src/modules/marketing-place/use-cases/generate-stock-product-qrcode.use-case";
import { RemoveProductFromStockUseCase } from "src/modules/marketing-place/use-cases/remove-product-stock.use-case";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import PaginationComponent from "src/components/navigation/pagination/pagination";

interface StockProduct {
    productId: number;
    amount: number;
    product: Product;
    stockId: number;
    supplierId: number;
}

export default function StockDetails() {
    const [searchText, setSearchText] = useState<string>("");
    const { myPermissions } = useContext(PermissionsContext);
    const router = useRouter();
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [stock, setStock] = useState<Stock | null>(null);
    const [totalItems, setTotalItems] = useState<number>(0);
    const [pageNumber, setPageNumber] = useState<number>(1);
    const { id } = router.query;
    const [modalAssociateProductIsOpen, setModalAssociateProductIsOpen] =
        useState<boolean>(false);
    const [products, setProducts] = useState<Array<Product>>([]);
    const [stockList, setStocksList] = useState<Array<Stock>>([]);
    const LIMIT_PER_PAGE = 20;
    const [selectedProducts, setSelectedProducts] = useState<Product | null>(
        null
    );
    const [selectedStockList, setSelectedStockList] = useState<
        number | null | undefined
    >(null);
    const [stockAssociatedProducts, setStockAssociatedProducts] = useState<
        Array<StockProduct>
    >([]);
    const [amount, setAmount] = useState<number>(0);
    const [reason, setReason] = useState<string>("");
    const [qrCode, setQrCode] = useState<string | null>(null);
    const [removeReason, setRemoveReason] = useState<string>("");
    const [selectedToRemove, setSelectedToRemove] =
        useState<StockProduct | null>(null);

    const handleRedirectToStocksPage = () =>
        router.push("/marketing-place/registered-stocks");

    function generateQrCode(
        productId: number,
        stockId: number,
        supplierId: number
    ) {
        new GenerateStockProductQrCodeUseCase()
            .handle({
                productId: productId,
                stockId: stockId,
                supplierId: supplierId,
            })
            .then((data) => {
                setQrCode(data.qrCode);
            })
            .catch(() => {
                toast.error("Não foi possível gerar o QRCode deste produto");
            })
            .finally(() => {
                finishLoading();
            });
    }

    useEffect(() => {
        if (id !== undefined) {
            const payload = {
                id: id,
            };

            new DetailsStockUseCase()
                .handle(payload)
                .then((data) => {
                    setStock(data);
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    finishLoading();
                });
        }
    }, [id]);

    const getStockAssociatedProducts = async () => {
        if (stock?.description) {
            const payload = {
                limit: 10,
                offset: 0,
                searchText: stock.description,
                showAllProducts: true,
            };

            new ListStocks()
                .handle(payload)
                .then((data) => {
                    const findThisStock = data?.items?.find(
                        (item: any) => item.description === stock.description
                    );
                    findThisStock &&
                        setStockAssociatedProducts(
                            findThisStock.GdaStockProduct
                        );
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {});
        }
    };

    useEffect(() => {
        if (stock?.description) {
            getStockAssociatedProducts();
        }
    }, [stock]);

    const handleDeleteStock = async (stockId: string | string[]) => {
        startLoading();

        const payload = {
            id: stockId,
        };

        new DeleteStock()
            .handle(payload)
            .then((data) => {
                toast.success("Estoque apagado com sucesso!");
                handleRedirectToStocksPage();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    const handleAssociateProduct = async () => {
        if (selectedProducts) {
            startLoading();

            const payload = {
                products: selectedProducts.id,
                stock: id,
                amount: amount,
                stockToRemove: selectedStockList,
                reason: reason,
            };

            if (id !== undefined && selectedStockList !== undefined) {
                new AssociateProducts()
                    .handle(payload)
                    .then((data) => {
                        toast.success(`Produtos vinculados com sucesso!`);
                    })
                    .catch((e) => {
                        const errorCode = e?.response?.data?.code;
                        if (errorCode === EXCEPTION_CODES.CAN_NOT_REMOVE_ITEM) {
                            return toast.error(
                                `Não é possível remover a quantidade de itens no produto.`
                            );
                        } else if (
                            e?.response?.data?.keys?.productType &&
                            e?.response?.data?.keys?.stockType
                        ) {
                            return toast.error(
                                `Não é possível associar um produto 
                                ${
                                    e?.response?.data?.keys?.productType ===
                                    "PHYSICAL"
                                        ? "físico"
                                        : "vitual"
                                }
                                a um estoque ${
                                    e?.response?.data?.keys?.stockType ===
                                    "PHYSICAL"
                                        ? "físico"
                                        : "vitual"
                                }.`
                            );
                        }
                        toast.error(`${e?.response?.data?.message}`);
                    })
                    .finally(() => {
                        finishLoading();
                        getStockAssociatedProducts();
                    });
            }
        }
    };

    const getStocks = async () => {
        startLoading();

        const payload = {
            id: selectedProducts?.id,
        };

        new ListProductStocks()
            .handle(payload)
            .then((data) => {
                setStocksList(data);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    const getProducts = async () => {
        startLoading();

        const payload = {
            limit: LIMIT_PER_PAGE,
            offset: 0,
            searchText: searchText,
        };

        new ListProducts()
            .handle(payload)
            .then((data) => {
                setProducts(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(async () => {
                await getStocks();
                finishLoading();
            });
    };

    useEffect(() => {
        getProducts();
    }, [searchText]);

    function handleRemoveProduct() {
        if (!selectedToRemove) {
            return;
        }

        new RemoveProductFromStockUseCase()
            .handle({
                productId: selectedToRemove.productId,
                reason: removeReason,
                stockId: selectedToRemove.stockId,
                supplierId: selectedToRemove.supplierId,
            })
            .then(() => {
                toast.success("Produto removido com sucesso!");
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(async () => {
                await getStocks();
                finishLoading();
            });
    }

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={`Detalhe de estoque: ${stock?.description}`}
                headerIcon={<Inventory />}
            />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                {stock && (
                    <Box
                        sx={{
                            width: "100%",
                            flexDirection: "column",
                            display: "flex",
                            gap: "10px",
                        }}
                    >
                        <List dense={true}>
                            <ListItem>
                                <ListItemIcon>
                                    <DriveFileRenameOutline />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Nome do estoque"
                                    secondary={stock.description}
                                />
                            </ListItem>
                            <Divider />
                            <ListItem>
                                <ListItemIcon>
                                    <LocationOn />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Unidade"
                                    secondary={stock.value}
                                />
                            </ListItem>
                            <Divider />
                            <ListItem>
                                <ListItemIcon>
                                    <Inventory />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Tipo de estoque"
                                    secondary={(() => {
                                        let typeReturn = "";
                                        if (stock?.type) {
                                            stock?.type === "PHYSICAL"
                                                ? (typeReturn = "FÍSICO")
                                                : (typeReturn = "VIRTUAL");
                                        }
                                        return typeReturn;
                                    })()}
                                />
                            </ListItem>
                            <ListItem>
                                <ListItemIcon>
                                    <CampaignIcon />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Campanha"
                                    secondary={stock?.campaign}
                                />
                            </ListItem>
                            <Divider />
                            <ListItem>
                                <ListItemIcon>
                                    <CalendarMonth />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Criado em"
                                    secondary={
                                        stock.createdAt
                                            ? DateUtils.formatDatePtBRWithTime(
                                                  stock.createdAt
                                              )
                                            : ""
                                    }
                                />
                            </ListItem>
                        </List>
                        <Card>
                            <PageHeader
                                title={"Produtos vinculados"}
                                headerIcon={<Category />}
                                addButtonTitle="Movimentar Produto"
                                addButtonAction={() => {
                                    setSearchText("");
                                    setModalAssociateProductIsOpen(true);
                                }}
                                addButtonIsDisabled={abilityFor(
                                    myPermissions
                                ).cannot("Editar Estoque", "Marketing Place")}
                            />
                            <Box
                                display={"flex"}
                                flexDirection={"column"}
                                gap={1}
                                p={2}
                            >
                                {stockAssociatedProducts.map((item, index) => (
                                    <Box
                                        key={index}
                                        display={"flex"}
                                        gap={2}
                                        sx={{
                                            p: "10px",
                                            border: "solid 1px #f2f2f2",
                                        }}
                                        justifyContent={"space-between"}
                                    >
                                        <Typography>
                                            Produto :{" "}
                                            {item.product.comercialName
                                                ? item.product.comercialName
                                                : item.product.description}
                                        </Typography>
                                        <Box
                                            display={"flex"}
                                            flexDirection={"row"}
                                            alignItems={"center"}
                                            gap={2}
                                        >
                                            {item?.amount >= 0 && (
                                                <Typography>
                                                    Quantidade : {item.amount}
                                                </Typography>
                                            )}
                                            <Button
                                                variant="outlined"
                                                onClick={() => {
                                                    generateQrCode(
                                                        item.productId,
                                                        item.stockId,
                                                        item.supplierId
                                                    );
                                                }}
                                            >
                                                QRCode
                                            </Button>
                                            <Button
                                                variant="outlined"
                                                onClick={() => {
                                                    setSelectedToRemove(item);
                                                }}
                                                color="error"
                                            >
                                                Remover
                                            </Button>
                                        </Box>
                                    </Box>
                                ))}
                            </Box>
                            {/* <Box>
                                <PaginationComponent 
                                    totalItems={totalItems}
                                    limit={30}
                                    page={pageNumber}
                                    getPage={(page: number) => setPageNumber(page)}
                                />
                            </Box> */}
                        </Card>
                    </Box>
                )}
                <Box display={"flex"} justifyContent={"flex-end"}>
                    <Button
                        color="error"
                        disabled={abilityFor(myPermissions).cannot(
                            "Editar Estoque",
                            "Marketing Place"
                        )}
                        onClick={() => {
                            if (id !== undefined) {
                                handleDeleteStock(id);
                            }
                        }}
                        variant="contained"
                    >
                        Apagar estoque
                    </Button>
                </Box>
            </Stack>
            <BaseModal
                width={"540px"}
                open={modalAssociateProductIsOpen}
                title={"Movimentar produto"}
                onClose={() => setModalAssociateProductIsOpen(false)}
            >
                <Box width={"100%"}>
                    <Autocomplete
                        size={"small"}
                        options={products}
                        getOptionLabel={(option) =>
                            option?.comercialName
                                ? option?.comercialName
                                : option?.description
                        }
                        onChange={(event, value) => {
                            setSelectedProducts(value);
                        }}
                        onInputChange={(e, text) => setSearchText(text)}
                        filterOptions={(x) => x}
                        filterSelectedOptions
                        disableClearable={false}
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
                                    {option?.comercialName
                                        ? option?.comercialName
                                        : option?.description}
                                </li>
                            );
                        }}
                        isOptionEqualToValue={(option, value) =>
                            option?.comercialName === value.comercialName
                        }
                    />
                    <Autocomplete
                        // multiple
                        size={"small"}
                        options={stockList}
                        getOptionLabel={(option) => option.description}
                        onChange={(event, value) => {
                            setSelectedStockList(value?.id);
                        }}
                        filterOptions={(x) => x}
                        filterSelectedOptions
                        disableClearable={false}
                        renderInput={(params) => (
                            <TextField
                                {...params}
                                variant="outlined"
                                label="De qual estoque devo mover o produto?"
                                placeholder="Buscar"
                            />
                        )}
                        renderOption={(props, option) => {
                            return (
                                <li {...props} key={option.id}>
                                    {`${option.description}`}
                                </li>
                            );
                        }}
                        isOptionEqualToValue={(option, value) =>
                            option?.id === value.id
                        }
                    />
                    <TextField
                        label={"Quantidade"}
                        size="small"
                        fullWidth
                        type="number"
                        value={amount}
                        onChange={(e) => setAmount(parseInt(e.target.value))}
                    ></TextField>
                    <TextField
                        label={"Motivo"}
                        size="small"
                        margin="normal"
                        fullWidth
                        type="string"
                        value={reason}
                        onChange={(e) => setReason(e.target.value)}
                    ></TextField>
                    <Button
                        variant="contained"
                        onClick={handleAssociateProduct}
                        fullWidth
                        sx={{ mt: 2 }}
                    >
                        Vincular produto
                    </Button>
                </Box>
            </BaseModal>
            <BaseModal
                width={"540px"}
                open={!!qrCode}
                title={"QRCode"}
                onClose={() => setQrCode(null)}
            >
                <Box width={"100%"}>
                    {qrCode && (
                        <CardMedia
                            component="img"
                            image={qrCode}
                            sx={{
                                width: "100px",
                                objectFit: "contain",
                            }}
                        />
                    )}
                </Box>
            </BaseModal>
            <BaseModal
                width={"540px"}
                open={!!selectedToRemove}
                title={"Remover produto"}
                onClose={() => setSelectedToRemove(null)}
            >
                <Box width={"100%"}>
                    <TextField
                        label="Motivo"
                        fullWidth
                        value={removeReason}
                        onChange={(e) => setRemoveReason(e.target.value)}
                    />
                    <Button
                        onClick={handleRemoveProduct}
                        fullWidth
                        color="error"
                        variant="contained"
                    >
                        Remover
                    </Button>
                </Box>
            </BaseModal>
        </Card>
    );
}

StockDetails.getLayout = getLayout("private");
