import Download from "@mui/icons-material/Download";
import ImportExport from "@mui/icons-material/ImportExport";
import { LoadingButton } from "@mui/lab";
import {
    Autocomplete,
    Button,
    InputAdornment,
    TextField,
    Tooltip,
    Typography,
} from "@mui/material";
import { Box, Stack } from "@mui/system";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { useLoadingState } from "src/hooks";
import { DetailsSectorUseCase, ListSectorsUseCase } from "src/modules";
import {
    ConnectSectorsToIndicators,
    DetailsIndicatorUseCase,
    ListIndicatorsUseCase,
} from "src/modules/indicators/use-cases";
import { ConnectIndicatorstoSector } from "src/modules/indicators/use-cases/connect-indicators-to-sector.use-case";
import { RemoveAssociatedSectorsUseCase } from "src/modules/indicators/use-cases/remove-associated-sectors.use-case";
import { ReportSectorIndicatorConnectUseCase } from "src/modules/indicators/use-cases/report-sector-indicator-connect";
import { Indicator, Sector } from "src/typings";
import GoalsBySectorModel from "src/typings/models/goals-by-sector.model";
import { PaginationModel } from "src/typings/models/pagination.model";
import { DateUtils, SheetBuilder, getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";

interface SectorGoalModel {
    sector: string;
    sectorId: Number;
    goal: string | Number;
}

export default function ConnectIndicators() {
    const { myPermissions } = useContext(PermissionsContext);
    const { isLoading, startLoading, finishLoading } = useLoadingState();
    const [indicators, setIndicators] = useState<Indicator[]>([]);
    const [sectors, setSectors] = useState<Sector[]>([]);
    const [selectedSector, setSelectedSector] = useState<number | null>(null);
    const [indicatorsSearchValue, setIndicatorsSearchValue] =
        useState<string>("");
    const [selectedIndicators, setSelectedIndicators] = useState<Indicator[]>(
        []
    );
    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const [goals, setGoals] = useState<SectorGoalModel[]>([]);
    const [sectorDetails, setSectorDetails] = useState<Sector | null>(null);
    const [historySelectedSectors, setHistorySelectedSectors] = useState<
        Indicator[]
    >([]);
    const [loading, setLoading] = useState<boolean>(false);
    const [modalExportReportIsOpen, setModalExportReportIsOpen] =
        useState<boolean>(false);
    const [reportLimit, setReportLimit] = useState<number>(1000);

    const router = useRouter();

    const getIndicatorsList = async () => {
        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: indicatorsSearchValue,
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

    // const handleRemoveAssociatedSector = (sectorId: number) => {
    //     if (sectorId && selectedIndicators) {
    //         const payload = {
    //             indicatorId: selectedIndicators,
    //             sectorId: sectorId,
    //         };

    //         new RemoveAssociatedSectorsUseCase()
    //             .handle(payload)
    //             .then((data) => {
    //                 toast.success("Associação removida com sucesso!");
    //             })
    //             .catch(() => {
    //                 toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
    //             });
    //     }
    // };

    const exportReport = async () => {
        setLoading(true);
        const payload = {
            limit: reportLimit,
            offset: 0,
        };
        await new ReportSectorIndicatorConnectUseCase()
            .handle(payload)
            .then((data) => {
                console.log(data);
                const docRows = data.items.map((item: any) => {
                    return [
                        item.indicator,
                        item.startedAt
                            ? DateUtils.formatDatePtBR(item.startedAt)
                            : "",
                        item.endedAt
                            ? DateUtils.formatDatePtBR(item.endedAt)
                            : "",
                        item.goal,
                        item.sector,
                        item.ative,
                        item.groupOne,
                        item.groupTwo,
                        item.groupThree,
                        item.groupFour,
                    ];
                });
                let indicatorSheetBuilder = new SheetBuilder();
                indicatorSheetBuilder
                    .setHeader([
                        "Indicador",
                        "DATA_INICIO",
                        "DATA_FIM",
                        "VALOR_META",
                        "ATRIBUTOS",
                        "ATIVO",
                        "GANHO_G1",
                        "GANHO_G2",
                        "GANHO_G3",
                        "GANHO_G4",
                    ])
                    .append(docRows)
                    .exportAs("Relatório Associação de indicador por setor");
                toast.success("Relatório exportado com sucesso!");
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                setLoading(false);
            });
    };

    const getSectorsList = async () => {
        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: sectorsSearchValue,
        };

        new ListSectorsUseCase()
            .handle(pagination)
            .then((data) => {
                setSectors(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    };

    useEffect(() => {
        getIndicatorsList();
    }, [indicatorsSearchValue]);

    useEffect(() => {
        getSectorsList();
    }, [sectorsSearchValue]);

    const handleConnectSectorsToIndicator = () => {
        if (!selectedSector) {
            return toast.warning("Selecione um setor");
        }

        if (selectedIndicators.length <= 0) {
            return toast.warning("Selecione pelo menos um indicador");
        }

        // if (goals.find((item) => item.goal === "")) {
        //     return toast.warning("Defina a meta de todos os setores");
        // }

        startLoading();

        // const goalsBySector: GoalsBySectorModel[] = goals.map((item) => {
        //     return {
        //         sectorId: item.sectorId,
        //         goal: item.goal,
        //     };
        // });

        new ConnectIndicatorstoSector()
            .handle({
                sectorId: selectedSector,
                indicatorsIds: selectedIndicators.map((sector) => sector.id),
            })
            .then((data) => {
                toast.success(`Setores associados com sucesso!`);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    const GoalListItem = (props: {
        sectorGoal: SectorGoalModel;
        getGoalValue: any;
    }) => {
        const { sectorGoal, getGoalValue } = props;
        const [goal, setGoal] = useState<string>("");

        return (
            <Box>
                <TextField
                    label={sectorGoal.sector}
                    sx={{ width: "100%" }}
                    type={"number"}
                    value={goal}
                    onChange={(e) => {
                        setGoal(e.target.value);
                        getGoalValue({
                            sector: sectorGoal.sector,
                            sectorId: sectorGoal.sectorId,
                            goal: parseInt(e.target.value),
                        });
                    }}
                    InputProps={{
                        startAdornment: (
                            <InputAdornment position="start">
                                <Typography fontSize={12}>meta</Typography>
                            </InputAdornment>
                        ),
                    }}
                />
            </Box>
        );
    };

    useEffect(() => {
        const arr = selectedIndicators.map((indicator) => {
            return {
                sector: indicator.name,
                sectorId: indicator.id,
                goal: "",
            };
        });
        setGoals(arr);
    }, [selectedIndicators]);

    const handleChangeGoal = (sectorGoal: SectorGoalModel) => {
        let arr = goals;
        const index = arr.findIndex(
            (obj) => obj.sectorId == sectorGoal.sectorId
        );
        arr[index].goal = sectorGoal.goal;
        setGoals(arr);
    };

    useEffect(() => {
        if (selectedSector) {
            const payload = {
                id: selectedSector.toString(),
            };

            new DetailsSectorUseCase()
                .handle(payload)
                .then((data) => {
                    setSectorDetails(data);
                    console.log(data);
                    data.indicators &&
                        setHistorySelectedSectors(data.indicators);
                    data.indicators && setSelectedIndicators(data.indicators);
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                });
        }
    }, [selectedSector]);

    return (
        <Box display={"flex"} width={"100%"} flexDirection={"column"} gap={2}>
            <Card width={"100%"} display={"flex"} flexDirection={"column"}>
                <PageHeader
                    title={"Associar indicadores a um setor"}
                    headerIcon={<ImportExport />}
                    secondayButtonAction={() =>
                        setModalExportReportIsOpen(true)
                    }
                    secondaryButtonTitle={"Exportar Relatório"}
                    secondaryButtonIcon={<Download />}
                />
                <Stack width={"100%"}>
                    <Box
                        sx={{
                            display: "flex",
                            flexDirection: "column",
                        }}
                    >
                        <Autocomplete
                            options={sectors}
                            disabled={abilityFor(myPermissions).cannot(
                                "Editar Indicadores",
                                "Indicadores"
                            )}
                            disableClearable={false}
                            size={"small"}
                            getOptionLabel={(option) => option.name}
                            onChange={(event, value) => {
                                value && setSelectedSector(value.id);
                            }}
                            onInputChange={(e, text) =>
                                setSectorsSearchValue(text)
                            }
                            filterOptions={(x) => x}
                            renderInput={(params) => (
                                <TextField
                                    {...params}
                                    variant="outlined"
                                    label="Selecione um setor"
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
                        <Autocomplete
                            multiple
                            disabled={abilityFor(myPermissions).cannot(
                                "Editar Indicadores",
                                "Indicadores"
                            )}
                            size={"small"}
                            value={selectedIndicators}
                            options={indicators}
                            getOptionLabel={(option) => option.name}
                            onChange={(event, value) => {
                                setSelectedIndicators(value);
                            }}
                            onInputChange={(e, text) =>
                                setIndicatorsSearchValue(text)
                            }
                            filterOptions={(x) => x}
                            filterSelectedOptions
                            renderInput={(params) => (
                                <TextField
                                    {...params}
                                    variant="outlined"
                                    label="Selecione um ou mais indicadores "
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
                </Stack>
            </Card>
            {/* {goals.length > 0 && (
                <Card display={"flex"} flexDirection={"column"}>
                    <PageHeader
                        title={"Defina as metas para cada setor"}
                        headerIcon={<Flag />}
                    />
                    <Stack width={"100%"} direction={"column"} gap={2}>
                        {goals.map((sectorGoal, index) => (
                            <GoalListItem
                                sectorGoal={sectorGoal}
                                key={index}
                                getGoalValue={(x: any) => handleChangeGoal(x)}
                            />
                        ))}
                    </Stack>
                </Card>
            )} */}
            {/* {historySelectedSectors && historySelectedSectors.length > 0 && (
                <Card width={"100%"} display={"flex"} flexDirection={"column"}>
                    <PageHeader
                        title={"Setores já associados a este indicador"}
                    />
                    <Box
                        sx={{
                            display: "flex",
                            flexDirection: "column",
                            gap: "10px",
                            p: 1,
                        }}
                    >
                        {historySelectedSectors.map((item, index) => (
                            <Card
                                key={index}
                                p={"10px"}
                                borderRadius={"10px"}
                                display={"flex"}
                                justifyContent={"space-between"}
                            >
                                {item.name}
                                <Button
                                    color="error"
                                    onClick={() =>
                                        handleRemoveAssociatedSector(item.id)
                                    }
                                >
                                    Remover associação
                                </Button>
                            </Card>
                        ))}
                    </Box>
                </Card>
            )} */}
            <Box display={"flex"} justifyContent={"flex-end"}>
                <Tooltip
                    title="Você não tem permissão para realizar esta ação."
                    disableHoverListener={abilityFor(myPermissions).can(
                        "Editar Indicadores",
                        "Indicadores"
                    )}
                    arrow
                >
                    <span>
                        <LoadingButton
                            loading={isLoading}
                            variant={"contained"}
                            onClick={handleConnectSectorsToIndicator}
                            disabled={abilityFor(myPermissions).cannot(
                                "Editar Indicadores",
                                "Indicadores"
                            )}
                        >
                            Associar Setores
                        </LoadingButton>
                    </span>
                </Tooltip>
            </Box>
            <BaseModal
                width={"540px"}
                open={modalExportReportIsOpen}
                title={"Exportar relatório"}
                onClose={() => setModalExportReportIsOpen(false)}
            >
                <Box width={"100%"}>
                    <Box display={"flex"} gap={2} width={"100%"}>
                        <TextField
                            label={"Limite de linhas"}
                            value={reportLimit}
                            type="number"
                            fullWidth
                            onChange={(e) => {
                                if (e.target.value === "") {
                                    setReportLimit(0);
                                } else {
                                    setReportLimit(parseInt(e.target.value));
                                }
                            }}
                        />
                    </Box>
                    <Button
                        variant="contained"
                        onClick={exportReport}
                        fullWidth
                        sx={{ mt: 2 }}
                    >
                        Exportar relatório
                    </Button>
                </Box>
            </BaseModal>
        </Box>
    );
}

ConnectIndicators.getLayout = getLayout("private");
