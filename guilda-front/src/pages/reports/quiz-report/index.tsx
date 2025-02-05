import Search from "@mui/icons-material/Search";
import InsertDriveFile from "@mui/icons-material/InsertDriveFile";
import {
  Autocomplete,
  Box,
  Button,
  FormControl,
  InputAdornment,
  InputLabel,
  MenuItem,
  Select,
  TextField,
  Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { Stack } from "@mui/system";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format, isValid, set } from "date-fns";
import { useContext, useState, useEffect } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useLoadingState } from "src/hooks";
import { ListMonetizationHierarchyUseCase } from "src/modules/monetization/use-cases/list-monetization-hierarchy";
import { DateUtils, SheetBuilder, getLayout } from "src/utils";
import { ListTransactions } from "src/modules/monetization/use-cases/list-transactions.use-case";
import { ModalMonetizationReportAdm } from "src/modules/monetization/fragments/modal-monetization-report-adm";
import { ModalMonetizationReportAdmDay } from "src/modules/monetization/fragments/modal-monetization-report-adm-day";
import { ModalMonetizationReportConsolidado } from "src/modules/monetization/fragments/modal-monetization-report-consolidado";
import { formatCurrency } from "src/utils/format-currency";
import abilityFor from "src/utils/ability-for";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { ReportQuizUseCase } from "src/modules/quiz/use-cases/report-quiz.use-case";
import { Client, PersonaAccountModel } from "src/typings";
import { ListPersonasV2UseCase } from "src/modules/personas/use-cases/list-personas-v2.use-case";
import { ListHierarchiesUseCase } from "src/modules/hierarchies/use-cases/list-hierarchies.use-case";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { ListClientUseCase } from "src/modules";
import { SitePersonaResponse, SitePersonaUseCase } from "src/modules/personas/use-cases/site-personas.use-case";
import { SearchAccountsUseCase } from "src/modules/personas/use-cases/search-accounts.use-case";

interface BalanceReportModel {
  hierarchy: string;
  sectors: Array<{
    sectorId: number;
    sector: string;
    monetization: number;
  }>;
}

function formatDetails(
  order: any,
  indicator: any,
  reason: any,
  observation: any
) {
  if (reason) {
    return `Motivo: ${reason}`;
  } else if (observation) {
    return `${observation}`;
  } else if (indicator) {
    return `Indicador: ${indicator.name}`;
  } else if (order) {
    return `Compra na loja - COD: ${order.id}`;
  }
}

