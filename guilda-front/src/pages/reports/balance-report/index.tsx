import Search from "@mui/icons-material/Search";
import InsertDriveFile from "@mui/icons-material/InsertDriveFile";
import {
    Box,
    Button,
    FormControl,
    InputAdornment,
    InputLabel,
    MenuItem,
    Select,
    TextField,
    Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { Stack } from "@mui/system";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format, isValid, set } from "date-fns";
import { useContext, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useLoadingState } from "src/hooks";
import { ListMonetizationHierarchyUseCase } from "src/modules/monetization/use-cases/list-monetization-hierarchy";
import { DateUtils, SheetBuilder, getLayout } from "src/utils";
import { ListTransactions } from "src/modules/monetization/use-cases/list-transactions.use-case";
import { ModalMonetizationReportAdm } from "src/modules/monetization/fragments/modal-monetization-report-adm";
import { ModalMonetizationReportAdmDay } from "src/modules/monetization/fragments/modal-monetization-report-adm-day";
import { ModalMonetizationReportConsolidado } from "src/modules/monetization/fragments/modal-monetization-report-consolidado";
import { formatCurrency } from "src/utils/format-currency";
import abilityFor from "src/utils/ability-for";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { UserInfoContext } from "src/contexts/user-context/user.context";

interface BalanceReportModel {
    hierarchy: string;
    sectors: Array<{
        sectorId: number;
        sector: string;
        monetization: number;
    }>;
}

function formatDetails(
    order: any,
    indicator: any,
    reason: any,
    observation: any
) {
    if (reason) {
        return `Motivo: ${reason}`;
    } else if (observation) {
        return `${observation}`;
    } else if (indicator) {
        return `Indicador: ${indicator.name}`;
    } else if (order) {
        return `Compra na loja - COD: ${order.id}`;
    }
}

