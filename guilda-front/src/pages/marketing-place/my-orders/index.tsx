import Chat from "@mui/icons-material/Chat";
import Category from "@mui/icons-material/Category";
import Sell from "@mui/icons-material/Sell";
import {
    Box,
    Button,
    CardMedia,
    Chip,
    CircularProgress,
    IconButton,
    Pagination,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import WithoutPermissionCard from "src/components/data-display/without-permission/without-permissions";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useLoadingState } from "src/hooks";
import { CollaboratorDetailUseCase } from "src/modules/collaborators/use-cases/collaborator-details.use-case";
import { OrderObservationsModal } from "src/modules/marketing-place/fragments/order-observations-modal";
import Showcase from "src/modules/marketing-place/fragments/showcase";
import { CancelOrderUseCase } from "src/modules/marketing-place/use-cases/cancel-order";
import { DeleteProduct } from "src/modules/marketing-place/use-cases/delete-product.use-case";
import { ListOrders } from "src/modules/marketing-place/use-cases/list-orders";
import { ListProducts } from "src/modules/marketing-place/use-cases/list-products";
import { Order } from "src/typings/models/order.model";
import { Product } from "src/typings/models/product.model";
import { DateUtils, getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";
import { formatCurrency } from "src/utils/format-currency";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export default function MyOrders() {
    const [searchText, setSearchText] = useState<string>("");
    const { myPermissions } = useContext(PermissionsContext);
    const router = useRouter();
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
    const LIMIT_PER_PAGE = 20;
    const [orders, setOrders] = useState<Order[]>([]);

    const handleRedirectToCreateProductPage = () =>
        router.push("/marketing-place/create-product");

    const getOrders = async () => {
        startLoading();

        const payload = {
            limit: LIMIT_PER_PAGE,
            offset: LIMIT_PER_PAGE * (page - 1),
            searchText: searchText,
        };

        new ListOrders()
            .handle(payload)
            .then((data) => {
                console.log(data);
                setOrders(data.items);
                setTotalPages(data.totalPages);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    // function handleCancelOrder(orderId: number) {
    //     const payload = {
    //         orderId: orderId,
    //     };

    //     new CancelOrderUseCase()
    //         .handle(payload)
    //         .then((data) => {
    //             toast.success("Pedido cancelado.");
    //             getOrders();
    //         })
    //         .catch(() => {
    //             toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
    //         })
    //         .finally(() => {
    //             finishLoading();
    //         });
    // }

    function hasObservationsCheck(order: Order): boolean {
        let hasObservations = false;

        if (
            order.observationOrder ||
            order.observationReleased ||
            order.deliveryNote ||
            order.observationDelivered ||
            order.observationCanceled
        ) {
            hasObservations = true;
        }

        {
            order.GdaOrderProduct.map((item, index) => {
                if (item.observationReleased) {
                    hasObservations = true;
                }
            });
        }

        {
            order.GdaOrderProduct.map((item, index) => {
                if (item.deliveryNote) {
                    hasObservations = true;
                }
            });
        }

        {
            order.GdaOrderProduct.map((item, index) => {
                if (item.observationDelivered) {
                    hasObservations = true;
                }
            });
        }

        {
            order.GdaOrderProduct.map((item, index) => {
                if (item.observationCanceled) {
                    hasObservations = true;
                }
            });
        }

        return hasObservations;
    }

    useEffect(() => {
        getOrders();
    }, [page, searchText]);

    interface ProductItemProps {
        data: Order;
    }

    const ProductItem = (props: ProductItemProps) => {
        const { data } = props;
        const [observationsOpen, setObservationsOpen] =
            useState<boolean>(false);

        return (
            <Box
                border={`solid 1px ${grey[200]}`}
                display={"flex"}
                width={"100%"}
                borderRadius={2}
                p={2}
                flexDirection={"column"}
                alignItems={"center"}
                gap={2}
            >
                <Box
                    display={"flex"}
                    alignItems={"center"}
                    gap={2}
                    justifyContent={"space-between"}
                    width={"100%"}
                >
                    <Box>
                        <Typography variant="body2" fontSize={"16px"}>
                            Número do pedido: {data.id}
                        </Typography>
                        <Typography
                            variant="body2"
                            fontSize={"12px"}
                            color={grey[600]}
                        >
                            {/* criado por: {collaboratorName}{" "}- */}
                            {data.createdAt &&
                                `criado em: ${DateUtils.formatDatePtBR(
                                    data.createdAt
                                )}`}
                            {data.orderById && ` - por : ${data.orderById}`}
                        </Typography>
                        {/* <Typography
                            variant="body2"
                            fontSize={"12px"}
                            color={grey[600]}
                        >
                            {data?.expireIn && data.orderStatus.status !== "EXPIRED" &&
                                `Expira em: ${DateUtils.formatDatePtBR(
                                    data.expireIn
                                )}`}
                        </Typography> */}
                    </Box>
                    <Chip
                        label={
                            data.orderStatusId == 1
                                ? "Pedido em aberto"
                                : data.orderStatusId == 4
                                ? "Pedido Liberado"
                                : data.orderStatusId == 3
                                ? "Pedido Cancelado"
                                : data.orderStatusId == 5
                                ? "Pedido Expirado"
                                : data.orderStatusId == 2
                                ? "Pedido Entregue"
                                : "Undefined"
                        }
                        color={
                            data.orderStatusId == 1
                                ? "warning"
                                : data.orderStatusId == 4
                                ? "success"
                                : data.orderStatusId == 3
                                ? "error"
                                : data.orderStatusId == 5
                                ? "error"
                                : data.orderStatusId == 2
                                ? "primary"
                                : "warning"
                        }
                        size="small"
                    />
                </Box>
                <Card width={"100%"}>
                    <PageHeader title={"Produtos"} headerIcon={<Category />} />
                    <Box
                        display={"flex"}
                        flexDirection={"column"}
                        gap={1}
                        p={2}
                    >
                        {data.GdaOrderProduct.map((item, index) => (
                            <Box
                                sx={{
                                    width: "100%",
                                    p: "10px",
                                    border: "solid 1px #f2f2f2",
                                    borderRadius: 2,
                                    display: "flex",
                                    gap: "50px",
                                    alignItems: "center",
                                }}
                                key={index}
                            >
                                <CardMedia
                                    component="img"
                                    alt="image"
                                    image={
                                        item?.product?.productImages[0]?.upload
                                            ?.url
                                    }
                                    sx={{ width: "70px" }}
                                />
                                <Typography width={"300px"}>
                                    {item.product.comercialName}
                                </Typography>
                                <Typography width={"200px"}>
                                    Quantidade: {item.amount}
                                </Typography>
                                {/* <Typography>{`Quantidade: ${item.product.description}`}</Typography> */}
                                {data.GdaOrderProduct.find(
                                    (i) => i.productId == item.productId && item.product.type == "PHYSICAL" && item.orderProductStatus === "RELEASED"
                                )?.expireIn && (
                                    <Typography>{`Expira em: ${DateUtils.formatDatePtBR(
                                        data.GdaOrderProduct.find(
                                            (i) => i.productId == item.productId
                                        )?.expireIn || new Date()
                                    )}`}</Typography>
                                )}
                                {item.product.type === "DIGITAL" && item?.product?.vouchers[0]?.voucherValidity
                                    ?
                                    <Typography>
                                        Válido até:
                                        {/* {`Válido até: ${DateUtils.formatDatePtBR(
                                        item.product?.vouchers[0]?.voucherValidity
                                        )}`} */}

                                        {" "}
                                        {`${
                                            item.product?.vouchers[0]?.voucherValidity ?
                                            item.product?.vouchers[0]?.voucherValidity
                                                ?.toString()
                                                .split("T")[0]
                                                .split("-")[2]
                                                :
                                                ""
                                        }/${
                                            item.product?.vouchers[0]?.voucherValidity ?
                                            item.product?.vouchers[0]?.voucherValidity
                                                ?.toString()
                                                .split("T")[0]
                                                .split("-")[1]
                                                :
                                                ""
                                        }/${
                                            item.product?.vouchers[0]?.voucherValidity ?
                                            item.product?.vouchers[0]?.voucherValidity
                                                ?.toString()
                                                .split("T")[0]
                                                .split("-")[0]
                                                :
                                                ""
                                        }`}

                                    </Typography>
                                    :
                                    null
                                }
                                <Typography>{`Valor em moedas: ${formatCurrency(
                                    item.product.price
                                )}`}</Typography>
                                {item.orderProductStatus && (
                                    <Chip
                                        label={
                                            item.orderProductStatus ==
                                            "RELEASED"
                                                ? "Liberado"
                                                : item.orderProductStatus ==
                                                  "CANCELED"
                                                ? "Cancelado"
                                                : item.orderProductStatus ==
                                                  "DELIVERED"
                                                ? "Entregue"
                                                : item.orderProductStatus ==
                                                  "EXPIRED"
                                                ? "Expirado"
                                                : ""
                                        }
                                        color={
                                            item.orderProductStatus ==
                                            "RELEASED"
                                                ? "success"
                                                : item.orderProductStatus ==
                                                  "CANCELED"
                                                ? "error"
                                                : item.orderProductStatus ==
                                                  "DELIVERED"
                                                ? "primary"
                                                : item.orderProductStatus ==
                                                  "EXPIRED"
                                                ? "error"
                                                : "default"
                                        }
                                        sx={{
                                            opacity:
                                                item.orderProductStatus ==
                                                    "DELIVERED" ||
                                                item.orderProductStatus ==
                                                    "RELEASED" ||
                                                item.orderProductStatus ==
                                                    "CANCELED" ||
                                                item.orderProductStatus ==
                                                    "EXPIRED"
                                                    ? 1
                                                    : 0,
                                        }}
                                        variant="outlined"
                                    />
                                )}
                            </Box>
                        ))}
                    </Box>
                </Card>
                <Box
                    display={"flex"}
                    width={"100%"}
                    justifyContent={"space-between"}
                    alignItems={"center"}
                >
                    <Typography fontSize={"12px"} color={grey[700]}>
                        Última atualização em:
                        {DateUtils.formatDatePtBR(data.lastUpdatedAt)}
                    </Typography>
                    {hasObservationsCheck(data) && (
                        <IconButton onClick={() => setObservationsOpen(true)}>
                            <Chat color="primary" />
                        </IconButton>
                    )}

                    <OrderObservationsModal
                        isOpen={observationsOpen}
                        onClose={() => setObservationsOpen(false)}
                        order={data}
                    />
                    {/* basemodal */}
                </Box>
                {/* {data.orderStatusId == 1 && (
                    <Box
                        display={"flex"}
                        justifyContent={"flex-end"}
                        width={"100%"}
                        gap={2}
                    >
                        <Button
                            variant="outlined"
                            color="error"
                            size="small"
                            onClick={() => handleCancelOrder(data.id)}
                        >
                            Cancelar pedido
                        </Button>
                    </Box>
                )} */}
            </Box>
        );
    };

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader title={"Meus pedidos"} headerIcon={<Sell />} />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                <TextField
                    placeholder="Buscar pedido"
                    onChange={(e) => setSearchText(e.target.value)}
                    value={searchText}
                />
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
                          "Ver Meus Pedidos",
                          "Marketing Place"
                      ) ? (
                        orders.map((order, index) => (
                            <ProductItem data={order} key={index} />
                        ))
                    ) : (
                        <WithoutPermissionCard />
                    )}
                    {}
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

MyOrders.getLayout = getLayout("private");
