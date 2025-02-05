import {
    Autocomplete,
    Box,
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    TextField,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format } from "date-fns";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListGroupsUseCase } from "src/modules/groups";
import { ListHierarchiesUseCase } from "src/modules/hierarchies/use-cases/list-hierarchies.use-case";
import { ListIndicatorsUseCase } from "src/modules/indicators/use-cases";
import { ListSectorsUseCase } from "src/modules/sectors";
import { guildaApiClient2 } from "src/services";
import { Group, Indicator, Sector } from "src/typings";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { PaginationModel } from "src/typings/models/pagination.model";
import { LoadingButton } from "@mui/lab";
import { SheetBuilder } from "src/utils";
import { ExportResultsReportUseCase } from "src/modules/home/use-cases/export-results-report";
import { ExportMonetizationAdmReportUseCase } from "../use-cases/export-report-adm-monetization.use-case";
import { ListCollaboratorsAllUseCase } from "src/modules/collaborators/use-cases/list-collaborators.use-case";
import { IndicatorsBySectorsUseCase } from "src/modules/home/use-cases/IndicatorsBySectors/IndicatorsBySectors.use-case";

interface ModalExportResultsProps {
    open: boolean;
    onClose: () => void;
}

export function ModalMonetizationReportAdm(props: ModalExportResultsProps) {
    const { onClose, open } = props;

    const { finishLoading, isLoading, startLoading } = useLoadingState();

    const [startDatePicker, setStartDatePicker] = useState<
        dateFns | Date | null
    >(null);
    const [endDatePicker, setEndDatePicker] = useState<dateFns | Date | null>(
        null
    );

    const [sector, setSector] = useState<Sector[]>([]);
    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const [sectors, setSectors] = useState<Sector[]>([]);
    const debouncedSectorSearchTerm: string = useDebounce<string>(
        sectorsSearchValue,
        400
    );

    const [group, setGroup] = useState<Group[]>([]);
    const [groupsSearchValue, setGroupsSearchValue] = useState<string>("");
    const [groups, setGroups] = useState<Group[]>([]);
    const debouncedGroupsSearchTerm: string = useDebounce<string>(
        groupsSearchValue,
        400
    );

    const [indicator, setIndicator] = useState<Indicator[]>([]);
    const [indicatorSearchValue, setIndicatorSearchValue] =
        useState<string>("");
    const [indicators, setIndicators] = useState<Indicator[]>([]);
    const debouncedIndicatorsSearchTerm: string = useDebounce<string>(
        indicatorSearchValue,
        400
    );

    const [hierarchie, setHierarchie] = useState<Hierarchie[]>([]);
    const [hierarchiesSearchValue, setHierarchiesSearchValue] =
        useState<string>("");
    const [hierarchies, setHierarchies] = useState<Hierarchie[]>([]);
    const debouncedHierarchiesSearchTerm: string = useDebounce<string>(
        hierarchiesSearchValue,
        400
    );

    const [orderBy, setOrderBy] = useState<string>("Melhor");

    const [collaborator, setCollaborator] = useState<
        {
            id: number;
        }[]
    >([]);
    const [collaboratorsSearchValue, setCollaboratorsSearchValue] =
        useState<string>("");
    const [collaborators, setCollaborators] = useState<
        {
            id: number;
            name: string;
            registry: string;
        }[]
    >([]);
    const debouncedCollaboratorsSearchTerm: string = useDebounce<string>(
        collaboratorsSearchValue,
        400
    );

    const getCollaboratorsList = async () => {
        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: collaboratorsSearchValue,
        };

        new ListCollaboratorsAllUseCase()
            .handle(pagination)
            .then((data) => {
                setCollaborators(data.items);
            })
            .catch(() => { });
    };

    useEffect(() => {
        getCollaboratorsList();
    }, [debouncedCollaboratorsSearchTerm]);

    const getHierarchiesList = async () => {
        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: hierarchiesSearchValue,
        };

        new ListHierarchiesUseCase()
            .handle(pagination)
            .then((data) => {
                setHierarchies(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    };

    useEffect(() => {
        getHierarchiesList();
    }, [debouncedHierarchiesSearchTerm]);

    const getIndicatorsList = async () => {
        if (!endDatePicker || !startDatePicker) {
            return;
        }

        new IndicatorsBySectorsUseCase()
            .handle({
                sectors: sector.map((item) => {
                    return {
                        id: item.id,
                    };
                }),
                monetize: true,
                dtInicial: format(
                    new Date(startDatePicker.toString()),
                    "yyyy-MM-dd"
                ),
                dtfinal: format(
                    new Date(endDatePicker.toString()),
                    "yyyy-MM-dd"
                ),
            })
            .then((data) => {
                setIndicators(data);
            })
            .catch(() => { });
    };

    useEffect(() => {
        sector.length > 0 && getIndicatorsList();
    }, [debouncedIndicatorsSearchTerm, sector]);

    const getGroupsList = async () => {
        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: groupsSearchValue,
        };

        new ListGroupsUseCase()
            .handle(pagination)
            .then((data) => {
                setGroups(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    };

    useEffect(() => {
        getGroupsList();
    }, [debouncedGroupsSearchTerm]);

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

    function formatServerDate(date: string) {
        const data = date.split("T")[0];
        const day = data.split("-")[2];
        const month = data.split("-")[1];
        const year = data.split("-")[0];

        return `${day}/${month}/${year}`;
    }

    useEffect(() => {
        getSectorsList();
    }, [debouncedSectorSearchTerm]);

    async function handleReportExtract() {
        if (!startDatePicker || !endDatePicker) {
            return toast.warning("Escolha as datas");
        }

        startLoading();

        const payload = {
            sectors: sector.map((item) => {
                return { id: item.id };
            }),
            groups: group.map((item) => {
                return { id: item.id };
            }),
            indicators: indicator.map((item) => {
                return { id: item.id };
            }),
            hierarchies: hierarchie.map((item) => {
                return { id: item.id };
            }),
            collaborators: collaborator.map((item) => {
                return { id: item.id };
            }),
            order: orderBy,
            dataInicial: format(
                new Date(startDatePicker.toString()),
                "yyyy-MM-dd"
            ),
            dataFinal: format(new Date(endDatePicker.toString()), "yyyy-MM-dd"),
        };

        new ExportMonetizationAdmReportUseCase()
            .handle(payload)
            .then((data) => {
                if (data.length <= 0) {
                    return toast.warning("Sem dados para exportar.");
                }
                const docRows = data.map((item: any) => {
                    return [
                        item.Mes.split(" ")[0].split("/")[0],
                        item.Ano,

                        { v: item.Matricula, t: "n" },
                        item.NomeColaborador,
                        item.Cargo,
                        item.DiasTrabalhados,
                        { v: item.Indicador, t: "n" },

                        item.NomeIndicador,
                        `${item.Meta}${item.Meta &&
                            item.TipoIndicador == "PERCENT" &&
                            item.Meta !== "-"
                            ? "%"
                            : ""
                        }`,
                        item.Resultado && !isNaN(parseInt(item.Resultado))
                            ? `${item.TipoIndicador == "INTEGER"
                                ? item.Resultado
                                : parseFloat(
                                    item.Resultado.replace(",", ".")
                                )
                                    .toFixed(2)
                                    .toString()
                                    .replace(".", ",")
                            }${item.TipoIndicador == "PERCENT" ? "%" : ""}`
                            : "",
                        typeof item.Percentual == "number" &&
                            !isNaN(item.Percentual)
                            ? `${item.Percentual.toFixed(2).replace(".", ",")}%`
                            : "",

                        { v: item.GanhoEmMoedas, t: "n" },
                        item.MetaMaximaMoedas
                            ? parseFloat(item.MetaMaximaMoedas)
                            : "",
                        item.Grupo,
                        item.DataAtualizacao
                            ? formatServerDate(item.DataAtualizacao)
                            : "",
                        { v: item.MatriculaSupervisor, t: "n" },

                        item.NomeSupervisor,
                        { v: item.MatriculaCoordenador, t: "n" },

                        item.NomeCoordenador,
                        { v: item.MatriculaGerente2, t: "n" },

                        item.NomeGerente2,
                        { v: item.MatriculaGerente1, t: "n" },

                        item.NomeGerente1,
                        { v: item.MatriculaDiretor, t: "n" },

                        item.NomeDiretor,
                        { v: item.MatriculaCEO, t: "n" },

                        item.NomeCEO,
                        { v: item.CodigoGIP, t: "n" },

                        item.Setor,
                        { v: item.CodigoGIPSubsetor, t: "n" },

                        item.Subsetor,
                        item.Home,
                        item.Turno,
                        item.Site,
                        `${item.Score.toString().replace(".", ",")}%`,
                    ];
                });
                let indicatorSheetBuilder = new SheetBuilder();
                indicatorSheetBuilder
                    .setHeader([
                        "Mes",
                        "Ano",
                        "Matricula",
                        "Nome do colaborador",
                        "Cargo",
                        "Dias trabalhados",
                        "Indicador",
                        "Nome do indicador",
                        "Meta",
                        "Resultado",
                        "Percentual",
                        "Ganho em Moedas",
                        "Meta máxima de moedas",
                        "Grupo",
                        "Data de atualização",
                        "Matrícula Supervisor",
                        "Nome Supervisor",
                        "Matrícula Coordenador",
                        "Nome Coordenador",
                        "Matrícula Gerente 2",
                        "Nome Gerente 2",
                        "Matrícula Gerente 1",
                        "Nome Gerente 1",
                        "Matrícula Diretor",
                        "Nome Diretor",
                        "Matrícula CEO",
                        "Nome CEO",
                        "Código GIP",
                        "Setor",
                        "Código GIP Subsetor",
                        "Subsetor",
                        "Home",
                        "Turno",
                        "Site",
                        "Score",
                    ])
                    .append(docRows)
                    .exportAs(`Relatório Analítico Monetização Consolidado`);
                toast.success("Relatório exportado com sucesso!");
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    return (
        <BaseModal
            width={"540px"}
            open={open}
            title={`Relatório Analítico Monetização Consolidado`}
            onClose={onClose}
        >
            <Box width={"100%"} display={"flex"} flexDirection={"column"}>
                <Box display={"flex"} flexDirection={"row"} gap={"10px"} mb={2}>
                    <LocalizationProvider dateAdapter={AdapterDateFns}>
                        <DatePicker
                            label="De"
                            value={startDatePicker}
                            onChange={(newValue) =>
                                setStartDatePicker(newValue)
                            }
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
                            value={endDatePicker}
                            onChange={(newValue) => setEndDatePicker(newValue)}
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
                <Autocomplete
                    multiple
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
                <Autocomplete
                    multiple
                    size={"small"}
                    options={groups}
                    getOptionLabel={(option) => option.name}
                    onChange={(event, value) => {
                        setGroup(value);
                    }}
                    onInputChange={(e, text) => setGroupsSearchValue(text)}
                    filterOptions={(x) => x}
                    filterSelectedOptions
                    renderInput={(params) => (
                        <TextField
                            {...params}
                            variant="outlined"
                            label="Selecione um ou mais grupos"
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
                    size={"small"}
                    options={indicators}
                    getOptionLabel={(option) => option.name}
                    onChange={(event, value) => {
                        setIndicator(value);
                    }}
                    onInputChange={(e, text) => setIndicatorSearchValue(text)}
                    renderInput={(params) => (
                        <TextField
                            {...params}
                            variant="outlined"
                            label="Selecione um ou mais indicadores"
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
                    size={"small"}
                    options={collaborators}
                    getOptionLabel={(option) => option.name}
                    onChange={(event, value) => {
                        setCollaborator(value as { id: number }[]);
                    }}
                    onInputChange={(e, text) =>
                        setCollaboratorsSearchValue(text)
                    }
                    filterOptions={(x) => x}
                    filterSelectedOptions
                    renderInput={(params) => (
                        <TextField
                            {...params}
                            variant="outlined"
                            label="Selecione um ou mais colaboradores"
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
                    size={"small"}
                    options={hierarchies}
                    getOptionLabel={(option) => option.levelName}
                    onChange={(event, value) => {
                        setHierarchie(value);
                    }}
                    onInputChange={(e, text) => setHierarchiesSearchValue(text)}
                    filterOptions={(x) => x}
                    filterSelectedOptions
                    renderInput={(params) => (
                        <TextField
                            {...params}
                            variant="outlined"
                            label="Selecione uma ou mais hierarquias"
                            placeholder="Buscar"
                        />
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
                />
                <FormControl fullWidth>
                    <InputLabel
                        size="small"
                        sx={{ background: "#fff", px: "5px" }}
                    >
                        Agrupar resultados por:
                    </InputLabel>
                    <Select
                        size="small"
                        value={orderBy}
                        onChange={(e) => setOrderBy(e.target.value)}
                    >
                        <MenuItem value={"Melhor"}>Melhor</MenuItem>
                        <MenuItem value={"Pior"}>Pior</MenuItem>
                    </Select>
                </FormControl>
                <LoadingButton
                    variant="contained"
                    onClick={handleReportExtract}
                    sx={{ mt: 2 }}
                    loading={isLoading}
                >
                    {isLoading ? "Aguarde..." : "Gerar Relatório"}
                </LoadingButton>
            </Box>
        </BaseModal>
    );
}
