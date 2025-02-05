import AccessTime from "@mui/icons-material/AccessTime";
import OpenInNew from "@mui/icons-material/OpenInNew";
import Person from "@mui/icons-material/Person";
import Category from "@mui/icons-material/Category";
import Download from "@mui/icons-material/Download";
import Sell from "@mui/icons-material/Sell";
import {
    Alert,
    Autocomplete,
    Box,
    Button,
    CardHeader,
    CardMedia,
    Chip,
    CircularProgress,
    FormControl,
    IconButton,
    InputLabel,
    MenuItem,
    Pagination,
    Select,
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
import { useDebounce, useLoadingState } from "src/hooks";
import { CollaboratorDetailUseCase } from "src/modules/collaborators/use-cases/collaborator-details.use-case";
import Showcase from "src/modules/marketing-place/fragments/showcase";
import { CancelOrderUseCase } from "src/modules/marketing-place/use-cases/cancel-order";
import { ConfirmOrderUseCase } from "src/modules/marketing-place/use-cases/confirm-order";
import { ConfirmReleasedOrderUseCase } from "src/modules/marketing-place/use-cases/confirm-released-order";
import { DeleteProduct } from "src/modules/marketing-place/use-cases/delete-product.use-case";
import { ListOrders } from "src/modules/marketing-place/use-cases/list-orders";
import { ListProducts } from "src/modules/marketing-place/use-cases/list-products";
import { Order } from "src/typings/models/order.model";
import { Product } from "src/typings/models/product.model";
import { DateUtils, SheetBuilder, getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";
import { ConfirmOrderQrCodeModal } from "src/modules/marketing-place/fragments/confirm-order-qrcode";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { Sector } from "src/typings";
import { ListSectorsUseCase } from "src/modules";
import { formatCurrency } from "src/utils/format-currency";
import { ReleaseProductUseCase } from "src/modules/marketing-place/use-cases/release-product.use-case";
import { UnReleaseProductUseCase } from "src/modules/marketing-place/use-cases/unrelease-product.use-case";
import { ListCollaboratorsAllUseCase } from "src/modules/collaborators/use-cases/list-collaborators.use-case";
import { PaginationModel } from "src/typings/models/pagination.model";
import { ConfirmOrderProductUseCase } from "src/modules/marketing-place/use-cases/confirm-order-product.use-case";
import { ConfirmProductQrCodeModal } from "src/components/data-display/confirm-product-qrcode/confirm-product-qrcode";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export default function ManageOrders() {
    const [searchText, setSearchText] = useState<string>("");
    const { myPermissions } = useContext(PermissionsContext);
    const router = useRouter();
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [page, setPage] = useState<number>(1);
    const [loading, setLoading] = useState<boolean>(false);
    const [visibility, setVisibility] = useState<string>("list");
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
    const [filteredBy, setFilteredBy] = useState<string>("");
    const [
        collaboratorNameOrIdentification,
        setCollaboratorNameOrIdentification,
    ] = useState<string>("");
    const [filteredByUf, setfilteredByUf] = useState<string>("");
    const [filteredByType, setFilteredByType] = useState<string>("");
    const [filteredBySector, setFilteredBySector] = useState<Sector | null>(
        null
    );
    // const [filteredByQuantity, setFilteredByQuantity] = useState<string>("");
    const [orderBySell, setOrderBySell] = useState<string>("");
    const [startDatePicker, setStartDatePicker] = useState<
        dateFns | Date | null
    >(null);
    const [endDatePicker, setEndDatePicker] = useState<dateFns | Date | null>(
        null
    );
    const [orderDetailsModal, setOrderDetailsModal] = useState<Order | null>(
        null
    );

    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const [sectors, setSectors] = useState<Sector[]>([]);
    const debouncedSectorSearchTerm: string = useDebounce<string>(
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
    }, [debouncedSectorSearchTerm]);

    const debouncedSearchTerm: string = useDebounce<string>(
        collaboratorNameOrIdentification,
        300
    );

    const debouncedFilteredByUf: string = useDebounce<string>(
        filteredByUf,
        300
    );
    // const debouncedFilteredByQuantity: string = useDebounce<string>(
    //     filteredByQuantity,
    //     300
    // );

    const handleRedirectToCreateProductPage = () =>
        router.push("/marketing-place/create-product");

    const getOrders = async (limit = LIMIT_PER_PAGE) => {
        startLoading();

        const payload = {
            limit,
            offset: LIMIT_PER_PAGE * (page - 1),
            searchText: searchText,
            isAll: true,
            status: filteredBy as string,
            type: filteredByType,
            collaboratorNameOrIdentification: collaboratorNameOrIdentification,
            uf: filteredByUf,
            sector: filteredBySector
                ? filteredBySector?.id?.toString()
                : undefined,
            // quantity: filteredByQuantity,
            orderBySell: orderBySell,
            startDate: startDatePicker as Date,
            endDate: endDatePicker as Date,
        };

        new ListOrders()
            .handle(payload)
            .then((data) => {
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

    useEffect(() => {
        getOrders();
    }, [
        page,
        filteredBy,
        filteredByType,
        debouncedSearchTerm,
        debouncedFilteredByUf,
        filteredBySector,
        // debouncedFilteredByQuantity,
        orderBySell,
        startDatePicker,
        endDatePicker,
        searchText,
    ]);

    function handleReleaseOrder(orderId: number, observation: string) {
        const payload = {
            orderId: orderId,
            observation,
        };

        new ConfirmReleasedOrderUseCase()
            .handle(payload)
            .then((data) => {
                toast.success("Pedido liberado!");
                getOrders();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    function handleReleaseProduct(
        orderId: number,
        productId: number,
        reason: string
    ) {
        new ReleaseProductUseCase()
            .handle({ orderId, productId, reason })
            .then((data) => {
                toast.success("Produto liberado!");
                getOrders();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    function handleDeleteProduct(
        orderId: number,
        productId: number,
        reason: string
    ) {
        new UnReleaseProductUseCase()
            .handle({ orderId, productId, reason })
            .then((data) => {
                toast.success("Produto removido!");
                getOrders();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    function handleConfirmProduct(
        orderId: number,
        productId: number,
        observationText: string,
        receivedBy: string
    ) {
        new ConfirmOrderProductUseCase()
            .handle({ orderId, productId, observationText, receivedBy })
            .then((data) => {
                toast.success("Produto confirmado!");
                getOrders();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    function handleCancelOrder(
        orderId: number,
        observation: string,
        reason: string
    ) {
        const payload = {
            orderId: orderId,
            observation,
            reason,
        };

        new CancelOrderUseCase()
            .handle(payload)
            .then((data) => {
                toast.success("Pedido cancelado.");
                getOrders();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    interface ProductItemProps {
        data: Order;
    }

    const ProductItem = (props: ProductItemProps) => {
        const { data } = props;

        const [isOpen, setIsOpen] = useState<boolean>(false);
        const [reason, setReason] = useState<string>("");
        const [observation, setObservation] = useState<string>("");
        const [selectedToRelease, setSelectedToRelease] = useState<
            number | null
        >(null);
        const [selectedToDelete, setSelectedToDelete] = useState<number | null>(
            null
        );
        const [selectedToConfirm, setSelectedToConfirm] = useState<
            number | null
        >(null);

        const [isOpenConfirmOrder, setIsOpenConfirmOrder] =
            useState<boolean>(false);
        const [isOpenReleaseOrder, setIsOpenReleaseOrder] =
            useState<boolean>(false);
        const [isOpenCancelOrder, setIsOpenCancelOrder] =
            useState<boolean>(false);

        const [isConfirmModalOpen, setIsConfirmModalOpen] =
            useState<boolean>(false);

        const [selectedToQrCode, setSelectedToQrCode] = useState<{
            productId: number;
            orderId: number;
            supplierId: number;
        } | null>(null);

        function handleConfirmOrder(
            orderId: number,
            observationText: string,
            receivedBy: string
        ) {
            // setIsConfirmModalOpen(true);

            const payload = {
                orderId: orderId,
                observationText: observationText,
                receivedBy: receivedBy,
            };

            new ConfirmOrderUseCase()
                .handle(payload)
                .then((data) => {
                    toast.success("Pedido confirmado!");
                    getOrders();
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    finishLoading();
                });
        }

        function ModalConfirmOrderProduct() {
            const [collaborator, setCollaborator] = useState<{
                id: number;
                name: string;
                registry: string;
            } | null>(null);
            const [collaboratorsSearchValue, setCollaboratorsSearchValue] =
                useState<string>("");
            const [collaborators, setCollaborators] = useState<
                {
                    id: number;
                    name: string;
                    registry: string;
                }[]
            >([]);
            const debouncedCollaboratorsSearchTerm: string =
                useDebounce<string>(collaboratorsSearchValue, 400);
            const [observationText, setObservationText] = useState<string>("");

            const getCollaboratorsList = async () => {
                const pagination: PaginationModel = {
                    limit: 20,
                    offset: 0,
                    searchText: collaboratorsSearchValue,
                };

                new ListCollaboratorsAllUseCase()
                    .handle(pagination)
                    .then((data) => {
                        setCollaborators(data.items);
                    })
                    .catch(() => {});
            };

            useEffect(() => {
                getCollaboratorsList();
            }, [debouncedCollaboratorsSearchTerm]);

            return (
                <BaseModal
                    width={"540px"}
                    open={Boolean(selectedToConfirm)}
                    title={`Confirmar produto`}
                    onClose={() => setSelectedToConfirm(null)}
                >
                    <Box
                        width={"100%"}
                        display={"flex"}
                        flexDirection={"column"}
                        gap={1}
                    >
                        <TextField
                            value={observation}
                            label={"Quem retirou o produto?"}
                            onChange={(e) => setObservation(e.target.value)}
                            disabled={true}
                        />
                        <Autocomplete
                            value={collaborator}
                            placeholder={"Selecione o colaborador"}
                            disableClearable={false}
                            fullWidth
                            onChange={(e, value) => {
                                setObservation(`${value?.id} - ${value?.name}`);
                            }}
                            onInputChange={(e, text) =>
                                setCollaboratorsSearchValue(text)
                            }
                            filterOptions={(options, { inputValue }) =>
                                options.filter(
                                    (item) =>
                                        item.id
                                            .toString()
                                            .includes(inputValue.toString()) ||
                                        item.name
                                            .toString()
                                            .toLowerCase()
                                            .includes(
                                                inputValue
                                                    .toString()
                                                    .toLowerCase()
                                            )
                                )
                            }
                            sx={{
                                mb: 0,
                                width: "100%",
                            }}
                            renderInput={(props) => (
                                <TextField
                                    {...props}
                                    size={"medium"}
                                    label={"Selecione o colaborador"}
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
                            getOptionLabel={(option) => `${option.name}`}
                            options={collaborators}
                        />
                        <TextField
                            value={observationText}
                            label={"Observações"}
                            onChange={(e) => setObservationText(e.target.value)}
                        />
                        <Button
                            fullWidth
                            variant="outlined"
                            onClick={() =>
                                selectedToConfirm &&
                                handleConfirmProduct(
                                    data.id,
                                    selectedToConfirm,
                                    observationText,
                                    observation
                                )
                            }
                        >
                            Confirmar produto
                        </Button>
                    </Box>
                </BaseModal>
            );
        }

        function ModalConfirmOrder() {
            const [collaborator, setCollaborator] = useState<{
                id: number;
                name: string;
                registry: string;
            } | null>(null);
            const [collaboratorsSearchValue, setCollaboratorsSearchValue] =
                useState<string>("");
            const [collaborators, setCollaborators] = useState<
                {
                    id: number;
                    name: string;
                    registry: string;
                }[]
            >([]);
            const debouncedCollaboratorsSearchTerm: string =
                useDebounce<string>(collaboratorsSearchValue, 400);
            const [observationText, setObservationText] = useState<string>("");

            const getCollaboratorsList = async () => {
                const pagination: PaginationModel = {
                    limit: 20,
                    offset: 0,
                    searchText: collaboratorsSearchValue,
                };

                new ListCollaboratorsAllUseCase()
                    .handle(pagination)
                    .then((data) => {
                        setCollaborators(data.items);
                    })
                    .catch(() => {});
            };

            useEffect(() => {
                getCollaboratorsList();
            }, [debouncedCollaboratorsSearchTerm]);

            return (
                <BaseModal
                    width={"540px"}
                    open={isOpenConfirmOrder}
                    title={`Confirmar pedido`}
                    onClose={() => setIsOpenConfirmOrder(false)}
                >
                    <Box
                        width={"100%"}
                        display={"flex"}
                        flexDirection={"column"}
                        gap={1}
                    >
                        <TextField
                            value={observation}
                            label={"Quem retirou o pedido?"}
                            onChange={(e) => setObservation(e.target.value)}
                            disabled={true}
                        />
                        <Autocomplete
                            value={collaborator}
                            placeholder={"Selecione o colaborador"}
                            disableClearable={false}
                            fullWidth
                            onChange={(e, value) => {
                                setObservation(`${value?.id} - ${value?.name}`);
                            }}
                            onInputChange={(e, text) =>
                                setCollaboratorsSearchValue(text)
                            }
                            filterOptions={(options, { inputValue }) =>
                                options.filter(
                                    (item) =>
                                        item.id
                                            .toString()
                                            .includes(inputValue.toString()) ||
                                        item.name
                                            .toString()
                                            .toLowerCase()
                                            .includes(
                                                inputValue
                                                    .toString()
                                                    .toLowerCase()
                                            )
                                )
                            }
                            sx={{
                                mb: 0,
                                width: "100%",
                            }}
                            renderInput={(props) => (
                                <TextField
                                    {...props}
                                    size={"medium"}
                                    label={"Selecione o colaborador"}
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
                            getOptionLabel={(option) => `${option.name}`}
                            options={collaborators}
                        />
                        <TextField
                            value={observationText}
                            label={"Observações"}
                            onChange={(e) => setObservationText(e.target.value)}
                        />
                        <Button
                            fullWidth
                            variant="outlined"
                            onClick={() =>
                                handleConfirmOrder(
                                    data.id,
                                    observationText,
                                    observation,
                                )
                            }
                        >
                            Confirmar pedido
                        </Button>
                    </Box>
                </BaseModal>
            );
        }

        return (
            <Card width={"100%"} display={"flex"} flexDirection={"column"}>
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
                            <Box display={"flex"} alignItems={"center"}>
                                <Typography
                                    variant="body2"
                                    fontSize={"16px"}
                                    lineHeight={"16px"}
                                >
                                    Código do pedido: {data.id}
                                </Typography>
                                <IconButton
                                    size="small"
                                    sx={{
                                        ml: "5px",
                                        mb: "2px",
                                    }}
                                    onClick={() => setOrderDetailsModal(data)}
                                >
                                    <OpenInNew
                                        sx={{
                                            fontSize: "20px",
                                        }}
                                        color="primary"
                                    />
                                </IconButton>
                            </Box>
                            <Typography
                                variant="body2"
                                fontSize={"12px"}
                                color={grey[600]}
                            >
                                {/* criado por: {collaboratorName}{" "} */}
                                {data.createdAt &&
                                    `criado em: ${DateUtils.formatDatePtBR(
                                        data.createdAt
                                    )}`}
                                {data.orderById &&
                                    ` - por : ${data.orderBy.name} - ${data.orderBy?.identification}`}
                            </Typography>
                           {/*  <Typography
                                variant="body2"
                                fontSize={"12px"}
                                color={grey[600]}
                            >
                                {data?.expireIn
                                 &&
                                    `Expira em: ${DateUtils.formatDatePtBR(
                                        data.expireIn
                                    )}`}
                            </Typography> */}
                            {data.observationOrder &&
                                data.observationOrder.length > 0 && (
                                    <Alert severity="warning" sx={{ mt: 2 }}>
                                        Poderá ser recebido por:{" "}
                                        {data.observationOrder}
                                    </Alert>
                                )}
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
                        <PageHeader
                            title={"Produtos"}
                            headerIcon={<Category />}
                        />
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
                                        justifyContent: "space-between",
                                    }}
                                    key={index}
                                >
                                    <Typography>
                                        {item.product.comercialName}
                                    </Typography>
                                    <Typography>{`Quantidade: ${
                                        data.GdaOrderProduct.find(
                                            (i) => i.productId == item.productId
                                        )?.amount
                                    }`}</Typography>
                                    {data.GdaOrderProduct.find(
                                        (i) => i.productId == item.productId && item.product.type == "PHYSICAL" && item.orderProductStatus === "RELEASED"
                                    )?.expireIn && (
                                        <Typography>{`Expira em: ${DateUtils.formatDatePtBR(
                                            data.GdaOrderProduct.find(
                                                (i) =>
                                                    i.productId ==
                                                    item.productId
                                            )?.expireIn || new Date()
                                        )}`}</Typography>
                                    )}
                                    <Typography>{`Valor em moedas: ${formatCurrency(
                                        item.product.price
                                    )}`}</Typography>

                                    <Box
                                        display={"flex"}
                                        gap={1}
                                        alignItems={"center"}
                                        flexDirection={"row"}
                                    >
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
                                        {item.orderProductStatus ==
                                            "ORDERED" && (
                                            <Button
                                                variant="outlined"
                                                onClick={() =>
                                                    setSelectedToRelease(
                                                        item.productId
                                                    )
                                                }
                                            >
                                                Liberar
                                            </Button>
                                        )}
                                        {item.orderProductStatus ==
                                            "ORDERED" && (
                                            <Button
                                                variant="outlined"
                                                onClick={() =>
                                                    setSelectedToDelete(
                                                        item.productId
                                                    )
                                                }
                                                color={"error"}
                                            >
                                                Remover
                                            </Button>
                                        )}
                                        {item.orderProductStatus ==
                                            "RELEASED" && (
                                            <Button
                                                variant="outlined"
                                                onClick={() =>
                                                    setSelectedToConfirm(
                                                        item.productId
                                                    )
                                                }
                                            >
                                                Confirmar
                                            </Button>
                                        )}
                                        {item.orderProductStatus ==
                                            "RELEASED" && (
                                            <>
                                                <Button
                                                    variant="contained"
                                                    onClick={() =>
                                                        setSelectedToQrCode({
                                                            productId:
                                                                item.productId,
                                                            orderId: data.id,
                                                            supplierId:
                                                                item.SupplierId,
                                                        })
                                                    }
                                                >
                                                    QR Code
                                                </Button>
                                            </>
                                        )}
                                    </Box>
                                </Box>
                            ))}
                            {selectedToQrCode && (
                                <ConfirmProductQrCodeModal
                                    isOpen={Boolean(selectedToQrCode)}
                                    onClose={async () => {
                                        setSelectedToQrCode(null);
                                        await getOrders();
                                    }}
                                    productId={selectedToQrCode.productId}
                                    orderId={selectedToQrCode.orderId}
                                />
                            )}
                            {selectedToConfirm && <ModalConfirmOrderProduct />}
                            <BaseModal
                                width={"540px"}
                                open={Boolean(selectedToRelease)}
                                title={`Liberar produto`}
                                onClose={() => setSelectedToRelease(null)}
                            >
                                <Box
                                    width={"100%"}
                                    display={"flex"}
                                    flexDirection={"column"}
                                    gap={1}
                                >
                                    <TextField
                                        label={"Motivo"}
                                        value={reason}
                                        onChange={(e) =>
                                            setReason(e.target.value)
                                        }
                                    />
                                    <Button
                                        fullWidth
                                        variant="outlined"
                                        onClick={() =>
                                            selectedToRelease &&
                                            handleReleaseProduct(
                                                data.id,
                                                selectedToRelease,
                                                reason
                                            )
                                        }
                                    >
                                        Liberar
                                    </Button>
                                </Box>
                            </BaseModal>
                            <BaseModal
                                width={"540px"}
                                open={Boolean(selectedToDelete)}
                                title={`Remover produto`}
                                onClose={() => setSelectedToDelete(null)}
                            >
                                <Box
                                    width={"100%"}
                                    display={"flex"}
                                    flexDirection={"column"}
                                    gap={1}
                                >
                                    <TextField
                                        label={"Motivo"}
                                        value={reason}
                                        onChange={(e) =>
                                            setReason(e.target.value)
                                        }
                                    />
                                    <Button
                                        fullWidth
                                        variant="outlined"
                                        onClick={() =>
                                            selectedToDelete &&
                                            handleDeleteProduct(
                                                data.id,
                                                selectedToDelete,
                                                reason
                                            )
                                        }
                                    >
                                        Remover produto
                                    </Button>
                                </Box>
                            </BaseModal>
                            {/* <BaseModal
                                width={"540px"}
                                open={Boolean(selectedToConfirm)}
                                title={`Confirmar produto`}
                                onClose={() => setSelectedToConfirm(null)}
                            >
                                <Box
                                    width={"100%"}
                                    display={"flex"}
                                    flexDirection={"column"}
                                    gap={1}
                                >
                                    <TextField
                                        label={"Motivo"}
                                        value={observation}
                                        onChange={(e) =>
                                            setObservation(e.target.value)
                                        }
                                    />
                                    <Button
                                        fullWidth
                                        variant="outlined"
                                        onClick={() =>
                                            selectedToConfirm &&
                                            handleConfirmProduct(
                                                data.id,
                                                selectedToConfirm,
                                                observation
                                            )
                                        }
                                    >
                                        Confirmar produto
                                    </Button>
                                </Box>
                            </BaseModal> */}
                        </Box>
                    </Card>
                    <Box display={"flex"} width={"100%"}>
                        <Typography fontSize={"12px"} color={grey[700]}>
                            Última atualização em:{" "}
                            {DateUtils.formatDatePtBR(data.lastUpdatedAt)} por:{" "}
                            {data.lastUpdatedBy?.name} -{" "}
                            {data.lastUpdatedBy?.identification}
                        </Typography>
                    </Box>
                    <Box display={"flex"} width={"100%"}>
                        <Typography fontSize={"16px"}>
                            Valor total em moedas: {data.totalPrice}
                        </Typography>
                    </Box>
                    {data.orderStatusId == 1 && (
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
                                onClick={() => setIsOpenCancelOrder(true)}
                            >
                                Cancelar pedido
                            </Button>
                            <Button
                                variant="contained"
                                color="success"
                                size="small"
                                onClick={() => setIsOpenReleaseOrder(true)}
                            >
                                Liberar pedido
                            </Button>
                            <BaseModal
                                width={"540px"}
                                open={isOpenReleaseOrder}
                                title={`Liberar pedido`}
                                onClose={() => setIsOpenReleaseOrder(false)}
                            >
                                <Box
                                    width={"100%"}
                                    display={"flex"}
                                    flexDirection={"column"}
                                    gap={1}
                                >
                                    <TextField
                                        value={observation}
                                        label={"Observações"}
                                        onChange={(e) =>
                                            setObservation(e.target.value)
                                        }
                                    />
                                    <Button
                                        fullWidth
                                        variant="outlined"
                                        onClick={() =>
                                            handleReleaseOrder(
                                                data.id,
                                                observation
                                            )
                                        }
                                    >
                                        Liberar pedido
                                    </Button>
                                </Box>
                            </BaseModal>
                        </Box>
                    )}
                    {data.orderStatusId == 4 && (
                        <Box
                            display={"flex"}
                            justifyContent={"flex-end"}
                            width={"100%"}
                        >
                            <Button
                                variant="contained"
                                color="primary"
                                size="small"
                                onClick={() => setIsOpenConfirmOrder(true)}
                            >
                                Confirmar pedido
                            </Button>
                            {isConfirmModalOpen && (
                                <ConfirmOrderQrCodeModal
                                    open={isConfirmModalOpen}
                                    onClose={() => setIsConfirmModalOpen(false)}
                                    orderId={data.id}
                                />
                            )}
                            {isOpenConfirmOrder && <ModalConfirmOrder />}
                        </Box>
                    )}
                    <BaseModal
                        width={"540px"}
                        open={isOpenCancelOrder}
                        title={`Cancelar pedido`}
                        onClose={() => setIsOpenCancelOrder(false)}
                    >
                        <Box
                            width={"100%"}
                            display={"flex"}
                            flexDirection={"column"}
                            gap={1}
                        >
                            <TextField
                                label={"Observações"}
                                value={observation}
                                onChange={(e) => setObservation(e.target.value)}
                            />
                            <TextField
                                label={"Motivo"}
                                value={reason}
                                onChange={(e) => setReason(e.target.value)}
                            />
                            <Button
                                fullWidth
                                variant="outlined"
                                onClick={() =>
                                    handleCancelOrder(
                                        data.id,
                                        observation,
                                        reason
                                    )
                                }
                            >
                                Cancelar pedido
                            </Button>
                        </Box>
                    </BaseModal>
                </Box>
            </Card>
        );
    };

    const exportOrders = async () => {
        setLoading(true);

        const payload = {
            offset: LIMIT_PER_PAGE * (page - 1),
            searchText: searchText,
            limit: 1000000000,
            isAll: true,
            status: filteredBy as string,
            type: filteredByType,
            collaboratorNameOrIdentification: collaboratorNameOrIdentification,
            uf: filteredByUf,
            sector: filteredBySector
                ? filteredBySector?.id?.toString()
                : undefined,
            // quantity: filteredByQuantity,
            orderBySell: orderBySell,
            startDate: startDatePicker as Date,
            endDate: endDatePicker as Date,
        };

        await new ListOrders()
            .handle(payload)
            .then((data) => {
                const docRows = data.items.map((item: any) => {
                    if (item.lastUpdatedBy?.homeBased !== undefined) {
                        item.lastUpdatedBy?.homeBased
                            ? (item.lastUpdatedBy.homeBased = "Sim")
                            : (item.lastUpdatedBy.homeBased = "Não");
                    }
                    if (item.GdaOrderProduct[0]?.product?.type !== undefined) {
                        item.GdaOrderProduct[0]?.product?.type === "PHYSICAL"
                            ? (item.GdaOrderProduct[0].product.type = "FÍSICO")
                            : (item.GdaOrderProduct[0].product.type =
                                  "DIGITAL");
                    }
                    item.orderStatus.status === "RELEASED"
                        ? (item.orderStatus.status = "LIBERADO")
                        : item.orderStatus.status;
                    item.orderStatus.status === "DELIVERED"
                        ? (item.orderStatus.status = "ENTREGUE")
                        : item.orderStatus.status;
                    item.orderStatus.status === "CANCELED"
                        ? (item.orderStatus.status = "CANCELADO")
                        : item.orderStatus.status;
                    item.orderStatus.status === "EXPIRED"
                        ? (item.orderStatus.status = "EXPIRADO")
                        : item.orderStatus.status;
                    item.orderStatus.status === "ORDERED"
                        ? (item.orderStatus.status = "ORDENADO")
                        : item.orderStatus.status;

                    return [
                        
                        item.id.toString(),
                        item.orderById,
                        item.orderStatus.status,
                        item.createdAt
                            ? DateUtils.formatDatePtBR(item.createdAt)
                            : "",
                        item.orderBy?.name,
                        item.orderBy?.historyHierarchyRelationship[0]
                            ?.levelName,
                        item.orderBy?.results[0]?.indicator
                            .historyIndicatorGroups[0]?.group?.name,
                        item.orderBy[0]?.CheckingAccount[0]
                            ?.GdaConsolidateCheckingAccount[0].indicador.id,
                        item.orderBy?.results[0]?.indicator
                            .historyIndicatorGroups[0]?.sectorId,
                        item.lastUpdatedBy?.homeBased,
                        item.GdaOrderProduct[0]?.product?.type,
                        item.GdaOrderProduct[0]?.product.description,
                        item.GdaOrderProduct[0]?.amount,
                        item.GdaOrderProduct[0]?.price,
                        item.lastUpdatedAt
                            ? DateUtils.formatDatePtBR(item.lastUpdatedAt)
                            : "",
                        item.lastUpdatedBy?.name,
                        item.deliveredBy?.name,
                        item.GdaOrderProduct[0]?.stock?.description,
                    ];
                });
                let indicatorSheetBuilder = new SheetBuilder();
                indicatorSheetBuilder
                    .setHeader([
                        "Código",
                        "Pedido criado por",
                        "Status",
                        "Criado em",
                        "Colaborador",
                        "Hierarquia",
                        "Grupo",
                        "Cod gip",
                        "Setor",
                        "Home(s/n)",
                        "Tipo de Produto",
                        "Nome do Produto",
                        "Quantidade",
                        "Valor em Moedas",
                        "Ultima atualização",
                        "Quem atualizou",
                        "Entregue por",
                        "Estoque",
                    ])
                    .append(docRows)
                    .exportAs("Pedidos Marketing Place");
                toast.success("Relatório exportado com sucesso!");
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                setLoading(false);
            });
    };

    if (visibility == "report") {
        return (
            <Card width={"100%"} display={"flex"} flexDirection={"column"}>
                <PageHeader
                    title={"Administrar pedidos"}
                    headerIcon={<Sell />}
                    secondayButtonAction={exportOrders}
                    secondaryButtonTitle={"Exportar Relatório"}
                    secondaryButtonIcon={<Download />}
                    addButtonAction={async () => {
                        getOrders();
                        setVisibility(
                            visibility == "report" ? "list" : "report"
                        );
                    }}
                    addButtonTitle="Alternar visualização"
                />
                <Box
                    display={"flex"}
                    p={"20px 20px"}
                    alignItems={"center"}
                    flexDirection={"column"}
                    gap={1}
                >
                    <Box display={"flex"} width={"100%"} gap={1}>
                        <FormControl sx={{ width: "100%" }}>
                            <InputLabel sx={{ backgroundColor: "#fff", px: 1 }}>
                                Filtrar por status:
                            </InputLabel>
                            <Select
                                onChange={(e) => setFilteredBy(e.target.value)}
                                value={filteredBy}
                                size="small"
                            >
                                <MenuItem value={""}>Todos os pedidos</MenuItem>
                                <MenuItem value={"RELEASED"}>
                                    Liberados
                                </MenuItem>
                                <MenuItem value={"DELIVERED"}>
                                    Entregues
                                </MenuItem>
                                <MenuItem value={"CANCELED"}>
                                    Cancelados
                                </MenuItem>
                                <MenuItem value={"EXPIRED"}>Expirados</MenuItem>
                                <MenuItem value={"ORDERED"}>
                                    Solicitados/Comprados
                                </MenuItem>
                            </Select>
                        </FormControl>
                        <Box
                            display={"flex"}
                            flexDirection={"row"}
                            gap={"10px"}
                        >
                            <LocalizationProvider dateAdapter={AdapterDateFns}>
                                <DatePicker
                                    label="De"
                                    value={startDatePicker}
                                    onChange={(newValue) => {
                                        setStartDatePicker(newValue);
                                    }}
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
                                    onChange={(newValue) =>
                                        setEndDatePicker(newValue)
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
                    </Box>
                    <Box display={"flex"} width={"100%"} gap={1}>
                        <FormControl sx={{ width: "100%" }}>
                            <TextField
                                label="Pesquisar por colaborador"
                                value={collaboratorNameOrIdentification}
                                onChange={(e) =>
                                    setCollaboratorNameOrIdentification(
                                        e.target.value
                                    )
                                }
                                sx={{ width: "100%" }}
                            />
                        </FormControl>
                        <FormControl sx={{ width: "100%" }}>
                            <TextField
                                label="Pesquisar por UF"
                                value={filteredByUf}
                                onChange={(e) =>
                                    setfilteredByUf(e.target.value)
                                }
                                sx={{ width: "100%" }}
                            />
                        </FormControl>

                        <Autocomplete
                            value={filteredBySector}
                            placeholder={"Setor"}
                            fullWidth
                            disableClearable={false}
                            onChange={(e, value) => setFilteredBySector(value)}
                            onInputChange={(e, text) =>
                                setSectorsSearchValue(text)
                            }
                            sx={{ mb: 0 }}
                            renderInput={(props) => (
                                <TextField
                                    {...props}
                                    size={"medium"}
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
                        {/* <FormControl sx={{ width: "100%" }}>
                            <TextField
                                label="Quantidade em Estoque"
                                value={filteredByQuantity}
                                onChange={(e) =>
                                    setFilteredByQuantity(e.target.value)
                                }
                                sx={{ width: "100%" }}
                            />
                        </FormControl> */}
                    </Box>
                    <Box display={"flex"} width={"100%"} gap={1}>
                        <FormControl sx={{ width: "100%" }}>
                            <InputLabel sx={{ backgroundColor: "#fff", px: 1 }}>
                                Filtrar por tipo:
                            </InputLabel>
                            <Select
                                onChange={(e) =>
                                    setFilteredByType(e.target.value)
                                }
                                value={filteredByType}
                                sx={{ width: "100%" }}
                                size="small"
                            >
                                <MenuItem value={""}>Todos os tipos</MenuItem>
                                <MenuItem value={"PHYSICAL"}>Físico</MenuItem>
                                <MenuItem value={"DIGITAL"}>Digital</MenuItem>
                            </Select>
                        </FormControl>
                        <FormControl sx={{ width: "100%" }}>
                            <InputLabel>Ordernar por:</InputLabel>
                            <Select
                                value={orderBySell}
                                label="Ordernar por:"
                                onChange={(e) => setOrderBySell(e.target.value)}
                                size="small"
                            >
                                <MenuItem value={""}>Padrão</MenuItem>
                                <MenuItem value={"bestSeller"}>
                                    Mais Vendidos
                                </MenuItem>
                                <MenuItem value={"lessSold"}>
                                    Menos Vendidos
                                </MenuItem>
                            </Select>
                        </FormControl>
                    </Box>
                </Box>
                <Stack px={2} py={3} m={0} width={"100%"} gap={2}>
                    {orders && orders.length > 0 && (
                        <Stack
                            px={2}
                            py={3}
                            width={"100%"}
                            gap={0.5}
                            sx={{ overflowX: "auto" }}
                        >
                            <Card
                                p={"5px 10px"}
                                display={"flex"}
                                alignItems={"center"}
                                width={"fit-content"}
                                gap={"30px"}
                            >
                                <Box
                                    minWidth={"150px"}
                                    maxWidth={"150px"}
                                    width={"150px"}
                                >
                                    <Typography fontSize={"12px"}>
                                        Código
                                    </Typography>
                                </Box>
                                <Box
                                    minWidth={"150px"}
                                    maxWidth={"150px"}
                                    width={"150px"}
                                >
                                    <Typography fontSize={"12px"}>
                                        Pedido criado por
                                    </Typography>
                                </Box>
                                <Box
                                    minWidth={"150px"}
                                    maxWidth={"150px"}
                                    width={"150px"}
                                >
                                    <Typography fontSize={"12px"}>
                                        Criado em
                                    </Typography>
                                </Box>
                                <Box
                                    minWidth={"150px"}
                                    maxWidth={"150px"}
                                    width={"150px"}
                                >
                                    <Typography fontSize={"12px"}>
                                        Colaborador
                                    </Typography>
                                </Box>
                                <Box
                                    minWidth={"150px"}
                                    maxWidth={"150px"}
                                    width={"150px"}
                                >
                                    <Typography fontSize={"12px"}>
                                        Hierarquia
                                    </Typography>
                                </Box>
                                <Box
                                    minWidth={"150px"}
                                    maxWidth={"150px"}
                                    width={"150px"}
                                >
                                    <Typography fontSize={"12px"}>
                                        Grupo
                                    </Typography>
                                </Box>
                                <Box
                                    minWidth={"150px"}
                                    maxWidth={"150px"}
                                    width={"150px"}
                                >
                                    <Typography fontSize={"12px"}>
                                        Setor
                                    </Typography>
                                </Box>
                                <Box
                                    minWidth={"150px"}
                                    maxWidth={"150px"}
                                    width={"150px"}
                                >
                                    <Typography fontSize={"12px"}>
                                        Tipo de produto
                                    </Typography>
                                </Box>
                                <Box
                                    minWidth={"150px"}
                                    maxWidth={"150px"}
                                    width={"150px"}
                                >
                                    <Typography fontSize={"12px"}>
                                        Nome do produto
                                    </Typography>
                                </Box>
                                <Box
                                    minWidth={"150px"}
                                    maxWidth={"150px"}
                                    width={"150px"}
                                >
                                    <Typography fontSize={"12px"}>
                                        Quantidade
                                    </Typography>
                                </Box>
                                <Box
                                    minWidth={"150px"}
                                    maxWidth={"150px"}
                                    width={"150px"}
                                >
                                    <Typography fontSize={"12px"}>
                                        Valor em moeda
                                    </Typography>
                                </Box>
                                <Box
                                    minWidth={"150px"}
                                    maxWidth={"150px"}
                                    width={"150px"}
                                >
                                    <Typography fontSize={"12px"}>
                                        Última atualização
                                    </Typography>
                                </Box>
                                <Box
                                    minWidth={"150px"}
                                    maxWidth={"150px"}
                                    width={"150px"}
                                >
                                    <Typography fontSize={"12px"}>
                                        Quem atualizou
                                    </Typography>
                                </Box>
                            </Card>
                            {orders.map((item: any, index: any) => (
                                <Card
                                    key={index}
                                    p={"5px 10px"}
                                    display={"flex"}
                                    alignItems={"center"}
                                    width={"fit-content"}
                                    gap={"30px"}
                                >
                                    <Box
                                        minWidth={"150px"}
                                        maxWidth={"150px"}
                                        width={"150px"}
                                    >
                                        <Typography
                                            fontSize={"12px"}
                                            fontWeight={"600"}
                                        >
                                            {item.id}
                                        </Typography>
                                    </Box>
                                    <Box
                                        minWidth={"150px"}
                                        maxWidth={"150px"}
                                        width={"150px"}
                                    >
                                        <Typography
                                            fontSize={"12px"}
                                            fontWeight={"600"}
                                        >
                                            {item.orderById}
                                        </Typography>
                                    </Box>
                                    <Box
                                        minWidth={"150px"}
                                        maxWidth={"150px"}
                                        width={"150px"}
                                    >
                                        <Typography
                                            fontSize={"12px"}
                                            fontWeight={"600"}
                                        >
                                            {DateUtils.formatDatePtBRWithTime(
                                                item.createdAt as Date
                                            )}
                                        </Typography>
                                    </Box>
                                    <Box
                                        minWidth={"150px"}
                                        maxWidth={"150px"}
                                        width={"150px"}
                                    >
                                        <Typography
                                            fontSize={"12px"}
                                            fontWeight={"600"}
                                        >
                                            {item.orderBy.name}
                                        </Typography>
                                    </Box>
                                    <Box
                                        minWidth={"150px"}
                                        maxWidth={"150px"}
                                        width={"150px"}
                                    >
                                        <Typography
                                            fontSize={"12px"}
                                            fontWeight={"600"}
                                        >
                                            {
                                                item.orderBy
                                                    ?.historyHierarchyRelationship[0]
                                                    ?.levelName
                                            }
                                        </Typography>
                                    </Box>
                                    <Box
                                        minWidth={"150px"}
                                        maxWidth={"150px"}
                                        width={"150px"}
                                    >
                                        <Typography
                                            fontSize={"12px"}
                                            fontWeight={"600"}
                                        >
                                            {
                                                item?.orderBy?.results[0]
                                                    ?.indicator
                                                    ?.historyIndicatorGroups[0]
                                                    ?.group?.name
                                            }
                                        </Typography>
                                    </Box>
                                    <Box
                                        minWidth={"150px"}
                                        maxWidth={"150px"}
                                        width={"150px"}
                                    >
                                        <Typography
                                            fontSize={"12px"}
                                            fontWeight={"600"}
                                        >
                                            {
                                                item?.orderBy
                                                    ?.ConsolidatedResult[0]
                                                    ?.sector?.name
                                            }
                                        </Typography>
                                    </Box>
                                    <Box
                                        minWidth={"150px"}
                                        maxWidth={"150px"}
                                        width={"150px"}
                                    >
                                        <Typography
                                            fontSize={"12px"}
                                            fontWeight={"600"}
                                        >
                                            {item?.GdaOrderProduct?.map(
                                                (item: any) => {
                                                    return `-${item.product.type}`;
                                                }
                                            )}
                                        </Typography>
                                    </Box>
                                    <Box
                                        minWidth={"150px"}
                                        maxWidth={"150px"}
                                        width={"150px"}
                                    >
                                        <Typography
                                            fontSize={"12px"}
                                            fontWeight={"600"}
                                        >
                                            {item?.GdaOrderProduct?.map(
                                                (item: any) => {
                                                    return `-${item.product.comercialName}`;
                                                }
                                            )}
                                        </Typography>
                                    </Box>
                                    <Box
                                        minWidth={"150px"}
                                        maxWidth={"150px"}
                                        width={"150px"}
                                    >
                                        <Typography
                                            fontSize={"12px"}
                                            fontWeight={"600"}
                                        >
                                            {item?.GdaOrderProduct?.map(
                                                (item: any) => {
                                                    return `-${item.amount}`;
                                                }
                                            )}
                                        </Typography>
                                    </Box>
                                    <Box
                                        minWidth={"150px"}
                                        maxWidth={"150px"}
                                        width={"150px"}
                                    >
                                        <Typography
                                            fontSize={"12px"}
                                            fontWeight={"600"}
                                        >
                                            {item.totalPrice}
                                        </Typography>
                                    </Box>
                                    <Box
                                        minWidth={"150px"}
                                        maxWidth={"150px"}
                                        width={"150px"}
                                    >
                                        <Typography
                                            fontSize={"12px"}
                                            fontWeight={"600"}
                                        >
                                            {DateUtils.formatDatePtBRWithTime(
                                                item.lastUpdatedAt as Date
                                            )}
                                        </Typography>
                                    </Box>
                                    <Box
                                        minWidth={"150px"}
                                        maxWidth={"150px"}
                                        width={"150px"}
                                    >
                                        <Typography
                                            fontSize={"12px"}
                                            fontWeight={"600"}
                                        >
                                            {item?.lastUpdatedBy?.name}
                                        </Typography>
                                    </Box>
                                </Card>
                            ))}
                        </Stack>
                    )}
                </Stack>
            </Card>
        );
    }

    return (
        <Box display={"flex"} flexDirection={"column"} gap={2} width={"100%"}>
            <Card width={"100%"} display={"flex"} flexDirection={"column"}>
                <PageHeader
                    title={"Administrar pedidos"}
                    headerIcon={<Sell />}
                    secondayButtonAction={exportOrders}
                    secondaryButtonTitle={"Exportar Relatório"}
                    secondaryButtonIcon={<Download />}
                    addButtonAction={async () => {
                        getOrders(1000000000);
                        setVisibility(
                            visibility == "report" ? "list" : "report"
                        );
                    }}
                    addButtonTitle="Alternar visualização"
                />
                <Box
                    display={"flex"}
                    p={"20px 20px"}
                    alignItems={"center"}
                    flexDirection={"column"}
                    gap={1}
                >
                    <Box display={"flex"} width={"100%"} gap={1}>
                        <FormControl sx={{ width: "100%" }}>
                            <InputLabel sx={{ backgroundColor: "#fff", px: 1 }}>
                                Filtrar por status:
                            </InputLabel>
                            <Select
                                onChange={(e) => setFilteredBy(e.target.value)}
                                value={filteredBy}
                                size="small"
                            >
                                <MenuItem value={""}>Todos os pedidos</MenuItem>
                                <MenuItem value={"RELEASED"}>
                                    Liberados
                                </MenuItem>
                                <MenuItem value={"DELIVERED"}>
                                    Entregues
                                </MenuItem>
                                <MenuItem value={"CANCELED"}>
                                    Cancelados
                                </MenuItem>
                                <MenuItem value={"EXPIRED"}>Expirados</MenuItem>
                                <MenuItem value={"ORDERED"}>
                                    Solicitados/Comprados
                                </MenuItem>
                            </Select>
                        </FormControl>
                        <Box
                            display={"flex"}
                            flexDirection={"row"}
                            gap={"10px"}
                        >
                            <LocalizationProvider dateAdapter={AdapterDateFns}>
                                <DatePicker
                                    label="De"
                                    value={startDatePicker}
                                    onChange={(newValue) => {
                                        setStartDatePicker(newValue);
                                    }}
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
                                    onChange={(newValue) =>
                                        setEndDatePicker(newValue)
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
                    </Box>
                    <Box display={"flex"} width={"100%"} gap={1}>
                        <FormControl sx={{ width: "100%" }}>
                            <TextField
                                label="Pesquisar por colaborador"
                                value={collaboratorNameOrIdentification}
                                onChange={(e) =>
                                    setCollaboratorNameOrIdentification(
                                        e.target.value
                                    )
                                }
                                sx={{ width: "100%" }}
                            />
                        </FormControl>
                        <FormControl sx={{ width: "100%" }}>
                            <TextField
                                label="Pesquisar por UF"
                                value={filteredByUf}
                                onChange={(e) =>
                                    setfilteredByUf(e.target.value)
                                }
                                sx={{ width: "100%" }}
                            />
                        </FormControl>

                        <Autocomplete
                            value={filteredBySector}
                            placeholder={"Setor"}
                            fullWidth
                            disableClearable={false}
                            onChange={(e, value) => setFilteredBySector(value)}
                            onInputChange={(e, text) =>
                                setSectorsSearchValue(text)
                            }
                            sx={{ mb: 0 }}
                            renderInput={(props) => (
                                <TextField
                                    {...props}
                                    size={"medium"}
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
                        {/* <FormControl sx={{ width: "100%" }}>
                        <TextField
                            label="Quantidade em Estoque"
                            value={filteredByQuantity}
                            onChange={(e) =>
                                setFilteredByQuantity(e.target.value)
                            }
                            sx={{ width: "100%" }}
                        />
                    </FormControl> */}
                    </Box>
                    <Box display={"flex"} width={"100%"} gap={1}>
                        <FormControl sx={{ width: "100%" }}>
                            <InputLabel sx={{ backgroundColor: "#fff", px: 1 }}>
                                Filtrar por tipo:
                            </InputLabel>
                            <Select
                                onChange={(e) =>
                                    setFilteredByType(e.target.value)
                                }
                                value={filteredByType}
                                sx={{ width: "100%" }}
                                size="small"
                            >
                                <MenuItem value={""}>Todos os tipos</MenuItem>
                                <MenuItem value={"PHYSICAL"}>Físico</MenuItem>
                                <MenuItem value={"DIGITAL"}>Digital</MenuItem>
                            </Select>
                        </FormControl>
                        <FormControl sx={{ width: "100%" }}>
                            <InputLabel>Ordernar por:</InputLabel>
                            <Select
                                value={orderBySell}
                                label="Ordernar por:"
                                onChange={(e) => setOrderBySell(e.target.value)}
                                size="small"
                            >
                                <MenuItem value={""}>Padrão</MenuItem>
                                <MenuItem value={"bestSeller"}>
                                    Mais Vendidos
                                </MenuItem>
                                <MenuItem value={"lessSold"}>
                                    Menos Vendidos
                                </MenuItem>
                            </Select>
                        </FormControl>
                    </Box>
                    <TextField
                        label="Pesquisar por nome, descrição ou código do produto"
                        fullWidth
                        value={searchText}
                        onChange={(e) => setSearchText(e.target.value)}
                    />
                </Box>
            </Card>
            <Box width={"100%"} gap={2}>
                {/* <TextField
                    placeholder="Buscar produto"
                    onChange={(e) => setSearchText(e.target.value)}
                    value={searchText}
                /> */}
                <Box display={"flex"} flexDirection={"column"} gap={2}>
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
                          "Ver administrar Pedidos",
                          "Marketing Place"
                      ) &&
                      abilityFor(myPermissions).can(
                          "Ver administrar Pedidos",
                          "Marketing Place"
                      ) ? (
                        orders.map((order, index) => (
                            <ProductItem data={order} key={index} />
                        ))
                    ) : (
                        <WithoutPermissionCard />
                    )}
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
            </Box>
            <BaseModal
                width={"540px"}
                open={orderDetailsModal ? true : false}
                title={`Detalhes do pedido: #${orderDetailsModal?.id}`}
                onClose={() => setOrderDetailsModal(null)}
            >
                <Box
                    width={"100%"}
                    display={"flex"}
                    flexDirection={"column"}
                    gap={1}
                >
                    <Card display={"flex"} gap={"10px"} p={"10px"}>
                        <Person />
                        <Box>
                            <Typography
                                sx={{ fontSize: "12px", color: grey[600] }}
                            >
                                Pedido feito por:
                            </Typography>
                            <Typography
                                sx={{ fontSize: "14px" }}
                                fontWeight={"500"}
                            >
                                {orderDetailsModal?.orderBy.name}
                            </Typography>
                            <Typography sx={{ fontSize: "12px" }}>
                                {orderDetailsModal?.orderBy.email}
                            </Typography>
                        </Box>
                    </Card>
                    <Card display={"flex"} gap={"10px"} p={"10px"}>
                        <AccessTime />
                        <Box width={"100%"}>
                            <Typography
                                sx={{ fontSize: "12px", color: grey[600] }}
                            >
                                Linha do tempo:
                            </Typography>
                            <Box
                                display={"flex"}
                                justifyContent={"space-between"}
                                alignItems={"center"}
                                width={"100%"}
                            >
                                <Typography
                                    sx={{ fontSize: "14px" }}
                                    fontWeight={"500"}
                                >
                                    Pedido criado em:
                                </Typography>
                                <Typography
                                    sx={{ fontSize: "14px" }}
                                    fontWeight={"500"}
                                >
                                    {orderDetailsModal &&
                                        DateUtils.formatDatePtBRWithTime(
                                            orderDetailsModal?.createdAt
                                        )}
                                </Typography>
                            </Box>
                            <Box
                                display={"flex"}
                                justifyContent={"space-between"}
                                alignItems={"center"}
                                width={"100%"}
                            >
                                <Typography
                                    sx={{ fontSize: "14px" }}
                                    fontWeight={"500"}
                                >
                                    Pedido liberado em:
                                </Typography>
                                <Typography
                                    sx={{ fontSize: "14px" }}
                                    fontWeight={"500"}
                                >
                                    {orderDetailsModal?.releasedAt
                                        ? DateUtils.formatDatePtBRWithTime(
                                              orderDetailsModal?.releasedAt
                                          )
                                        : "--"}
                                </Typography>
                            </Box>
                            <Box
                                display={"flex"}
                                justifyContent={"space-between"}
                                alignItems={"center"}
                                width={"100%"}
                            >
                                <Typography
                                    sx={{ fontSize: "14px" }}
                                    fontWeight={"500"}
                                >
                                    Pedido entregue em:
                                </Typography>
                                <Typography
                                    sx={{ fontSize: "14px" }}
                                    fontWeight={"500"}
                                >
                                    {orderDetailsModal?.deliveredAt
                                        ? DateUtils.formatDatePtBRWithTime(
                                              orderDetailsModal?.deliveredAt
                                          )
                                        : "--"}
                                </Typography>
                            </Box>
                        </Box>
                    </Card>
                </Box>
            </BaseModal>
        </Box>
    );
}

ManageOrders.getLayout = getLayout("private");
