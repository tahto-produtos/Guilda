import Category from "@mui/icons-material/Category";
import ConfirmationNumber from "@mui/icons-material/ConfirmationNumber";
import OpenInNew from "@mui/icons-material/OpenInNew";
import {
    Box,
    Button,
    CircularProgress,
    IconButton,
    Pagination,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { isAxiosError } from "axios";
import Image from "next/image";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import WithoutPermissionCard from "src/components/data-display/without-permission/without-permissions";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { useLoadingState } from "src/hooks";
import { AuthVerify } from "src/modules/auth/use-cases/auth-verify/auth-verify.use-case";
import { CollaboratorDetailUseCase } from "src/modules/collaborators/use-cases/collaborator-details.use-case";
import Showcase from "src/modules/marketing-place/fragments/showcase";
import { DeleteProduct } from "src/modules/marketing-place/use-cases/delete-product.use-case";
import { ListMyVouchers } from "src/modules/marketing-place/use-cases/list-my-vouchers.use-case";
import { ListProducts } from "src/modules/marketing-place/use-cases/list-products";
import { ListVoucherCodes } from "src/modules/marketing-place/use-cases/list-voucher-codes.use-case";
import { EXCEPTION_CODES } from "src/typings";
import { Product } from "src/typings/models/product.model";
import { Voucher } from "src/typings/models/voucher.model";
import { DateUtils, getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";

export default function MyVouchers() {
    const [searchText, setSearchText] = useState<string>("");
    const { myPermissions } = useContext(PermissionsContext);
    const { myUser } = useContext(UserInfoContext);
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
    const LIMIT_PER_PAGE = 499;
    const [vouchers, setVouchers] = useState<Voucher[]>([]);
    const [selectedVoucher, setSelectedVoucher] = useState<Voucher | null>(
        null
    );
    const [voucherModalIsOpen, setVoucherModalIsOpen] = useState(false);
    const [loadingCodes, setLoadingCodes] = useState(false);
    const [voucherCodes, setVoucherCodes] = useState<string>("");
    const [isAuthVerified, setIsAuthVerified] = useState<boolean>(false);
    const [authVerifyUser, setAuthVerifyUser] = useState<string>("");
    const [authVerifyPassword, setAuthVerifyPassword] = useState<string>("");
    const [authVerifyIsLoading, setAuthVerifyIsLoading] =
        useState<boolean>(false);

    useEffect(() => {
        setAuthVerifyUser("");
        setAuthVerifyPassword("");
        setIsAuthVerified(false);
    }, [selectedVoucher]);

    const getVouchers = async () => {
        startLoading();

        const payload = {
            limit: LIMIT_PER_PAGE,
            offset: LIMIT_PER_PAGE * (page - 1),
        };

        new ListMyVouchers()
            .handle(payload)
            .then((data) => {
                setVouchers(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    const getVoucherCodes = async () => {
        if (selectedVoucher) {
            setLoadingCodes(true);

            const payload = {
                voucherId: selectedVoucher.id,
            };

            new ListVoucherCodes()
                .handle(payload)
                .then((data) => {
                    setVoucherCodes(data.voucher.voucher);
                })
                .catch((e) => {
                    if (e?.response?.data?.message == "Forbidden resource") {
                        toast.warning(
                            "Você não tem permissão para ver um voucher"
                        );
                    } else {
                        toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                    }
                })
                .finally(() => {
                    setLoadingCodes(false);
                });
        }
    };

    function handleAuthVerify() {
        if (!myUser) return;

        setAuthVerifyIsLoading(true);

        const payload = {
            username: authVerifyUser,
            password: authVerifyPassword,
            currentUserId: myUser.id,
        };

        new AuthVerify()
            .handle(payload)
            .then((data) => {
                console.log(data);
                if (data == true) {
                    setIsAuthVerified(true);
                } else {
                    toast.warning("Usuário não autenticado");
                }
            })
            .catch((e) => {
                if (isAxiosError(e)) {
                    const errorCode = e?.response?.data?.code;
                    let message = INTERNAL_SERVER_ERROR_MESSAGE;

                    if (
                        errorCode === EXCEPTION_CODES.PASSWORD_NOT_MATCH ||
                        errorCode === EXCEPTION_CODES.NOT_FOUND
                    ) {
                        message = "Usuário e senha não combinam";
                    }

                    toast.error(message);
                }
            })
            .finally(() => {
                finishLoading();
                setAuthVerifyIsLoading(false);
            });
    }

    useEffect(() => {
        selectedVoucher && isAuthVerified && getVoucherCodes();
    }, [selectedVoucher, isAuthVerified]);

    useEffect(() => {
        getVouchers();
    }, [page]);

    const VoucherItem = (props: { data: Voucher }) => {
        const { collaboratorId, createdAt, id, voucherId } = props.data;
        let valDate;
        if (props?.data?.voucher?.product?.validity) {
            valDate = props?.data?.voucher?.voucherValidity
                ? props?.data?.voucher?.voucherValidity
                : props?.data?.voucher?.product?.validity;
            // valDate.setMinutes(valDate.getMinutes() + valDate.getTimezoneOffset());
        }

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
                    <Box>
                        <Typography variant="body2" fontSize={"16px"}>
                            Voucher -{" "}
                            {props?.data?.voucher?.product?.comercialName}
                        </Typography>
                        <Typography
                            variant="body2"
                            fontSize={"12px"}
                            color={grey[600]}
                        >
                            {createdAt &&
                                `criado em: ${
                                    createdAt
                                        ?.toString()
                                        .split("T")[0]
                                        .split("-")[2]
                                }/${
                                    createdAt
                                        ?.toString()
                                        .split("T")[0]
                                        .split("-")[1]
                                }/${
                                    createdAt
                                        ?.toString()
                                        .split("T")[0]
                                        .split("-")[0]
                                }`}
                            {}
                        </Typography>
                        {props?.data?.voucher?.product?.validity && (
                            <Typography
                                variant="body2"
                                fontSize={"12px"}
                                color={grey[600]}
                            >
                                Válido até -{" "}
                                {`${
                                    valDate
                                        ?.toString()
                                        .split("T")[0]
                                        .split("-")[2]
                                }/${
                                    valDate
                                        ?.toString()
                                        .split("T")[0]
                                        .split("-")[1]
                                }/${
                                    valDate
                                        ?.toString()
                                        .split("T")[0]
                                        .split("-")[0]
                                }`}
                            </Typography>
                        )}
                    </Box>
                </Box>
                {abilityFor(myPermissions).can(
                    "Ver Voucher",
                    "Marketing Place"
                ) && (
                    <IconButton
                        onClick={() => {
                            setSelectedVoucher(props.data);
                            setVoucherModalIsOpen(true);
                        }}
                    >
                        <OpenInNew color="primary" />
                    </IconButton>
                )}
            </Box>
        );
    };

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Meus vouchers"}
                headerIcon={<ConfirmationNumber />}
            />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                {/* <TextField
                    placeholder="Buscar produto"
                    onChange={(e) => setSearchText(e.target.value)}
                    value={searchText}
                /> */}
                <Box display={"flex"} flexDirection={"column"} gap={1}>
                    {isLoading ? (
                        <Box
                            display={"flex"}
                            justifyContent={"center"}
                            alignItems={"center"}
                            p={4}
                            width={"100%"}
                        >
                            <CircularProgress />
                        </Box>
                    ) : (
                        vouchers.map((voucher, index) => (
                            <VoucherItem key={index} data={voucher} />
                        ))
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
            </Stack>
            <BaseModal
                width={"540px"}
                open={voucherModalIsOpen}
                title={`${selectedVoucher?.voucher?.product?.comercialName} #${selectedVoucher?.id}`}
                onClose={() => {
                    setVoucherModalIsOpen(false);
                    setIsAuthVerified(false);
                    setSelectedVoucher(null);
                }}
            >
                <Box
                    width={"100%"}
                    display={"flex"}
                    flexDirection={"column"}
                    gap={1}
                >
                    {authVerifyIsLoading && (
                        <Box
                            display={"flex"}
                            alignItems={"center"}
                            justifyContent={"center"}
                            gap={1}
                            py={4}
                        >
                            <CircularProgress />
                            <Typography>Aguarde...</Typography>
                        </Box>
                    )}
                    {!isAuthVerified && !authVerifyIsLoading && (
                        <Box
                            width={"100%"}
                            display={"flex"}
                            flexDirection={"column"}
                            gap={1}
                        >
                            <TextField
                                label="Nome de Usuário"
                                fullWidth
                                value={authVerifyUser}
                                onChange={(e) =>
                                    setAuthVerifyUser(e.target.value)
                                }
                            />
                            <TextField
                                label="Senha"
                                fullWidth
                                value={authVerifyPassword}
                                onChange={(e) =>
                                    setAuthVerifyPassword(e.target.value)
                                }
                                type="password"
                            />
                            <Button
                                variant="contained"
                                onClick={handleAuthVerify}
                                disabled={authVerifyIsLoading}
                            >
                                Autenticar para ver o voucher
                            </Button>
                        </Box>
                    )}
                    {!loadingCodes && isAuthVerified && (
                        <>
                            <Typography variant="body2">
                                Motivo de recebimento:{" "}
                                {selectedVoucher?.reasonPurchase ||
                                    "indefinido"}
                            </Typography>
                            <Typography variant="body2">
                                Descrição do voucher:{" "}
                                {selectedVoucher?.voucher.product.description}
                            </Typography>
                            <Typography variant="body2">
                                Data de validade:{" "}
                                {`${
                                    selectedVoucher?.voucher.voucherValidity
                                        ? selectedVoucher?.voucher.voucherValidity
                                              ?.toString()
                                              .split("T")[0]
                                              .split("-")[2]
                                        : selectedVoucher?.voucher.product.validity
                                              ?.toString()
                                              .split("T")[0]
                                              .split("-")[2]
                                }/${
                                    selectedVoucher?.voucher.voucherValidity
                                        ? selectedVoucher?.voucher.voucherValidity
                                              ?.toString()
                                              .split("T")[0]
                                              .split("-")[1]
                                        : selectedVoucher?.voucher.product.validity
                                              ?.toString()
                                              .split("T")[0]
                                              .split("-")[1]
                                }/${
                                    selectedVoucher?.voucher.voucherValidity
                                        ? selectedVoucher?.voucher.voucherValidity
                                              ?.toString()
                                              .split("T")[0]
                                              .split("-")[0]
                                        : selectedVoucher?.voucher.product.validity
                                              ?.toString()
                                              .split("T")[0]
                                              .split("-")[0]
                                }`}
                            </Typography>
                        </>
                    )}
                    {!loadingCodes &&
                        isAuthVerified &&
                        voucherCodes.split(",").map((voucher, index) => (
                            <Card sx={{ p: "10px" }} key={index}>
                                {voucher}
                            </Card>
                        ))}
                    {loadingCodes && <CircularProgress />}
                </Box>
            </BaseModal>
        </Card>
    );
}

MyVouchers.getLayout = getLayout("private");
