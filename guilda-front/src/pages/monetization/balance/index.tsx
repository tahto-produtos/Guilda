import AccountBalance from "@mui/icons-material/AccountBalance";
import AccountBalanceWallet from "@mui/icons-material/AccountBalanceWallet";
import East from "@mui/icons-material/East";
import West from "@mui/icons-material/West";
import {
    Avatar,
    Box,
    Button,
    ButtonGroup,
    Pagination,
    Typography,
} from "@mui/material";
import { blue, grey } from "@mui/material/colors";
import { Stack } from "@mui/system";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format, isValid } from "date-fns";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { ListBalanceTable } from "src/modules/monetization";
import { ListTransactions } from "src/modules/monetization/use-cases/list-transactions.use-case";
import { TableDataModel } from "src/typings";
import { PaginationModel } from "src/typings/models/pagination.model";
import { DateUtils, SheetBuilder, getLayout } from "src/utils";
import { formatCurrency } from "src/utils/format-currency";

export default function MonetizationBalance() {
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [tableData, setTableData] = useState<TableDataModel | null>(null);
    const { myUser } = useContext(UserInfoContext);
    const [filterByDays, setFilterByDays] = useState<
        "30" | "60" | "90" | "custom"
    >("30");
    const today = DateUtils.formatDateIsoEndOfDay(new Date());
    const [accountBalance, setAccountBalance] = useState<number>(0);
    const [customDateOpen, setCustomDateOpen] = useState<boolean>(false);
    const [startDatePicker, setStartDatePicker] = useState<
        dateFns | Date | null
    >(null);
    const [endDatePicker, setEndDatePicker] = useState<dateFns | Date | null>(
        null
    );
    const [page, setPage] = useState<number>(1);
    const limit = 30;

    const handleChangePagination = (
        event: React.ChangeEvent<unknown>,
        value: number
    ) => {
        setPage(value);
    };

    const sumInputValues = () => {
        if (tableData?.items) {
            const arrInputs = tableData.items.map((item) => item.input);
            const sum = arrInputs.reduce((partialSum, a) => partialSum + a, 0);
            return sum;
        } else {
            return 0;
        }
    };

    const sumOutputValues = () => {
        if (tableData?.items) {
            const arrInputs = tableData.items.map((item) => item.output);
            const sum = arrInputs.reduce((partialSum, a) => partialSum + a, 0);
            return sum;
        } else {
            return 0;
        }
    };

    const GetTransactions = async () => {
        if (!myUser) return;

        if (customDateOpen) {
            if (!startDatePicker || !endDatePicker) {
                return setTableData(null);
            }
            if (isValid(startDatePicker) && isValid(endDatePicker)) {
            } else {
                return setTableData(null);
            }
        }

        setIsLoading(true);

        if (customDateOpen && startDatePicker && endDatePicker) {
            const customPayload = {
                userId: myUser.id,
                dateMin: format(
                    new Date(startDatePicker.toString()),
                    "yyyy-MM-dd"
                ),
                dateMax: format(
                    new Date(endDatePicker.toString()),
                    "yyyy-MM-dd"
                ),
                filter: "collaborator",
                value: myUser.id.toString(),
                limit: limit,
                offset: limit * (page - 1),
            };

            new ListTransactions()
                .handle(customPayload)
                .then((data) => {
                    setTableData({
                        items: data.checkingAccount,
                        totalItems: data.totalItems,
                    });
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    setIsLoading(false);
                });
        } else {
            const dateMin = new Date();
            dateMin.setDate(dateMin.getDate() - parseInt(filterByDays));

            const payload = {
                userId: myUser.id,
                dateMin: DateUtils.formatDateIsoEndOfDay(dateMin),
                dateMax: today,
                filter: "collaborator",
                value: myUser.id.toString(),
                limit: limit,
                offset: limit * (page - 1),
            };

            new ListTransactions()
                .handle(payload)
                .then((data) => {
                    setTableData({
                        items: data.checkingAccount,
                        totalItems: data.totalItems,
                    });
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    setIsLoading(false);
                });
        }
    };

    useEffect(() => {
        myUser && GetTransactions();
    }, [myUser, filterByDays, page]);

    useEffect(() => {
        let lastElement = tableData?.items[0]?.total
            ? tableData?.items[0]?.total
            : tableData?.items[0]?.balance;
        lastElement && setAccountBalance(lastElement);
    }, [tableData]);

    return (
        <Card
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"space-between"}
        >
            <PageHeader
                title={`Extrato da Conta`}
                headerIcon={<AccountBalance />}
            />
            <Stack px={2} py={3} width={"100%"}>
                <Box
                    display={"flex"}
                    flexDirection={"column"}
                    justifyContent={"flex-start"}
                    gap={2}
                    mb={4}
                >
                    <Box display={"flex"} flexDirection={"row"} gap={2}>
                        <Box
                            display={"flex"}
                            gap={2}
                            alignItems={"center"}
                            border={`solid 1px ${grey[200]}`}
                            px={2}
                            py={1}
                            borderRadius={2}
                        >
                            <AccountBalanceWallet />
                            <Box
                                display={"flex"}
                                flexDirection={"column"}
                                gap={0.2}
                            >
                                <Typography
                                    variant="body1"
                                    lineHeight={"16px"}
                                    fontSize={12}
                                >
                                    Saldo da conta
                                </Typography>
                                <Typography
                                    padding={0}
                                    margin={0}
                                    lineHeight={"16px"}
                                    fontSize={18}
                                    fontWeight={500}
                                >
                                    {formatCurrency(accountBalance)}
                                </Typography>
                            </Box>
                        </Box>
                    </Box>
                </Box>

                <Stack
                    direction={"row"}
                    sx={{ p: 0, m: 0 }}
                    alignItems={"center"}
                    justifyContent={"flex-end"}
                    gap={"10px"}
                >
                    <Typography variant="body2" sx={{ color: grey[500] }}>
                        Filtro (dias)
                    </Typography>
                    <ButtonGroup
                        disableElevation
                        variant="contained"
                        color="inherit"
                        disabled={isLoading}
                    >
                        <Button
                            color={
                                filterByDays === "30" ? "primary" : "inherit"
                            }
                            onClick={() => {
                                setCustomDateOpen(false);
                                setFilterByDays("30");
                            }}
                        >
                            30
                        </Button>
                        <Button
                            color={
                                filterByDays === "60" ? "primary" : "inherit"
                            }
                            onClick={() => {
                                setCustomDateOpen(false);
                                setFilterByDays("60");
                            }}
                        >
                            60
                        </Button>
                        <Button
                            color={
                                filterByDays === "90" ? "primary" : "inherit"
                            }
                            onClick={() => {
                                setCustomDateOpen(false);
                                setFilterByDays("90");
                            }}
                        >
                            90
                        </Button>
                        <Button
                            color={
                                filterByDays === "custom"
                                    ? "primary"
                                    : "inherit"
                            }
                            onClick={() => {
                                setFilterByDays("custom");
                                setCustomDateOpen(true);
                            }}
                        >
                            Personalizada
                        </Button>
                    </ButtonGroup>
                </Stack>
                {customDateOpen && (
                    <Box
                        display={"flex"}
                        flexDirection={"row"}
                        gap={"10px"}
                        mt={2}
                    >
                        <LocalizationProvider dateAdapter={AdapterDateFns}>
                            <DatePicker
                                label="De"
                                value={startDatePicker}
                                onChange={(newValue) =>
                                    setStartDatePicker(newValue)
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
                        <Button
                            variant="contained"
                            sx={{ minWidth: "100px" }}
                            onClick={GetTransactions}
                        >
                            Pesquisar
                        </Button>
                    </Box>
                )}

                <ListBalanceTable
                    tableData={tableData || { items: [], totalItems: 0 }}
                    isLoading={isLoading}
                />
                <Box display={"flex"} justifyContent={"flex-end"} mt={2}>
                    {tableData && (
                        <Pagination
                            onChange={handleChangePagination}
                            page={page}
                            count={Math.ceil(tableData.totalItems / limit)}
                        />
                    )}
                </Box>
                <Box display={"flex"} flexDirection={"row"} gap={2}>
                    <Box
                        display={"flex"}
                        gap={2}
                        alignItems={"center"}
                        border={`solid 1px ${grey[200]}`}
                        px={2}
                        py={1}
                        borderRadius={2}
                    >
                        <East color="success" />
                        <Box
                            display={"flex"}
                            flexDirection={"column"}
                            gap={0.2}
                        >
                            <Typography
                                variant="body1"
                                lineHeight={"16px"}
                                fontSize={12}
                            >
                                Entrada
                            </Typography>
                            <Typography
                                padding={0}
                                margin={0}
                                lineHeight={"16px"}
                                fontSize={18}
                                fontWeight={500}
                            >
                                {formatCurrency(sumInputValues())}
                            </Typography>
                        </Box>
                    </Box>
                    <Box
                        display={"flex"}
                        gap={2}
                        alignItems={"center"}
                        border={`solid 1px ${grey[200]}`}
                        px={2}
                        py={1}
                        borderRadius={2}
                    >
                        <West color="error" />
                        <Box
                            display={"flex"}
                            flexDirection={"column"}
                            gap={0.2}
                        >
                            <Typography
                                variant="body1"
                                lineHeight={"16px"}
                                fontSize={12}
                            >
                                Saída
                            </Typography>
                            <Typography
                                padding={0}
                                margin={0}
                                lineHeight={"16px"}
                                fontSize={18}
                                fontWeight={500}
                            >
                                {formatCurrency(sumOutputValues())}
                            </Typography>
                        </Box>
                    </Box>
                </Box>
            </Stack>
        </Card>
    );
}

MonetizationBalance.getLayout = getLayout("private");
