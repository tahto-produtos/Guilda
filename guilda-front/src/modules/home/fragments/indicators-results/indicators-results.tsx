import InsertChart from "@mui/icons-material/InsertChart";
import {
    Box,
    Button,
    ButtonGroup,
    CircularProgress,
    LinearProgress,
    Stack,
    Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { useContext, useEffect, useState } from "react";
import { Card, PageHeader } from "src/components";
import { useLoadingState } from "src/hooks";
import { ListIndicatorsResults } from "../../use-cases/list-indicators-results/list-indicators-results";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { DateUtils } from "src/utils";
import { formatISO, startOfDay } from "date-fns";
import { ResultsIndicatorsSectors } from "../../use-cases/results-indicators-sectors";
import { ListIndicatorBasketUseCase } from "../../use-cases/get-indicator-basket/get-indicator-basket.use-case";
import { UserInfoContext } from "src/contexts/user-context/user.context";

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
    const toFixedNum = type == "INTEGER" ? 0 : 2;

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
                        {progress.toFixed(toFixedNum) + isPercent}
                    </Typography>
                    {goal && `/${goal}`}
                </Typography>
            </Box>
            {isPercent ? (
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
            )}
        </Box>
    );
};

interface IndicatorResultModel {
    indicator: {
        type: string;
        name: string;
    };
    result: number;
    goal: number;
}

export default function IndicatorsResults(props: { userId: number }) {
    const { userId } = props;
    const [indicatorsResults, setIndicatorsResults] = useState<
        IndicatorResultModel[]
    >([]);
    const { myUser } = useContext(UserInfoContext);
    const { finishLoading, isLoading, startLoading } = useLoadingState();

    const [dateFilter, setDateFilter] = useState<"day" | "week" | "month">(
        "month"
    );
    const [indicatorBasket, setIndicatorBasket] =
        useState<IndicatorResultModel | null>(null);

    const todayEndOfDay = DateUtils.formatDateIsoEndOfDay(new Date());

    function dayRules() {
        const startDate = new Date();
        startDate.setDate(startDate.getDate() - 1);

        return {
            startDate: DateUtils.formatDateIsoEndOfDay(startDate),
            endDate: todayEndOfDay,
        };
    }

    function weekRules() {
        const startDate = new Date();
        startDate.setDate(startDate.getDate() - 7);

        return {
            startDate: DateUtils.formatDateIsoEndOfDay(startDate),
            endDate: todayEndOfDay,
        };
    }

    function monthRules() {
        const today = new Date();
        const thisMonth = today.getMonth();
        const thisYear = today.getFullYear();
        const startDate = formatISO(
            new Date(startOfDay(new Date(thisYear, thisMonth, 1)))
        );

        return {
            startDate: startDate,
            endDate: todayEndOfDay,
        };
    }

    const getIndicatorsResults = async () => {
        startLoading();

        const dateFilterRules =
            dateFilter == "day"
                ? dayRules()
                : dateFilter == "week"
                ? weekRules()
                : monthRules();

        const payload = {
            collaboratorId: userId,
            startDate: dateFilterRules.startDate,
            endDate: dateFilterRules.endDate,
            // startDate: "2023-07-11",
            // endDate: "2023-07-11",
        };

        new ListIndicatorsResults()
            .handle(payload)
            .then((data) => {
                setIndicatorsResults(data);

                const payload = {
                    startDate: dateFilterRules.startDate,
                    endDate: dateFilterRules.endDate,
                };

                new ListIndicatorBasketUseCase()
                    .handle(payload)
                    .then((data) => {
                        setIndicatorBasket(data);
                    })
                    .catch(() => {
                        toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                    })
                    .finally(() => {});
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });

        // new ResultsIndicatorsSectors()
        //     .handle(payload)
        //     .then((data) => {
        //         setIndicatorsResults(data);
        //         console.log(data);
        //     })
        //     .catch(() => {
        //         toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
        //     })
        //     .finally(() => {
        //         finishLoading();
        //     });
    };

    useEffect(() => {
        userId && getIndicatorsResults();
    }, [userId, dateFilter]);

    return (
        <Card
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"space-between"}
        >
            <PageHeader
                title="Resultado dos indicadores"
                headerIcon={<InsertChart />}
            />
            <Stack px={2} py={3} width={"100%"} gap={2}>
                <Box
                    display={"flex"}
                    flexDirection={"row"}
                    justifyContent={"flex-end"}
                >
                    <ButtonGroup
                        disableElevation
                        variant="contained"
                        color="inherit"
                        disabled={isLoading}
                    >
                        <Button
                            color={dateFilter === "day" ? "primary" : "inherit"}
                            onClick={() => setDateFilter("day")}
                        >
                            Dia
                        </Button>
                        <Button
                            color={
                                dateFilter === "week" ? "primary" : "inherit"
                            }
                            onClick={() => setDateFilter("week")}
                        >
                            Semana
                        </Button>
                        <Button
                            color={
                                dateFilter === "month" ? "primary" : "inherit"
                            }
                            onClick={() => setDateFilter("month")}
                        >
                            Mês
                        </Button>
                    </ButtonGroup>
                </Box>
                {indicatorBasket && (
                    <ResultProgress
                        indicatorName={indicatorBasket.indicator.name}
                        progress={indicatorBasket.result || 0}
                        goal={indicatorBasket.goal}
                        type={indicatorBasket.indicator.type}
                    />
                )}

                {isLoading ? (
                    <CircularProgress />
                ) : indicatorsResults.length > 0 ? (
                    indicatorsResults.map((item, index) => (
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
            </Stack>
        </Card>
    );
}
