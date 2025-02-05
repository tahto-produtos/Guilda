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
import { grey } from "@mui/material/colors";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
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
import { CreateActionEscalationUseCase } from "src/modules/escalation/use-cases/create-action-escalation.use-case";
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
import { SearchAccountsUseCase } from "src/modules/personas/use-cases/search-accounts.use-case";
import { Indicator, Period, Sector, SectorAndSubsector } from "src/typings";
import { GroupNew } from "src/typings/models/group-new.model";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { PaginationModel } from "src/typings/models/pagination.model";
import { getLayout } from "src/utils";
import { capitalizeText } from "src/utils/capitalizeText";

export default function CreateActionView() {
  const { myUser } = useContext(UserInfoContext);
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [searchText, setSearchText] = useState<string>("");
  const debouncedSearchText: string = useDebounce<string>(searchText, 400);
  const router = useRouter();
  const theme = useTheme();

  const [actionPerformed, setActionPerformed] = useState<string>("");
  const [actionEscalations, setActionEscalations] = useState<
    LoadActionEscalation[]
  >([]);
  const [actionEscalationSelected, setActionEscalationSelected] =
    useState<LoadActionEscalation | null>(null);
  const [searchAction, setSearchAction] = useState<string>("");
  const [name, setName] = useState<string>("");
  const [description, setDescription] = useState<string>("");

  const [hierarchies, setHierarchies] = useState<Hierarchie[]>([]);
  const [selectedHierarchie, setSelectedHierachie] = useState<Hierarchie[]>([]);

  const [sector, setSector] = useState<SectorAndSubsector | null>(null);
  const [sectors, setSectors] = useState<SectorAndSubsector[]>([]);
  const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
  const debouncedSectorSearchTerm: string = useDebounce<string>(
    sectorsSearchValue,
    400
  );

  const [subsector, setSubsector] = useState<SectorAndSubsector | null>(null);
  const [subsectors, setSubsectors] = useState<SectorAndSubsector[]>([]);
  const [selectedSubsector, setSelectedSubsector] =
    useState<SectorAndSubsector | null>(null);
  const [subsectorsSearchValue, setSubsectorsSearchValue] =
    useState<string>("");
  const debouncedSubsectorsSearchTerm: string = useDebounce<string>(
    subsectorsSearchValue,
    400
  );

  const [groups, setGroups] = useState<GroupNew[]>([]);
  const [selectedGroups, setSelectedGroups] = useState<GroupNew | null>(null);

  const [indicatorsSearchValue, setIndicatorsSearchValue] =
    useState<string>("");
  const [selectedIndicators, setSelectedIndicators] =
    useState<Indicator | null>(null);
  const [indicators, setIndicators] = useState<Indicator[]>([]);

  const [startDate, setStartDate] = useState<dateFns | null>(null);
  const [endDate, setEndDate] = useState<dateFns | null>(null);

  const [collaborators, setCollaborators] = useState<
    {
      id: number;
      name: string;
      registry: string;
    }[]
  >([]);
  const [selectedCollaborator, setSelectedCollaborator] = useState<{
    id: number;
    name: string;
    registry: string;
  } | null>(null);
  const [selectedCollaboratorAction, setSelectedCollaboratorAction] = useState<{
    id: number;
    name: string;
    registry: string;
  } | null>(null);
  const [collaboratorSearch, setCollaboratorSearch] = useState<string>("");
  const [numberStage, setNumberStage] = useState<number>(0);
  const [hierarchyStage, setHierarchyStage] = useState<Hierarchie | null>(null);
  const [selectedStages, setSelectedStage] = useState<{
      IDGDA_HIERARCHY: number[];
      NUMBER_STAGE: number;
      HIERARCHY: Hierarchie;
  }[]>([]);

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

  function getSubsectors() {
    startLoading();

    new ListSectorsAndSubsectrosUseCase()
      .handle({
        isSubsector: true,
        sector: subsectorsSearchValue,
      })
      .then((data) => {
        setSubsectors(data);
      })
      .catch(() => {
        toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
      })
      .finally(() => {
        finishLoading();
      });
  }
  useEffect(() => {
    getSubsectors();
  }, [debouncedSubsectorsSearchTerm]);

  async function getFeedbacks() {
    startLoading();

    await new LoadLibraryActionEscalationUseCase()
      .handle({
        CREATEDATFROM: "",
        CREATEDATTO: "",
        STARTEDATFROM: "",
        STARTEDATTO: "",
        ENDEDATFROM: "",
        ENDEDATTO: "",
        NAME: searchAction,
        DESCRIPTION: "",
        INDICATOR: [],
        LIMIT: 5,
        PAGE: 1,
        AUTOMATIC: 0,
      })
      .then((data) => {
        setActionEscalations(data.LoadActionEscalation);
      })
      .catch(() => {})
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    getFeedbacks();
  }, [searchAction]);

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
    getCollaborators(collaboratorSearch);
  }, [collaboratorSearch]);

  async function handleCreate() {
    if (
      !actionPerformed ||
      !selectedIndicators ||
      !endDate ||
      !startDate ||
      !selectedCollaboratorAction ||
      !selectedCollaborator ||
      !sector
    )
      return;
    startLoading();
    await new CreateActionEscalationUseCase()
      .handle({
        DESCRIPTION: description,
        ENDED_AT: format(new Date(endDate.toString()), "yyyy-MM-dd"),
        IDGDA_INDICATOR: selectedIndicators.id,
        IDGDA_PERSONA_RESPONSIBLE_CREATION: selectedCollaboratorAction?.id,
        IDGDA_PERSONA_RESPONSIBLE_ACTION: selectedCollaborator?.id,
        IDGDA_SECTOR: sector?.id,
        IDGDA_SUBSECTOR: subsector?.id ?? 0,
        NAME: name,
        ACTION_REALIZED: actionPerformed,
        STARTED_AT: format(new Date(startDate.toString()), "yyyy-MM-dd"),
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
            <Typography fontWeight={"700"}>Criação de plano de ação</Typography>
          </Link>
        </Breadcrumbs>
      </Stack>
      <ContentArea sx={{ py: " 40px" }}>
        <Stack px={"40px"}>
          <PageTitle
            icon={<ListAltOutlined sx={{ fontSize: "40px" }} />}
            title="Criação de plano de ação"
            loading={isLoading}
          ></PageTitle>
          <Divider />
          <Stack direction={"row"} gap={"16px"} mt={"30px"}>
            <LocalizationProvider dateAdapter={AdapterDateFns}>
              <DatePicker
                label="De"
                value={startDate}
                onChange={(newValue) => setStartDate(newValue)}
                slotProps={{
                  textField: {
                    fullWidth: true,
                    sx: {
                      minWidth: "180px",
                      svg: {
                        color: grey[500],
                      },
                    },
                  },
                }}
              />
            </LocalizationProvider>
            <LocalizationProvider dateAdapter={AdapterDateFns}>
              <DatePicker
                label="Até"
                value={endDate}
                onChange={(newValue) => setEndDate(newValue)}
                slotProps={{
                  textField: {
                    fullWidth: true,
                    sx: {
                      minWidth: "180px",
                      svg: {
                        color: grey[500],
                      },
                    },
                  },
                }}
              />
            </LocalizationProvider>
          </Stack>
          <Stack mt={"40px"} direction={"row"} gap="16px">
            <TextField
              onChange={(e) => setActionPerformed(e.target.value)}
              value={actionPerformed}
              label="Ação realizada"
              fullWidth
            />
            <Autocomplete
              value={sector}
              placeholder={"Setor"}
              limitTags={1}
              disableClearable={false}
              onChange={(e, value) => {
                setSector(value);
              }}
              fullWidth
              onInputChange={(e, text) => setSectorsSearchValue(text)}
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
              getOptionLabel={(option) => `${option.id} - ${option.name}`}
              options={sectors}
            />
          </Stack>
          <Stack direction={"row"} gap="16px">
            <Autocomplete
              value={subsector}
              placeholder={"Subsetor"}
              limitTags={1}
              disableClearable={false}
              onChange={(e, value) => {
                setSubsector(value);
              }}
              fullWidth
              onInputChange={(e, text) => setSubsectorsSearchValue(text)}
              renderInput={(props) => (
                <TextField
                  {...props}
                  variant="outlined"
                  label="Subsetores"
                  placeholder="Buscar"
                />
              )}
              isOptionEqualToValue={(option, value) =>
                option.name === value.name
              }
              getOptionLabel={(option) => `${option.id} - ${option.name}`}
              options={subsectors}
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
            gap={"30px"}
            mt={"30px"}
          >
            <Autocomplete
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
                <TextField {...props} label={"Responsável pela criação"} />
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
              fullWidth
              value={selectedCollaboratorAction}
              options={collaborators}
              onInputChange={(e, text) => setCollaboratorSearch(text)}
              getOptionLabel={(option) => option.name}
              onChange={(event, value) => {
                setSelectedCollaboratorAction(value);
              }}
              filterSelectedOptions
              renderInput={(props) => (
                <TextField {...props} label={"Responsável pela ação"} />
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
          </Stack>
          <Stack
            direction={"row"}
            alignItems={"center"}
            mt={"30px"}
            gap={"30px"}
          >
            <TextField
              onChange={(e) => setName(e.target.value)}
              value={name}
              label="Nome ação"
              fullWidth
            />
            <TextField
              onChange={(e) => setDescription(e.target.value)}
              value={description}
              label="Descrição"
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

CreateActionView.getLayout = getLayout("private");
