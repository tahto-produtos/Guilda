import FileOpen from "@mui/icons-material/FileOpen";
import Category from "@mui/icons-material/Category";
import Download from "@mui/icons-material/Download";
import Inventory from "@mui/icons-material/Inventory";
import OpenInNew from "@mui/icons-material/OpenInNew";
import {
    Box,
    Button,
    Checkbox,
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
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format, isValid } from "date-fns";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import WithoutPermissionCard from "src/components/data-display/without-permission/without-permissions";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useLoadingState } from "src/hooks";
import { CollaboratorDetailUseCase } from "src/modules/collaborators/use-cases/collaborator-details.use-case";
import Showcase from "src/modules/marketing-place/fragments/showcase";
import { AssociateProducts } from "src/modules/marketing-place/use-cases/associate-products.use-case";
import { DeleteStock } from "src/modules/marketing-place/use-cases/delete-stock";
import { GenerateStockReportUseCase } from "src/modules/marketing-place/use-cases/generate-stock-report.use-case";
import { ListProducts } from "src/modules/marketing-place/use-cases/list-products";
import { ListStockTypes } from "src/modules/marketing-place/use-cases/list-stock-types";
import { ListStocks } from "src/modules/marketing-place/use-cases/list-stocks";
import { Product } from "src/typings/models/product.model";
import { StockReportItem } from "src/typings/models/stock-report-item";
import { StockType } from "src/typings/models/stock-type.model";
import { Stock } from "src/typings/models/stock.model";
import { DateUtils, SheetBuilder, getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";
import { CompleteStockReport } from "src/modules/marketing-place/use-cases/complete-stock-report.use-case";
import { StockReport } from "src/typings/models/stock-report.model";
import { formatCurrency } from "src/utils/format-currency";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export default function StockReportScreen() {
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
    const LIMIT_PER_PAGE = 20;
    const [startDate, setStartDate] = useState<dateFns | null>(null);
    const [endDate, setEndDate] = useState<dateFns | null>(null);
    const [stockReportData, setStockReportData] = useState<StockReport[]>([]);
    const [totalItems, setTotalItems] = useState<number | null>(null);
    const [bestSeller, setBestSeller] = useState<"true" | "false" | "">("");
    const [city, setCity] = useState<string>("");
    const [status, setStatus] = useState<"ATIVO" | "INATIVO" | "">("");
    const [visibilityType, setVisibilityType] = useState<
        "HIERARCHY" | "GROUP" | "CLIENT" | "SECTOR" | ""
    >("");
    const [visibilityValue, setVisibilityValue] = useState<string>("");

    useEffect(() => {
        setPage(1);
        setStockReportData([]);
        setTotalItems(0);
    }, [startDate, endDate]);

    const getStockReport = async () => {
        if (startDate && endDate && isValid(startDate) && isValid(endDate)) {
            startLoading();
            const formatedStartDate = format(
                new Date(startDate.toString()),
                "yyyy-MM-dd"
            );
            const formatedEndDate = format(
                new Date(endDate.toString()),
                "yyyy-MM-dd"
            );

            const payload = {
                limit: LIMIT_PER_PAGE,
                offset: LIMIT_PER_PAGE * (page - 1),
                startDate: formatedStartDate,
                endDate: formatedEndDate,
                city: city ? city : undefined,
                bestSeller:
                    bestSeller == "true"
                        ? true
                        : bestSeller == "false"
                        ? false
                        : undefined,
                status: status ? status : undefined,
                visibilityType: visibilityType ? visibilityType : undefined,
                visibilityValue: visibilityValue ? visibilityValue : undefined,
                productStatus: status ? status : undefined,
            };

            new CompleteStockReport()
                .handle(payload)
                .then((data) => {
                    setStockReportData(data.items);
                    setTotalPages(data.totalPages);
                    setTotalItems(data.totalItems);
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    finishLoading();
                });
        }
    };

    useEffect(() => {
        stockReportData.length > 0 && getStockReport();
    }, [page]);

    function handleReportExport() {
        if (
            startDate &&
            endDate &&
            isValid(startDate) &&
            isValid(endDate) &&
            totalItems
        ) {
            startLoading();

            const formatedStartDate = format(
                new Date(startDate.toString()),
                "yyyy-MM-dd"
            );
            const formatedEndDate = format(
                new Date(endDate.toString()),
                "yyyy-MM-dd"
            );

            const payload = {
                startDate: formatedStartDate,
                endDate: formatedEndDate,
                limit: totalItems,
                offset: 0,
                city: city ? city : undefined,
                bestSeller:
                    bestSeller == "true"
                        ? true
                        : bestSeller == "false"
                        ? false
                        : undefined,
                status: status ? status : undefined,
                visibilityType: visibilityType ? visibilityType : undefined,
                visibilityValue: visibilityValue ? visibilityValue : undefined,
                productStatus: status ? status : undefined,
            };

            new CompleteStockReport()
                .handle(payload)
                .then((data) => {
                    const docRows = data.items.map((item: StockReport) => {
                        return [
                            item.type,
                            item.code,
                            item.name,
                            item.status,
                            item.inputs[0]?.amount,
                            item.outputsBySales[0]?.amount,
                            item.generalOutputs[0]?.amount,
                        ];
                    });
                    let indicatorSheetBuilder = new SheetBuilder();
                    indicatorSheetBuilder
                        .setHeader([
                            "Tipo",
                            "Código",
                            "Nome produto comercial",
                            "Status",
                            "Entradas",
                            "Saídas por venda",
                            "Saídas gerais",
                        ])
                        .append(docRows)
                        .exportAs(`Relatório de Estoque Completo`);
                    toast.success("Relatório exportado com sucesso!");
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    finishLoading();
                });
        } else {
            toast.warning("Visualize o relatório antes de exporta-lo");
        }
    }

    if (
        abilityFor(myPermissions).cannot("Relatorio de estoque", "Relatorios")
    ) {
        return (
            <Card width={"100%"} display={"flex"} flexDirection={"column"}>
                <WithoutPermissionCard />
            </Card>
        );
    }

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Relatório de estoque"}
                headerIcon={<FileOpen />}
            />
            <Stack px={2} py={4} width={"100%"} gap={2}>
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
                <Box
                    display={"flex"}
                    gap={1}
                    width={"100%"}
                    alignItems={"center"}
                >
                    <TextField
                        value={city}
                        onChange={(e) => setCity(e.target.value)}
                        size="small"
                        fullWidth
                        label="Cidade"
                    />
                    <FormControl sx={{ width: "100%" }}>
                        <InputLabel
                            sx={{ backgroundColor: "#fff" }}
                            size="small"
                        >
                            Status
                        </InputLabel>
                        <Select
                            value={status}
                            onChange={(e) =>
                                setStatus(
                                    e.target.value as "ATIVO" | "INATIVO" | ""
                                )
                            }
                            fullWidth
                            size="small"
                        >
                            <MenuItem value={""}>Todos</MenuItem>
                            <MenuItem value={"ATIVO"}>Ativo</MenuItem>
                            <MenuItem value={"INATIVO"}>Inativo</MenuItem>
                        </Select>
                    </FormControl>
                </Box>
                <Box
                    display={"flex"}
                    gap={1}
                    width={"100%"}
                    alignItems={"center"}
                >
                    <FormControl sx={{ width: "100%" }}>
                        <InputLabel
                            sx={{ backgroundColor: "#fff" }}
                            size="small"
                        >
                            Tipo de visibilidade
                        </InputLabel>
                        <Select
                            value={visibilityType}
                            onChange={(e) =>
                                setVisibilityType(
                                    e.target.value as
                                        | "HIERARCHY"
                                        | "GROUP"
                                        | "CLIENT"
                                        | "SECTOR"
                                        | ""
                                )
                            }
                            fullWidth
                            size="small"
                        >
                            <MenuItem value={""}>Todos</MenuItem>
                            <MenuItem value={"HIERARCHY"}>Hierarquia</MenuItem>
                            <MenuItem value={"GROUP"}>Grupo</MenuItem>
                            <MenuItem value={"CLIENT"}>Cliente</MenuItem>
                            <MenuItem value={"SECTOR"}>Setor</MenuItem>
                        </Select>
                    </FormControl>
                    <TextField
                        value={visibilityValue}
                        onChange={(e) => setVisibilityValue(e.target.value)}
                        size="small"
                        fullWidth
                        label="Valor de visualização"
                    />
                </Box>
                <Box
                    display={"flex"}
                    gap={1}
                    width={"100%"}
                    alignItems={"center"}
                >
                    <FormControl sx={{ width: "100%" }}>
                        <InputLabel
                            sx={{ backgroundColor: "#fff" }}
                            size="small"
                        >
                            Mais vendidos?
                        </InputLabel>
                        <Select
                            value={bestSeller}
                            onChange={(e) =>
                                setBestSeller(
                                    e.target.value as "true" | "false" | ""
                                )
                            }
                            fullWidth
                            size="small"
                        >
                            <MenuItem value={""}>Todos</MenuItem>
                            <MenuItem value={"true"}>Mais vendidos</MenuItem>
                            <MenuItem value={"false"}>Menos vendidos</MenuItem>
                        </Select>
                    </FormControl>
                </Box>
                <Box display={"flex"} width={"100%"} gap={2}>
                    <Button
                        variant="contained"
                        fullWidth
                        disabled={!startDate || !endDate}
                        onClick={getStockReport}
                    >
                        Visualizar relatório
                    </Button>
                    <Button
                        variant="outlined"
                        fullWidth
                        disabled={!totalItems}
                        onClick={handleReportExport}
                    >
                        Exportar relatório
                    </Button>
                </Box>
                <Box display={"flex"} flexDirection={"column"} gap={1}>
                    {stockReportData.map((item, index) => (
                        <Card
                            key={index}
                            p={"10px 20px"}
                            display={"flex"}
                            flexDirection={"column"}
                            style={{ backgroundColor: grey[200] }}
                        >
                            <Box
                                width={"100%"}
                                display={"flex"}
                                justifyContent={"space-between"}
                                alignItems={"center"}
                            >
                                <Box>
                                    <Typography
                                        variant="body1"
                                        fontSize={"11px"}
                                        sx={{ color: grey[600] }}
                                    >
                                        {item.type.toLowerCase() == "physical"
                                            ? "Físico"
                                            : "Digital"}{" "}
                                        - #{item.code}
                                    </Typography>
                                    <Typography
                                        variant="body1"
                                        fontSize={"15px"}
                                        fontWeight={"600"}
                                    >
                                        {item.name}
                                    </Typography>
                                </Box>
                                <Box
                                    display={"flex"}
                                    flexDirection={"column"}
                                    alignItems={"flex-end"}
                                >
                                    <Typography
                                        variant="body1"
                                        fontSize={"12px"}
                                    >
                                        Valor:{" "}
                                        <Typography
                                            variant="body1"
                                            fontSize={"12px"}
                                            component={"span"}
                                            fontWeight={"600"}
                                        >
                                            {formatCurrency(item.productPrice)}{" "}
                                            moedas
                                        </Typography>
                                    </Typography>
                                    <Typography
                                        variant="body1"
                                        fontSize={"12px"}
                                    >
                                        Valor total:{" "}
                                        <Typography
                                            variant="body1"
                                            fontSize={"12px"}
                                            component={"span"}
                                            fontWeight={"600"}
                                        >
                                            {formatCurrency(item.totalCost)}{" "}
                                            moedas
                                        </Typography>
                                    </Typography>
                                </Box>
                            </Box>
                            <Card
                                display={"flex"}
                                justifyContent={"space-between"}
                                p={"10px 20px"}
                                mt={1}
                            >
                                <Box sx={{ width: "100%" }}>
                                    <Typography
                                        variant="body1"
                                        fontSize={"13px"}
                                        fontWeight={"600"}
                                    >
                                        Entradas
                                    </Typography>
                                    {item.inputs.map((input, index) => (
                                        <Box
                                            width={"100%"}
                                            display={"flex"}
                                            gap={2}
                                            key={index}
                                        >
                                            <Typography
                                                variant="body1"
                                                fontSize={"12px"}
                                            >
                                                {input.stockName} -{" "}
                                                <Typography
                                                    variant="body1"
                                                    fontSize={"12px"}
                                                    component={"span"}
                                                    fontWeight={"600"}
                                                >
                                                    {input.amount}
                                                </Typography>
                                            </Typography>
                                        </Box>
                                    ))}
                                </Box>
                                <Box sx={{ width: "100%" }}>
                                    <Typography
                                        variant="body1"
                                        fontSize={"13px"}
                                        fontWeight={"600"}
                                    >
                                        Saídas por venda
                                    </Typography>
                                    {item.outputsBySales.map((input, index) => (
                                        <Box
                                            width={"100%"}
                                            display={"flex"}
                                            gap={2}
                                            key={index}
                                        >
                                            <Typography
                                                variant="body1"
                                                fontSize={"12px"}
                                            >
                                                {input.stockName} -{" "}
                                                <Typography
                                                    variant="body1"
                                                    fontSize={"12px"}
                                                    component={"span"}
                                                    fontWeight={"600"}
                                                >
                                                    {input.amount}
                                                </Typography>
                                            </Typography>
                                        </Box>
                                    ))}
                                </Box>
                                <Box sx={{ width: "100%" }}>
                                    <Typography
                                        variant="body1"
                                        fontSize={"13px"}
                                        fontWeight={"600"}
                                    >
                                        Saídas
                                    </Typography>
                                    {item.generalOutputs.map((input, index) => (
                                        <Box
                                            width={"100%"}
                                            display={"flex"}
                                            gap={2}
                                            key={index}
                                        >
                                            <Typography
                                                variant="body1"
                                                fontSize={"12px"}
                                            >
                                                {input.stockName} -{" "}
                                                <Typography
                                                    variant="body1"
                                                    fontSize={"12px"}
                                                    component={"span"}
                                                    fontWeight={"600"}
                                                >
                                                    {input.amount}
                                                </Typography>
                                            </Typography>
                                        </Box>
                                    ))}
                                </Box>
                            </Card>
                        </Card>
                    ))}
                </Box>
                <Box
                    display={"flex"}
                    justifyContent={"flex-end"}
                    alignItems={"center"}
                >
                    {stockReportData.length > 0 && (
                        <Pagination
                            count={totalPages}
                            page={page}
                            onChange={handleChangePagination}
                            disabled={isLoading}
                        />
                    )}
                </Box>
            </Stack>
        </Card>
    );
}

StockReportScreen.getLayout = getLayout("private");
