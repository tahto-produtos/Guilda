import {
    Add,
    DangerousOutlined,
    FeedbackOutlined,
    FilterAlt,
    FilterAltOutlined,
    HomeOutlined,
    LinearScale,
    ListAltOutlined,
    PageviewOutlined,
    SendTimeExtensionOutlined,
} from "@mui/icons-material";
import {
    Autocomplete,
    Box,
    Breadcrumbs,
    Button,
    CircularProgress,
    Divider,
    Link,
    Stack,
    TextField,
    Typography,
    lighten,
    useTheme,
} from "@mui/material";
import { format } from "date-fns";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { QuizContext } from "src/contexts/quiz-provider/quiz.context";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { useDebounce, useLoadingState } from "src/hooks";
import {
    ListPeriodUseCase,
    ListSectorsAndSubsectrosUseCase,
    ListSectorsUseCase,
} from "src/modules";
import { ActionPlanTable } from "src/modules/escalation/fragments/action-plan-table";
import { PerformanceIndicator } from "src/modules/escalation/fragments/performance-indicator";
import { CreateAutoActionUseCase } from "src/modules/escalation/use-cases/create-auto-action.use-case";
import { CreateHistoryActionUseCase } from "src/modules/escalation/use-cases/create-history-action.use-case";
import { CreateStageActionUseCase } from "src/modules/escalation/use-cases/create-stage-action.use-case";
import {
    LoadActionEscalation,
    LoadLibraryActionEscalationUseCase,
} from "src/modules/escalation/use-cases/load-library-action-escalation";
import { CreateInfractionButton } from "src/modules/feedback/fragments/create-infraction-button";
import { InfractionsTable } from "src/modules/feedback/fragments/infractions-table";
import { ListGroupsNewUseCase } from "src/modules/groups/use-cases/list-groups-new";

import { ListHierarchiesEscalationUseCase } from "src/modules/hierarchies/use-cases/list-hierarchies-escalation.use-case";
import { ListIndicatorsUseCase } from "src/modules/indicators/use-cases";
import { Indicator, Period, Sector, SectorAndSubsector } from "src/typings";
import { GroupNew } from "src/typings/models/group-new.model";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { PaginationModel } from "src/typings/models/pagination.model";
import { getLayout } from "src/utils";
import { capitalizeText } from "src/utils/capitalizeText";

