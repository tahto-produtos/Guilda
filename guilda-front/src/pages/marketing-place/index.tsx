import Storefront from "@mui/icons-material/Storefront";
import {
    Alert,
    Box,
    Button,
    CardMedia,
    FormControl,
    InputLabel,
    MenuItem,
    Pagination,
    Select,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { useRouter } from "next/router";
import { useContext, useEffect, useRef, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useDebounce, useLoadingState } from "src/hooks";
import Showcase from "src/modules/marketing-place/fragments/showcase";
import { ListProducts } from "src/modules/marketing-place/use-cases/list-products";
import { ListStocks } from "src/modules/marketing-place/use-cases/list-stocks";
import { Product } from "src/typings/models/product.model";
import { StockProduct } from "src/typings/models/stock-product.model";
import { Stock } from "src/typings/models/stock.model";
import { getLayout } from "src/utils";
import Carousel from "react-multi-carousel";
import "react-multi-carousel/lib/styles.css";
import { ListDisplayProductsUseCase } from "src/modules/marketing-place/use-cases/list-display-products.use-case";
import { BasketGeneralUser } from "src/modules/indicators/use-cases/basket-general-user.use-case";
import { ListStockStore } from "src/modules/marketing-place/use-cases/list-stock-store.use-case";
import { BaseModal } from "src/components/feedback";
import { grey } from "@mui/material/colors";
import { LoadingButton } from "@mui/lab";
import { ShoppingCartContext } from "src/contexts/shopping-cart-context/shopping-cart-context";
import { ListMyStoreProducts } from "src/modules/marketing-place/use-cases/list-my-store-products.use-case";
import { StoreProduct } from "src/typings/models/store-products";
import { UserInfoContext } from "src/contexts/user-context/user.context";

interface DisplayProduct {
    DESCRIPTION: string;
    IDGDA_STOCK_PRODUCT: number;
    PRICE: number;
    URL: string;
    COMERCIAL_NAME: string;
    SALE_LIMIT: number;
    NOME_VITRINE: string;
}

interface GridItemProps {
    name: string | number | undefined;
    price?: string | number | undefined;
    id: string | number | undefined;
    image: string;
    description: string;
    saleLimit?: number;
}
export default function MarketingPlace() {
    const { updateCart, shoppingCartIsLoading } =
        useContext(ShoppingCartContext);
    const elementoRef = useRef<any>(null);
    const [larguraElemento, setLarguraElemento] = useState(0);
    const [searchText, setSearchText] = useState<string>("");
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [page, setPage] = useState<number>(1);
    const handleChangePagination = (
        event: React.ChangeEvent<unknown>,
        value: number
    ) => {
        setPage(value);
    };
    const [totalPages, setTotalPages] = useState<number>(0);
    const [products, setProducts] = useState<Array<StoreProduct>>([]);
    const [orderedBy, setOrderedBy] = useState<string>("");
    const { myUser, userGroup, userGroupName } = useContext(UserInfoContext);
    const [displayProducts, setDisplayProducts] = useState<DisplayProduct[]>(
        []
    );
    const [searchProductText, setSearchProductText] = useState<string>("");
    const [selectedProduct, setSelectedProduct] =
        useState<GridItemProps | null>(null);
    const [orderNotes, setOrderNotes] = useState<string>("");
    const modalProductIsOpen = selectedProduct ? true : false;
    const [itemQnt, setItemQnt] = useState<number>(1);

    const sizeListener = elementoRef?.current?.offsetWidth;

    useEffect(() => {
        const elemento = elementoRef.current;

        if (elemento) {
            const largura = elemento.offsetWidth;
            setLarguraElemento(largura);
        }

        // Adicione um ouvinte de redimensionamento da janela, se necessário
        const handleResize = () => {
            if (elemento) {
                const largura = elemento.offsetWidth;
                setLarguraElemento(largura);
            }
        };

        window.addEventListener("resize", handleResize);

        return () => {
            // Certifique-se de remover o ouvinte de redimensionamento ao desmontar o componente
            window.removeEventListener("resize", handleResize);
        };
    }, [sizeListener]);

    const getStock = async () => {
        startLoading();

        const payload = {
            limit: 999,
            offset: 0,
            searchText: searchProductText,
            highlight: orderedBy == "highlight",
            oldStock: orderedBy == "oldStock",
            releasedStock: orderedBy == "releasedStock",
            userGroup: userGroupName || "",
        };

        new ListMyStoreProducts()
            .handle(payload)
            .then((data) => {
                setProducts(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    useEffect(() => {
        userGroupName !== null && getStock();
    }, [orderedBy, searchProductText, userGroupName]);

    useEffect(() => {
        // getProducts();
    }, [page]);

    async function getDisplayProducts() {
        if (userGroup == null || !myUser) {
            return;
        }

        await new ListDisplayProductsUseCase()
            .handle(myUser.hierarchyId, userGroup, myUser.id)
            .then((data) => {
                setDisplayProducts(data);
            })
            .catch(() => {})
            .finally(() => {
                finishLoading();
            });
    }

    useEffect(() => {
        myUser && !!userGroup && getDisplayProducts();
    }, [myUser, userGroup]);

    const responsive = {
        mobile: {
            breakpoint: { max: 4000, min: 0 },
            items: 1,
        },
    };

    function CarouselItem({ product }: { product: DisplayProduct }) {
        return (
            <Box
                width={"100%"}
                height={"400px"}
                display={"flex"}
                justifyContent={"center"}
                alignItems={"center"}
                sx={{ backgroundColor: "#2a2052" }}
            >
                <CardMedia
                    component="img"
                    alt="Uploaded image"
                    image={product.URL}
                    sx={{
                        width: "300px",
                        height: "300px",
                        borderRadius: "20px",
                    }}
                />
                <Stack direction={"column"} gap={"20px"}>
                    <Typography
                        variant="h1"
                        color={"#fff"}
                        lineHeight={"14px"}
                        fontSize={"14px"}
                        maxWidth={"400px"}
                        textTransform={"uppercase"}
                        letterSpacing={1}
                    >
                        {product.NOME_VITRINE}
                    </Typography>
                    <Typography
                        variant="h1"
                        color={"#fff"}
                        lineHeight={"42px"}
                        fontSize={"40px"}
                        maxWidth={"400px"}
                    >
                        {product.COMERCIAL_NAME}
                    </Typography>
                    <Typography
                        variant="h1"
                        color={"#fff"}
                        fontSize={"25px"}
                        maxWidth={"400px"}
                    >
                        {product.PRICE.toLocaleString("pt-BR")} moeda
                        {product.PRICE > 1 ? "s" : ""}
                    </Typography>
                    <Button
                        variant="contained"
                        color="success"
                        sx={{ width: "200px", mt: "20px" }}
                        onClick={() =>
                            setSelectedProduct({
                                id: product.IDGDA_STOCK_PRODUCT,
                                image: product.URL,
                                name: product.COMERCIAL_NAME,
                                price: product.PRICE.toLocaleString("pt-BR"),
                                description: product.DESCRIPTION,
                                saleLimit: product.SALE_LIMIT,
                            })
                        }
                    >
                        Ver produto
                    </Button>
                </Stack>
            </Box>
        );
    }

    return (
        <Box display="flex" flexDirection={"column"} width={"100%"}>
            {displayProducts &&
                Array.isArray(displayProducts) &&
                elementoRef &&
                displayProducts?.length > 0 && (
                    <Box
                        sx={{
                            width: larguraElemento,
                            mb: "40px",
                            height: "400px",
                            boxShadow: "0px 10px 10px 0px #e0e0e0",
                        }}
                    >
                        <Carousel
                            responsive={responsive}
                            autoPlay={true}
                            infinite={true}
                            autoPlaySpeed={5000}
                        >
                            {displayProducts?.map((product, index) => (
                                <CarouselItem key={index} product={product} />
                            ))}
                        </Carousel>
                    </Box>
                )}
            <Card
                width={"100%"}
                display={"flex"}
                flexDirection={"column"}
                ref={elementoRef}
            >
                <PageHeader title={"Loja"} headerIcon={<Storefront />} />
                <Stack px={2} py={4} width={"100%"} gap={2}>
                    {/* <TextField
                    placeholder="Buscar produto"
                    onChange={(e) => setSearchText(e.target.value)}
                    value={searchText}
                /> */}
                    <TextField
                        label="Buscar produto"
                        onChange={(e) => setSearchProductText(e.target.value)}
                    />
                    <FormControl sx={{ m: 1, width: "100%" }}>
                        <InputLabel sx={{ backgroundColor: "#fff", px: 1 }}>
                            Ordenar por:
                        </InputLabel>
                        <Select
                            onChange={(e) => setOrderedBy(e.target.value)}
                            value={orderedBy}
                        >
                            <MenuItem value={"default"}>Padrão</MenuItem>
                            {/* <MenuItem value={"highlight"}>Destaques</MenuItem> */}
                            {/* <MenuItem value={"bestSeller"}>Mais Vendidos</MenuItem> */}
                            {/* <MenuItem value={"lessSold"}>Menos Vendidos</MenuItem> */}
                            <MenuItem value={"oldStock"}>Mais Antigos</MenuItem>
                            <MenuItem value={"releasedStock"}>
                                Mais Novos
                            </MenuItem>
                        </Select>
                    </FormControl>
                    {products && products.length > 0 && (
                        <Showcase
                            products={products}
                            isLoading={isLoading}
                        ></Showcase>
                    )}
                    {/* <Box
                    display={"flex"}
                    justifyContent={"flex-end"}
                    alignItems={"center"}
                >
                    <Pagination
                        count={totalPages}
                        page={page}
                        onChange={handleChangePagination}
                        disabled={isLoading}
                    />
                </Box> */}
                </Stack>
            </Card>
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
        </Box>
    );
}

MarketingPlace.getLayout = getLayout("private");