export default function BalanceReport() {
    const { myPermissions } = useContext(PermissionsContext);
    const [startDate, setStartDate] = useState<dateFns | null>(null);
    const [endDate, setEndDate] = useState<dateFns | null>(null);
    const [data, setData] = useState<any>(null);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [reportList, setReportList] = useState<BalanceReportModel[]>([]);
    const [searchText, setSearchText] = useState<string>("");
    const [filteredBy, setFilteredBy] = useState<string>("");
    const [filterValue, setFilterValue] = useState<string>("");
    const [limit, setLimit] = useState<number>(100);
    const { myUser } = useContext(UserInfoContext);
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [isOpenDay, setIsOpenDay] = useState<boolean>(false);

    const [isOpenConsolidado, setIsOpenConsolidado] = useState<boolean>(false);

    function handleExtract() {
        startLoading();
        function isOutput(output: number) {
            if (output > 0) {
                return "Saída";
            } else {
                return "Entrada";
            }
        }

        function formatDetails(
            order: any,
            indicator: any,
            reason: any,
            observation: any
        ) {
            if (reason) {
                return `Motivo: ${reason}`;
            } else if (observation) {
                return `${observation}`;
            } else if (indicator) {
                return `Indicador: ${indicator.name}`;
            } else if (order) {
                return `Compra na loja - COD: ${order.id}`;
            }
        }

        if (data.length <= 0) {
            return toast.warning("Sem dados para exportar.");
        }

        try {
            const docRows = data.map((item: any) => {
                return [
                    item.createdAt &&
                        DateUtils.formatDatePtBRWithTime(
                            item.createdAt as Date
                        ).split(" ")[0],
                    item.createdAt &&
                        DateUtils.formatDatePtBRWithTime(
                            item.createdAt as Date
                        ).split(" ")[1],
                    isOutput(item.output),
/*                     `${item.input}`,
                    `${item.output}`,
                    `${item.balance}`, */
                    { v: item.input, t: "n" },
                    { v: item.output, t: "n" },
                    { v: item.balance, t: "n" },
                    item.resultDate && formatResultDate(item?.resultDate || ""),
                    formatDetails(
                        item.order,
                        item.indicator,
                        item.reason,
                        item.observation
                    ),
                ];
            });
            let indicatorSheetBuilder = new SheetBuilder();
            indicatorSheetBuilder
                .setHeader([
                    "Criado em",
                    "Hora",
                    "Entrada/Saída",
                    "Entrada",
                    "Saída",
                    "Saldo",
                    "Data de referência do pagamento",
                    "Detalhe",
                ])
                .append(docRows)
                .exportAs(`Relatório de Monetização`);
            toast.success("Relatório exportado com sucesso!");
        } catch (error) {
            toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
        } finally {
            finishLoading();
        }
    }

    function handleReportExtractExport() {
        if (!myUser) return;

        let formatedStartDate = null;
        let formatedEndDate = null;

        if (startDate && endDate && isValid(startDate) && isValid(endDate)) {
            formatedStartDate = format(
                new Date(startDate.toString()),
                "yyyy-MM-dd"
            );
            formatedEndDate = format(
                new Date(endDate.toString()),
                "yyyy-MM-dd"
            );
        } else {
            toast.warning("Selecione datas válidas");
            return;
        }

        startLoading();

        const payload = {
            userId: myUser.id,
            dateMin: formatedStartDate,
            dateMax: formatedEndDate,
            filter: filteredBy ? filteredBy : "collaborator",
            value: filterValue ? filterValue : myUser.id.toString(),
            limit: limit ? limit : 100,
            offset: 0,
        };

        new ListTransactions()
            .handle(payload)
            .then((data) => {
                setData(data.checkingAccount);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    function formatResultDate(dateString: string) {
        if (dateString) {
            const date = dateString.split("T")[0];
            const dateSplited = date.split("-");
            return `${dateSplited[2]}/${dateSplited[1]}/${dateSplited[0]}`;
        } else {
            return "";
        }
    }

    return (
        <Card
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"space-between"}
        >
            <PageHeader
                title={`Relatório de monetização`}
                headerIcon={<InsertDriveFile />}
            />
            <Stack px={2} py={3} width={"100%"} gap={2}>
                <Box display={"flex"} gap={2} width={"100%"}>
                    <LocalizationProvider dateAdapter={AdapterDateFns}>
                        <DatePicker
                            label="De"
                            value={startDate}
                            onChange={(newValue) => setStartDate(newValue)}
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
                            value={endDate}
                            onChange={(newValue) => setEndDate(newValue)}
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
                {/* <Box
                    display={"flex"}
                    gap={2}
                    width={"100%"}
                    alignItems={"center"}
                >
                    <FormControl sx={{ width: "100%" }}>
                        <InputLabel sx={{ backgroundColor: "#fff", px: 1 }}>
                            Filtrar por:
                        </InputLabel>
                        <Select
                            onChange={(e) => setFilteredBy(e.target.value)}
                            value={filteredBy}
                        >
                            <MenuItem value={""}>Sem filtro</MenuItem>
                            <MenuItem value={"collaborator"}>
                                Colaborador
                            </MenuItem>
                            <MenuItem value={"registry"}>Matricula</MenuItem>
                            <MenuItem value={"sector"}>Setor</MenuItem>
                            <MenuItem value={"hierarchy"}>Hierarquia</MenuItem>
                            <MenuItem value={"group"}>Grupo</MenuItem>
                        </Select>
                    </FormControl>
                    {filteredBy && (
                        <TextField
                            label="Valor"
                            fullWidth
                            value={filterValue}
                            onChange={(e) => setFilterValue(e.target.value)}
                        />
                    )}
                </Box> */}
                <TextField
                    label="Limite de dados"
                    fullWidth
                    value={limit}
                    onChange={(e) => setLimit(parseInt(e.target.value))}
                />

                <Button variant="contained" onClick={handleReportExtractExport}>
                    Ver relatório
                </Button>
                {myPermissions &&
                    abilityFor(myPermissions).can(
                        "Relatorio de monetização",
                        "Relatorios"
                    ) && (
                        <>
                            <Button
                                variant="contained"
                                color="success"
                                onClick={() => setIsOpenDay(true)}
                            >
                                Relatório Analítico Monetização Diário
                            </Button>
                            <Button
                                variant="contained"
                                color="success"
                                onClick={() => setIsOpen(true)}
                            >
                                Relatório Analítico Monetização Consolidado
                            </Button>
                            <Button
                                variant="contained"
                                color="success"
                                onClick={() => setIsOpenConsolidado(true)}
                            >
                                Relatório Saldo Acumulado
                            </Button>
                        </>
                    )}
                {isOpen && (
                    <ModalMonetizationReportAdm
                        open={isOpen}
                        onClose={() => setIsOpen(false)}
                    />
                )}
                {isOpenDay && (
                    <ModalMonetizationReportAdmDay
                        open={isOpenDay}
                        onClose={() => setIsOpenDay(false)}
                    />
                )}
                {isOpenConsolidado && (
                    <ModalMonetizationReportConsolidado
                        open={isOpenConsolidado}
                        onClose={() => setIsOpenConsolidado(false)}
                    />
                )}
                {data && (
                    <Button variant="outlined" onClick={handleExtract}>
                        Exportar Relatório
                    </Button>
                )}
            </Stack>
            {data && (
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
                                Criado em:
                            </Typography>
                        </Box>
                        <Box
                            minWidth={"150px"}
                            maxWidth={"150px"}
                            width={"150px"}
                        >
                            <Typography fontSize={"12px"}>Hora:</Typography>
                        </Box>
                        <Box
                            minWidth={"150px"}
                            maxWidth={"150px"}
                            width={"150px"}
                        >
                            <Typography fontSize={"12px"}>
                                Entrada/Saída
                            </Typography>
                        </Box>
                        <Box
                            minWidth={"150px"}
                            maxWidth={"150px"}
                            width={"150px"}
                        >
                            <Typography fontSize={"12px"}>Entrada</Typography>
                        </Box>
                        <Box
                            minWidth={"150px"}
                            maxWidth={"150px"}
                            width={"150px"}
                        >
                            <Typography fontSize={"12px"}>Saida</Typography>
                        </Box>
                        <Box
                            minWidth={"150px"}
                            maxWidth={"150px"}
                            width={"150px"}
                        >
                            <Typography fontSize={"12px"}>Saldo</Typography>
                        </Box>
                        <Box
                            minWidth={"150px"}
                            maxWidth={"150px"}
                            width={"150px"}
                        >
                            <Typography fontSize={"12px"}>
                                Data de referência do pagamento
                            </Typography>
                        </Box>
                        <Box
                            minWidth={"150px"}
                            maxWidth={"150px"}
                            width={"150px"}
                        >
                            <Typography fontSize={"12px"}>Detalhe</Typography>
                        </Box>
                    </Card>
                    {data[0] &&
                        data.map((item: any, index: any) => (
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
                                        {item.createdAt &&
                                            DateUtils.formatDatePtBRWithTime(
                                                item.createdAt as Date
                                            ).split(" ")[0]}
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
                                        {item.createdAt &&
                                            DateUtils.formatDatePtBRWithTime(
                                                item.createdAt as Date
                                            ).split(" ")[1]}
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
                                        color={
                                            item.output > 0
                                                ? "error.main"
                                                : "success.main"
                                        }
                                    >
                                        {item.output > 0 ? "Saída" : "Entrada"}
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
                                        {formatCurrency(item.input)}
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
                                        {formatCurrency(item.output)}
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
                                        {formatCurrency(item.balance)}
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
                                        {formatResultDate(
                                            item?.resultDate || ""
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
                                        {formatDetails(
                                            item.order,
                                            item.indicator,
                                            item.reason,
                                            item.observation
                                        )}
                                    </Typography>
                                </Box>
                            </Card>
                        ))}
                </Stack>
            )}
        </Card>
    );
}

BalanceReport.getLayout = getLayout("private");
