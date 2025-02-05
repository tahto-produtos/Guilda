import Settings from "@mui/icons-material/Settings";
import {
    Autocomplete,
    Box,
    Button,
    CircularProgress,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListSectorsUseCase } from "src/modules";
import { BasketIndicatorSector } from "src/modules/indicators/use-cases/basketIndicatorSector.use-case";
import { ListBasketMetricsUseCase } from "src/modules/indicators/use-cases/get-basket-metrics.use-case";
import { UpdateBasketIndicatorSector } from "src/modules/indicators/use-cases/update-basket-indicator-sector.use-case";
import { UpdateBasketMetricsUseCase } from "src/modules/indicators/use-cases/update-basket-metrics.use-case";
import { Sector } from "src/typings";
import { getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";

interface BasketIndicator {
    codIndicator: string;
    nameIndicator: string;
    weightIndicator: string;
}

interface GroupMetric {
    groupId: string;
    metricMin: string;
}

export default function IndicatorBasket() {
    const { isLoading, startLoading, finishLoading } = useLoadingState();
    const { myPermissions } = useContext(PermissionsContext);

    const [basketIndicators, setBasketIndicators] = useState<BasketIndicator[]>(
        []
    );
    const [basketMetrics, setBasketMetrics] = useState<GroupMetric[]>([]);

    const [sector, setSector] = useState<Sector | null>(null);
    const [sectors, setSectors] = useState<Sector[]>([]);
    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const debouncedSectorSearchTerm: string = useDebounce<string>(
        sectorsSearchValue,
        400
    );

    const getBasketMetrics = async () => {
        new ListBasketMetricsUseCase()
            .handle()
            .then((data) => {
                setBasketMetrics(data);
            })
            .catch(() => {});
    };

    useEffect(() => {
        getBasketMetrics();
    }, []);

    const getBasketIndicatorSector = async () => {
        if (!sector) {
            return;
        }

        new BasketIndicatorSector()
            .handle(sector.id)
            .then((data) => {
                setBasketIndicators(data);
            })
            .catch(() => {});
    };

    useEffect(() => {
        sector && getBasketIndicatorSector();
    }, [sector]);

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

    function BasketIndicatorItem({ item }: { item: BasketIndicator }) {
        const [weight, setWeight] = useState<string>(item.weightIndicator);

        function handleChangeWeight(value: string) {
            setWeight(value);
        }

        function handleSave() {
            if (!sector) {
                return;
            }

            new UpdateBasketIndicatorSector()
                .handle(sector.id.toString(), item.codIndicator, weight)
                .then((data) => {
                    toast.success("Peso alterado com sucesso!");
                })
                .catch(() => {
                    toast.error("Erro ao alterar o peso");
                });
        }

        return (
            <Box
                p={2}
                border={"solid 1px #e1e1e1"}
                display={"flex"}
                justifyContent={"space-between"}
                alignItems={"center"}
            >
                <Typography variant="body2">
                    Indicador: {item.nameIndicator}
                </Typography>
                <Box display={"flex"} gap={2} alignItems={"center"}>
                    <TextField
                        value={weight}
                        size="small"
                        label="Peso"
                        onChange={(e) => handleChangeWeight(e.target.value)}
                    />
                    <Button
                        variant="contained"
                        disabled={weight === item.weightIndicator}
                        onClick={handleSave}
                    >
                        Salvar
                    </Button>
                </Box>
            </Box>
        );
    }

    function BasketMetricsItem({ item }: { item: GroupMetric }) {
        const [metricMin, setMetricMin] = useState<string>(item.metricMin);

        function handleChangeWeight(value: string) {
            setMetricMin(value);
        }

        function handleSave() {
            new UpdateBasketMetricsUseCase()
                .handle(item.groupId, metricMin)
                .then((data) => {
                    toast.success("Peso alterado com sucesso!");
                })
                .catch(() => {
                    toast.error("Erro ao alterar o peso");
                });
        }

        return (
            <Box
                p={2}
                border={"solid 1px #e1e1e1"}
                display={"flex"}
                justifyContent={"space-between"}
                alignItems={"center"}
            >
                <Typography variant="body2">Grupo: {item.groupId}</Typography>
                <Box display={"flex"} gap={2} alignItems={"center"}>
                    <TextField
                        value={metricMin}
                        size="small"
                        label="Min."
                        onChange={(e) => handleChangeWeight(e.target.value)}
                    />
                    {abilityFor(myPermissions).can(
                        "Configurar Cesta de Indicadores",
                        "Indicadores"
                    ) && (
                        <Button
                            variant="contained"
                            disabled={metricMin === item.metricMin}
                            onClick={handleSave}
                        >
                            Salvar
                        </Button>
                    )}
                </Box>
            </Box>
        );
    }

    if (
        abilityFor(myPermissions).cannot(
            "Ver Cesta de Indicadores",
            "Indicadores"
        )
    ) {
        return null;
    }

    return (
        <Box
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            gap={2}
            pb={10}
        >
            <Card width={"100%"} display={"flex"} flexDirection={"column"}>
                <PageHeader
                    title={"Cesta de indicadores"}
                    headerIcon={<Settings />}
                />
                <Stack px={2} py={3} width={"100%"} gap={1}>
                    <Autocomplete
                        value={sector}
                        placeholder={"Setor"}
                        disableClearable={false}
                        onChange={(e, value) => {
                            setSector(value);
                        }}
                        onInputChange={(e, text) => setSectorsSearchValue(text)}
                        sx={{
                            mb: 0,
                            maxWidth: "400px",
                            width: "100%",
                        }}
                        renderInput={(props) => (
                            <TextField
                                {...props}
                                size={"medium"}
                                label={"Setor"}
                                InputProps={{
                                    ...props.InputProps,
                                    endAdornment: (
                                        <>
                                            {isLoading ? (
                                                <CircularProgress
                                                    color="primary"
                                                    size={20}
                                                />
                                            ) : (
                                                props.InputProps.endAdornment
                                            )}
                                        </>
                                    ),
                                }}
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
                    <Box
                        display={"flex"}
                        flexDirection={"column"}
                        gap={2}
                        mt={2}
                    >
                        {basketIndicators.map((item, index) => (
                            <BasketIndicatorItem key={index} item={item} />
                        ))}
                    </Box>
                </Stack>
            </Card>
            <Card width={"100%"} display={"flex"} flexDirection={"column"}>
                <PageHeader
                    title={"Métricas da cesta"}
                    headerIcon={<Settings />}
                />
                <Stack px={2} py={3} width={"100%"} gap={1}>
                    <Box
                        display={"flex"}
                        flexDirection={"column"}
                        gap={2}
                        mt={2}
                    >
                        {basketMetrics.map((item, index) => (
                            <BasketMetricsItem key={index} item={item} />
                        ))}
                    </Box>
                </Stack>
            </Card>
        </Box>
    );
}

IndicatorBasket.getLayout = getLayout("private");
