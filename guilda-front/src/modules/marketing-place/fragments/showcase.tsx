import { Theme } from "@emotion/react";
import {
    Alert,
    Box,
    Button,
    CircularProgress,
    Grid,
    SxProps,
    TextField,
    Typography,
} from "@mui/material";
import { styled } from "@mui/material/styles";
import Paper from "@mui/material/Paper";
import { grey } from "@mui/material/colors";
import AddShoppingCart from "@mui/icons-material/AddShoppingCart";
import { Product } from "src/typings/models/product.model";
import { useLoadingState } from "src/hooks";
import { CreateOrderUseCase } from "../use-cases/create-order";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useContext, useEffect, useState } from "react";
import { Stock } from "src/typings/models/stock.model";
import { StockProduct } from "src/typings/models/stock-product.model";
import { BaseModal } from "src/components/feedback";
import { ShoppingCartContext } from "src/contexts/shopping-cart-context/shopping-cart-context";
import { LoadingButton } from "@mui/lab";
import { formatCurrency } from "src/utils/format-currency";
import { StoreProduct } from "src/typings/models/store-products";

export interface ShowcaseProduct {
    comercialNameion: string | number | undefined;
    stockProductId: string | number | undefined;
    price: string | number | undefined;
}

export interface ShowcaseProps {
    sx?: SxProps<Theme>;
    products: StoreProduct[];
    isLoading?: boolean;
}

