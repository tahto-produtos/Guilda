import FileOpen from "@mui/icons-material/FileOpen";
import { Card, PageHeader } from "src/components";
import { getLayout } from "src/utils";
import {
    Autocomplete,
    Box,
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    Stack,
    TextField,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format } from "date-fns";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListGroupsUseCase } from "src/modules/groups";
import { ListHierarchiesUseCase } from "src/modules/hierarchies/use-cases/list-hierarchies.use-case";
import { Group, Indicator, Sector } from "src/typings";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { PaginationModel } from "src/typings/models/pagination.model";
import { LoadingButton } from "@mui/lab";
import { SheetBuilder } from "src/utils";
import { IndicatorsBySectorsUseCase } from "src/modules/home/use-cases/IndicatorsBySectors/IndicatorsBySectors.use-case";
import { SectorsByHierachyUseCase } from "src/modules/home/use-cases/SectorsByHierarchy/SectorsByHierarchy.use-case";
import { ExportResultsReportUseCase } from "src/modules/home/use-cases/export-results-report";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { guildaApiClient2 } from "src/services";

export default function ResultsReport() {
    const { myUser } = useContext(UserInfoContext);

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
            .catch(() => { });
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
            .catch(() => { });
    };

    useEffect(() => {
        getGroupsList();
    }, [debouncedGroupsSearchTerm]);

    const getSectorsList = async () => {
        if (!endDatePicker || !startDatePicker || !myUser) {
            return;
        }

        new SectorsByHierachyUseCase()
            .handle({
                codCollaborator: myUser.id,
                sector: sectorsSearchValue,
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
                setSectors(data);
            })
            .catch(() => { });
    };

    useEffect(() => {
        myUser && endDatePicker && startDatePicker && getSectorsList();
    }, [debouncedSectorSearchTerm, myUser, endDatePicker, startDatePicker]);

    var client = guildaApiClient2;
    const handleDownload = async (fileName: any) => {
        try {
            const response = await client.get(`/DownloadFileSAS?fileName=${fileName}`);
            const sasUrl = response.data;

            // Create an anchor element and simulate a click to download the file
            const link = document.createElement('a');
            link.href = sasUrl;
            link.download = fileName;
            link.target = '_blank'; // Ensure the link opens in a new tab/window to avoid navigation
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        } catch (error) {
            console.error('Error fetching SAS URL:', error);
        }
    };
    /* const downloadFile = async (fileName: any) => {
        try {

            const response = await client.get(`/DownloadFile?fileName=${fileName}`, {
                responseType: 'blob',  // Important for binary data
                headers: {
                    'Accept': 'application/octet-stream',
                }
            });

            const url = window.URL.createObjectURL(new Blob([response.data]));
            const a = document.createElement('a');
            a.href = url;
            a.download = fileName;
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
        } catch (error) {
            console.error('Failed to download file', error);
        }
    }; */

    async function handleReportExtract() {
        if (!myUser) return;

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
            order: orderBy,
            dataInicial: startDatePicker
                ? format(new Date(startDatePicker.toString()), "yyyy-MM-dd")
                : undefined,
            dataFinal: endDatePicker
                ? format(new Date(endDatePicker.toString()), "yyyy-MM-dd")
                : undefined,
            CollaboratorId: myUser.id,
        };

        new ExportResultsReportUseCase()
            .handle(payload)
            .then((data) => {
                if (data.length <= 0) {
                    return toast.warning("Sem dados para exportar.");
                }

                if (data[0].TypeFile == "TXT") {
                    handleDownload(data[0].FileName);
                    return toast.success("Download de TXT colocado na fila!");
                }

                const docRows = data.map((item: any) => {
                    return [
                        item.Data
                            ? `${item.Data.split(" ")[0].split("/")[1] <= 9
                                ? `0${item.Data.split(" ")[0].split(
                                    "/"
                                )[1]
                                }`
                                : item.Data.split(" ")[0].split("/")[1]
                            }/${item.Data.split(" ")[0].split("/")[0]}/${item.Data.split(" ")[0].split("/")[2]
                            }`
                            : "",
                        item.Ano,
                        item.Mes,

                        { v: item.CodigoGIP, t: "n" },
                        item.Setor,

                        { v: item.CodigoGIPSubsetor, t: "n" },
                        item.Subsetor,

                        { v: item.CodigoIndicador, t: "n" },
                        item.NomeIndicador,

                        /*                         item.Resultado && !isNaN(parseFloat(item.Resultado))
                                                    ? `${item.TipoIndicador === "INTEGER" || item.TipoIndicador === "HOUR"
                                                        ? { v: item.Resultado.replace(",", "."), t: "n" } // Se INTEGER ou HOUR, exporta como número inteiro
                                                        : parseFloat(item.Resultado.replace(",", ".")).toFixed(2) // Se decimal, mantém a precisão
                                                    }${item.TipoIndicador === "PERCENT" ? "%" : ""}` // Adiciona o "%" apenas na string, não no valor exportado
                                                    : "", */
                        item.Resultado && !isNaN(parseInt(item.Resultado))
                            ? `${item.TipoIndicador == "INTEGER" || item.TipoIndicador == "HOUR"
                                ? item.Resultado.replace(",", ".")
                                : parseFloat(
                                    item.Resultado.replace(",", ".")
                                )
                                    .toFixed(2)
                                    .toString()
                                    .replace(".", ",")
                            }${item.TipoIndicador == "PERCENT" ? "%" : ""}`
                            : "",
                        `${item.Meta}${item.Meta &&
                            item.TipoIndicador == "PERCENT" &&
                            item.Meta !== "-"
                            ? "%"
                            : ""
                        }`,
                        item.PercentualDeAtingimento &&
                            !isNaN(parseInt(item.PercentualDeAtingimento))
                            ? `${parseFloat(item.PercentualDeAtingimento)
                                .toFixed(2)
                                .toString()
                                .replace(".", ",")}%`
                            : `${item.PercentualDeAtingimento}${item.PercentualDeAtingimento == "-" ? "" : "%"
                            }`,
                        /* item.Resultado && !isNaN(parseInt(item.Resultado))
                            ? `${item.TipoIndicador == "INTEGER" || item.TipoIndicador == "HOUR"
                                ? item.Resultado
                                : parseFloat(
                                    item.Resultado.replace(",", ".")
                                )
                                    .toFixed(2)
                                    .toString()
                                    .replace(".", ",")
                            }${item.TipoIndicador == "PERCENT" ? "%" : ""}`
                            : "",
                        `${item.Meta}${item.Meta &&
                            item.TipoIndicador == "PERCENT" &&
                            item.Meta !== "-"
                            ? "%"
                            : ""
                        }`,
                        item.PercentualDeAtingimento &&
                            !isNaN(parseInt(item.PercentualDeAtingimento))
                            ? `${parseFloat(item.PercentualDeAtingimento)
                                .toFixed(2)
                                .toString()
                                .replace(".", ",")}%`
                            : `${item.PercentualDeAtingimento}${item.PercentualDeAtingimento == "-" ? "" : "%"
                            }`, */
                        item.Grupo,
                        item.Cargo,

                        { v: item.MatriculaDoColaborador, t: "n" },
                        item.NomeAgente,

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
                        // item.MatriculaCEO,-----
                        // item.NomeCEO,----
                        item.StatusHome,
                        item.Uf,
                        item.TurnoDoAgente,
                        `${item.Score.toString().replace(".", ",")}%`,
                    ];
                });
                let indicatorSheetBuilder = new SheetBuilder();
                indicatorSheetBuilder
                    .setHeader([
                        "Data",
                        "Ano",
                        "Mes",
                        "Codigo GIP",
                        "Setor",
                        "Código GIP Subsetor",
                        "Subsetor",
                        "Codigo Indicador",
                        "Nome Indicador",
                        "Resultado",
                        "Meta",
                        "Percentual De Atingimento",
                        "Grupo",
                        "Cargo",
                        "Matricula Do Colaborador",
                        "Nome Agente",
                        "Matricula Supervisor",
                        "Nome Supervisor",
                        "Matricula Coordenador",
                        "Nome Coordenador",
                        "Matricula Gerente 2",
                        "Nome Gerente 2",
                        "MatriculaGerente1",
                        "Nome Gerente 1",
                        "Matricula Diretor",
                        "Nome Diretor",
                        // "MatriculaCEO",
                        // "NomeCEO",
                        "Status Home",
                        "Uf",
                        "Turno Do Agente",
                        "Score",
                    ])
                    .append(docRows)
                    .exportAs(`Relatório_resultados`);
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
        <Box display="flex" flexDirection={"column"} width={"100%"}>
            <Card width={"100%"} display={"flex"} flexDirection={"column"}>
                <PageHeader
                    title={"Relatório de resultados"}
                    headerIcon={<FileOpen />}
                />
                <Stack px={2} py={4} width={"100%"} gap={2}>
                    <Box
                        width={"100%"}
                        display={"flex"}
                        flexDirection={"column"}
                    >
                        <Box
                            display={"flex"}
                            flexDirection={"row"}
                            gap={"10px"}
                            mb={2}
                        >
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
                                    onChange={(newValue) =>
                                        setEndDatePicker(newValue)
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
                        </Box>
                        <Autocomplete
                            multiple
                            size={"small"}
                            options={sectors}
                            getOptionLabel={(option) => option.name}
                            onChange={(event, value) => {
                                setSector(value);
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
                        <Autocomplete
                            multiple
                            size={"small"}
                            options={groups}
                            getOptionLabel={(option) => option.name}
                            onChange={(event, value) => {
                                setGroup(value);
                            }}
                            onInputChange={(e, text) =>
                                setGroupsSearchValue(text)
                            }
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
                            onInputChange={(e, text) =>
                                setIndicatorSearchValue(text)
                            }
                            filterOptions={(options, { inputValue }) =>
                                options.filter((item) =>
                                    item.name
                                        .toLocaleLowerCase()
                                        .includes(
                                            inputValue.toLocaleLowerCase()
                                        )
                                )
                            }
                            filterSelectedOptions
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
                            options={hierarchies}
                            getOptionLabel={(option) => option.levelName}
                            onChange={(event, value) => {
                                setHierarchie(value);
                            }}
                            onInputChange={(e, text) =>
                                setHierarchiesSearchValue(text)
                            }
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
                </Stack>
            </Card>
        </Box>
    );
}

ResultsReport.getLayout = getLayout("private");
