import EditOutlined from "@mui/icons-material/EditOutlined";
import {
  Box,
  Button,
  Divider,
  Pagination,
  Stack,
  TextField,
  Typography,
  useTheme,
  Checkbox,
  List,
  ListItem,
  IconButton,
  ListItemIcon,
  ListItemText,
  Chip,
  Breadcrumbs,
  Link,
  Drawer,
} from "@mui/material";
import { Autocomplete, LoadingButton } from "@mui/lab";
import { useRouter } from "next/router";
import { useEffect, useState, useContext } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { BaseModal, ConfirmModal } from "src/components/feedback";
import { EmptyState } from "src/components/feedback/empty-state/empty-state";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { useLoadingState } from "src/hooks";
import {
  CreateAnswerUseCase,
  CreateQuizUseCase,
  DeleteAnswerUseCase,
  DeleteQuestionUseCase,
  DeleteQuizUseCase,
  ListAnswerUseCase,
  ListClientUseCase,
  ListHomeFloorUseCase,
  ListItemAnswer,
  ListPeriodUseCase,
  ListQuestionsUseCase,
  ListQuizzesUseCase,
  ListSectorsAndSubsectrosUseCase,
  ListTypeQuestionsUseCase,
  ModalEditAnswer,
  ModalEditQuestion,
  ModalEditQuiz,
} from "src/modules";
import {
  Client,
  HomeFloor,
  ListAnswerResponseModel,
  ListQuestionsResponseModel,
  ListQuizResponseModel,
  ListTypeQuestionResponseModel,
  Period,
  PersonaAccountModel,
  QuizModel,
  SectorAndSubsector,
  TypeQuestionModel,
} from "src/typings";
import { getLayout, DateUtils } from "src/utils";
import { capitalizeText } from "src/utils/capitalizeText";
import { ListPersonasV2UseCase } from "src/modules/personas/use-cases/list-personas-v2.use-case";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import {
  DatePicker,
  DateTimePicker,
  LocalizationProvider,
} from "@mui/x-date-pickers";
import { GroupNew } from "src/typings/models/group-new.model";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { ListGroupsNewUseCase } from "src/modules/groups/use-cases/list-groups-new";
import { ListHierarchiesUseCase } from "src/modules/hierarchies/use-cases/list-hierarchies.use-case";
import { ListCollaboratorsAllUseCase } from "src/modules/collaborators/use-cases/list-collaborators.use-case";
import { ActionButton } from "src/components/inputs/action-button/action-button";
import { format } from "date-fns";
import { ListItemQuestion } from "src/modules";
import FolderIcon from "@mui/icons-material/Folder";
import DeleteIcon from "@mui/icons-material/Delete";
import { CreateQuestionsUseCase } from "src/modules/quiz/use-cases/questions/create-question.use-case";
import { grey } from "@mui/material/colors";
import { SearchAccountsUseCase } from "src/modules/personas/use-cases/search-accounts.use-case";
import {
  SitePersonaResponse,
  SitePersonaUseCase,
} from "src/modules/personas/use-cases/site-personas.use-case";
import { DuplicateQuizUseCase } from "src/modules/quiz/duplicate-quiz.use-case";
import {
  AddCircleOutline,
  FactCheckOutlined,
  FilterList,
  HomeOutlined,
} from "@mui/icons-material";
import { ProfileImage } from "src/components/data-display/profile-image/profile-image";

