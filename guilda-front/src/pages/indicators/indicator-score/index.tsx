import FileOpen from "@mui/icons-material/FileOpen";
import { Autocomplete, Button, TextField, Typography } from "@mui/material";
import { Box } from "@mui/system";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListSectorsUseCase } from "src/modules";
import { ListIndicatorsUseCase } from "src/modules/indicators/use-cases";
import { ListIndicatorsByScoreUseCase } from "src/modules/indicators/use-cases/ListIndicatorsByScore.use-case";
import { ListScoreBySector } from "src/modules/indicators/use-cases/ListScoreBySectors.use-case";
import { ScoreInputUseCase } from "src/modules/indicators/use-cases/ScoreInput.use-case";
import { Indicator, Sector } from "src/typings";
import { PaginationModel } from "src/typings/models/pagination.model";
import { SheetBuilder, getLayout } from "src/utils";

interface IndicatorScore {
    indicatorId: number;
    name: string;
    Score: string;
}

export default function IndicatorScoreView() {
    const { isLoading, startLoading, finishLoading } = useLoadingState();
    const { myUser } = useContext(UserInfoContext);

    const [isAddNewOpen, setIsAddNewOpen] = useState<boolean>(false);
    const [newScoreValue, setNewScoreValue] = useState<number>(0);

    const [sector, setSector] = useState<Sector | null>(null);
    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const [sectors, setSectors] = useState<Sector[]>([]);
    const debouncedSectorSearchTerm: string = useDebounce<string>(
        sectorsSearchValue,
        400
    );

    const [indicator, setIndicator] = useState<Indicator | null>(null);
    const [indicatorSearchValue, setIndicatorSearchValue] =
        useState<string>("");
    const [indicators, setIndicators] = useState<Indicator[]>([]);
    const debouncedIndicatorsSearchTerm: string = useDebounce<string>(
        indicatorSearchValue,
        400
    );

    const [reportLoading, setReportLoading] = useState<boolean>(false);

    const [newScores, setNewScores] = useState<
        { id: number; score: number; name?: string }[]
    >([]);

    const [indicatorScores, setIndicatorScores] = useState<IndicatorScore[]>(
        []
    );

    function handleExportReport() {
        setReportLoading(true);

        new ListScoreBySector()
            .handle()
            .then((data) => {
                const docRows = data.map((item: any) => {
                    return [
                        item.CODGIP,
                        item.SETOR,
                        item.CODINDICADOR,
                        item.INDICADOR,
                        item.SCORE,
                    ];
                });
                let indicatorSheetBuilder = new SheetBuilder();
                indicatorSheetBuilder
                    .setHeader([
                        "CodGip",
                        "Setor",
                        "CodIndicador",
                        "Indicador",
                        "Score",
                    ])
                    .append(docRows)
                    .exportAs(`Relatório de Score dos indicadores`);
                toast.success("Relatório exportado com sucesso!");
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                setReportLoading(false);
            });
    }

    const getIndicatorsList = async () => {
        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: indicatorSearchValue,
        };

        new ListIndicatorsUseCase()
            .handle(pagination)
            .then((data) => {
                setIndicators(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    };

    useEffect(() => {
        getIndicatorsList();
    }, [debouncedIndicatorsSearchTerm]);

    const getSectorsList = async () => {
        const pagination = {
            limit: 20,
            offset: 0,
            searchText: sectorsSearchValue,
        };

        new ListSectorsUseCase()
            .handle(pagination)
            .then((data) => {
                setSectors(data.items);
            })
            .catch(() => {});
    };

    useEffect(() => {
        getSectorsList();
    }, [debouncedSectorSearchTerm]);

    async function getIndicatorsByScore() {
        if (!sector) {
            return;
        }

        new ListIndicatorsByScoreUseCase()
            .handle({ SECTORID: sector.id })
            .then((data) => {
                setIndicatorScores(data);
            })
            .catch(() => {});
    }

    useEffect(() => {
        sector && getIndicatorsByScore();
    }, [sector]);

    function handleSave() {
        if (!sector || !myUser) {
            return;
        }

        function mountIndicators() {
            let arr: { id: number; score: number }[] = [];
            // if (newScores.length > 0) {
            //     arr = newScores.map((item) => {
            //         return { id: item.id, score: item.score };
            //     });
            // }

            const mapIndicatorsScores = indicatorScores.map((item) => {
                return {
                    id: item.indicatorId,
                    score: parseFloat(item.Score) * 100,
                };
            });

            arr = [...arr, ...mapIndicatorsScores];

            return arr;
        }

        new ScoreInputUseCase()
            .handle({
                Indicators: mountIndicators(),
                Matricula: myUser.id,
                Sectorid: sector.id,
            })
            .then((data) => {
                toast.success(data);
            })
            .catch((e) => {
                toast.error(e?.response?.data?.Message || "Error");
            });
    }

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Score dos indicadores"}
                headerIcon={<FileOpen />}
                addButtonAction={handleExportReport}
                addButtonTitle="Exportar relatório"
            />
            <Box
                display={"flex"}
                justifyContent={"flex-end"}
                px={2}
                py={3}
                flexDirection={"column"}
                gap={2}
            >
                <Box width={"100%"}>
                    <Autocomplete
                        size={"small"}
                        options={sectors}
                        getOptionLabel={(option) => option.name}
                        onChange={(event, value) => {
                            setSector(value);
                        }}
                        onInputChange={(e, text) => setSectorsSearchValue(text)}
                        filterOptions={(x) => x}
                        filterSelectedOptions
                        renderInput={(params) => (
                            <TextField
                                {...params}
                                variant="outlined"
                                label="Selecione um setores"
                                placeholder="Buscar"
                            />
                        )}
                        renderOption={(props, option) => {
                            return (
                                <li {...props} key={option.id}>
                                    {option.name}
                                </li>
                            );
                        }}
                        isOptionEqualToValue={(option, value) =>
                            option.name === value.name
                        }
                    />
                </Box>
                <Box
                    width={"100%"}
                    display={"flex"}
                    flexDirection={"column"}
                    gap={1}
                >
                    {indicatorScores.map((score, index) => {
                        return (
                            <Box key={index} display={"flex"} gap={1}>
                                <TextField
                                    size="small"
                                    value={score.name}
                                    disabled={true}
                                />
                                <TextField
                                    size="small"
                                    label="Score"
                                    value={(
                                        parseFloat(score.Score) * 100
                                    ).toFixed()}
                                    onChange={(e) => {
                                        const indexOf =
                                            indicatorScores.findIndex(
                                                (item) =>
                                                    item.indicatorId ==
                                                    score.indicatorId
                                            );
                                        const arr = [...indicatorScores];
                                        if (!e.target.value) {
                                            arr[indexOf].Score = (0).toString();
                                            setIndicatorScores(arr);
                                            return;
                                        }
                                        arr[indexOf].Score = (
                                            parseInt(e.target.value) / 100
                                        ).toString();
                                        setIndicatorScores(arr);
                                    }}
                                />
                                {/* <Button
                                    variant="outlined"
                                    color="error"
                                    onClick={() => {
                                        const arr = indicatorScores.filter(
                                            (item) =>
                                                item.indicatorId !==
                                                score.indicatorId
                                        );
                                        setIndicatorScores(arr);
                                    }}
                                >
                                    Remover
                                </Button> */}
                            </Box>
                        );
                    })}
                </Box>
                {isAddNewOpen && (
                    <Box width={"100%"}>
                        <Box
                            p={2}
                            border={`solid 1px #e1e1e1`}
                            borderRadius={2}
                            display={"flex"}
                            flexDirection={"column"}
                            gap={2}
                        >
                            <Typography variant="body2" fontWeight={"600"}>
                                Novo indicador
                            </Typography>
                            {/* <Box
                                width={"100%"}
                                display={"flex"}
                                flexDirection={"column"}
                                gap={1}
                            >
                                {newScores.map((score, index) => {
                                    return (
                                        <Box
                                            key={index}
                                            display={"flex"}
                                            gap={1}
                                        >
                                            <TextField
                                                size="small"
                                                value={score.name}
                                                disabled={true}
                                            />
                                            <TextField
                                                size="small"
                                                label="Score"
                                                value={score.score}
                                                disabled={true}
                                            />
                                            <Button
                                                variant="outlined"
                                                color="error"
                                                onClick={() => {
                                                    const arr =
                                                        newScores.filter(
                                                            (item) =>
                                                                item.id !==
                                                                score.id
                                                        );
                                                    setNewScores(arr);
                                                }}
                                            >
                                                Remover
                                            </Button>
                                        </Box>
                                    );
                                })}
                            </Box> */}
                            <Box
                                width={"100%"}
                                display={"flex"}
                                alignItems={"center"}
                                gap={1}
                            >
                                <Autocomplete
                                    size={"small"}
                                    options={indicators}
                                    sx={{
                                        maxWidth: "300px",
                                        width: "100%",
                                        mb: "0",
                                    }}
                                    getOptionLabel={(option) => option.name}
                                    onChange={(event, value) => {
                                        setIndicator(value);
                                    }}
                                    onInputChange={(e, text) =>
                                        setIndicatorSearchValue(text)
                                    }
                                    filterOptions={(x) => x}
                                    filterSelectedOptions
                                    disableClearable={false}
                                    renderInput={(params) => (
                                        <TextField
                                            {...params}
                                            variant="outlined"
                                            label="Selecione um indicador"
                                            placeholder="Buscar"
                                        />
                                    )}
                                    renderOption={(props, option) => {
                                        return (
                                            <li {...props} key={option.id}>
                                                {option.name}
                                            </li>
                                        );
                                    }}
                                    isOptionEqualToValue={(option, value) =>
                                        option.name === value.name
                                    }
                                />
                                <TextField
                                    size="small"
                                    label="Score"
                                    value={newScoreValue}
                                    onChange={(e) =>
                                        setNewScoreValue(
                                            parseInt(e.target.value) || 0
                                        )
                                    }
                                />
                                <Button
                                    variant="outlined"
                                    disabled={!indicator}
                                    onClick={() => {
                                        if (!indicator) {
                                            return;
                                        }
                                        if (
                                            indicatorScores.find(
                                                (item) =>
                                                    item.indicatorId ==
                                                    indicator.id
                                            )
                                        ) {
                                            return toast.warning(
                                                "Este indicador já foi adicionado"
                                            );
                                        }
                                        const arr = [
                                            ...indicatorScores,
                                            {
                                                indicatorId: indicator.id,
                                                Score: (
                                                    newScoreValue / 100
                                                ).toString(),
                                                name: indicator.name,
                                            },
                                        ];
                                        setIndicatorScores(arr);
                                    }}
                                >
                                    Salvar
                                </Button>
                            </Box>
                        </Box>
                    </Box>
                )}
                <Box display={"flex"} gap={1}>
                    <Button
                        variant="outlined"
                        size="small"
                        onClick={() => setIsAddNewOpen(true)}
                    >
                        Adicionar indicador
                    </Button>
                    <Button
                        variant="contained"
                        size="small"
                        onClick={handleSave}
                    >
                        Salvar Alterações
                    </Button>
                </Box>
            </Box>
        </Card>
    );
}

IndicatorScoreView.getLayout = getLayout("private");
