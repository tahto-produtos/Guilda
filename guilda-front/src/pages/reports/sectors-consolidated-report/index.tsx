import FileOpen from "@mui/icons-material/FileOpen";
import { LoadingButton } from "@mui/lab";
import {
    Autocomplete,
    Box,
    Checkbox,
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    TextField,
    Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format } from "date-fns";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListSectorsUseCase } from "src/modules";
import { ListIndicatorsUseCase } from "src/modules/indicators/use-cases";
import { ReportConsolidatedSectorUseCase } from "src/modules/sectors/use-cases/ReportConsolidatedSector.use-case";
import { Indicator, Sector } from "src/typings";
import { PaginationModel } from "src/typings/models/pagination.model";
import { SheetBuilder, getLayout } from "src/utils";

export default function SectorsConsolidatedReport() {
    const { isLoading, startLoading, finishLoading } = useLoadingState();

    const [startDatePicker, setStartDatePicker] = useState<
        dateFns | Date | null
    >(null);
    const [endDatePicker, setEndDatePicker] = useState<dateFns | Date | null>(
        null
    );

    const [order, setOrder] = useState<"Melhor" | "Pior">("Melhor");

    const [sector, setSector] = useState<Sector[]>([]);
    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const [sectors, setSectors] = useState<Sector[]>([]);
    const debouncedSectorSearchTerm: string = useDebounce<string>(
        sectorsSearchValue,
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

    const [consolidado, setConsolidado] = useState<boolean>(false);

    const handleCheckboxOnChange = (e: any) => {
        if (e.target.checked === true) {
            setConsolidado(true);
        } else {
            setConsolidado(false);
        }
    };

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

    async function handleReportExtract() {
        if (!startDatePicker || !endDatePicker) {
            return toast.warning("Escolha as datas");
        }

        startLoading();

        const payload = {
            sectors: sector.map((item) => {
                return { id: item.id };
            }),
            indicators: indicator.map((item) => {
                return { id: item.id };
            }),
            order: order,
            consolidado: !consolidado,
            dataInicial: format(
                new Date(startDatePicker.toString()),
                "yyyy-MM-dd"
            ),
            dataFinal: format(new Date(endDatePicker.toString()), "yyyy-MM-dd"),
        };

        new ReportConsolidatedSectorUseCase()
            .handle(payload)
            .then((data) => {
                if (data.length <= 0) {
                    return toast.warning("Sem dados para exportar.");
                }
                const docRows = data.map((item: any) => {
                    return [
                        item.Mes,
                        item.Ano,
                        item.DataReferencia
                            ? `${
                                  item.DataReferencia.split(" ")[0].split(
                                      "/"
                                  )[1] <= 9
                                      ? `0${
                                            item.DataReferencia.split(
                                                " "
                                            )[0].split("/")[1]
                                        }`
                                      : item.DataReferencia.split(" ")[0].split(
                                            "/"
                                        )[1]
                              }/${
                                  item.DataReferencia.split(" ")[0].split(
                                      "/"
                                  )[0]
                              }/${
                                  item.DataReferencia.split(" ")[0].split(
                                      "/"
                                  )[2]
                              }`
                            : "",
                        
                        { v: item.Codigo_Gip, t: "n" },
                        item.Setor,
                        
                        { v: item.CodigoGIPSubsetor, t: "n" },
                        item.Subsetor,
                        
                        { v: item.Indicador, t: "n" },
                        item.NomeDoIndicador,
                        `${item.Meta}${
                            item.Meta == "-"
                                ? ""
                                : item.TipoIndicador == "PERCENT" &&
                                  item.Meta.toString().includes(",")
                                ? ""
                                : item.TipoIndicador == "PERCENT" &&
                                  item.Meta.toString().includes(".")
                                ? ""
                                : item.TipoIndicador !== "PERCENT"
                                ? ""
                                : item.Meta
                                ? ",00"
                                : ""
                        }${
                            item.Meta &&
                            item.TipoIndicador == "PERCENT" &&
                            item.Meta !== "-"
                                ? "%"
                                : ""
                        }`,
                        `${item.TipoIndicador == "HOUR" ? item.ResultadoHora :
                            item.Resultado.toString().replace(".", ",")}${
                            item.TipoIndicador == "PERCENT" &&
                            item.Resultado.toString().includes(",")
                                ? ""
                                : item.TipoIndicador == "PERCENT" &&
                                  item.Resultado.toString().includes(".")
                                ? ""
                                : item.TipoIndicador !== "PERCENT"
                                ? ""
                                : item.Resultado
                                ? ",00"
                                : ""
                        }${item.TipoIndicador == "PERCENT" ? "%" : ""}`,
                        `${item.Percentual.toString().replace(".", ",")}%`,
                        { v: item.GanhoEmMoedas, t: "n" },
                        
                        { v: item.MetaMaximaDeMoedas, t: "n" },
                        
                        item.Grupo,
                        item.DataAtualizacao
                            ? `${
                                  item.DataAtualizacao.split("T")[0].split(
                                      "-"
                                  )[2]
                              }/${
                                  item.DataAtualizacao.split("T")[0].split(
                                      "-"
                                  )[1]
                              }/${
                                  item.DataAtualizacao.split("T")[0].split(
                                      "-"
                                  )[0]
                              }`
                            : "",
                        item.Site,
                        { v: item.MatriculaGerente1, t: "n" },
                        
                        item.NomeGerente1,
                        { v: item.MatriculaGerente2, t: "n" },
                        
                        item.NomeGerente2,
                        { v: item.MatriculaDiretor, t: "n" },
                        
                        item.NomeDiretor,
                        item.ContempladosDiamante
                            ? `${item.ContempladosDiamante.toString().replace(
                                  ".",
                                  ","
                              )}%`
                            : "",
                        item.ContempladosOuro
                            ? `${item.ContempladosOuro.toString().replace(
                                  ".",
                                  ","
                              )}%`
                            : "",
                        item.ContempladosPrata
                            ? `${item.ContempladosPrata.toString().replace(
                                  ".",
                                  ","
                              )}%`
                            : "",
                        item.ContempladosBronze
                            ? `${item.ContempladosBronze.toString().replace(
                                  ".",
                                  ","
                              )}%`
                            : "",
                        `${item.Score.toString().replace(".", ",")}%`,
                    ];
                });
                let indicatorSheetBuilder = new SheetBuilder();
                indicatorSheetBuilder
                    .setHeader([
                        "Mês",
                        "Ano",
                        "Data de Referencia",
                        "Código GIP",
                        "SETOR",
                        "Código GIP Subsetor",
                        "Subsetor",
                        "Indicador",
                        "Nome do indicador",
                        "Meta",
                        "Resultado",
                        "Percentual",
                        "Ganho em Moedas",
                        "Meta máxima de moedas",
                        "Grupo",
                        "Data de atualização",
                        "Site",
                        "MATRICULA DO GERENTE 1",
                        "NOME DO GERENTE 1",
                        "MATRICULA DO GERENTE 2",
                        "NOME DO GERENTE 2",
                        "MATRICULA DO DIRETOR",
                        "NOME DO DIRETOR",
                        "CONTEMPLADOS DIAMANTE",
                        "CONTEMPLADOS OURO",
                        "CONTEMPLADOS PRATA",
                        "CONTEMPLADOS BRONZE",
                        "Score",
                    ])
                    .append(docRows)
                    .exportAs(`Relatório consolidado de setores`);
                toast.success("Relatório exportado com sucesso!");
            })
            .catch((e) => {
                const msg = e?.response?.data?.Message;

                msg
                    ? toast.error(msg)
                    : toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Relatório consolidado de setores"}
                headerIcon={<FileOpen />}
            />
            <Box display={"flex"} px={2} py={3} flexDirection={"column"}>
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
                               {option.id} - {option.name}
                            </li>
                        );
                    }}
                    isOptionEqualToValue={(option, value) =>
                        option.name === value.name &&
                        option.id === value.id
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
                    filterOptions={(x) => x}
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
                <FormControl sx={{ width: "100%" }}>
                    <InputLabel sx={{ backgroundColor: "#fff" }} size="small">
                        Ordenar por:
                    </InputLabel>
                    <Select
                        onChange={(e) =>
                            setOrder(e.target.value as "Melhor" | "Pior")
                        }
                        value={order}
                        size="small"
                    >
                        <MenuItem value={"Melhor"}>Melhor</MenuItem>
                        <MenuItem value={"Pior"}>Pior</MenuItem>
                    </Select>
                </FormControl>
                <Box
                    display={"flex"}
                    alignItems={"center"}
                    flexDirection={"row"}
                    border={"solid 1px #e1e1e1"}
                    mt={2}
                    px={2}
                >
                    <Typography variant="body2">
                        Resultado consolidado do setor dia a dia
                    </Typography>
                    <Checkbox
                        onChange={(event) => handleCheckboxOnChange(event)}
                        checked={consolidado}
                        size="small"
                    />
                </Box>
                <LoadingButton
                    loading={isLoading}
                    variant={"contained"}
                    type={"submit"}
                    form={"create-sector-form"}
                    sx={{ mt: 2 }}
                    fullWidth
                    onClick={handleReportExtract}
                >
                    Exportar relatório
                </LoadingButton>
            </Box>
        </Card>
    );
}

SectorsConsolidatedReport.getLayout = getLayout("private");