export default function ListQuizzes() {
  const { myUser } = useContext(UserInfoContext);
  const [quizzes, setQuizzes] = useState<QuizModel[]>([]);
  const [quizSelected, setQuizSelected] = useState<QuizModel | undefined>(
    undefined
  );
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [orientation, setOrientation] = useState<string>("");
  const [totalPages, setTotalPages] = useState<number | null>(null);
  const [isOpenModalCreateQuiz, setIsOpenModalCreateQuiz] =
    useState<boolean>(false);
  const [isOpenModalDeleteQuiz, setIsOpenModalDeleteQuiz] =
    useState<boolean>(false);
  const [isOpenModalViewQuestionsQuiz, setIsOpenModalViewQuestionsQuiz] =
    useState<boolean>(false);
  const [codQuiz, setCodQuiz] = useState<number>(0);
  const [selectedQuizzes, setSelectedQuizzes] = useState<number[]>([]);
  /*
        START SECTION CREATE QUIZ   
    */
  const [searchTextPersonaDemandant, setSearchTextPersonaDemandant] =
    useState<string>("");
  const [demandantSelected, setDemandantSelected] =
    useState<PersonaAccountModel | null>(null);
  const [personasDemant, setPersonasDemandant] = useState<
    PersonaAccountModel[]
  >([]);
  const [searchTextPersonaResponsible, setSearchTextPersonaResponsible] =
    useState<string>("");
  const [responsibleSelected, setResponsibleSelected] =
    useState<PersonaAccountModel | null>(null);
  const [personasResponsibles, setPersonasResponsible] = useState<
    PersonaAccountModel[]
  >([]);
  const [titleQuiz, setTitleQuiz] = useState<string>("");
  const [descriptionQuiz, setDescriptionQuiz] = useState<string>("");
  const [dateStartQuiz, setDateStartQuiz] = useState<string | null>("");
  const [dateEndQuiz, setDateEndQuiz] = useState<string | null>("");
  const [monetizatioValue, setMonetizationValue] = useState<number>(0);
  const [percentMonetizatioValue, setPercentMonetizationValue] =
    useState<number>(0);
  const [requiredQuiz, setRequiredQuiz] = useState<boolean>(false);
  const [sectors, setSectors] = useState<SectorAndSubsector[]>([]);
  const [selectedSectors, setSelectedSectors] = useState<SectorAndSubsector[]>(
    []
  );
  const [sectorSearch, setSectorSearch] = useState<string>("");
  const [subSectors, setSubSectors] = useState<SectorAndSubsector[]>([]);
  const [selectedSubSector, setSelectedSubSector] = useState<
    SectorAndSubsector[]
  >([]);
  const [subSectorSearch, setSubSectorSearch] = useState<string>("");
  const [periods, setPeriods] = useState<Period[]>([]);
  const [selectedPeriods, setSelectedPeriods] = useState<Period[]>([]);
  const [groups, setGroups] = useState<GroupNew[]>([]);
  const [selectedGroups, setSelectedGroups] = useState<GroupNew[]>([]);
  const [clients, setClients] = useState<Client[]>([]);
  const [selectedClient, setSelectedClient] = useState<Client[]>([]);
  const [homesFloors, setHomesFloors] = useState<HomeFloor[]>([]);
  const [isOpenDelete2, setIsOpenDelete2] = useState(false);
  const [isOpenDelete3, setIsOpenDelete3] = useState<number | null>(null);
  const [selectedHomeFloor, setSelectedHomeFloor] = useState<HomeFloor[]>([]);
  const [sites, setSites] = useState<SitePersonaResponse[]>([]);
  const [selectedSites, setSelectedSites] = useState<SitePersonaResponse[]>([]);
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
  const [relevance, setRelevance] = useState<boolean>(false);
  const [isOpenModalEditQuiz, setIsOpenModalEditQuiz] =
    useState<boolean>(false);
  /*
   * END SECTION CREATE QUIZ
   */

  /*
   *   START SECTION QUESTIONS QUIZ
   */
  const [questions, setQuestions] = useState<ListQuestionsResponseModel[]>([]);
  const [questionSelected, setQuestionSelected] = useState<
    ListQuestionsResponseModel | undefined
  >(undefined);
  const [idQuestion, setIdQuestion] = useState<number>(0);
  const [isOpenModalCreateQuestion, setIsOpenModalCreateQuestion] =
    useState<boolean>(false);
  const [typesQuestions, setTypesQuestions] = useState<
    ListTypeQuestionResponseModel[] | null
  >(null);
  const [typeQuestionSelected, setTypeQuestionSelected] =
    useState<TypeQuestionModel | null>(null);
  const [questionDescription, setQuestionDescription] = useState<string>("");
  const [image, setImage] = useState<File[]>([]);
  const [timeQuestion, setTimeQuestion] = useState<number>(0);
  const [isOpenModalViewAnswer, setIsOpenModalViewAnswer] =
    useState<boolean>(false);
  const [isOpenModalCreateAnswer, setIsOpenModalCreateAnswer] =
    useState<boolean>(false);
  const [answers, setAnswers] = useState<ListAnswerResponseModel[]>([]);
  const [answerDescription, setAnswerDescription] = useState<string>("");
  const [answerCorrect, setAnswerCorrect] = useState<boolean>(false);
  const [imageAnsWer, setImageAnswer] = useState<File[]>([]);
  const [isOpenModalEditQuestion, setIsOpenModalEditQuestion] =
    useState<boolean>(false);
  const [isOpenModalEditAnswer, setIsOpenModalEditAnswer] =
    useState<boolean>(false);
  const [answerSelected, setAnswerSelected] =
    useState<ListAnswerResponseModel>();
  /*
   *   END SECTION QUESTIONS QUIZ
   */

  /**
   * FILTERS
   */
  const [statusFilter, setStatusFilter] = useState<
    {
      id: number;
      name: string;
    }[]
  >([
    {
      id: 1,
      name: "Em andamento",
    },
    {
      id: 1,
      name: "Finalizado",
    },
    {
      id: 1,
      name: "Pendente de edição",
    },
  ]);
  const [statusFilterSelected, setStatusFilterSelected] = useState<string>("");
  const [titleSearch, setTitleSearch] = useState<string>("");
  const [selectedSitesFilter, setSelectedSitesFilter] = useState<
    SitePersonaResponse[]
  >([]);
  const [startDate, setStartDate] = useState<dateFns | null>(null);
  const [endDate, setEndDate] = useState<dateFns | null>(null);
  const [isOpenFilter, setIsOpenFilter] = useState(false);
  const [startDateE, setStartDateE] = useState<dateFns | null>(null);
  const [endDateE, setEndDateE] = useState<dateFns | null>(null);
  const [startDateP, setStartDateP] = useState<dateFns | null>(null);
  const [endDateP, setEndDateP] = useState<dateFns | null>(null);
  const [wordsFilter, setWordsFilter] = useState<string>("");
  const [selectedCollaboratorFilter, setSelectedCollaboratorFilter] = useState<{
    id: number;
    name: string;
    registry: string;
  } | null>(null);
  const [collaboratorSearchFilter, setCollaboratorSearchFilter] =
    useState<string>("");
  const [searchTextPersonaDemandantFilter, setSearchTextPersonaDemandantFilter] =
    useState<string>("");
  const [demandantFilterSelected, setDemandantFilterSelected] =
    useState<PersonaAccountModel | null>(null);
  const [searchTextPersonaResponsibleFilter, setSearchTextPersonaResponsibleFilter] =
    useState<string>("");
  const [responsibleFilterSelected, setResponsibleFilterSelected] =
    useState<PersonaAccountModel | null>(null);
  const [selectedHierarchiesFilter, setSelectedHierachiesFilter] = useState<Hierarchie[]>(
    []
  );
  const [selectedClientFilter, setSelectedClientFilter] = useState<Client[]>([]);
  /**
   * END FILTERS
   */

  const theme = useTheme();
  const router = useRouter();
  const [page, setPage] = useState(1);

  const handleChangePagination = (
    event: React.ChangeEvent<unknown>,
    value: number
  ) => {
    setPage(value);
  };

  /* useEffect(() => {
        setPage(1);
    }, []); */

  function handleOnSelect(id: number) {
    let newArr = [...selectedQuizzes];

    if (newArr.includes(id)) {
      newArr = newArr.filter((item) => item !== id);
    } else {
      newArr.push(id);
    }

    setSelectedQuizzes(newArr);
  }

  async function ListQuiz() {
    startLoading();

    const payload = {
      CREATEDATFROM: startDate
        ? format(new Date(startDate.toString()), "yyyy-MM-dd")
        : "",
      CREATEDATTO: endDate
        ? format(new Date(endDate.toString()), "yyyy-MM-dd")
        : "",
      STARTEDATFROM: startDateP
        ? format(new Date(startDateP.toString()), "yyyy-MM-dd")
        : "",
      STARTEDATTO: endDateP
        ? format(new Date(endDateP.toString()), "yyyy-MM-dd")
        : "",
      ENDEDATFROM: startDateE
        ? format(new Date(startDateE.toString()), "yyyy-MM-dd")
        : "",
      ENDEDATTO: endDateE
        ? format(new Date(endDateE.toString()), "yyyy-MM-dd")
        : "",
      TITLE: titleSearch,
      SITES: selectedSitesFilter.map((site) => Number(site.id)),
      STATUS: statusFilterSelected,
      WORD: wordsFilter,
      IDCREATOR:
        selectedCollaboratorFilter && selectedCollaboratorFilter != null
          ? selectedCollaboratorFilter.id
          : 0,
      DESCRIPTION: "",
      REQUIRED: 0,
      LIMIT: 10,
      PAGE: page,
      FLAGRELEVANCE: relevance ? 1 : 0,
      HIERARCHY: selectedHierarchiesFilter.map((hierarchie) => Number(hierarchie.id)),
      QUEM_CRIOU: responsibleFilterSelected ? [responsibleFilterSelected?.IDGDA_PERSONA_USER] : [],
      QUEM_SOLICITOU: demandantFilterSelected ? [demandantFilterSelected?.IDGDA_PERSONA_USER] : [],
      CLIENT: selectedClientFilter.map((client) => Number(client.id)),
    };

    new ListQuizzesUseCase()
      .handle(payload)
      .then((data) => {
        setTotalPages(data.totalpages);
        setQuizzes(data.LoadLibraryQuiz);
      })
      .catch(() => {
        toast.error("Erro ao listar os quiz.");
      })
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    ListQuiz();
  }, [
    page,
    titleSearch,
    selectedSitesFilter,
    startDate,
    endDate,
    startDateE,
    endDateE,
    startDateP,
    endDateP,
    statusFilterSelected,
    wordsFilter,
    selectedCollaboratorFilter,
    relevance,
    selectedHierarchiesFilter,
    selectedClientFilter,
    demandantFilterSelected,
    responsibleFilterSelected,
  ]);

  async function ListPersonasDemandant() {
    startLoading();

    const payload = {
      accountPersona: searchTextPersonaDemandant,
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
    ListPersonasDemandant();
  }, [searchTextPersonaDemandant]);

  async function ListPersonasResponsibles() {
    startLoading();

    const payload = {
      accountPersona: searchTextPersonaResponsible,
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
    ListPersonasResponsibles();
  }, [searchTextPersonaResponsible]);

  const getSectorsAndSubSectors = async (isSubsector = false, sector = "") => {
    startLoading();

    new ListSectorsAndSubsectrosUseCase()
      .handle({
        isSubsector,
        sector,
      })
      .then((data) => {
        if (isSubsector) {
          setSubSectors(data);
        } else {
          setSectors(data);
        }
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

  useEffect(() => {
    getSectorsAndSubSectors(true, subSectorSearch);
  }, [subSectorSearch]);

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
      ListPersonasDemandant();
      ListPersonasResponsibles();
    }
  }, [myUser]);

  const getGroups = async (codCollaborator: number) => {
    startLoading();

    await new ListGroupsNewUseCase()
      .handle({
        codCollaborator,
      })
      .then((data) => {
        setGroups(data);
      })
      .catch(() => {
        toast.error("Falha ao carregar grupos.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    if (myUser && myUser?.id) {
      getGroups(myUser?.id);
    }
  }, []);

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

  useEffect(() => {
    getCollaborators(collaboratorSearchFilter);
  }, [collaboratorSearchFilter]);

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

  const getHomeOrFloor = async (codCollaborator: number) => {
    startLoading();

    await new ListHomeFloorUseCase()
      .handle({
        codCollaborator,
      })
      .then((data) => {
        setHomesFloors(data);
      })
      .catch(() => {
        toast.error("Falha ao carregar home ou piso.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    if (myUser && myUser?.id) {
      getHomeOrFloor(myUser?.id);
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

  const handleCreateQuiz = async () => {
    if(!myUser) return;
    startLoading();

    const quizModel: QuizModel = {
      codQuiz: 0,
      startedAt: dateStartQuiz
      ? format(new Date(dateStartQuiz.toString()), "yyyy-MM-dd HH:mm:ss")
      : "",
      createdAt: new Date().toDateString(),
      idDemandant: demandantSelected == null ? 0 : demandantSelected?.IDGDA_PERSONA_USER,
      nameDemandant: demandantSelected == null ? "" : demandantSelected?.NOME,
      idResponsible: responsibleSelected == null ? 0 : responsibleSelected?.IDGDA_PERSONA_USER,
      nameResponsible: responsibleSelected == null ? "" : responsibleSelected?.NOME,
      title: titleQuiz,
      description: descriptionQuiz,
      required: requiredQuiz ? 1 : 0,
      Monetization: monetizatioValue,
      PercentMonetization: percentMonetizatioValue,
      sendedAt: "",
      //sendedAt: dateStartQuiz ? format(new Date(dateStartQuiz.toString()), "yyyy-MM-dd HH:mm:ss") : "",
      endedAt: dateEndQuiz ? format(new Date(dateEndQuiz.toString()), "yyyy-MM-dd HH:mm:ss") : "",
      idCreator: myUser.id,
      nameCreator: myUser.name,
      status: "",
    };

    await new CreateQuizUseCase()
      .handle({
        IDGDA_COLLABORATOR_DEMANDANT:
          demandantSelected == null ? 0 : demandantSelected?.IDGDA_PERSONA_USER,
        IDGDA_COLLABORATOR_RESPONSIBLE:
          responsibleSelected == null
            ? 0
            : responsibleSelected?.IDGDA_PERSONA_USER,
        TITLE: titleQuiz,
        ORIENTATION: orientation,
        DESCRIPTION: descriptionQuiz,
        REQUIRED: requiredQuiz ? 1 : 0,
        MONETIZATION: monetizatioValue,
        PERCENT_MONETIZATION: percentMonetizatioValue,
        STARTED_AT: dateStartQuiz
          ? format(new Date(dateStartQuiz.toString()), "yyyy-MM-dd HH:mm:ss")
          : "",
        ENDED_AT: dateEndQuiz
          ? format(new Date(dateEndQuiz.toString()), "yyyy-MM-dd HH:mm:ss")
          : "",
        visibility: {
          sector: selectedSectors.map((sector) => Number(sector.id)),
          subSector: selectedSubSector.map((subsector) => Number(subsector.id)),
          period: selectedPeriods.map((period) => Number(period.id)),
          hierarchy: selectedHierarchies.map((hierarchie) =>
            Number(hierarchie.id)
          ),
          group: selectedGroups.map((group) => Number(group.id)),
          userId: selectedCollaborator.map((collaborator) =>
            Number(collaborator.id)
          ),
          client: selectedClient.map((client) => Number(client.id)),
          homeOrFloor: selectedHomeFloor.map((homeFloor) =>
            Number(homeFloor.id)
          ),
          site: selectedSites.map((site) => Number(site.id)),
        },
      })
      .then((data) => {
        if (data.toString().includes("Erro"))
        {
          return toast.error(data);
        }
        setDemandantSelected(null);
        setResponsibleSelected(null);
        setTitleQuiz("");
        setDescriptionQuiz("");
        setDateStartQuiz("");
        setDateEndQuiz("");
        setRequiredQuiz(false);
        setMonetizationValue(0);
        setPercentMonetizationValue(0);
        setSelectedSectors([]);
        setSelectedSubSector([]);
        setSelectedPeriods([]);
        setSelectedGroups([]);
        setSelectedCollaborator([]);
        setSelectedHierachies([]);
        setSelectedClient([]);
        setSelectedHomeFloor([]);
        setIsOpenModalCreateQuiz(false);
        toast.success("Quiz criado com sucesso!", {
          position: "bottom-left",
        });
        ListQuiz();
        
        quizModel.codQuiz = Number(data);
        handleOpenModalViewQuestionsQuiz(data, quizModel);
      })
      .catch(() => {
          toast.error("Falha ao criar o quiz.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  const handleDeleteQuiz = async (id: number) => {
    startLoading();

    await new DeleteQuizUseCase()
      .handle({
        IDGDA_QUIZ: [id],
        VALIDADETED: true,
      })
      .then((data) => {
        toast.success(data);
        setIsOpenModalDeleteQuiz(false);
        setCodQuiz(0);
        ListQuiz();
      })
      .catch(() => {
        toast.error("Falha ao deletar quiz.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  const handleClearQuiz = async () => {
    startLoading();

    await new DeleteQuizUseCase()
      .handle({
        IDGDA_QUIZ: selectedQuizzes,
        VALIDADETED: true,
      })
      .then((data) => {
        toast.success(data);
        setIsOpenModalDeleteQuiz(false);
        setCodQuiz(0);
        ListQuiz();
      })
      .catch(() => {
        toast.error("Falha ao deletar quiz.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  const handleDuplicateQuiz = async (codQuizD: number) => {
    startLoading();

    await new DuplicateQuizUseCase()
      .handle({
        IDGDA_QUIZ: codQuizD,
      })
      .then((data) => {
        toast.success("Quiz duplicado com sucesso.");
        setCodQuiz(0);
        ListQuiz();
      })
      .catch(() => {
        toast.error("Falha ao duplicar quiz.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  const handleOpenModalDeleteQuiz = (idQuiz: number) => {
    setIsOpenModalDeleteQuiz(!isOpenModalDeleteQuiz);
    setCodQuiz(idQuiz);
  };

  const handleOpenModalEditQuiz = (idQuiz: number, quizData: QuizModel) => {
    setIsOpenModalEditQuiz(!isOpenModalEditQuiz);
    setQuizSelected(quizData);
    setCodQuiz(idQuiz);
  };

  const handleOpenModalViewQuestionsQuiz = (idQuiz: number, quizData: QuizModel) => {
    setIsOpenModalViewQuestionsQuiz(!isOpenModalViewQuestionsQuiz);
    setCodQuiz(idQuiz);
    getQuestionsQuiz(idQuiz);
    setQuizSelected(quizData);
  };

  const getQuestionsQuiz = async (idQuiz: number) => {
    startLoading();
    setQuestions([]);
    await new ListQuestionsUseCase()
      .handle({
        quiz: idQuiz,
        word: "",
      })
      .then((data) => {
        setQuestions(data);
      })
      .catch(() => {
        toast.error("Falha ao carregar perguntas.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  const handleDeleteQuizQuestion = async (idQuizQuestion: number) => {
    startLoading();

    await new DeleteQuestionUseCase()
      .handle({
        IDGDA_QUIZ_QUESTION: idQuizQuestion,
        VALIDATED: true,
      })
      .then((data) => {
        getQuestionsQuiz(codQuiz);
        setIsOpenModalCreateQuestion(false);
        toast.success(data, {
          position: "bottom-left",
        });
      })
      .catch((er) => {
        if (er.response.data.Message) {
          toast.error(er.response.data.Message, {
            position: "bottom-left",
          });
        } else {
          toast.error("Falha ao deletar pergunta.", {
            position: "bottom-left",
          });
        }
      })
      .finally(() => {
        finishLoading();
      });
  };

  const handleOpenModalEditQuestion = (
    idQuiz: number,
    quizQuestionData: ListQuestionsResponseModel
  ) => {
    setIsOpenModalEditQuestion(!isOpenModalEditQuestion);
    setQuestionSelected(quizQuestionData);
    setCodQuiz(idQuiz);
  };

  const getTypeQuestionsQuiz = async () => {
    startLoading();

    await new ListTypeQuestionsUseCase()
      .handle()
      .then((data) => {
        setTypesQuestions(data);
      })
      .catch((er) => {
        toast.error("Falha ao carregar os tipos de pergunta.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    getTypeQuestionsQuiz();
  }, []);

  const handleUploadFilesQuestion = (event: any) => {
    const files = event.target.files;

    if (files.length > 0) {
      const imagesArray = Array.from(files);

      imagesArray.forEach((file, index) => {
        const reader = new FileReader();
        reader.readAsDataURL(file as File);
        reader.onloadend = () => {};
      });

      setImage(imagesArray as File[]);
    }
  };

  const handleRemoveImageQuestion = (imageRemove: any) => {
    const updatedImages = image.filter((img) => img.name !== imageRemove.name);
    setImage(updatedImages);
  };

  const handleCreateQuestion = async () => {
    startLoading();

    await new CreateQuestionsUseCase()
      .handle({
        IDGDA_QUIZ: codQuiz,
        IDGDA_QUIZ_QUESTION_TYPE:
          typeQuestionSelected?.IDGDA_QUIZ_QUESTION_TYPE || 0,
        QUESTION: questionDescription,
        TIME_ANSWER: timeQuestion,
        file: image,
      })
      .then((data) => {
        setTypeQuestionSelected(null);
        setQuestionDescription("");
        setTimeQuestion(0);
        setImage([]);
        getQuestionsQuiz(codQuiz);
        setIsOpenModalCreateQuestion(false);
        toast.success(data, {
          position: "bottom-left",
        });
      })
      .catch(() => {
        toast.error("Falha ao criar pergunta.", {
          position: "bottom-left",
        });
      })
      .finally(() => {
        finishLoading();
      });
  };

  const handleOpenModalAnswers = (idQuestionParam: number) => {
    setIdQuestion(idQuestionParam);
    setIsOpenModalViewAnswer(!isOpenModalViewAnswer);
    geAnswersQuestion(idQuestionParam);
  };

  const geAnswersQuestion = async (idQuestionParam: number) => {
    startLoading();

    await new ListAnswerUseCase()
      .handle({
        idquestion: idQuestionParam,
        word: "",
      })
      .then((data) => {
        setAnswers(data);
      })
      .catch((er) => {
        toast.error("Falha ao carregar as respostas.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  const handleUploadFilesAnswer = (event: any) => {
    const files = event.target.files;

    if (files.length > 0) {
      const imagesArray = Array.from(files);

      imagesArray.forEach((file, index) => {
        const reader = new FileReader();
        reader.readAsDataURL(file as File);
        reader.onloadend = () => {};
      });

      setImageAnswer(imagesArray as File[]);
    }
  };

  const handleRemoveImageAnswer = (imageRemove: any) => {
    const updatedImages = image.filter((img) => img.name !== imageRemove.name);
    setImageAnswer(updatedImages);
  };

  const handleCreateAnswer = async () => {
    startLoading();

    await new CreateAnswerUseCase()
      .handle({
        IDGDA_QUIZ_QUESTION: idQuestion,
        QUESTION: answerDescription,
        RIGHT_ANSWER: answerCorrect,
        file: imageAnsWer,
      })
      .then((data) => {
        setAnswerDescription("");
        setAnswerCorrect(false);
        setImageAnswer([]);
        setIsOpenModalCreateAnswer(false);
        geAnswersQuestion(idQuestion);
        toast.success(data, {
          position: "bottom-left",
        });
      })
      .catch(() => {
        toast.error("Falha ao criar resposta.", {
          position: "bottom-left",
        });
      })
      .finally(() => {
        finishLoading();
      });
  };

  const handleDeleteQuizAnswer = async (idQuizAnswer: number) => {
    startLoading();

    await new DeleteAnswerUseCase()
      .handle({
        IDGDA_QUIZ_ANSWER: idQuizAnswer,
        VALIDATED: true,
      })
      .then((data) => {
        geAnswersQuestion(idQuizAnswer);
        toast.success(data, {
          position: "bottom-left",
        });
      })
      .catch((er) => {
        if (er.response.data.Message) {
          toast.error(er.response.data.Message, {
            position: "bottom-left",
          });
        } else {
          toast.error("Falha ao deletar pergunta.", {
            position: "bottom-left",
          });
        }
      })
      .finally(() => {
        finishLoading();
      });
  };

  const handleOpenModalEditAnswer = (
    idQuiz: number,
    quizQuestionAnswer: ListAnswerResponseModel
  ) => {
    setIsOpenModalEditAnswer(!isOpenModalEditAnswer);
    setAnswerSelected(quizQuestionAnswer);
    setCodQuiz(idQuiz);
  };

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

              textDecoration: "none",
            }}
            color={theme.palette.background.default}
          >
            <Typography fontWeight={"700"}>Quiz</Typography>
          </Link>
        </Breadcrumbs>
      </Stack>
      <ContentArea sx={{ py: "40px" }}>
        <PageTitle
          icon={<FactCheckOutlined sx={{ fontSize: "40px" }} />}
          title="Quiz"
          loading={isLoading}
        >
          <Stack
            direction={"row"}
            alignItems={"center"}
            gap={"16px"}
            width={"100%"}
            justifyContent={"flex-end"}
          >
            <Button
              variant={"contained"}
              onClick={() => setIsOpenModalCreateQuiz(!isOpenModalCreateQuiz)}
              disabled={false}
              endIcon={<AddCircleOutline />}
            >
              Criar novo Quiz
            </Button>
            <Button variant={"contained"} onClick={() => setIsOpenFilter(true)}>
              Filtrar
            </Button>
            <Drawer
              open={isOpenModalCreateQuiz}
              title={`Criar Quiz`}
              onClose={() => setIsOpenModalCreateQuiz(!isOpenModalCreateQuiz)}
              anchor={"right"}
              PaperProps={{
                sx: {
                  borderTopLeftRadius: "16px",
                  borderBottomLeftRadius: "16px",
                },
              }}
            >
              <Box
                width={"100%"}
                display={"flex"}
                flexDirection={"column"}
                gap={"24px"}
                p={"24px"}
                minWidth={"400px"}
              >
                <Typography fontSize={"18px"} fontWeight={"700"} mb={"10px"}>
                  Criar Quiz
                </Typography>
                <Autocomplete
                  placeholder={"Demandante"}
                  onChange={(_, demandant) => setDemandantSelected(demandant)}
                  onInputChange={(e, text) =>
                    setSearchTextPersonaDemandant(text)
                  }
                  isOptionEqualToValue={(option, value) =>
                    option.NOME === value.NOME
                  }
                  renderInput={(props) => (
                    <TextField {...props} size={"small"} label={"Demandante"} />
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
                  placeholder={"Responsável"}
                  onChange={(_, demandant) => setResponsibleSelected(demandant)}
                  onInputChange={(e, text) =>
                    setSearchTextPersonaResponsible(text)
                  }
                  isOptionEqualToValue={(option, value) =>
                    option.NOME === value.NOME
                  }
                  renderInput={(props) => (
                    <TextField
                      {...props}
                      size={"small"}
                      label={"Responsável"}
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
                <TextField
                  value={titleQuiz}
                  label="Título"
                  onChange={(e) => setTitleQuiz(e.target.value)}
                />
                <TextField
                  multiline
                  value={descriptionQuiz}
                  onChange={(e) => setDescriptionQuiz(e.target.value)}
                  rows={4}
                  placeholder="Descrição"
                />
                <TextField
                  value={orientation}
                  multiline
                  rows={2}
                  label="Orientação"
                  onChange={(e) => setOrientation(e.target.value)}
                />
                <LocalizationProvider dateAdapter={AdapterDateFns}>
                  <DateTimePicker
                    label="Data e hora de início"
                    value={dateStartQuiz}
                    onChange={(value) => setDateStartQuiz(value)}
                  />
                </LocalizationProvider>
                <LocalizationProvider dateAdapter={AdapterDateFns}>
                  <DateTimePicker
                    label="Data e hora de fim"
                    value={dateEndQuiz}
                    onChange={(value) => setDateEndQuiz(value)}
                  />
                </LocalizationProvider>
                <Stack direction={"row"} alignItems={"center"} gap={"30px"}>
                  <Checkbox
                    checked={requiredQuiz}
                    onChange={(e) => setRequiredQuiz(e.target.checked)}
                  />
                  <Typography variant="caption" sx={{ textWrap: "nowrap" }}>
                    Quiz Obrigatório?
                  </Typography>
                </Stack>
                <TextField
                  value={monetizatioValue}
                  defaultValue={0}
                  label="Moedas monetização"
                  type="number"
                  onChange={(e) => setMonetizationValue(Number(e.target.value))}
                />
                <TextField
                  value={percentMonetizatioValue}
                  defaultValue={0}
                  label="Percentual de resposta"
                  type="number"
                  onChange={(e) =>
                    setPercentMonetizationValue(Number(e.target.value))
                  }
                />
                <Divider />
                <Typography variant="h3">Visibilidade</Typography>
                <Autocomplete
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
                    <TextField {...props} label={"Setor"} />
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
                  multiple
                  fullWidth
                  value={selectedSubSector}
                  options={subSectors}
                  getOptionLabel={(option) => option.name}
                  onChange={(event, value) => {
                    setSelectedSubSector(value);
                  }}
                  onInputChange={(e, text) => setSubSectorSearch(text)}
                  filterOptions={(x) => x}
                  filterSelectedOptions
                  renderInput={(props) => (
                    <TextField {...props} label={"Subsetor"} />
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
                  multiple
                  fullWidth
                  value={selectedPeriods}
                  options={periods}
                  getOptionLabel={(option) => option.name}
                  onChange={(event, value) => {
                    setSelectedPeriods(value);
                  }}
                  filterSelectedOptions
                  renderInput={(props) => (
                    <TextField {...props} label={"Turno"} />
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
                  multiple
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
                  multiple
                  fullWidth
                  disableClearable={false}
                  value={selectedCollaborator}
                  options={collaborators}
                  getOptionLabel={(option) => option.name}
                  onChange={(event, value) => {
                    setSelectedCollaborator(value);
                  }}
                  onInputChange={(e, text) => setCollaboratorSearch(text)}
                  filterSelectedOptions
                  renderInput={(props) => (
                    <TextField {...props} label={"Colaboradores"} />
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
                  multiple
                  fullWidth
                  value={selectedHomeFloor}
                  options={homesFloors}
                  getOptionLabel={(option) => option.name}
                  onChange={(event, value) => {
                    setSelectedHomeFloor(value);
                  }}
                  filterSelectedOptions
                  renderInput={(props) => (
                    <TextField {...props} label={"Home/Piso"} />
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
                  multiple
                  fullWidth
                  value={selectedSites}
                  options={sites}
                  getOptionLabel={(option) => option.name}
                  onChange={(event, value) => {
                    setSelectedSites(value);
                  }}
                  filterSelectedOptions
                  renderInput={(props) => (
                    <TextField {...props} label={"Sites"} />
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
                <ActionButton
                  title={"Criar quiz"}
                  loading={isLoading}
                  isActive={false}
                  onClick={() => handleCreateQuiz()}
                />
              </Box>
            </Drawer>
          </Stack>
        </PageTitle>
        <Divider />

        <Stack
          direction={"row"}
          justifyContent={"flex-end"}
          width={"100%"}
          py={"15px"}
        >
          {selectedQuizzes.length > 0 && (
            <Button
              color="error"
              variant="contained"
              size="small"
              onClick={() => setIsOpenDelete2(true)}
            >
              Apagar selecionados
            </Button>
          )}
          <ConfirmModal
            onClose={() => setIsOpenDelete2(false)}
            onConfirm={handleClearQuiz}
            open={isOpenDelete2}
          />
          <Button
            color="secondary"
            sx={{ fontWeight: "700", color: "secondary.main" }}
            onClick={() => {
              setSelectedQuizzes(quizzes.map((item) => item.codQuiz));
            }}
          >
            Selecionar todos
          </Button>
          <Button
            color="error"
            sx={{ fontWeight: "700", color: "error.main" }}
            onClick={() => {
              setSelectedQuizzes([]);
            }}
          >
            Remover seleções
          </Button>
        </Stack>
        <Stack gap={"24px"}>
          {quizzes.length > 0 || isLoading ? (
            quizzes.map((quiz, index) => (
              <Box
                key={index}
                border={`solid 1px ${
                  quiz.status == "Finalizado"
                    ? theme.palette.secondary.main
                    : theme.palette.primary.main
                }`}
                display={"flex"}
                width={"100%"}
                borderRadius={"16px"}
                p={"24px"}
                alignItems={"center"}
                justifyContent={"space-between"}
              >
                <Box
                  display={"flex"}
                  alignItems={"center"}
                  gap={2}
                  width={"100%"}
                >
                  <Box width={"100%"}>
                    <Stack
                      width={"100%"}
                      justifyContent={"space-between"}
                      alignItems={"center"}
                      direction={"row"}
                    >
                      <Stack>
                        <Typography
                          variant="body2"
                          fontSize={"16px"}
                          fontWeight={"600"}
                          fontFamily={"Montserrat"}
                        >
                          {capitalizeText(quiz.title || "")}
                          <Typography
                            variant="body2"
                            component={"span"}
                            fontSize={"16px"}
                            fontWeight={"600"}
                            fontFamily={"Montserrat"}
                            color={
                              quiz.status == "Finalizado"
                                ? "secondary"
                                : "primary"
                            }
                            ml={1}
                          >
                            #{quiz.codQuiz}
                          </Typography>
                          <Typography
                            variant="body2"
                            fontSize={"16px"}
                            component={"span"}
                            fontWeight={"600"}
                            fontFamily={"Montserrat"}
                            color={"text.secondary"}
                          >
                            {" "}
                            {quiz.description && `- ${quiz.description}`}
                          </Typography>
                        </Typography>

                        <Typography
                          variant="body2"
                          fontSize={"14px"}
                          mt={"8px"}
                        >
                          Criado em{" "}
                          {format(new Date(quiz.createdAt), "dd/MM/yyyy")}
                          {quiz.startedAt ? ` - Iniciado em ${DateUtils.formatDateTimePtBR(quiz.startedAt)}` : ``}
                          {quiz.endedAt ? ` - Finalizado em ${DateUtils.formatDateTimePtBR(quiz.endedAt)}` : ``}
                        </Typography>
                      </Stack>
                      <Box display={"flex"} gap={1} alignItems={"center"}>
                        <Button
                          color="primary"
                          variant="text"
                          sx={{
                            fontWeight: "700",
                            color:
                              quiz.status == "Finalizado"
                                ? theme.palette.secondary.main
                                : theme.palette.primary.main,
                          }}
                          onClick={() =>
                            handleOpenModalViewQuestionsQuiz(quiz.codQuiz, quiz)
                          }
                        >
                          Ver +
                        </Button>
                        <Button
                          color="primary"
                          sx={{
                            fontWeight: "700",
                            color:
                              quiz.status == "Finalizado"
                                ? theme.palette.secondary.main
                                : theme.palette.primary.main,
                          }}
                          variant="text"
                          onClick={() => handleDuplicateQuiz(quiz.codQuiz)}
                        >
                          Duplicar
                        </Button>
                        {quiz.sendedAt == null ? (
                          <Button
                            color="primary"
                            sx={{
                              fontWeight: "700",
                              color:
                                quiz.status == "Finalizado"
                                  ? theme.palette.secondary.main
                                  : theme.palette.primary.main,
                            }}
                            variant="text"
                            onClick={() =>
                              handleOpenModalEditQuiz(quiz.codQuiz, quiz)
                            }
                          >
                            Editar
                          </Button>
                        ) : null}
                        <Button
                          color="error"
                          disabled={false}
                          sx={{
                            fontWeight: "700",
                            color: theme.palette.error.main,
                          }}
                          variant="text"
                          onClick={() => setIsOpenDelete3(quiz.codQuiz)}
                        >
                          Excluir
                        </Button>

                        <Checkbox
                          onClick={() => handleOnSelect(quiz.codQuiz)}
                          checked={selectedQuizzes.includes(quiz.codQuiz)}
                        />
                      </Box>
                    </Stack>
                    <Stack
                      direction={"row"}
                      alignItems={"center"}
                      mt={"30px"}
                      gap={"83px"}
                    >
                      <Stack
                        direction={"row"}
                        alignItems={"center"}
                        gap={"9px"}
                      >
                        <ProfileImage
                          width="50px"
                          height="50px"
                          name={quiz.nameCreator}
                        />
                        <Typography fontSize={"14px"} fontWeight={"600"}>
                          {capitalizeText(quiz.nameCreator || "")}
                        </Typography>
                      </Stack>
                      <Stack direction={"column"} gap={"9px"}>
                        <Typography fontSize={"14px"} fontWeight={"400"}>
                          Status
                        </Typography>
                        <Typography
                          fontSize={"14px"}
                          fontWeight={"700"}
                          color={
                            quiz.status == "Finalizado"
                              ? "secondary"
                              : "primary"
                          }
                        >
                          {quiz.status}
                        </Typography>
                      </Stack>
                    </Stack>
                    {/* <Typography
                                            variant="body2"
                                            fontSize={"13px"}
                                            mt={1}
                                            fontWeight={"500"}
                                        >
                                            Criado por: {quiz.nameCreator}{" "}
                                        </Typography>
                                        <Chip
                                            size="small"
                                            sx={{ mt: "10px" }}
                                            label={quiz.status}
                                        /> */}
                  </Box>
                </Box>
              </Box>
            ))
          ) : (
            <EmptyState title={`Nenhum quiz encontrado`} />
          )}
        </Stack>
        <ModalEditQuiz
          //idQuiz={codQuiz}
          idQuiz={0}
          quiz={quizSelected}
          isOpenModal={isOpenModalEditQuiz}
          onClose={() => {
            setIsOpenModalEditQuiz(!isOpenModalEditQuiz);
            ListQuiz();
          }}
        />
        <Drawer
          open={isOpenModalViewQuestionsQuiz}
          anchor={"right"}
          title={`Perguntas`}
          PaperProps={{
            sx: {
              borderTopLeftRadius: "16px",
              borderBottomLeftRadius: "16px",
            },
          }}
          onClose={() =>
            setIsOpenModalViewQuestionsQuiz(!isOpenModalViewQuestionsQuiz)
          }
        >
          <Box
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            gap={1}
            p={"24px"}
          >
            <Stack
              direction={"row"}
              alignItems={"center"}
              gap={"16px"}
              width={"100%"}
              justifyContent={"flex-end"}
            >
              <Typography>{}</Typography>
              <Button
                variant={"contained"}
                onClick={() =>
                  setIsOpenModalCreateQuestion(!isOpenModalCreateQuiz)
                }
                disabled={quizSelected?.sendedAt ? true : false}
              >
                Criar Pergunta
              </Button>
              <Drawer
                anchor={"right"}
                PaperProps={{
                  sx: {
                    borderTopLeftRadius: "16px",
                    borderBottomLeftRadius: "16px",
                  },
                }}
                open={isOpenModalCreateQuestion}
                title={`Criar pergunta`}
                onClose={() =>
                  setIsOpenModalCreateQuestion(!isOpenModalCreateQuestion)
                }
              >
                <Box
                  width={"100%"}
                  p={"24px"}
                  minWidth={"400px"}
                  display={"flex"}
                  flexDirection={"column"}
                  gap={1}
                >
                  <Typography fontSize={"18px"} fontWeight={"700"} mb={"20px"}>
                    Criar pergunta
                  </Typography>
                  <Autocomplete
                    placeholder={"Tipo"}
                    onChange={(_, type) => setTypeQuestionSelected(type)}
                    isOptionEqualToValue={(option, value) =>
                      option.TYPE === value.TYPE
                    }
                    renderInput={(props) => (
                      <TextField {...props} size={"small"} label={"Tipo"} />
                    )}
                    renderOption={(props, option) => {
                      return (
                        <li {...props} key={option.IDGDA_QUIZ_QUESTION_TYPE}>
                          {option.TYPE}
                        </li>
                      );
                    }}
                    renderTags={() => null}
                    getOptionLabel={(option) => option.TYPE}
                    options={typesQuestions || []}
                    sx={{ mb: 0 }}
                  />
                  <TextField
                    multiline
                    value={questionDescription}
                    onChange={(e) => setQuestionDescription(e.target.value)}
                    rows={4}
                    placeholder="Escreva aqui a pergunta..."
                  />
                  <TextField
                    value={timeQuestion}
                    label="Tempo para resposta (em segundos)"
                    type="number"
                    onChange={(e) => setTimeQuestion(Number(e.target.value))}
                  />
                </Box>
                <Box
                  mt={3}
                  display={"flex"}
                  flexDirection={"column"}
                  gap={1}
                  p={"24px"}
                  minWidth={"400px"}
                >
                  <input
                    accept="image/*"
                    style={{ display: "none" }}
                    id="image-upload"
                    type="file"
                    onChange={handleUploadFilesQuestion}
                    multiple
                  />
                  {image.map((img, index) => (
                    <List dense={true} key={index}>
                      <ListItem
                        secondaryAction={
                          <IconButton
                            edge="end"
                            aria-label="delete"
                            onClick={() => handleRemoveImageQuestion(img)}
                          >
                            <DeleteIcon />
                          </IconButton>
                        }
                      >
                        <ListItemIcon>
                          <FolderIcon />
                        </ListItemIcon>
                        <ListItemText
                          primary={img.name}
                          primaryTypographyProps={{
                            fontFamily: "Open Sans",
                          }}
                        />
                      </ListItem>
                    </List>
                  ))}
                  <label htmlFor="image-upload">
                    <Button
                      variant="outlined"
                      color="primary"
                      component="span"
                      fullWidth
                    >
                      Selecionar arquivo
                    </Button>
                  </label>
                  <Button
                    variant={"outlined"}
                    onClick={() => handleCreateQuestion()}
                    disabled={false}
                    size={"small"}
                  >
                    Salvar
                  </Button>
                </Box>
              </Drawer>
              <Drawer
                anchor={"right"}
                PaperProps={{
                  sx: {
                    borderTopLeftRadius: "16px",
                    borderBottomLeftRadius: "16px",
                  },
                }}
                open={isOpenModalViewAnswer}
                title={`Respostas`}
                onClose={() => setIsOpenModalViewAnswer(!isOpenModalViewAnswer)}
              >
                <Box
                  width={"100%"}
                  display={"flex"}
                  flexDirection={"column"}
                  gap={1}
                  p={"24px"}
                  minWidth={"400px"}
                >
                  <Stack
                    direction={"row"}
                    alignItems={"center"}
                    gap={"16px"}
                    width={"100%"}
                    justifyContent={"space-between"}
                    mb={"20px"}
                  >
                    <Typography fontSize={"18px"} fontWeight={"700"}>
                      Respostas
                    </Typography>
                    <Button
                      variant={"contained"}
                      onClick={() =>
                        setIsOpenModalCreateAnswer(!isOpenModalCreateAnswer)
                      }
                      disabled={quizSelected?.sendedAt ? true : false}
                    >
                      Criar Resposta
                    </Button>
                    <Drawer
                      anchor={"right"}
                      PaperProps={{
                        sx: {
                          borderTopLeftRadius: "16px",
                          borderBottomLeftRadius: "16px",
                        },
                      }}
                      open={isOpenModalCreateAnswer}
                      title={`Criar resposta`}
                      onClose={() =>
                        setIsOpenModalCreateAnswer(!isOpenModalCreateAnswer)
                      }
                    >
                      <Box
                        width={"100%"}
                        display={"flex"}
                        flexDirection={"column"}
                        gap={1}
                        p={"24px"}
                        minWidth={"400px"}
                      >
                        <Typography
                          fontSize={"18px"}
                          fontWeight={"700"}
                          mb={"20px"}
                        >
                          Criar resposta
                        </Typography>
                        <TextField
                          value={answerDescription}
                          onChange={(e) => setAnswerDescription(e.target.value)}
                          rows={4}
                          placeholder="Resposta"
                        />
                        <Stack
                          direction={"row"}
                          alignItems={"center"}
                          gap={"30px"}
                        >
                          <Checkbox
                            checked={answerCorrect}
                            onChange={(e) => setAnswerCorrect(e.target.checked)}
                          />
                          <Typography
                            variant="caption"
                            sx={{
                              textWrap: "nowrap",
                            }}
                          >
                            Resposta certa?
                          </Typography>
                        </Stack>
                        <Box
                          mt={3}
                          display={"flex"}
                          flexDirection={"column"}
                          gap={1}
                        >
                          <input
                            accept="image/*"
                            style={{
                              display: "none",
                            }}
                            id="image-upload-answer"
                            type="file"
                            onChange={handleUploadFilesAnswer}
                            multiple
                          />
                          {imageAnsWer.map((img, index) => (
                            <List dense={true} key={index}>
                              <ListItem
                                secondaryAction={
                                  <IconButton
                                    edge="end"
                                    aria-label="delete"
                                    onClick={() => handleRemoveImageAnswer(img)}
                                  >
                                    <DeleteIcon />
                                  </IconButton>
                                }
                              >
                                <ListItemIcon>
                                  <FolderIcon />
                                </ListItemIcon>
                                <ListItemText
                                  primary={img.name}
                                  primaryTypographyProps={{
                                    fontFamily: "Open Sans",
                                  }}
                                />
                              </ListItem>
                            </List>
                          ))}
                          <label htmlFor="image-upload-answer">
                            <Button
                              variant="outlined"
                              color="primary"
                              component="span"
                              fullWidth
                            >
                              Selecionar arquivo
                            </Button>
                          </label>
                          <Button
                            variant={"outlined"}
                            onClick={() => handleCreateAnswer()}
                            disabled={false}
                            size={"small"}
                          >
                            Salvar
                          </Button>
                        </Box>
                      </Box>
                    </Drawer>
                  </Stack>
                  {answers.length > 0
                    ? answers.map((answer, index) => (
                        <ListItemAnswer
                          key={index}
                          answer={answer}
                          disableButtons={quizSelected?.sendedAt ? true : false}
                          onDelete={() =>
                            handleDeleteQuizAnswer(answer.IDGDA_QUIZ_ANSWER)
                          }
                          onViewAnswers={() => {
                            return true;
                          }}
                          onEdit={() => handleOpenModalEditAnswer(0, answer)}
                        />
                      ))
                    : "Sem respostas cadastradas"}
                  <ModalEditAnswer
                    idAnswer={0}
                    answer={answerSelected}
                    isOpenModal={isOpenModalEditAnswer}
                    onClose={() => {
                      setIsOpenModalEditAnswer(!isOpenModalEditAnswer);
                      geAnswersQuestion(idQuestion);
                    }}
                  />
                </Box>
              </Drawer>
            </Stack>
            {questions.length > 0
              ? questions.map((question, index) => (
                  <ListItemQuestion
                    key={index}
                    question={question}
                    disableButtons={quizSelected?.sendedAt ? true : false}
                    onDelete={() =>
                      handleDeleteQuizQuestion(question.IDGDA_QUIZ_QUESTION)
                    }
                    onViewAnswers={() =>
                      handleOpenModalAnswers(question.IDGDA_QUIZ_QUESTION)
                    }
                    onEdit={() =>
                      handleOpenModalEditQuestion(codQuiz, question)
                    }
                  />
                ))
              : "Sem perguntas cadastradas"}
            <ModalEditQuestion
              idQuestion={0}
              question={questionSelected}
              isOpenModal={isOpenModalEditQuestion}
              onClose={() => {
                setIsOpenModalEditQuestion(!isOpenModalEditQuestion);
                getQuestionsQuiz(codQuiz);
              }}
            />
          </Box>
        </Drawer>
        <Stack justifyContent={"flex-end"} direction={"row"}>
          <Pagination
            sx={{ mt: "20px" }}
            count={totalPages || 0}
            page={page}
            onChange={handleChangePagination}
            disabled={isLoading}
          />
        </Stack>
      </ContentArea>
      {isOpenDelete3 && (
        <ConfirmModal
          onClose={() => setIsOpenDelete3(null)}
          onConfirm={() => handleDeleteQuiz(isOpenDelete3)}
          open={isOpenDelete3}
        />
      )}
      <Drawer
        open={isOpenFilter}
        anchor={"right"}
        onClose={() => setIsOpenFilter(false)}
        PaperProps={{
          sx: {
            borderTopLeftRadius: "16px",
            borderBottomLeftRadius: "16px",
          },
        }}
      >
        <Stack
          width={"100vw"}
          minWidth={"100%"}
          maxWidth={"430px"}
          direction={"column"}
          height={"100vh"}
          gap={"20px"}
          py={"40px"}
          px={"35px"}
        >
          <Typography
            fontSize={"20px"}
            fontWeight={"600"}
            alignItems={"center"}
            gap={"10px"}
            display={"flex"}
            flexDirection={"row"}
          >
            <FilterList />
            Filtar
          </Typography>
          <TextField
            value={titleSearch}
            label="Título"
            onChange={(e) => setTitleSearch(e.target.value)}
            style={{ width: "100%" }}
          />
          <Autocomplete
            multiple
            style={{ width: "100%" }}
            disableClearable={false}
            value={selectedSitesFilter}
            options={sites}
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
              setSelectedSitesFilter(value);
            }}
            filterSelectedOptions
            renderInput={(props) => <TextField {...props} label={"Sites"} />}
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
          {/* <Autocomplete
            placeholder={"Quem solicitou"}
            value={demandantFilterSelected}
            disableClearable={false}
            onChange={(_, demandant) => setDemandantFilterSelected(demandant)}
            onInputChange={(e, text) =>
              setSearchTextPersonaDemandantFilter(text)
            }
            isOptionEqualToValue={(option, value) =>
              option.NOME === value.NOME
            }
            renderInput={(props) => (
              <TextField {...props} label={"Quem solicitou"} />
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
          /> */}
          {/* <Autocomplete
            placeholder={"Quem criou"}
            value={responsibleFilterSelected}
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
          /> */}
          {/* <Autocomplete
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
          /> */}
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Data Criação Inicial"
              value={startDate}
              onChange={(newValue) => setStartDate(newValue)}
              slotProps={{
                textField: {
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
              label="Data Criação Final"
              value={endDate}
              onChange={(newValue) => setEndDate(newValue)}
              slotProps={{
                textField: {
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
              label="Data Publicação Inicial"
              value={startDateP}
              onChange={(newValue) => setStartDateP(newValue)}
              slotProps={{
                textField: {
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
              label="Data Publicação Final"
              value={endDateP}
              onChange={(newValue) => setEndDateP(newValue)}
              slotProps={{
                textField: {
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
              label="Data Expiração Inicial"
              value={startDateE}
              onChange={(newValue) => setStartDateE(newValue)}
              slotProps={{
                textField: {
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
              label="Data Expiração Final"
              value={endDateE}
              onChange={(newValue) => setEndDateE(newValue)}
              slotProps={{
                textField: {
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
          <TextField
            value={wordsFilter}
            label="Palavra chave das perguntas"
            onChange={(e) => setWordsFilter(e.target.value)}
            style={{ width: "100%" }}
          />
          <Autocomplete
            style={{ width: "100%" }}
            disableClearable={false}
            options={statusFilter}
            getOptionLabel={(option) => option.name}
            onInputChange={(e, text) => setStatusFilterSelected(text)}
            filterSelectedOptions
            renderInput={(props) => <TextField {...props} label={"Status"} />}
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
              <TextField {...props} label={"Colaboradores"} />
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
          <Stack
            width={"100%"}
            direction={"row"}
            alignItems={"center"}
            border={`solid 1px ${theme.palette.grey[300]}`}
            borderRadius={"8px"}
            px={"24px"}
            sx={{ cursor: "pointer" }}
            py={"10px"}
            onClick={() => setRelevance(!relevance)}
            justifyContent={"space-between"}
          >
            <Typography fontSize={"16px"} fontWeight={"500"}>
              Relevância
            </Typography>
            <Checkbox checked={relevance} />
          </Stack>
        </Stack>
      </Drawer>
    </ContentCard>
  );
}

ListQuizzes.getLayout = getLayout("private");
