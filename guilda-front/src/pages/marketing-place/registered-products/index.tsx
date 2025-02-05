import Star from "@mui/icons-material/Star";
import Category from "@mui/icons-material/Category";
import {
    Box,
    Button,
    CardMedia,
    CircularProgress,
    FormControl,
    InputLabel,
    MenuItem,
    Pagination,
    Select,
    Slider,
    Stack,
    TextField,
    Typography,
    useTheme,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import Image from "next/image";
import { useRouter } from "next/router";
import { useContext, useEffect, useState, useRef } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import WithoutPermissionCard from "src/components/data-display/without-permission/without-permissions";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useDebounce, useLoadingState } from "src/hooks";
import { CollaboratorDetailUseCase } from "src/modules/collaborators/use-cases/collaborator-details.use-case";
import Showcase from "src/modules/marketing-place/fragments/showcase";
import { DeleteProduct } from "src/modules/marketing-place/use-cases/delete-product.use-case";
import { ListProducts } from "src/modules/marketing-place/use-cases/list-products";
import { MaxValueProduct } from "src/modules/marketing-place/use-cases/get-value-max-products";
import { Product } from "src/typings/models/product.model";
import { DateUtils, getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";
import { formatCurrency } from "src/utils/format-currency";
import Link from "@mui/material/Link";
import { useReactToPrint } from "react-to-print";
import { TagToPrintProduct } from "src/modules/marketing-place/fragments/tag-to-print-product";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export default function RegisteredProducts() {
    const [searchText, setSearchText] = useState<string>("");
    const { myPermissions } = useContext(PermissionsContext);
    const router = useRouter();
    const theme = useTheme();
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [page, setPage] = useState<number>(1);
    const handleChangePagination = (
        event: React.ChangeEvent<unknown>,
        value: number
    ) => {
        setPage(value);
    };
    const [totalPages, setTotalPages] = useState<number>(0);
    const [products, setProducts] = useState<Array<Product>>([]);

    const [productState, setProductState] = useState<Product | null>(null);
    const [orderBy, setOrderBy] = useState<string>("default");
    const [filterByBestSellers, setFilterByBestSellers] =
        useState<boolean>(false);
    const [isOpenQRCode, setIsOpenQRcode] = useState<boolean>(false);
    const [qrCodeState, setQrCodeState] = useState<string>("");
    const [isVisiblePrintTag, setIsVisiblePrintTag] = useState(false);
    const LIMIT_PER_PAGE = 20;

    const minDistance = 10;
    const [minMaxValue, setMinMaxValue] = useState<number[]>([0, 30000]);
    const [maxValueFix, setmaxValueFix] = useState<number>(30000);
    const debouncedMinMaxValue: number[] = useDebounce<number[]>(
        minMaxValue,
        300
    );
    const debouncedSearchTerm: string = useDebounce<string>(searchText, 500);
    const elementoRef = useRef<any>();
    const promiseResolveRef = useRef<any>();

    useEffect(() => {
        if (isVisiblePrintTag && promiseResolveRef.current) {
            // Resolves the Promise, letting `react-to-print` know that the DOM updates are completed
            promiseResolveRef.current();
        }
    }, [isVisiblePrintTag]);

    useEffect(() => {
        getValueMaxProducts();
    }, []); 

    function valuetext(value: number) {
        return `${value}°C`;
    }

    const handleChange1 = (
        event: Event,
        newValue: number | number[],
        activeThumb: number
    ) => {
        if (!Array.isArray(newValue)) {
            return;
        }

        if (activeThumb === 0) {
            setMinMaxValue([
                Math.min(newValue[0], minMaxValue[1] - minDistance),
                minMaxValue[1],
            ]);
        } else {
            setMinMaxValue([
                minMaxValue[0],
                Math.max(newValue[1], minMaxValue[0] + minDistance),
            ]);
        }
    };

    const handleRedirectToCreateProductPage = () =>
        router.push("/marketing-place/create-product");

    const handleCloseModalQRCode = () => setIsOpenQRcode(false);

    const handleOpenModalQRCode = (qrCode: string, product: Product) => {
        setProductState(product);
        setQrCodeState(qrCode);
        setIsOpenQRcode(true);
    };

    const handlePrintTagProduct = useReactToPrint({
        content: () => elementoRef.current,
        onBeforeGetContent: () => {
            return new Promise((resolve) => {
                promiseResolveRef.current = resolve;
                setIsVisiblePrintTag(true);
            });
        },
        onAfterPrint: () => {
            promiseResolveRef.current = null;
            setIsVisiblePrintTag(false);
        },
    });


    const getValueMaxProducts = async () => {
        startLoading();

        new MaxValueProduct()
            .handle()
            .then((data) => {
                setMinMaxValue([0, data.valueProduct]);
                setmaxValueFix(data.valueProduct);
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
            offset: LIMIT_PER_PAGE * (page - 1),
            searchText: searchText,
            recents: orderBy == "recents" ? true : false,
            bestSeller: filterByBestSellers,
            priceMin: minMaxValue[0],
            priceMax: minMaxValue[1],
        };

        new ListProducts()
            .handle(payload)
            .then((data) => {
                setProducts(data.items);
                setTotalPages(data.totalPages);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    useEffect(() => {
        if (abilityFor(myPermissions).can("Ver Produto", "Marketing Place")) {
            getProducts();
        }
    }, [
        page,
        debouncedSearchTerm,
        myPermissions,
        orderBy,
        filterByBestSellers,
        debouncedMinMaxValue,
    ]);

    const handleDeleteProduct = async (productId: string | number) => {
        startLoading();

        const payload = {
            id: productId,
        };

        new DeleteProduct()
            .handle(payload)
            .then((data) => {
                toast.success("Produto apagado com sucesso!");
                getProducts();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    interface ProductItemProps {
        code: string;
        id: number;
        collaboratorId: number;
        createdAt: string | Date | null;
        description: string;
        comercialName: string;
        price: string | number | undefined;
        image: string;
        qrCode: string;
        type: string;
    }

    const ProductItem = (props: ProductItemProps) => {
        const {
            code,
            collaboratorId,
            createdAt,
            id,
            description,
            comercialName,
            price,
            image,
            qrCode,
            type,
        } = props;
        const [collaboratorName, setCollaboratorName] = useState<string>("");

        // const getCollaboratorName = (id: number) => {
        //     new CollaboratorDetailUseCase()
        //         .handle(id)
        //         .then((data) => {
        //             setCollaboratorName(data.name);
        //         })
        //         .catch(() => {
        //             toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
        //         })
        //         .finally(() => {
        //             finishLoading();
        //         });
        // };

        // useEffect(() => {
        //     getCollaboratorName(collaboratorId);
        // }, []);

        return (
            <Box
                border={`solid 1px ${grey[200]}`}
                display={"flex"}
                width={"100%"}
                borderRadius={2}
                p={2}
                alignItems={"center"}
                justifyContent={"space-between"}
            >
                <Box display={"flex"} alignItems={"center"} gap={2}>
                    <Box
                        sx={{
                            width: "70px",
                            height: "70px",
                            backgroundColor: grey[100],
                            borderRadius: 2,
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
                    <Box>
                        <Typography variant="body2" fontSize={"16px"}>
                            {comercialName}
                            <Typography
                                variant="body2"
                                component={"span"}
                                fontSize={"12px"}
                                color={"primary"}
                                ml={1}
                                fontWeight={"500"}
                            >
                                #{code}
                            </Typography>
                        </Typography>
                        <Typography
                            variant="body2"
                            fontSize={"12px"}
                            color={grey[600]}
                        >
                            {/* criado por: {collaboratorName}{" "} */}
                            {createdAt &&
                                `criado em: ${DateUtils.formatDatePtBRWithTime(
                                    createdAt
                                )}`}
                            { }
                        </Typography>
                        <Typography
                            variant="body2"
                            fontSize={"13px"}
                            mt={1}
                            fontWeight={"500"}
                        >
                            {price &&
                                formatCurrency(parseInt(price.toString()))}{" "}
                            moedas
                        </Typography>
                    </Box>
                </Box>
                <Box display={"flex"} gap={1}>
                    <Button
                        color="primary"
                        variant="outlined"
                        onClick={() =>
                            handleOpenModalQRCode(qrCode, {
                                description,
                                comercialName,
                                price,
                                qrCode,
                                code,
                                collaboratorId,
                                createdAt,
                                id,
                                productImages: [],
                                deletedAt: null,
                                amount: 0,
                                type,
                            })
                        }
                    >
                        QRCode
                    </Button>
                    <Button
                        color="primary"
                        variant="outlined"
                        onClick={() =>
                            router.push(`/marketing-place/create-product/${id}`)
                        }
                    >
                        Copiar produto
                    </Button>
                    <Button
                        color="primary"
                        variant="outlined"
                        onClick={() =>
                            router.push(
                                `/marketing-place/registered-products/${id}`
                            )
                        }
                    >
                        Editar produto
                    </Button>
                    <Button
                        color="error"
                        disabled={abilityFor(myPermissions).cannot(
                            "Cadastrar Produto",
                            "Marketing Place"
                        )}
                        variant="contained"
                        onClick={() => handleDeleteProduct(id)}
                    >
                        Apagar produto
                    </Button>
                </Box>
            </Box>
        );
    };

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Produtos registrados"}
                headerIcon={<Category />}
                addButtonAction={handleRedirectToCreateProductPage}
                addButtonTitle="Novo Produto"
                addButtonIsDisabled={abilityFor(myPermissions).cannot(
                    "Cadastrar Produto",
                    "Marketing Place"
                )}
            />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                <TextField
                    placeholder="Buscar produto"
                    onChange={(e) => setSearchText(e.target.value)}
                    value={searchText}
                />
                <Box
                    display={"flex"}
                    alignItems={"center"}
                    gap={1}
                    justifyContent={"space-between"}
                >
                    <Box display={"flex"} alignItems={"center"} gap={1}>
                        <FormControl sx={{ width: "150px" }}>
                            <InputLabel>Ordernar por:</InputLabel>
                            <Select
                                value={orderBy}
                                label="Ordernar por:"
                                onChange={(e) => setOrderBy(e.target.value)}
                                size="small"
                            >
                                <MenuItem value={"default"}>Padrão</MenuItem>
                                <MenuItem value={"recents"}>
                                    Mais recente
                                </MenuItem>
                            </Select>
                        </FormControl>
                        <Box
                            display={"flex"}
                            alignItems={"center"}
                            gap={"3px"}
                            border={`solid 1px ${filterByBestSellers
                                    ? theme.palette.primary.main
                                    : grey[300]
                                }`}
                            p={"3px 6px"}
                            borderRadius={2}
                            sx={{ cursor: "pointer" }}
                            onClick={() =>
                                setFilterByBestSellers(!filterByBestSellers)
                            }
                        >
                            <Star
                                sx={{
                                    fontSize: "15px",
                                    color: filterByBestSellers
                                        ? theme.palette.primary.main
                                        : "#000",
                                }}
                            />
                            <Typography
                                fontSize={"12px"}
                                sx={{
                                    color: filterByBestSellers
                                        ? theme.palette.primary.main
                                        : "#000",
                                }}
                            >
                                Filtrar por mais vendidos
                            </Typography>
                        </Box>
                    </Box>
                    <Box
                        width={"200px"}
                        sx={{
                            display: "flex",
                            flexDirection: "column",
                            justifyContent: "center",
                        }}
                    >
                        <Typography fontSize={"12px"} fontWeight={"500"}>
                            Valor do produto
                        </Typography>
                        <Slider
                            getAriaLabel={() => "Minimum distance"}
                            value={minMaxValue}
                            onChange={handleChange1}
                            valueLabelDisplay="auto"
                            getAriaValueText={valuetext}
                            disableSwap
                            min={1}
                            max={maxValueFix}
                        />
                    </Box>
                </Box>
                <Box display={"flex"} flexDirection={"column"} gap={1}>
                    {isLoading || !myPermissions ? (
                        <Box
                            display={"flex"}
                            justifyContent={"center"}
                            alignItems={"center"}
                            p={4}
                            width={"100%"}
                        >
                            <CircularProgress />
                        </Box>
                    ) : abilityFor(myPermissions).can(
                        "Ver Produto",
                        "Marketing Place"
                    ) ? (
                        products.map((product, index) => (
                            <ProductItem
                                key={index}
                                description={product.description}
                                comercialName={product?.comercialName}
                                code={product.code}
                                id={product.id}
                                collaboratorId={product.collaboratorId}
                                createdAt={product.createdAt}
                                price={product.price}
                                image={
                                    product?.productImages?.length > 0
                                        ? product?.productImages[0]?.upload.url
                                        : ""
                                }
                                qrCode={product.qrCode}
                                type={product.type}
                            />
                        ))
                    ) : (
                        <WithoutPermissionCard />
                    )}
                    <BaseModal
                        width={"540px"}
                        open={isOpenQRCode}
                        title={`QrCode`}
                        onClose={handleCloseModalQRCode}
                    >
                        <Box
                            width={"100%"}
                            display={"flex"}
                            flexDirection={"column"}
                            gap={1}
                            sx={{ alignItems: "center" }}
                        >
                            {qrCodeState && (
                                <CardMedia
                                    component="img"
                                    image={qrCodeState}
                                    sx={{
                                        width: "300px",
                                        objectFit: "contain",
                                    }}
                                />
                            )}
                            {productState?.code && productState?.code}
                            <Link
                                component="button"
                                variant="body2"
                                onClick={handlePrintTagProduct}
                            >
                                Imprimir Etiqueta
                            </Link>
                            <TagToPrintProduct
                                code={productState?.code}
                                description={productState?.description}
                                comercialName={productState?.comercialName}
                                price={productState?.price}
                                qrCode={qrCodeState}
                                isVisible={isVisiblePrintTag}
                                ref={elementoRef}
                            />
                        </Box>
                    </BaseModal>
                </Box>
                <Box
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
                </Box>
            </Stack>
        </Card>
    );
}

RegisteredProducts.getLayout = getLayout("private");
