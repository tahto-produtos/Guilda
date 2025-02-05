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
import {ExportDayLoggedsReportUseCase} from "../../../modules/home-v2/use-cases/export-day-loggeds-report.use-case";

export default function DaysLoggedReport() {
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

    const [hierarchie, setHierarchie] = useState<Hierarchie[]>([]);
    const [hierarchiesSearchValue, setHierarchiesSearchValue] =
        useState<string>("");
    const [hierarchies, setHierarchies] = useState<Hierarchie[]>([]);
    const debouncedHierarchiesSearchTerm: string = useDebounce<string>(
        hierarchiesSearchValue,
        400
    );

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
            .catch(() => {});
    };

    useEffect(() => {
        getHierarchiesList();
    }, [debouncedHierarchiesSearchTerm]);

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
            .catch(() => {});
    };

    useEffect(() => {
        myUser && endDatePicker && startDatePicker && getSectorsList();
    }, [debouncedSectorSearchTerm, myUser, endDatePicker, startDatePicker]);

    async function handleReportExtract() {
        if (!myUser) return;

        startLoading();

        const payload = {
            sectors: sector.map((item) => {
                return item.id;
            }),
            hierarchies: hierarchie.map((item) => {
                return item.id;
            }),
            dataInicial: startDatePicker
                ? format(new Date(startDatePicker.toString()), "yyyy-MM-dd")
                : undefined,
            dataFinal: endDatePicker
                ? format(new Date(endDatePicker.toString()), "yyyy-MM-dd")
                : undefined,
            CollaboratorId: myUser.id,
        };


        new ExportDayLoggedsReportUseCase()
            .handle(payload)
            .then((data) => {
                if (data.length <= 0) {
                    return toast.warning("Sem dados para exportar.");
                }
                const docRows = data.map((item: any) => {
                    return [
                        item.DATA_ESCALA,
                        item.DATA_ACESSO,
                        item.DATA_ATUALIZACAO,
                        { v: item.BC, t: "n" },
                        item.NOME,
                        item.CARGO,
                        item.ESCALA,
                        item.ACESSO,
                        { v: item.COD_INDICADOR, t: "n" },

                        item.INDICADOR,
                        item.TEMPO_LOGADO,
                        { v: item.MATRICULA_SUPERVISOR, t: "n" },

                        item.NOME_SUPERVISOR,
                        { v: item.MATRICULA_COORDENADOR, t: "n" },

                        item.NOME_COORDENADOR,
                        { v: item.MATRICULA_GERENTE2, t: "n" },

                        item.NOME_GERENTE2,
                        { v: item.MATRICULA_GERENTE1, t: "n" },

                        item.NOME_GERENTE1,
                        { v: item.MATRICULA_DIRETOR, t: "n" },

                        item.NOME_DIRETOR,
                        { v: item.MATRICULA_CEO, t: "n" },

                        item.NOME_CEO,
                        { v: item.CODGIP, t: "n" },

                        item.SETOR,
                        { v: item.CODGIP_SUBSETOR, t: "n" },

                        item.SUBSETOR,
                        item.TURNO,
                        item.SITE,
                        item.HOME_BASED,
                        item.CLIENTE
                    ];
                });
                let dayLoggedSheetBuilder = new SheetBuilder();
                dayLoggedSheetBuilder
                    .setHeader([
                        "Data Escala",
                        "Data Acesso",
                        "Data última atualização",
                        "BC",
                        "Nome",
                        "Cargo",
                        "Escala",
                        "Acesso",
                        "Código Indicador",
                        "Indicador",
                        "Tempo Logado",
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
                        "Turno",
                        "Site",
                        "Home",
                        "Cliente"
                    ])
                    .append(docRows)
                    .exportAs(`Relatório_tempo_logado`);
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
                    title={"Relatório de tempo logado"}
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

DaysLoggedReport.getLayout = getLayout("private");
