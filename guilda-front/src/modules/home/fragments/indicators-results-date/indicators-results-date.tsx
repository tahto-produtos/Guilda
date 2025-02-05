import InsertChart from "@mui/icons-material/InsertChart";
import {
    Box,
    Button,
    FormControl,
    InputLabel,
    LinearProgress,
    MenuItem,
    Select,
    Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { useState } from "react";
import { Card, PageHeader } from "src/components";
import { ResultsIndicatorsDate } from "../../use-cases/results-indicators-date";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import {
    addDays,
    addMonths,
    addWeeks,
    endOfMonth,
    endOfWeek,
    format,
} from "date-fns";
import { useLoadingState } from "src/hooks";
import { Sector } from "src/typings";
import { LoadingButton } from "@mui/lab";
import { truncateDecimals } from "src/utils/truncate-decimals";

interface IndicatorResultModel {
    indicator: {
        type: string;
        name: string;
    };
    sector: Sector;
    result: number;
    goal: number;
}

interface ResultProgressProps {
    indicatorName: string;
    progress: number;
    goal: number;
    type: string;
}

const ResultProgress = (props: ResultProgressProps) => {
    const { indicatorName, progress, goal, type } = props;

    const isCompleted = progress >= goal;
    const isPercent = type == "PERCENT" ? "%" : "";
    const toFixedNum = type == "INTEGER" ? 0 : 1;

    return (
        <Box
            display={"flex"}
            flexDirection={"column"}
            gap={1}
            p={"20px"}
            border={`solid 1px ${grey[100]}`}
            borderRadius={2}
        >
            <Box
                justifyContent={"space-between"}
                display={"flex"}
                flexDirection={"row"}
            >
                <Typography fontWeight={"500"}>{indicatorName}</Typography>
                <Typography fontWeight={"500"} display={"flex"}>
                    <Typography
                        fontWeight={"600"}
                        component={"span"}
                        color={"primary"}
                    >
                        {/* {progress &&
                            truncateDecimals(progress, toFixedNum) + isPercent} */}
                        {progress && progress.toFixed(toFixedNum)}
                        {isPercent}
                    </Typography>
                    {goal && `/${goal}`}
                    {goal && isPercent}
                </Typography>
            </Box>
            <LinearProgress
                variant="determinate"
                value={isCompleted ? 100 : (progress / goal) * 100}
                sx={{ borderRadius: "20px" }}
            />
            {/* {isPercent ? (
                <LinearProgress
                    variant="determinate"
                    value={progress >= 100 ? 100 : progress}
                    sx={{ borderRadius: "20px" }}
                />
            ) : (
                <LinearProgress
                    variant="determinate"
                    value={isCompleted ? 100 : (progress / goal) * 100}
                    sx={{ borderRadius: "20px" }}
                />
            )} */}
        </Box>
    );
};

const DateItem = (props: {
    data: { date: string; items: IndicatorResultModel[] };
}) => {
    const { data } = props;

    const [isOpen, setIsOpen] = useState<boolean>(false);

    return (
        <Box
            display={"flex"}
            flexDirection={"column"}
            border={`solid 1px ${grey[100]}`}
            borderRadius={2}
            width={"100%"}
        >
            <Box
                p={"20px"}
                onClick={() => setIsOpen(!isOpen)}
                sx={{ cursor: "pointer" }}
            >
                <Typography fontWeight={"500"}>{data.date}</Typography>
            </Box>
            {isOpen && (
                <Card
                    p={"20px"}
                    display={"flex"}
                    flexDirection={"column"}
                    gap={1}
                >
                    {data.items.length > 0 ? (
                        data.items.map((item, index) => (
                            <ResultProgress
                                indicatorName={item.indicator.name}
                                progress={item.result}
                                goal={item.goal}
                                key={index}
                                type={item.indicator.type}
                            />
                        ))
                    ) : (
                        <Box
                            width={"100%"}
                            display={"flex"}
                            flexDirection={"row"}
                            justifyContent={"center"}
                            alignItems={"center"}
                            p={4}
                        >
                            <Typography color={grey[400]}>
                                Não foi encontrado resultados
                            </Typography>
                        </Box>
                    )}
                </Card>
            )}
        </Box>
    );
};

export function IndicatorsResultsDate() {
    const [startDate, setStartDate] = useState<string | null>(null);
    const [endDate, setEndDate] = useState<string | null>(null);
    const [groupBy, setGroupBy] = useState<string>("MONTH");
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [formatedData, setFormatedData] = useState<Array<{
        date: string;
        items: Array<IndicatorResultModel>;
    }> | null>(null);

    const getIndicatorsResults = async () => {
        if (!startDate || !endDate) {
            return toast.warning("Preencha todos os campos");
        }

        const endDateFormated =
            endDate && format(new Date(endDate), "yyyy-MM-dd");
        const startDateFormated =
            startDate && format(new Date(startDate), "yyyy-MM-dd");

        startLoading();
        const payload = {
            groupBy: groupBy,
            startDate: startDateFormated,
            endDate: endDateFormated,
        };
        new ResultsIndicatorsDate()
            .handle(payload)
            .then((data) => {
                if (groupBy == "DAY") {
                    const formatResults = data.map(
                        (item: IndicatorResultModel, index: number) => {
                            return {
                                date: format(
                                    addDays(new Date(startDate), index),
                                    "dd-MM-yyyy"
                                ),
                                items: item,
                            };
                        }
                    );
                    setFormatedData(formatResults);
                }
                if (groupBy == "WEEK") {
                    const formatResults = data.map(
                        (item: IndicatorResultModel, index: number) => {
                            return {
                                date: `${format(
                                    addWeeks(new Date(startDate), index),
                                    "dd-MM-yyyy"
                                )} - ${format(
                                    addWeeks(new Date(startDate), index + 1),
                                    "dd-MM-yyyy"
                                )} `,
                                items: item,
                            };
                        }
                    );
                    setFormatedData(formatResults);
                }
                if (groupBy == "MONTH") {
                    const formatResults = data.map(
                        (item: IndicatorResultModel, index: number) => {
                            return {
                                date: `${format(
                                    addMonths(new Date(startDate), index),
                                    "dd-MM-yyyy"
                                )} - ${format(
                                    endOfMonth(
                                        addMonths(new Date(startDate), index)
                                    ),
                                    "dd-MM-yyyy"
                                )} `,
                                items: item,
                            };
                        }
                    );
                    setFormatedData(formatResults);
                }
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    return (
        <Card
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"space-between"}
            gap={2}
        >
            <PageHeader
                title="Resultado dos indicadores agrupados por data"
                headerIcon={<InsertChart />}
            />
            <Box display={"flex"} px={"20px"} gap={2}>
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
                <FormControl fullWidth>
                    <InputLabel
                        size="small"
                        sx={{ background: "#fff", px: "5px" }}
                    >
                        Agrupar resultados por:
                    </InputLabel>
                    <Select
                        size="small"
                        value={groupBy}
                        onChange={(e) => setGroupBy(e.target.value)}
                    >
                        <MenuItem value={"DAY"}>Dia</MenuItem>
                        <MenuItem value={"WEEK"}>Semana</MenuItem>
                        <MenuItem value={"MONTH"}>Mês</MenuItem>
                    </Select>
                </FormControl>
            </Box>
            <Box display={"flex"} px={2}>
                <LoadingButton
                    fullWidth
                    variant="contained"
                    onClick={getIndicatorsResults}
                    loading={isLoading}
                >
                    Exibir resultados
                </LoadingButton>
            </Box>
            <Box display={"flex"} p={2} flexDirection={"column"} gap={1}>
                {formatedData?.map((item, index) => (
                    <DateItem data={item} key={index} />
                ))}
            </Box>
        </Card>
    );
}