export default function CreateAutoActionView() {
    const { myUser } = useContext(UserInfoContext);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [searchText, setSearchText] = useState<string>("");
    const debouncedSearchText: string = useDebounce<string>(searchText, 400);
    const router = useRouter();
    const theme = useTheme();

    const [actionEscalations, setActionEscalations] = useState<
        LoadActionEscalation[]
    >([]);
    const [actionEscalationSelected, setActionEscalationSelected] =
        useState<LoadActionEscalation | null>(null);
    const [searchAction, setSearchAction] = useState<string>("");
    const [percentage, setPercentage] = useState<number>(0);
    const [tolerance, setTolerance] = useState<number>(0);

    const [hierarchies, setHierarchies] = useState<Hierarchie[]>([]);
    const [selectedHierarchie, setSelectedHierachie] = useState<Hierarchie>();

    const [sector, setSector] = useState<SectorAndSubsector | null>(null);
    const [sectors, setSectors] = useState<SectorAndSubsector[]>([]);
    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const debouncedSectorSearchTerm: string = useDebounce<string>(
        sectorsSearchValue,
        400
    );

    const [sectorSub, setSectorSub] = useState<SectorAndSubsector | null>(null);
    const [sectorsSub, setSectorsSub] = useState<SectorAndSubsector[]>([]);
    const [sectorsSearchValueSub, setSectorsSearchValueSub] =
        useState<string>("");
    const debouncedSectorSearchTermSub: string = useDebounce<string>(
        sectorsSearchValueSub,
        400
    );

    const [periods, setPeriods] = useState<Period[]>([]);
    const [selectedPeriods, setSelectedPeriods] = useState<Period | null>(null);

    const [numberStage, setNumberStage] = useState<number>(0);
    const [hierarchyStage, setHierarchyStage] = useState<Hierarchie | null>(null);
    const [selectedStages, setSelectedStage] = useState<{
        IDGDA_HIERARCHY: number[];
        NUMBER_STAGE: number;
        HIERARCHY: Hierarchie;
    }[]>([]);

    const [groups, setGroups] = useState<GroupNew[]>([]);
    const [selectedGroups, setSelectedGroups] = useState<GroupNew | null>(null);

    const [indicatorsSearchValue, setIndicatorsSearchValue] =
        useState<string>("");
    const [selectedIndicators, setSelectedIndicators] =
        useState<Indicator | null>(null);
    const [indicators, setIndicators] = useState<Indicator[]>([]);

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
            })
            .finally(() => {});
    };

    useEffect(() => {
        getIndicatorsList();
    }, [indicatorsSearchValue]);

    function getSectors() {
        startLoading();

        new ListSectorsAndSubsectrosUseCase()
            .handle({
                isSubsector: false,
                sector: sectorsSearchValue,
            })
            .then((data) => {
                setSectors(data);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }
    useEffect(() => {
        getSectors();
    }, [debouncedSectorSearchTerm]);

    function getSectorsSub() {
        startLoading();

        new ListSectorsAndSubsectrosUseCase()
            .handle({
                isSubsector: true,
                sector: sectorsSearchValue,
            })
            .then((data) => {
                setSectorsSub(data);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }
    useEffect(() => {
        getSectorsSub();
    }, [debouncedSectorSearchTermSub]);

    const getHierarchies = async () => {
        startLoading();

        await new ListHierarchiesEscalationUseCase()
            .handle({
                limit: 100,
                offset: 0,
            })
            .then((data) => {
                const hierarchiesResponse: Hierarchie[] = data.items;
                setHierarchies(hierarchiesResponse);
            })
            .catch(() => {
                toast.error("Falha ao carregar hierarquias.");
            })
            .finally(() => {
                finishLoading();
            });
    };

    useEffect(() => {
        getHierarchies();
    }, []);

    const getGroups = async (codCollaborator: number) => {
        startLoading();

        await new ListGroupsNewUseCase()
            .handle({
                codCollaborator,
            })
            .then((data) => {
                setGroups(data);
            })
            .catch(() => {})
            .finally(() => {
                finishLoading();
            });
    };

    useEffect(() => {
        if (myUser && myUser?.id) {
            getGroups(myUser?.id);
        }
    }, [myUser]);

    const getPeriods = async (codCollaborator: number) => {
        startLoading();

        await new ListPeriodUseCase()
            .handle({
                codCollaborator,
            })
            .then((data) => {
                setPeriods(data);
            })
            .catch(() => {
                toast.error("Falha ao carregar turnos.");
            })
            .finally(() => {
                finishLoading();
            });
    };

    useEffect(() => {
        if (myUser && myUser?.id) {
            getPeriods(myUser?.id);
        }
    }, [myUser]);

    async function handleCreate() {
        /* if (!selectedGroups || !selectedIndicators || !sector) */
        if (!selectedIndicators || !sector)
            return;
        startLoading();
        console.log('[LOG]', selectedStages);
        await new CreateAutoActionUseCase()
            .handle({
                IDGDA_INDICATOR: selectedIndicators?.id,
                IDGDA_SECTOR: sector.id,
                IDGDA_SUBSECTOR: sectorSub ? sectorSub.id : null,
                GROUP: selectedGroups ? selectedGroups.id : 0,
                PERCENTAGE_DETOUR: percentage,
                TOLERANCE_RANGE: tolerance,
                LIST_STAGES: selectedStages,
            })
            .then((data) => {
                toast.success("Criado com sucesso!");
            })
            .catch(() => {
                toast.error("Falha ao criar.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    return (
        <ContentCard sx={{ p: 0 }}>
            <Stack
                width={"100%"}
                height={"80px"}
                sx={{
                    borderTopLeftRadius: "16px",
                    borderTopRightRadius: "16px",
                }}
                bgcolor={theme.palette.secondary.main}
                pl={"80px"}
                justifyContent={"center"}
            >
                <Breadcrumbs
                    aria-label="breadcrumb"
                    sx={{
                        color: theme.palette.background.default,
                    }}
                >
                    <Link
                        underline="hover"
                        sx={{ display: "flex", alignItems: "center" }}
                        color={theme.palette.background.default}
                        href="/"
                    >
                        <HomeOutlined
                            sx={{
                                mr: 0.5,
                                color: theme.palette.background.default,
                            }}
                        />
                    </Link>
                    <Link
                        sx={{
                            display: "flex",
                            alignItems: "center",
                            cursor: "pointer",
                            textDecoration: "none",
                        }}
                        color={theme.palette.background.default}
                        onClick={() => router.push("/escalation")}
                    >
                        <Typography fontWeight={"400"}>Escalation</Typography>
                    </Link>
                    <Link
                        sx={{
                            display: "flex",
                            alignItems: "center",

                            textDecoration: "none",
                        }}
                        color={theme.palette.background.default}
                    >
                        <Typography fontWeight={"700"}>
                            Criação de ação automática
                        </Typography>
                    </Link>
                </Breadcrumbs>
            </Stack>
            <ContentArea sx={{ py: " 40px" }}>
                <Stack px={"40px"}>
                    <PageTitle
                        icon={<ListAltOutlined sx={{ fontSize: "40px" }} />}
                        title="Criação de ação automática"
                        loading={isLoading}
                    ></PageTitle>
                    <Divider />
                    <Stack mt={"40px"} direction={"row"} gap="16px">
                        <Autocomplete
                            value={sector}
                            placeholder={"Setor"}
                            limitTags={1}
                            disableClearable={false}
                            onChange={(e, value) => {
                                setSector(value);
                            }}
                            fullWidth
                            onInputChange={(e, text) =>
                                setSectorsSearchValue(text)
                            }
                            renderInput={(props) => (
                                <TextField
                                    {...props}
                                    variant="outlined"
                                    label="Setores"
                                    placeholder="Buscar"
                                />
                            )}
                            isOptionEqualToValue={(option, value) =>
                                option.name === value.name
                            }
                            getOptionLabel={(option) =>
                                `${option.id} - ${option.name}`
                            }
                            options={sectors}
                        />
                        <Autocomplete
                            value={sectorSub}
                            placeholder={"SubSetor"}
                            limitTags={1}
                            disableClearable={false}
                            onChange={(e, value) => {
                                setSectorSub(value);
                            }}
                            fullWidth
                            onInputChange={(e, text) =>
                                setSectorsSearchValueSub(text)
                            }
                            renderInput={(props) => (
                                <TextField
                                    {...props}
                                    variant="outlined"
                                    label="SubSetores"
                                    placeholder="Buscar"
                                />
                            )}
                            isOptionEqualToValue={(option, value) =>
                                option.name === value.name
                            }
                            getOptionLabel={(option) =>
                                `${option.id} - ${option.name}`
                            }
                            options={sectorsSub}
                        />
                    </Stack>
                    <Stack direction={"row"} gap="16px">
                        <Autocomplete
                            fullWidth
                            value={selectedGroups}
                            options={groups}
                            getOptionLabel={(option) => option.name}
                            onChange={(event, value) => {
                                setSelectedGroups(value);
                            }}
                            filterSelectedOptions
                            renderInput={(props) => (
                                <TextField {...props} label={"Grupos"} />
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
                            sx={{ m: 0 }}
                        />
                        <Autocomplete
                            fullWidth
                            value={selectedIndicators}
                            options={indicators}
                            getOptionLabel={(option) => option.name}
                            onChange={(event, value) => {
                                setSelectedIndicators(value);
                            }}
                            filterSelectedOptions
                            renderInput={(props) => (
                                <TextField {...props} label={"Indicador"} />
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
                            sx={{ m: 0 }}
                        />
                    </Stack>
                    <Stack
                        direction={"row"}
                        alignItems={"center"}
                        mt={"30px"}
                        gap={"30px"}
                    >
                        <TextField
                            onChange={(e) =>
                                setPercentage(parseInt(e.target.value))
                            }
                            value={percentage}
                            InputProps={{
                                type: "number",
                            }}
                            label="Percentual"
                            fullWidth
                        />
                        <TextField
                            onChange={(e) =>
                                setTolerance(parseFloat(e.target.value))
                            } 
                            value={tolerance}
                            InputProps={{
                                type: "number",
                            }}
                            label="Tolerancia"
                            fullWidth
                        />
                    </Stack>
                    <Divider sx={{ marginTop: "20px" }} />
                    <Stack direction={"row"} gap="16px">
                        <Typography 
                            sx={{ 
                                marginTop: "20px"
                            }}
                        >
                            Stages
                        </Typography>
                    </Stack>
                    <Stack 
                        direction={"row"}
                        alignItems={"center"}
                        mt={"30px"}
                        gap={"30px"}
                    >
                        <TextField
                            onChange={(e) => setNumberStage(parseInt(e.target.value))}
                            value={numberStage}
                            InputProps={{
                                type: "number",
                            }}
                            label="Número do Stage"
                            fullWidth
                        />
                        <Autocomplete
                            fullWidth
                            value={hierarchyStage}
                            options={hierarchies}
                            getOptionLabel={(option) => option.levelName}
                            onChange={(event, value) => {
                                setHierarchyStage(value);
                            }}
                            filterSelectedOptions
                            renderInput={(props) => (
                                <TextField {...props} label={"Hierarquias"} />
                            )}
                            renderOption={(props, option) => {
                                return (
                                <li {...props} key={option.id}>
                                    {option.levelName}
                                </li>
                                );
                            }}
                            isOptionEqualToValue={(option, value) =>
                                option.levelName === value.levelName
                            }
                            sx={{ m: 0 }}
                        />
                        <Button
                            variant="outlined"
                            onClick={() => {
                                if (!numberStage || !hierarchyStage) {
                                    return toast.warning(
                                        "Adicione um valor"
                                    );
                                }
                                
                                let arr = selectedStages.slice();
                                arr.push({
                                    IDGDA_HIERARCHY: [hierarchyStage.id],
                                    NUMBER_STAGE: numberStage,
                                    HIERARCHY: hierarchyStage,
                                });
                                setHierarchyStage(null);
                                setNumberStage(0);
                                setSelectedStage(arr);
                            }}
                        >
                            <Add />
                        </Button>
                    </Stack>
                    <Box
                        width={"100%"}
                        display={"flex"}
                        flexDirection={"column"}
                        gap={1}
                        mt={2}
                    >
                        {selectedStages.map((item, index) => (
                            <Box
                                sx={{
                                    display: "flex",
                                    alignItems: "center",
                                    width: "400px",
                                    justifyContent: "space-between",
                                }}
                                key={index}
                            >
                                <Box display={"flex"} gap={"5px"}>
                                    <Typography>
                                        {item.NUMBER_STAGE} 
                                    </Typography>
                                    {" - "}
                                    <Typography>
                                        {item.HIERARCHY.levelName}
                                    </Typography>
                                </Box>

                                <Button
                                    color="error"
                                    onClick={() => {
                                        let arr = selectedStages.slice();
                                        arr.splice(index, 1);
                                        setSelectedStage(arr);
                                    }}
                                >
                                    remover
                                </Button>
                            </Box>
                        ))}
                    </Box>
                    
                    <Stack direction={"row"} mt={"40px"}>
                        <Button variant="contained" onClick={handleCreate}>
                            Confirmar e criar
                        </Button>
                    </Stack>
                </Stack>
            </ContentArea>
        </ContentCard>
    );
}

CreateAutoActionView.getLayout = getLayout("private");