export default function Showcase(props: ShowcaseProps) {
    const { updateCart, shoppingCartIsLoading } =
        useContext(ShoppingCartContext);
    const { sx, products, isLoading } = props;
    const [loadingCreateOrder, setLoadingCreateOrder] =
        useState<boolean>(false);
    const [productList, setProductList] = useState<ShowcaseProduct[]>([]);
    const [selectedProduct, setSelectedProduct] =
        useState<GridItemProps | null>(null);
    const [orderNotes, setOrderNotes] = useState<string>("");
    const modalProductIsOpen = selectedProduct ? true : false;
    const [itemQnt, setItemQnt] = useState<number>(1);

    const loadingAll = isLoading || loadingCreateOrder;

    const Item = styled(Paper)(({ theme }) => ({
        boxShadow: "none",
        border: `solid 1px ${grey[200]}`,
        padding: "10px 10px",
        display: "flex",
        flexDirection: "column",
        justifyContent: "center",
        alignItems: "center",
    }));

    useEffect(() => {
        setItemQnt(1);
    }, [selectedProduct]);

    const handleCreateOrder = async (
        stockProductId: number | string | undefined,
        amount: number | string | undefined
    ) => {
        const payload = {
            stockProductId: stockProductId,
            amount: amount,
            whoReceived: orderNotes.length > 0 ? orderNotes : null,
        };

        new CreateOrderUseCase()
            .handle(payload)
            .then((data) => {
                toast.success("Pedido criado com sucesso!");
            })
            .catch((e) => {
                if (e?.response?.data?.Message !== "") {
                    toast.warning(e?.response?.data?.Message);
                } else {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                }
                /* if (e.response.data.code === "OUT_OF_RANGE") {
                    toast.warning(
                        `Você tem apenas ${e.response.data.keys.currentBalance} moedas e o item custa ${e.response.data.keys.totalPrice} moedas`
                    );
                } else if (e.response.data.code === "OUT_OF_STOCK") {
                    toast.warning(`Sem itens no estoque`);
                }
                else if (e?.response?.data?.Message == "Moedas insuficientes!") {
                    toast.warning("Você não tem moedas suficientes");
                } else if (e?.response?.data?.Message == "Nenhum produto no carrinho!") {
                    toast.warning("Nenhum produto no carrinho!");
                 } else {
                    console.log("Aqui");
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                } */
            })
            .finally(() => {});
    };

    interface GridItemProps {
        name: string | number | undefined;
        price?: string | number | undefined;
        id: string | number | undefined;
        image: string;
        description: string;
        saleLimit?: number;
    }

    const GridItem = (props: GridItemProps) => {
        const { name, price, id, image } = props;
        return (
            <Grid item xs={3}>
                <Item
                    sx={{
                        height: "400px",
                        display: "flex",
                        flexDirection: "column",
                        justifyContent: "flex-start",
                    }}
                >
                    <Box
                        sx={{
                            width: "250px",
                            height: "250px",
                            backgroundColor: grey[200],
                        }}
                    >
                        {image && image !== undefined && (
                            <img
                                alt="imagem"
                                src={image}
                                style={{ width: "100%", height: "100%" }}
                            />
                        )}
                    </Box>
                    <Typography
                        mt={"10px"}
                        variant="body2"
                        fontSize={"16px"}
                        fontWeight={"500"}
                        textAlign={"center"}
                        height={"50px"}
                        overflow={"hidden"}
                    >
                        {name}
                    </Typography>
                    {price && (
                        <Typography variant="body2" color={grey[700]}>
                            {formatCurrency(parseInt(price.toString()))} moedas
                        </Typography>
                    )}
                    <Button
                        variant="contained"
                        color="success"
                        sx={{ mt: 1 }}
                        onClick={() => setSelectedProduct(props)}
                        fullWidth
                    >
                        Ver produto
                    </Button>
                </Item>
            </Grid>
        );
    };

    return (
        <Grid container spacing={1}>
            {loadingAll && (
                <Box
                    display={"flex"}
                    justifyContent={"center"}
                    alignItems={"center"}
                    p={4}
                    width={"100%"}
                >
                    <CircularProgress />
                </Box>
            )}
            {!loadingAll &&
                products.map((item, index) => (
                    <GridItem
                        name={item?.comercialName}
                        key={index}
                        price={item.price}
                        id={item.GdaStockProduct[0]?.id}
                        image={
                            item?.productImages?.length > 0
                                ? item?.productImages[0]?.upload.url
                                : ""
                        }
                        description={item.description}
                        saleLimit={item?.saleLimit || undefined}
                    />
                ))}
            <BaseModal
                width={"540px"}
                open={modalProductIsOpen}
                title={"Detalhes do produto"}
                onClose={() => setSelectedProduct(null)}
            >
                <Box width={"100%"}>
                    <Box display={"flex"} gap={2} width={"100%"} mt={2}>
                        <Box
                            sx={{
                                width: "200px",
                                height: "200px",
                                backgroundColor: grey[200],
                            }}
                        >
                            {selectedProduct &&
                                selectedProduct?.image !== undefined && (
                                    <img
                                        alt="imagem"
                                        src={selectedProduct?.image}
                                        style={{
                                            width: "200px",
                                            height: "200px",
                                        }}
                                    />
                                )}
                        </Box>
                        <Box
                            width={"100%"}
                            display={"flex"}
                            gap={2}
                            flexDirection={"column"}
                            justifyContent={"center"}
                        >
                            <Box
                                display={"flex"}
                                flexDirection={"column"}
                                gap={"1px"}
                            >
                                <Typography
                                    mt={"10px"}
                                    variant="body2"
                                    fontSize={"22px"}
                                    fontWeight={"500"}
                                >
                                    {selectedProduct?.name}
                                </Typography>
                                <Typography variant="body2" fontSize={"14px"}>
                                    {selectedProduct?.description}
                                </Typography>
                            </Box>

                            {selectedProduct?.price && (
                                <Typography
                                    variant="body2"
                                    color={grey[700]}
                                    fontSize={"20px"}
                                >
                                    {selectedProduct?.price.toLocaleString(
                                        "pt-BR"
                                    )}{" "}
                                    moedas
                                </Typography>
                            )}
                            {/* <TextField
                                fullWidth
                                label="Quem vai receber/retirar o produto?"
                                value={orderNotes}
                                onChange={(e) => setOrderNotes(e.target.value)}
                            /> */}
                        </Box>
                    </Box>
                    <TextField
                        type="number"
                        label="Quantidade"
                        size="small"
                        value={itemQnt}
                        fullWidth
                        sx={{
                            mt: 4,
                            "& .MuiOutlinedInput-input": {
                                textAlign: "center",
                            },
                        }}
                        onChange={(event) => {
                            const number = parseInt(event.target.value);

                            if (Number.isNaN(number)) {
                                setItemQnt(1);
                            } else if (number <= 0) {
                                setItemQnt(1);
                            } else {
                                setItemQnt(number);
                            }
                        }}
                    />
                    <LoadingButton
                        variant="contained"
                        onClick={() =>
                            selectedProduct &&
                            selectedProduct.id &&
                            updateCart({
                                stockProductId: parseInt(
                                    selectedProduct?.id?.toString()
                                ),
                                amount: itemQnt,
                            })
                        }
                        fullWidth
                        sx={{ mt: 2 }}
                        color="success"
                        loading={shoppingCartIsLoading}
                    >
                        Adicionar ao carrinho
                    </LoadingButton>
                    {selectedProduct?.saleLimit && (
                        <Alert severity="info" sx={{ mt: 1 }}>
                            Limite de compra: {selectedProduct?.saleLimit}{" "}
                            produtos
                        </Alert>
                    )}
                </Box>
            </BaseModal>
        </Grid>
    );
}
