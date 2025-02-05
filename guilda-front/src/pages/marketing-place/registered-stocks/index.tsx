import Category from "@mui/icons-material/Category";
import Download from "@mui/icons-material/Download";
import Inventory from "@mui/icons-material/Inventory";
import OpenInNew from "@mui/icons-material/OpenInNew";
import {
    Box,
    Button,
    IconButton,
    Pagination,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { isAxiosError } from "axios";
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
import { ListStocks } from "src/modules/marketing-place/use-cases/list-stocks";
import { EXCEPTION_CODES } from "src/typings";
import { StockType } from "src/typings/models/stock-type.model";
import { Stock } from "src/typings/models/stock.model";
import { DateUtils, getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";
import { formatCurrency } from "src/utils/format-currency";

export default function RegisteredStocks() {
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
    const [stocks, setStocks] = useState<Array<Stock>>([]);
    const LIMIT_PER_PAGE = 20;
    const [typeList, setTypeList] = useState<StockType[]>([]);

    const handleRedirectToCreateStockPage = () =>
        router.push("/marketing-place/create-stock");

    const getStocks = async () => {
        startLoading();

        const payload = {
            limit: LIMIT_PER_PAGE,
            offset: LIMIT_PER_PAGE * (page - 1),
            searchText: searchText,
        };

        new ListStocks()
            .handle(payload)
            .then((data) => {
                setStocks(data.items);
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
        getStocks();
    }, [page]);

    const StockItem = (props: { data: Stock }) => {
        const { data } = props;
        const [isAuthVerified, setIsAuthVerified] = useState<boolean>(false);
        const [authVerifyUser, setAuthVerifyUser] = useState<string>("");
        const [authVerifyPassword, setAuthVerifyPassword] =
            useState<string>("");
        const [authVerifyIsLoading, setAuthVerifyIsLoading] =
            useState<boolean>(false);
        const { myUser } = useContext(UserInfoContext);

        const [authIsOpen, setAuthIsOpen] = useState<boolean>(false);

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
                .then((response) => {
                    if (response == true) {
                        setIsAuthVerified(true);
                        router.push(
                            `/marketing-place/registered-stocks/${data.id}`
                        );
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
                    setAuthVerifyIsLoading(false);
                });
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
                <BaseModal
                    width={"540px"}
                    open={authIsOpen}
                    title={`Confirme seu login`}
                    onClose={() => setAuthIsOpen(false)}
                >
                    <Stack
                        direction={"column"}
                        gap={2}
                        justifyContent={"center"}
                        alignItems={"center"}
                    >
                        <Typography variant="body2">
                            É necessário confirmar seu login para acessar este
                            conteúdo
                        </Typography>
                        <Box
                            width={"100%"}
                            display={"flex"}
                            flexDirection={"column"}
                            gap={1}
                            maxWidth={"400px"}
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
                                Autenticar
                            </Button>
                        </Box>
                    </Stack>
                </BaseModal>
                <Box display={"flex"} alignItems={"center"} gap={2}>
                    <Inventory />
                    <Box>
                        <Typography variant="body2" fontSize={"16px"}>
                            {data.description} -{" "}
                            {`(${formatCurrency(data.totalAmount)} produtos)`}
                            <Typography
                                variant="body2"
                                component={"span"}
                                fontSize={"12px"}
                                color={"primary"}
                                ml={1}
                                fontWeight={"500"}
                            >
                                {
                                    typeList?.find(
                                        (item) => item.id === data.typeId
                                    )?.type
                                }
                            </Typography>
                        </Typography>
                        <Typography
                            variant="body2"
                            fontSize={"12px"}
                            color={grey[600]}
                        >
                            {data.createdAt &&
                                `criado em: ${DateUtils.formatDatePtBRWithTime(
                                    data.createdAt
                                )}`}
                            {}
                        </Typography>
                    </Box>
                </Box>
                {data.id && (
                    <IconButton
                        onClick={() => {
                            if (data.type == "DIGITAL") {
                                return setAuthIsOpen(true);
                            }
                            router.push(
                                `/marketing-place/registered-stocks/${data.id}`
                            );
                        }}
                    >
                        <OpenInNew color="primary" />
                    </IconButton>
                )}
                {!data.id && (
                    <IconButton
                        onClick={() => {
                            if (
                                data.description
                                    .toLocaleLowerCase()
                                    .includes("virtuais") &&
                                data.description
                                    .toLocaleLowerCase()
                                    .includes("físicos")
                            ) {
                                return router.push({
                                    pathname: `/marketing-place/registered-stocks/stock-total`,
                                    query: { type: "" },
                                });
                            }
                            if (
                                data.description
                                    .toLocaleLowerCase()
                                    .includes("virtuais")
                            ) {
                                return router.push({
                                    pathname: `/marketing-place/registered-stocks/stock-total`,
                                    query: { type: "DIGITAL" },
                                });
                            }
                            if (
                                data.description
                                    .toLocaleLowerCase()
                                    .includes("físicos")
                            ) {
                                return router.push({
                                    pathname: `/marketing-place/registered-stocks/stock-total`,
                                    query: { type: "PHYSICAL" },
                                });
                            }
                            router.push(
                                `/marketing-place/registered-stocks/stock-total`
                            );
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
            <PageHeader title={"Estoques"} headerIcon={<Inventory />} />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                {abilityFor(myPermissions).can(
                    "Ver Estoque",
                    "Marketing Place"
                ) ? (
                    <Box display={"flex"} flexDirection={"column"} gap={1}>
                        {stocks.map((stock, index) => (
                            <StockItem key={index} data={stock} />
                        ))}
                    </Box>
                ) : (
                    <WithoutPermissionCard />
                )}

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

RegisteredStocks.getLayout = getLayout("private");
