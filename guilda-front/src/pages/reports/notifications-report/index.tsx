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
import {
  Client,
  Group,
  Indicator,
  Sector,
  SectorAndSubsector,
} from "src/typings";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { PaginationModel } from "src/typings/models/pagination.model";
import { LoadingButton } from "@mui/lab";
import { SheetBuilder } from "src/utils";
import { IndicatorsBySectorsUseCase } from "src/modules/home/use-cases/IndicatorsBySectors/IndicatorsBySectors.use-case";
import { SectorsByHierachyUseCase } from "src/modules/home/use-cases/SectorsByHierarchy/SectorsByHierarchy.use-case";
import { ExportResultsReportUseCase } from "src/modules/home/use-cases/export-results-report";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import {
  ListClientUseCase,
  ListSectorsAndSubsectrosUseCase,
} from "src/modules";
import { SitePersonaUseCase } from "src/modules/personas/use-cases/site-personas.use-case";
import { ListCollaboratorsAllUseCase } from "src/modules/collaborators/use-cases/list-collaborators.use-case";
import { Site } from "src/typings/models/site.model";
import { ReportNotificationUseCase } from "src/modules/notifications/use-cases/report-notification.use-case";

export default function NotificationsReport() {
  const { myUser } = useContext(UserInfoContext);

  const { finishLoading, isLoading, startLoading } = useLoadingState();

  const [startDatePicker, setStartDatePicker] = useState<dateFns | Date | null>(
    null
  );
  const [endDatePicker, setEndDatePicker] = useState<dateFns | Date | null>(
    null
  );

  const [sectors, setSectors] = useState<SectorAndSubsector[]>([]);
  const [selectedSectors, setSelectedSectors] = useState<SectorAndSubsector[]>(
    []
  );
  const [sectorSearch, setSectorSearch] = useState<string>("");
  const [clients, setClients] = useState<Client[]>([]);
  const [selectedClient, setSelectedClient] = useState<Client[]>([]);
  const [collaborators, setCollaborators] = useState<
    {
      id: number;
      name: string;
      registry: string;
    }[]
  >([]);
  const [selectedCollaborator, setSelectedCollaborator] = useState<
    {
      id: number;
      name: string;
      registry: string;
    }[]
  >([]);
  const [collaboratorSearch, setCollaboratorSearch] = useState<string>("");
  const [hierarchies, setHierarchies] = useState<Hierarchie[]>([]);
  const [selectedHierarchies, setSelectedHierachies] = useState<Hierarchie[]>(
    []
  );

  const [site, setSite] = useState<Site[]>([]);
  const [sites, setSites] = useState<Site[]>([]);

  const getHierarchies = async () => {
    startLoading();

    await new ListHierarchiesUseCase()
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

  const getClients = async (codCollaborator: number) => {
    startLoading();

    await new ListClientUseCase()
      .handle({
        codCollaborator,
      })
      .then((data) => {
        setClients(data);
      })
      .catch(() => {
        toast.error("Falha ao carregar clients.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    if (myUser && myUser?.id) {
      getClients(myUser?.id);
    }
  }, []);

  async function ListSitePersona() {
    await new SitePersonaUseCase()
      .handle()
      .then((data) => {
        setSites(data);
      })
      .catch(() => {
        toast.error("Erro ao listar Sites.");
      })
      .finally(() => {});
  }

  useEffect(() => {
    ListSitePersona();
  }, []);

  const getCollaborators = async (searchText: string) => {
    startLoading();

    await new ListCollaboratorsAllUseCase()
      .handle({
        limit: 10,
        offset: 0,
        searchText,
      })
      .then((data) => {
        setCollaborators(data.items);
      })
      .catch(() => {
        toast.error("Falha ao carregar colaboradores.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    getCollaborators(collaboratorSearch);
  }, [collaboratorSearch]);

  const getSectorsAndSubSectors = async (isSubsector = false, sector = "") => {
    startLoading();

    await new ListSectorsAndSubsectrosUseCase()
      .handle({
        isSubsector,
        sector,
      })
      .then((data) => {
        setSectors(data);
      })
      .catch(() => {
        toast.error(
          `Falha ao carregar ${isSubsector ? "subsetores" : "setores"}.`
        );
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    getSectorsAndSubSectors(false, sectorSearch);
  }, [sectorSearch]);

  async function handleReportExtract() {
    if (!endDatePicker || !startDatePicker)
      return toast.warning("selecione as datas");
    if (!myUser || !endDatePicker || !startDatePicker) return;

    startLoading();

    new ReportNotificationUseCase()
      .handle({
        Client: selectedClient.map((item) => item.id),
        DataFinal: format(new Date(endDatePicker.toString()), "yyyy-MM-dd"),
        DataInicial: format(new Date(startDatePicker.toString()), "yyyy-MM-dd"),
        Destination: selectedCollaborator.map((item) => item.id),
        Hierarchies: selectedHierarchies.map((item) => item.id),
        Sectors: selectedSectors.map((item) => item.id),
        Site: site.map((item) => item.id),
      })
      .then((data) => {
        if (data.length <= 0) {
          return toast.warning("Sem dados para exportar.");
        }

        const docRows = data.map((item: any) => {
          return [
            item?.DATA_ENVIO?.toString() || "",
            item?.TEMA?.toString() || "",
            item?.GRUPO?.toString() || "",
            { v: item?.IDCOLLABORATOR?.toString() || "", t: "n" },

            item?.BC?.toString() || "",
            item?.NOME?.toString() || "",
            item?.CARGO?.toString() || "",
            item?.SITE?.toString() || "",
            item?.NOTIFICACAO?.toString() || "",

            { v: item?.MATRICULA_SUPERVISOR?.toString() || "", t: "n" },
            item?.NOME_SUPERVISOR?.toString() || "",

            { v: item?.MATRICULA_COORDENADOR?.toString() || "", t: "n" },
            item?.NOME_COORDENADOR?.toString() || "",

            { v: item?.MATRICULA_GERENTE2?.toString() || "", t: "n" },
            item?.NOME_GERENTE2?.toString() || "",

            { v: item?.MATRICULA_GERENTE1?.toString() || "", t: "n" },
            item?.NOME_GERENTE1?.toString() || "",

            { v: item?.MATRICULA_DIRETOR?.toString() || "", t: "n" },
            item?.NOME_DIRETOR?.toString() || "",

            { v: item?.MATRICULA_CEO?.toString() || "", t: "n" },
            item?.NOME_CEO?.toString() || "",

            { v: item?.CODGIP?.toString() || "", t: "n" },
            item?.SETOR?.toString() || "",

            { v: item?.COD_GIP_SUB?.toString() || "", t: "n" },
            item?.SUBSETOR?.toString() || "",
            item?.IDCOLABORADOR_ENVIO?.toString() || "",
            item?.NOME_ENVIO?.toString() || "",
            item?.SETOR_ENVIO?.toString() || "",
            item?.DATA_INICIO?.toString() || "",
            item?.DATA_EXPIRACAO?.toString() || "",
            item?.STATUS?.toString() || "",
            item?.IDCOLLABORADOR_EXCLUSAO?.toString() || "",
            item?.NOME_EXCLUSAO?.toString() || "",
            item?.SETOR_EXCLUSAO?.toString() || "",
            item?.CLIENTE?.toString() || "",
          ];
        });
        let indicatorSheetBuilder = new SheetBuilder();
        indicatorSheetBuilder
          .setHeader([
            "DATA_ENVIO",
            "TEMA",
            "GRUPO",
            "IDCOLLABORATOR",
            "BC",
            "NOME_DESTINATARIO",
            "CARGO",
            "SITE",
            "NOTIFICACAO",
            "MATRICULA_SUPERVISOR",
            "NOME_SUPERVISOR",
            "MATRICULA_COORDENADOR",
            "NOME_COORDENADOR",
            "MATRICULA_GERENTE2",
            "NOME_GERENTE2",
            "MATRICULA_GERENTE1",
            "NOME_GERENTE1",
            "MATRICULA_DIRETOR",
            "NOME_DIRETOR",
            "MATRICULA_CEO",
            "NOME_CEO",
            "CODGIP",
            "SETOR",
            "COD_GIP_SUB",
            "SUBSETOR",
            "IDCOLABORADOR_ENVIO",
            "NOME_ENVIO",
            "SETOR_ENVIO",
            "DATA_INICIO",
            "DATA_EXPIRACAO",
            "STATUS",
            "IDCOLLABORADOR_EXCLUSAO",
            "NOME_EXCLUSAO",
            "SETOR_EXCLUSAO",
            "CLIENTE",
          ])
          .append(docRows)
          .exportAs(`Relatório_notificações`);
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
          title={"Relatório de notificações"}
          headerIcon={<FileOpen />}
        />
        <Stack px={2} py={4} width={"100%"} gap={2}>
          <Box width={"100%"} display={"flex"} flexDirection={"column"}>
            <Box display={"flex"} flexDirection={"row"} gap={"10px"} mb={2}>
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <DatePicker
                  label="De"
                  value={startDatePicker}
                  onChange={(newValue) => setStartDatePicker(newValue)}
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
            <Stack gap={"20px"}>
              <Autocomplete
                size="small"
                multiple
                fullWidth
                value={selectedSectors}
                options={sectors}
                getOptionLabel={(option) => option.name}
                onChange={(event, value) => {
                  setSelectedSectors(value);
                }}
                onInputChange={(e, text) => setSectorSearch(text)}
                filterOptions={(x) => x}
                filterSelectedOptions
                renderInput={(props) => (
                  <TextField {...props} label={"Setores"} />
                )}
                renderOption={(props, option) => {
                  return (
                    <li {...props} key={option.id}>
                        {option.id} - {option.name}
                    </li>
                  );
                }}
                isOptionEqualToValue={(option, value) =>
                  option.name === value.name
                }
                sx={{ m: 0 }}
              />
              <Autocomplete
                size="small"
                multiple
                fullWidth
                value={selectedCollaborator}
                options={collaborators}
                onInputChange={(e, text) => setCollaboratorSearch(text)}
                getOptionLabel={(option) => option.name}
                onChange={(event, value) => {
                  setSelectedCollaborator(value);
                }}
                filterSelectedOptions
                renderInput={(props) => (
                  <TextField {...props} label={"Colaboradores"} />
                )}
                renderOption={(props, option) => {
                  return (
                    <li {...props} key={option.id}>
                      {option.registry} - {option.name}
                    </li>
                  );
                }}
                isOptionEqualToValue={(option, value) =>
                  option.name === value.name
                }
                sx={{ m: 0 }}
              />
              <Autocomplete
                size="small"
                multiple
                fullWidth
                value={selectedHierarchies}
                options={hierarchies}
                getOptionLabel={(option) => option.levelName}
                onChange={(event, value) => {
                  setSelectedHierachies(value);
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
              <Autocomplete
                size="small"
                multiple
                fullWidth
                value={selectedClient}
                options={clients}
                getOptionLabel={(option) => option.name}
                onChange={(event, value) => {
                  setSelectedClient(value);
                }}
                filterSelectedOptions
                renderInput={(props) => (
                  <TextField {...props} label={"Cliente"} />
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
                size="small"
                multiple
                value={site}
                placeholder={"Site"}
                disableClearable={false}
                onChange={(e, value) => {
                  setSite(value);
                }}
                isOptionEqualToValue={(option, value) => option.id == value.id}
                disableCloseOnSelect
                renderInput={(props) => <TextField {...props} label={"Site"} />}
                getOptionLabel={(option) => option.name}
                options={sites}
                fullWidth
                renderOption={(props, option) => {
                  return (
                    <li {...props} key={option.id}>
                      {option.name}
                    </li>
                  );
                }}
                sx={{ m: 0 }}
              />
            </Stack>
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

NotificationsReport.getLayout = getLayout("private");
