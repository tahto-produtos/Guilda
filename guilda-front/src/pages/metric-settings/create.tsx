import { useContext, useEffect, useState } from "react";
import {
    Box,
    Divider,
    InputAdornment,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { LoadingButton } from "@mui/lab";
import { toast } from "react-toastify";

import { getLayout } from "../../utils";
import { useLoadingState } from "../../hooks";
import { Card, PageHeader } from "../../components";
import {
    CreateGroupSettingsForm,
    SectorAndIndicatorForm,
    SectorAndIndicatorSchema,
} from "../../modules/groups/forms";
import {
    Group,
    HistoryIndicatorSector,
    Indicator,
    Sector,
} from "../../typings";
import {
    CopyMetricsModal,
    CreateMetricsUseCase,
    GetLastGroupSettingsUseCase,
    ListGroupsUseCase,
} from "../../modules";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "../../constants";
import { PaginationModel } from "src/typings/models/pagination.model";
import { MetricMinMaxSchema } from "src/modules/groups/forms/metric-min-max";
import { grey } from "@mui/material/colors";
import { ConnectSectorsToIndicators } from "src/modules/indicators/use-cases";
import GroupMonetizationSettings from "src/modules/groups/forms/group-monetization-settings/group-monetization-settings";
import { Can } from "@casl/react";
import abilityFor from "src/utils/ability-for";
import PermDataSetting from "@mui/icons-material/PermDataSetting";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

interface GoalsByGroupModel {
    groupId: number;
    metricMin: number;
    metricMax: number;
}

export default function Settings() {
    const { isLoading, startLoading, finishLoading } = useLoadingState();
    const { myPermissions } = useContext(PermissionsContext);
    const [modalCopyMetricsVisible, setModalCopyMetricsVisible] =
        useState<boolean>(false);
    const [groups, setGroups] = useState<Group[]>([]);
    const [historyIsFetched, setHistoryIsFetched] = useState(false);
    const [historyIndicatorSector, setHistoryIndicatorSector] = useState<
        HistoryIndicatorSector[]
    >([]);
    const [sectorAndIndicator, setSectorAndIndicator] =
        useState<SectorAndIndicatorSchema>();
    const [myPeriod, setPeriod] = useState<Number | string>("");
    const [goal, setGoal] = useState<Number | string>("");
    const [isAssociated, setIsAssociated] = useState<boolean>(false);
    const isFormDisabled =
        (!historyIsFetched && historyIndicatorSector?.length === 0) ||
        !isAssociated;
    const [goalsByGroup, setGoalsByGroup] = useState<GoalsByGroupModel[]>([]);
    const [monetizationByGroup, setMonetizationByGroup] = useState<
        Array<{ group: number; value: number }>
    >([]);

    useEffect(() => {
        const pagination: PaginationModel = {
            limit: 10,
            offset: 0,
        };

        new ListGroupsUseCase()
            .handle(pagination)
            .then((data) => setGroups(data.items))
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }, []);

    const onSelectSectorAndIndicator = async (
        sectorAndIndicatorSchema: SectorAndIndicatorSchema
    ) => {

        const { sector, indicator, period } = sectorAndIndicatorSchema;
        if (sector == null && indicator == null && period == "") {
            setHistoryIndicatorSector([]);
            setGoal("");
            return;
        }
 
        new GetLastGroupSettingsUseCase()
            .handle({
                sectorId: sector?.id || undefined,
                indicatorId: indicator!.id,
                period,
            })
            .then((data) => {
                setHistoryIndicatorSector(data.groups);
                setGoal(data.goal !== undefined ? data.goal : "");
                setIsAssociated(true);
            })
            .catch((e) => {
                if (e.response.data.code == "ASSOCIATION_NOT_FOUND") {
                    toast.error(
                        `Associação entre o setor ${`(${sector?.name})`} e o indicador ${`(${indicator?.name})`} não encontrada`
                    );
                } else {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                }
            });
        period && setPeriod(period);
        setSectorAndIndicator(sectorAndIndicatorSchema);
        setHistoryIsFetched(true);
    };

    const handleMetricsChange = (group: Group, metrics: MetricMinMaxSchema) => {

        const { min, max } = metrics;
        console.log("a");
        console.log(min);
        console.log(max);
        let newMetrics = goalsByGroup.slice();
        const index = newMetrics.findIndex(
            (obj: any) => obj.groupId === group.id
        );

        if (index < 0) {
            const newGoal = {
                groupId: group.id,
                metricMin: min,
                metricMax: 0, // Inicialmente, podes deixar o `max` como `null` ou outro valor
            };

            newMetrics.push(newGoal);
        }
        else {
            newMetrics[index].metricMin = min;
            newMetrics[index].metricMax = max;


        }


        setGoalsByGroup(newMetrics);
    };

    const handleCreateMetrics = async (
        sector?: number,
        indicators?: number[],
    ) => {
        if (!sectorAndIndicator) {
            return toast.warning("Escolha um setor e um indicador");
        }

        if (!sectorAndIndicator) {
            return toast.warning("Escolha um setor e um indicador");
        }

        if (myPeriod == "" || myPeriod == undefined) {
            return toast.warning("Defina um período");
        }

        if (
            goalsByGroup.find(
                (item) => item.metricMin == undefined
                // && item.metricMax == undefined
            )
        ) {
            return toast.warning("Defina todas as metas por grupo");
        }

        if (monetizationByGroup.find((item) => Number.isNaN(item.value))) {
            return toast.warning("Defina todas as monetizações");
        }
        console.log(monetizationByGroup);
        const payload = {
            sectorId: sector
                ? sector
                : sectorAndIndicator?.sector?.id || undefined,
            indicatorsIds: indicators
                ? indicators
                : [sectorAndIndicator!.indicator!.id],
            period: myPeriod ? myPeriod : undefined,
            metricSettings: goalsByGroup.map((metric) => ({
                groupId: metric.groupId,
                metricMin: metric.metricMin,
                // metricMax: metric.metricMax,
                monetization: monetizationByGroup.find(
                    (item) => item.group === metric.groupId
                )?.value,
                goal: goal,
            })),
        };
        console.log(payload);
        try {
            startLoading();
            await new CreateMetricsUseCase().handle(payload).then((data) => {
                toast.success("Métricas atualizadas com sucesso!");
            });
            //await handleChangeGoal();
        } catch (e: any) {
            if (e.response.data.code == "ASSOCIATION_NOT_FOUND") {
                toast.warning(
                    "Os indicadores em amarelo não estão associados ao setor selecionado"
                );
            } else {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            }
        } finally {
            finishLoading();
        }
    };

    const handleChangeGoal = async () => {
        if (sectorAndIndicator?.indicator && sectorAndIndicator.sector) {
            new ConnectSectorsToIndicators()
                .handle({
                    indicatorId: sectorAndIndicator.indicator.id,
                    sectorsIds: [sectorAndIndicator.sector.id],
                    goalsBySector: [
                        {
                            sectorId: sectorAndIndicator.sector.id,
                            goal: goal.toString(),
                        },
                    ],
                    period: myPeriod ? myPeriod : undefined,
                })
                .then((data) => {
                    toast.success("Meta atualizada com sucesso!");
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
        if (historyIndicatorSector) {
            const goals = historyIndicatorSector.map((item) => {
                return {
                    groupId: item.id,
                    metricMin: item.metrics?.metricMin,
                    metricMax: item.metrics?.metricMax,
                };
            });
            setGoalsByGroup(goals);

            const monetizationHistory = historyIndicatorSector.map((item) => {
                return {
                    group: item.id,
                    value: item.metrics?.monetization,
                };
            });
            setMonetizationByGroup(monetizationHistory);
        }
    }, [historyIndicatorSector]);

    const getMonetizationValues = (
        group: Group,
        monetizationValue: Number | string
    ) => {
        console.log("monetization");
        const arr = monetizationByGroup.slice();
        const value = parseInt(monetizationValue.toString());
        console.log(value);
        if (arr.find((item) => item.group === group.id)) {
            const objIndex = arr.findIndex((obj) => obj.group == group.id);
            arr[objIndex].value = value;
            setMonetizationByGroup(arr);
        } else {
            arr.push({ group: group.id, value: value });
            setMonetizationByGroup(arr);
        }
    };

    function handleCopyMetrics(values: {
        sector: Sector;
        indicators: Indicator[];
    }) {
        //const sectorId = values.sector.id;
        //const indicatorsIds = values.indicators.map((item) => item.id);
        //handleCreateMetrics(sectorId, indicatorsIds);
    }

    return (
        <Card display={"flex"} width={"100%"} flexDirection={"column"}>
            <PageHeader
                title={"Criar Configuração de métricas"}
                headerIcon={<PermDataSetting />}
                addButtonAction={() => setModalCopyMetricsVisible(true)}
                addButtonTitle="Copiar métricas"
                addButtonIsDisabled={!sectorAndIndicator}
            />

            <Stack width={"100%"}>
                <Box>
                    <Can
                        I="Ver Metricas"
                        a="Metricas"
                        ability={abilityFor(myPermissions)}
                    >
                        <>
                            <Box>
                                <SectorAndIndicatorForm
                                    id={"select-sector-and-indicator"}
                                    onSubmit={onSelectSectorAndIndicator}
                                />
                            </Box>
                            <Box>
                                <TextField
                                    label={"Meta"}
                                    sx={{ width: "100%" }}
                                    type={"number"}
                                    value={goal}
                                    onChange={(e) => {
                                        setGoal(e.target.value);
                                    }}
                                    disabled={isFormDisabled}
                                    inputProps={{
                                        step: 0.01,
                                    }}
                                    InputProps={{
                                        startAdornment: (
                                            <InputAdornment position="start">
                                                <Typography fontSize={12}>
                                                    meta
                                                </Typography>
                                            </InputAdornment>
                                        ),
                                    }}
                                />
                            </Box>
                            <Divider sx={{ mt: "20px", mb: "20px" }} />
                            <Typography
                                fontSize={16}
                                fontWeight={400}
                                mb={3}
                                mt={1}
                                color={isFormDisabled ? grey[500] : "black"}
                            >
                                Atingimento por grupo
                            </Typography>
                            <CreateGroupSettingsForm
                                onMetricsChange={handleMetricsChange}
                                disabled={isFormDisabled}
                                metrics={historyIndicatorSector}
                                groups={groups}
                            />
                            <Divider sx={{ mt: "20px", mb: "20px" }} />
                            <Typography
                                fontSize={16}
                                fontWeight={400}
                                mb={3}
                                mt={1}
                                color={isFormDisabled ? grey[500] : "black"}
                            >
                                Monetização por grupo
                            </Typography>
                            <GroupMonetizationSettings
                                groups={groups}
                                disabled={isFormDisabled}
                                getMonetizationValues={(
                                    group,
                                    monetizationValue
                                ) =>
                                    getMonetizationValues(
                                        group,
                                        monetizationValue
                                    )
                                }
                                historyIndicatorSector={historyIndicatorSector}
                            />
                        </>
                    </Can>
                </Box>
                <Can
                    I="Editar Metricas"
                    a="Metricas"
                    ability={abilityFor(myPermissions)}
                >
                    <Box display={"flex"} justifyContent={"flex-end"}>
                        <LoadingButton
                            loading={isLoading}
                            variant={"contained"}
                            onClick={() => handleCreateMetrics()}
                        >
                            Salvar
                        </LoadingButton>
                    </Box>
                </Can>
            </Stack>
            <CopyMetricsModal
                open={modalCopyMetricsVisible}
                onConfirm={(values: any) => handleCopyMetrics(values)}
                onClose={() => setModalCopyMetricsVisible(false)}
            />
        </Card>
    );
}

Settings.getLayout = getLayout("private");
