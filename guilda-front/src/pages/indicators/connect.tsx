import Flag from "@mui/icons-material/Flag";
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
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { useLoadingState } from "src/hooks";
import { ListSectorsUseCase } from "src/modules";
import {
    ConnectSectorsToIndicators,
    DetailsIndicatorUseCase,
    ListIndicatorsUseCase,
} from "src/modules/indicators/use-cases";
import { RemoveAssociatedSectorsUseCase } from "src/modules/indicators/use-cases/remove-associated-sectors.use-case";
import { Indicator, Sector } from "src/typings";
import GoalsBySectorModel from "src/typings/models/goals-by-sector.model";
import { PaginationModel } from "src/typings/models/pagination.model";
import { getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";

interface SectorGoalModel {
    sector: string;
    sectorId: Number;
    goal: string | Number;
}

export default function Connect() {
    const { myPermissions } = useContext(PermissionsContext);
    const { isLoading, startLoading, finishLoading } = useLoadingState();
    const [indicators, setIndicators] = useState<Indicator[]>([]);
    const [sectors, setSectors] = useState<Sector[]>([]);
    const [selectedIndicators, setSelectedIndicators] = useState<number | null>(
        null
    );
    const [indicatorsSearchValue, setIndicatorsSearchValue] =
        useState<string>("");
    const [selectedSectors, setSelectedSectors] = useState<Sector[]>([]);
    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const [goals, setGoals] = useState<SectorGoalModel[]>([]);
    const [indicatorDetails, setIndicatorDetails] = useState<Indicator | null>(
        null
    );
    const [historySelectedSectors, setHistorySelectedSectors] = useState<
        Sector[]
    >([]);

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

    const handleRemoveAssociatedSector = (sectorId: number) => {
        if (sectorId && selectedIndicators) {
            const payload = {
                indicatorId: selectedIndicators,
                sectorId: sectorId,
            };

            new RemoveAssociatedSectorsUseCase()
                .handle(payload)
                .then((data) => {
                    toast.success("Associação removida com sucesso!");
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                });
        }
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
        if (!selectedIndicators) {
            return toast.warning("Selecione um indicador");
        }

        if (selectedSectors.length <= 0) {
            return toast.warning("Selecione pelo menos um setor");
        }

        if (goals.find((item) => item.goal === "")) {
            return toast.warning("Defina a meta de todos os setores");
        }

        startLoading();

        const goalsBySector: GoalsBySectorModel[] = goals.map((item) => {
            return {
                sectorId: item.sectorId,
                goal: item.goal,
            };
        });

        new ConnectSectorsToIndicators()
            .handle({
                indicatorId: selectedIndicators,
                sectorsIds: selectedSectors.map((sector) => sector.id),
                goalsBySector: goalsBySector,
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
        const arr = selectedSectors.map((sector) => {
            return {
                sector: sector.name,
                sectorId: sector.id,
                goal: "",
            };
        });
        setGoals(arr);
    }, [selectedSectors]);

    const handleChangeGoal = (sectorGoal: SectorGoalModel) => {
        let arr = goals;
        const index = arr.findIndex(
            (obj) => obj.sectorId == sectorGoal.sectorId
        );
        arr[index].goal = sectorGoal.goal;
        setGoals(arr);
    };

    useEffect(() => {
        if (selectedIndicators) {
            const payload = {
                id: selectedIndicators,
            };

            new DetailsIndicatorUseCase()
                .handle(payload)
                .then((data) => {
                    setIndicatorDetails(data);
                    setHistorySelectedSectors(data.sectors);
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                });
        }
    }, [selectedIndicators]);

    return (
        <Box display={"flex"} width={"100%"} flexDirection={"column"} gap={2}>
            <Card width={"100%"} display={"flex"} flexDirection={"column"}>
                <PageHeader
                    title={"Associar setores a um indicador"}
                    headerIcon={<ImportExport />}
                />
                <Stack width={"100%"}>
                    <Box
                        sx={{
                            display: "flex",
                            flexDirection: "column",
                        }}
                    >
                        <Autocomplete
                            options={indicators}
                            disabled={abilityFor(myPermissions).cannot(
                                "Editar Indicadores",
                                "Indicadores"
                            )}
                            disableClearable={false}
                            size={"small"}
                            getOptionLabel={(option) => option.name}
                            onChange={(event, value) => {
                                value && setSelectedIndicators(value.id);
                            }}
                            onInputChange={(e, text) =>
                                setIndicatorsSearchValue(text)
                            }
                            filterOptions={(x) => x}
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
                        <Autocomplete
                            multiple
                            disabled={abilityFor(myPermissions).cannot(
                                "Editar Indicadores",
                                "Indicadores"
                            )}
                            size={"small"}
                            options={sectors}
                            getOptionLabel={(option) => option.name}
                            onChange={(event, value) => {
                                setSelectedSectors(value);
                            }}
                            onInputChange={(e, text) =>
                                setSectorsSearchValue(text)
                            }
                            filterOptions={(x) => x}
                            filterSelectedOptions
                            renderInput={(params) => (
                                <TextField
                                    {...params}
                                    variant="outlined"
                                    label="Selecione um ou mais setores"
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
            {goals.length > 0 && (
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
            )}
            {historySelectedSectors && historySelectedSectors.length > 0 && (
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
            )}
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
        </Box>
    );
}

Connect.getLayout = getLayout("private");