export default function BalanceReport() {
  const { myPermissions } = useContext(PermissionsContext);
  const [startDate, setStartDate] = useState<dateFns | null>(null);
  const [endDate, setEndDate] = useState<dateFns | null>(null);

  const [startDateC, setStartDateC] = useState<dateFns | null>(null);
  const [endDateC, setEndDateC] = useState<dateFns | null>(null);

  const [startDateP, setStartDateP] = useState<dateFns | null>(null);
  const [endDateP, setEndDateP] = useState<dateFns | null>(null);

  const [startDateR, setStartDateR] = useState<dateFns | null>(null);
  const [endDateR, setEndDateR] = useState<dateFns | null>(null);

  const [data, setData] = useState<any>(null);
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const { myUser } = useContext(UserInfoContext);

  const [personasDemant, setPersonasDemandant] = useState<
    PersonaAccountModel[]
  >([]);
  const [searchTextPersonaDemandantFilter, setSearchTextPersonaDemandantFilter] =
    useState<string>("");
  const [demandantFilterSelected, setDemandantFilterSelected] =
    useState<PersonaAccountModel | null>(null);
  const [personasResponsibles, setPersonasResponsible] = useState<
    PersonaAccountModel[]
  >([]);
  const [searchTextPersonaResponsibleFilter, setSearchTextPersonaResponsibleFilter] =
    useState<string>("");
  const [responsibleFilterSelected, setResponsibleFilterSelected] =
    useState<PersonaAccountModel | null>(null);
  const [page, setPage] = useState(1);
  const [selectedHierarchiesFilter, setSelectedHierachiesFilter] = useState<Hierarchie[]>(
    []
  );
  const [selectedClientFilter, setSelectedClientFilter] = useState<Client[]>([]);
  const [hierarchies, setHierarchies] = useState<Hierarchie[]>([]);
  const [clients, setClients] = useState<Client[]>([]);
  const [sites, setSites] = useState<SitePersonaResponse[]>([]);
  const [selectedSitesFilter, setSelectedSitesFilter] = useState<
    SitePersonaResponse[]
  >([]);
  const [collaborators, setCollaborators] = useState<
    {
      id: number;
      name: string;
      registry: string;
    }[]
  >([]);
  const [selectedCollaboratorFilter, setSelectedCollaboratorFilter] = useState<{
    id: number;
    name: string;
    registry: string;
  }[]>([]);
  const [collaboratorSearchFilter, setCollaboratorSearchFilter] =
    useState<string>("");
  const [titleQuiz, setTitleQuiz] = useState<string>("");

  function handleReportExtractExport() {
    if (!myUser) return;

    if(startDateC) {
      if(!endDateC) return toast.warning("É necessário preencher as duas datas de criação.");
    }

    if(endDateC) {
      if(!startDateC) return toast.warning("É necessário preencher as duas datas de criação.");
    }

    if(startDateP) {
      if(!endDateP) return toast.warning("É necessário preencher as duas datas de publicação.");
    }

    if(endDateP) {
      if(!startDateP) return toast.warning("É necessário preencher as duas datas de publicação.");
    }

    if(startDateR) {
      if(!endDateR) return toast.warning("É necessário preencher as duas datas de resposta.");
    }

    if(endDateR) {
      if(!startDateR) return toast.warning("É necessário preencher as duas datas de resposta.");
    }

    startLoading();

    new ReportQuizUseCase()
      .handle({
        DataInicial: startDate
          ? format(new Date(startDate.toString()), "yyyy-MM-dd")
          : undefined,
        DataFinal: endDate
          ? format(new Date(endDate.toString()), "yyyy-MM-dd")
          : undefined,
        DataInicialCriacaoQuiz: startDateC
          ? format(new Date(startDateC.toString()), "yyyy-MM-dd")
          : undefined,
        DataFinalCriacaoQuiz: endDateC
          ? format(new Date(endDateC.toString()), "yyyy-MM-dd")
          : undefined,
        DataInicialPublicacaoQuiz: startDateP
          ? format(new Date(startDateP.toString()), "yyyy-MM-dd")
          : undefined,
        DataFinalPublicacaoQuiz: endDateP
          ? format(new Date(endDateP.toString()), "yyyy-MM-dd")
          : undefined,
        DataInicialRespostaQuiz: startDateR
          ? format(new Date(startDateR.toString()), "yyyy-MM-dd")
          : undefined,
        DataFinalRespostaQuiz: endDateR
          ? format(new Date(endDateR.toString()), "yyyy-MM-dd")
          : undefined,
        Title: titleQuiz,
        Users: selectedCollaboratorFilter ? selectedCollaboratorFilter.map((client) => Number(client.id)) : [],
        Hierachies: selectedHierarchiesFilter.map((hierarchie) => Number(hierarchie.id)),
        Cities: [],
        Sites: selectedSitesFilter.map((site) => Number(site.id)),
        Clients: selectedClientFilter.map((client) => Number(client.id)),
        idRequest: responsibleFilterSelected ? responsibleFilterSelected?.IDGDA_PERSONA_USER : 0,
        idCreated: demandantFilterSelected ? demandantFilterSelected?.IDGDA_PERSONA_USER : 0,
      })
      .then((data) => {
        if (data.length <= 0) {
          return toast.warning("Sem dados para exportar.");
        }

        const docRows = data.map((item: any) => {
          return [
            item?.NOME_QUIZ?.toString() || "",
            item?.DESCRICAO_QUIZ?.toString() || "",
            item?.NOME_CRIADOR?.toString() || "",
            { v: item?.MATRICULA_RESPOSTA?.toString() || "", t: "n" },
            
            item?.NOME_RESPOSTA?.toString() || "",
            item?.HOME_BASED?.toString() || "",
            item?.CARGO?.toString() || "",
            { v: item?.MATRICULA_SUPERVISOR?.toString() || "", t: "n" },
            
            item?.NOME_SUPERVISOR?.toString() || "",
            { v: item?.MATRICULA_COORDENADOR?.toString() || "", t: "n" },
            
            item?.NOME_COORDENADOR?.toString() || "",
            { v:  item?.MATRICULA_GERENTE2?.toString() || "", t: "n" },
           
            item?.NOME_GERENTE2?.toString() || "",
            { v: item?.MATRICULA_GERENTE1?.toString() || "", t: "n" },
            
            item?.NOME_GERENTE1?.toString() || "",
            { v: item?.MATRICULA_DIRETOR?.toString() || "", t: "n" },
            
            item?.NOME_DIRETOR?.toString() || "",
            { v: item?.MATRICULA_CEO?.toString() || "", t: "n" },
            
            item?.NOME_CEO?.toString() || "",
            { v: item?.CODGIP?.toString() || "", t: "n" },
            
            item?.SETOR?.toString() || "",
            { v: item?.CODGIP_REFERENCE?.toString() || "", t: "n" },
            
            item?.SETOR_REFERENCE?.toString() || "",
            item?.TIPO_PERGUNTA?.toString() || "",
            item?.PERGUNTA?.toString() || "",
            item?.RESPOSTA_CORRETA?.toString() || "",
            item?.RESPOSTA_SELECIONADA?.toString() || "",
            item?.RESPOSTA_DISPONIVEL?.toString() || "",
            item?.DELETADO_EM?.toString() || "",
            item?.DELETADO_POR?.toString() || "",
          ];
        });
        let indicatorSheetBuilder = new SheetBuilder();
        indicatorSheetBuilder
          .setHeader([
            "NOME_QUIZ",
            "DESCRICAO_QUIZ",
            "NOME_CRIADOR",
            "MATRICULA_RESPOSTA",
            "NOME_RESPOSTA",
            "HOME_BASED",
            "CARGO",
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
            "CODGIP_REFERENCE",
            "SETOR_REFERENCE",
            "TIPO_PERGUNTA",
            "PERGUNTA",
            "RESPOSTA_CORRETA",
            "RESPOSTA_SELECIONADA",
            "RESPOSTA_DISPONIVEL",
            "DELETADO_EM",
            "DELETADO_POR",
          ])
          .append(docRows)
          .exportAs(`Relatório_quizzes`);
      })
      .catch(() => {
        toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
      })
      .finally(() => {
        finishLoading();
      });
  }

  function formatResultDate(dateString: string) {
    if (dateString) {
      const date = dateString.split("T")[0];
      const dateSplited = date.split("-");
      return `${dateSplited[2]}/${dateSplited[1]}/${dateSplited[0]}`;
    } else {
      return "";
    }
  }

  async function ListPersonasDemandantFilter() {
    startLoading();

    const payload = {
      accountPersona: searchTextPersonaDemandantFilter,
      limit: 10,
      page: page,
    };

    new ListPersonasV2UseCase()
      .handle(payload)
      .then((data) => {
        setPersonasDemandant(data.ACCOUNTS);
      })
      .catch((e) => {
        toast.error("Erro ao listar os demandantes. ");
      })
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    ListPersonasDemandantFilter();
  }, [searchTextPersonaDemandantFilter]);

  async function ListPersonasResponsiblesFilter() {
    startLoading();

    const payload = {
      accountPersona: searchTextPersonaResponsibleFilter,
      limit: 10,
      page: page,
    };

    new ListPersonasV2UseCase()
      .handle(payload)
      .then((data) => {
        setPersonasResponsible(data.ACCOUNTS);
      })
      .catch((e) => {
        toast.error("Erro ao listar os responsáveis.");
      })
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    ListPersonasResponsiblesFilter();
  }, [searchTextPersonaResponsibleFilter]);

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

  const getSites = async (codCollaborator: number) => {
    startLoading();

    await new SitePersonaUseCase()
      .handle()
      .then((data) => {
        setSites(data);
      })
      .catch(() => {
        toast.error("Falha ao carregar sites.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    if (myUser && myUser?.id) {
      getSites(myUser?.id);
    }
  }, []);

  const getCollaborators = async (searchText: string) => {
    startLoading();

    await new SearchAccountsUseCase()
      .handle({
        limit: 10,
        page: 1,
        Collaborator: searchText,
      })
      .then((data) => {
        setCollaborators(data[0].account);
      })
      .catch(() => {
        toast.error("Falha ao carregar colaboradores.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    getCollaborators(collaboratorSearchFilter);
  }, [collaboratorSearchFilter]);
  
  return (
    <Card
      width={"100%"}
      display={"flex"}
      flexDirection={"column"}
      justifyContent={"space-between"}
    >
      <PageHeader
        title={`Relatório de Quiz`}
        headerIcon={<InsertDriveFile />}
      />
      <Stack px={2} py={3} width={"100%"} gap={2}>
        {/* <Box display={"flex"} gap={2} width={"100%"}>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Data Inicial"
              value={startDate}
              onChange={(newValue) => setStartDate(newValue)}
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
              label="Data Final"
              value={endDate}
              onChange={(newValue) => setEndDate(newValue)}
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
        </Box> */}

        <Box display={"flex"} gap={2} width={"100%"}>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Data Criação (início)"
              value={startDateC}
              onChange={(newValue) => setStartDateC(newValue)}
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
              label="Data Criação (final)"
              value={endDateC}
              onChange={(newValue) => setEndDateC(newValue)}
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

        <Box display={"flex"} gap={2} width={"100%"}>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Data Publicação (início)"
              value={startDateP}
              onChange={(newValue) => setStartDateP(newValue)}
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
              label="Data Publicação (final)"
              value={endDateP}
              onChange={(newValue) => setEndDateP(newValue)}
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

        <Box display={"flex"} gap={2} width={"100%"}>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Data Resposta (início)"
              value={startDateR}
              onChange={(newValue) => setStartDateR(newValue)}
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
              label="Data Resposta (final)"
              value={endDateR}
              onChange={(newValue) => setEndDateR(newValue)}
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

        <Box display={"flex"} gap={2} width={"100%"}>
          <Autocomplete
            placeholder={"Quem solicitou"}
            value={demandantFilterSelected}
            disableClearable={false}
            fullWidth
            onChange={(_, demandant) => setDemandantFilterSelected(demandant)}
            onInputChange={(e, text) =>
              setSearchTextPersonaDemandantFilter(text)
            }
            isOptionEqualToValue={(option, value) =>
              option.NOME === value.NOME
            }
            renderInput={(props) => (
              <TextField {...props} size="small" label={"Quem solicitou"} />
            )}
            renderOption={(props, option) => {
              return (
                <li {...props} key={option.IDGDA_PERSONA_USER}>
                  {option.NOME}
                </li>
              );
            }}
            renderTags={() => null}
            getOptionLabel={(option) => option.NOME}
            options={personasDemant}
            sx={{ mb: 0 }}
          />
          <Autocomplete
            placeholder={"Quem criou"}
            value={responsibleFilterSelected}
            fullWidth
            onChange={(_, demandant) => setResponsibleFilterSelected(demandant)}
            onInputChange={(e, text) =>
              setSearchTextPersonaResponsibleFilter(text)
            }
            isOptionEqualToValue={(option, value) =>
              option.NOME === value.NOME
            }
            renderInput={(props) => (
              <TextField
                {...props}
                size="small"
                label={"Quem criou"}
              />
            )}
            renderOption={(props, option) => {
              return (
                <li {...props} key={option.IDGDA_PERSONA_USER}>
                  {option.NOME}
                </li>
              );
            }}
            renderTags={() => null}
            getOptionLabel={(option) => option.NOME}
            options={personasResponsibles}
            sx={{ mb: 0 }}
          />
        </Box>

        <Box display={"flex"} gap={2} width={"100%"}>
          <Autocomplete
            multiple
            fullWidth
            value={selectedHierarchiesFilter}
            options={hierarchies}
            getOptionLabel={(option) => option.levelName}
            onChange={(event, value) => {
              setSelectedHierachiesFilter(value);
            }}
            filterSelectedOptions
            renderInput={(props) => (
              <TextField {...props} size="small" label={"Hierarquias"} />
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
            multiple
            fullWidth
            value={selectedClientFilter}
            options={clients}
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
              setSelectedClientFilter(value);
            }}
            filterSelectedOptions
            renderInput={(props) => (
              <TextField {...props} size="small" label={"Cliente"} />
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
        </Box>

        <Box display={"flex"} gap={2} width={"100%"}>
          <Autocomplete
            multiple
            fullWidth
            disableClearable={false}
            value={selectedSitesFilter}
            options={sites}
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
              setSelectedSitesFilter(value);
            }}
            filterSelectedOptions
            renderInput={(props) => <TextField {...props} size="small" label={"Sites"} />}
            renderOption={(props, option) => {
              return (
                <li {...props} key={option.id}>
                  {option.name}
                </li>
              );
            }}
            isOptionEqualToValue={(option, value) => option.name === value.name}
            sx={{ m: 0 }}
          />
          <Autocomplete
            multiple
            fullWidth
            value={selectedCollaboratorFilter}
            options={collaborators}
            disableClearable={false}
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
              setSelectedCollaboratorFilter(value);
            }}
            onInputChange={(e, text) => setCollaboratorSearchFilter(text)}
            filterSelectedOptions
            renderInput={(props) => (
              <TextField {...props} size="small" label={"Colaboradores"} />
            )}
            renderOption={(props, option) => {
              return (
                <li {...props} key={option.id}>
                  {option.id} - {option.name}
                </li>
              );
            }}
            isOptionEqualToValue={(option, value) => option.name === value.name}
            sx={{ m: 0 }}
          />
        </Box>

        <Box display={"flex"} gap={2} width={"100%"}>
          <TextField
            value={titleQuiz}
            label="Título"
            size="small"
            fullWidth
            onChange={(e) => setTitleQuiz(e.target.value)}
          />
        </Box>
        <Button
          onClick={handleReportExtractExport}
          sx={{ mt: "20px" }}
          variant="contained"
        >
          Exportar Relatório
        </Button>
      </Stack>
    </Card>
  );
}

BalanceReport.getLayout = getLayout("private");
