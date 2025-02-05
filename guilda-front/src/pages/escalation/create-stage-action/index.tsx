import {
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
  Divider,
  Link,
  Stack,
  TextField,
  Typography,
  lighten,
  useTheme,
} from "@mui/material";
import { format } from "date-fns";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { QuizContext } from "src/contexts/quiz-provider/quiz.context";
import { useDebounce, useLoadingState } from "src/hooks";
import { ActionPlanTable } from "src/modules/escalation/fragments/action-plan-table";
import { PerformanceIndicator } from "src/modules/escalation/fragments/performance-indicator";
import { CreateHistoryActionUseCase } from "src/modules/escalation/use-cases/create-history-action.use-case";
import { CreateStageActionUseCase } from "src/modules/escalation/use-cases/create-stage-action.use-case";
import {
  LoadActionEscalation,
  LoadLibraryActionEscalationUseCase,
} from "src/modules/escalation/use-cases/load-library-action-escalation";
import { CreateInfractionButton } from "src/modules/feedback/fragments/create-infraction-button";
import { ListHierarchiesEscalationUseCase } from "src/modules/hierarchies/use-cases/list-hierarchies-escalation.use-case";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { getLayout } from "src/utils";
import { capitalizeText } from "src/utils/capitalizeText";
import { DeleteStageUseCase } from "src/modules/escalation/use-cases/delete-stage.use-case";

export default function CreateHistoryActionView() {
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [searchText, setSearchText] = useState<string>("");
  const debouncedSearchText: string = useDebounce<string>(searchText, 400);
  const router = useRouter();
  const theme = useTheme();

  const [actionEscalations, setActionEscalations] = useState<
    LoadActionEscalation[]
  >([]);
  const [actionEscalationSelected, setActionEscalationSelected] =
    useState<LoadActionEscalation | null>(null);
  const [searchAction, setSearchAction] = useState<string>("");
  const [stageNumber, setStageNumber] = useState<number>(0);

  const [hierarchies, setHierarchies] = useState<Hierarchie[]>([]);
  const [selectedHierarchie, setSelectedHierachie] = useState<Hierarchie[]>([]);
  const actionId = router.query.action;
  const isAutomatic = router.query.isAutomatic;
  const loadedStageNumber = router.query.stageNumber;
  const deleteId = router.query.deleteId;
  const hierarchyName = router.query.hierarchy;

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

  async function deleteStage(id: number) {

    startLoading();

    await new DeleteStageUseCase()
      .handle({
        AUTOMATIC: Number(isAutomatic),
        IDGDA_ESCALATION_ACTION_STAGE: Number(deleteId),
      })
      .then((data) => {

      })
      .catch(() => {})
      .finally(() => {
      
      });
  }

  async function handleCreate() {
    if (!actionId || !isAutomatic) return;

    if (stageNumber == 0){
      toast.error("O número deve ser superior a 0.");
      return;
    }

    if(loadedStageNumber !== "0"){
      deleteStage(Number(actionId))
    }

    startLoading();
    await new CreateStageActionUseCase()
      .handle({
        AUTOMATIC: parseInt(isAutomatic.toString()) || 0,
        IDGDA_ESCALATION_ACTION: parseInt(actionId.toString()),
        IDGDA_HIERARCHY: selectedHierarchie.map((i) => i.id),
        NUMBER_STAGE: stageNumber,
      })
      .then((data) => {
        if(loadedStageNumber !== "0"){
          toast.success("Editado com sucesso!");
        }else{
          toast.success("Criado com sucesso!");
        }
        
      })
      .catch(() => {
        toast.error("Falha ao criar.");
      })
      .finally(() => {
        finishLoading();
        router.push(
          `/escalation/${parseInt(actionId.toString())}-${parseInt(isAutomatic.toString()) || 0}`
        );
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
            <Typography fontWeight={"700"}>
              {loadedStageNumber !== "0" ? "Edição de histórico de ação" : "Criação de histórico de ação"}
            </Typography>
          </Link>
        </Breadcrumbs>
      </Stack>
      <ContentArea sx={{ py: " 40px" }}>
        <Stack px={"40px"}>
          <PageTitle
            icon={<ListAltOutlined sx={{ fontSize: "40px" }} />}
            title={loadedStageNumber !== "0" ? "Edição de ação de Stage" : "Criação de ação de Stage"}
            loading={isLoading}
          ></PageTitle>
          <Divider />
          <Stack mt={"40px"} direction={"row"} gap="16px">
            {/* <Autocomplete
              options={actionEscalations}
              getOptionLabel={(option) => option.NOME}
              onChange={(event, value) => {
                setActionEscalationSelected(value);
              }}
              onInputChange={(e, text) => setSearchAction(text)}
              filterOptions={(x) => x}
              filterSelectedOptions
              fullWidth
              renderInput={(params) => (
                <TextField
                  {...params}
                  variant="outlined"
                  label="Selecione uma ação"
                  placeholder="Buscar"
                />
              )}
              renderOption={(props, option) => {
                return (
                  <li {...props} key={option.IDGDA_ESCALATION_ACTION}>
                    {option.NOME}
                  </li>
                );
              }}
              isOptionEqualToValue={(option, value) =>
                option.NOME === value.NOME
              }
            /> */}
            <TextField
              onChange={(e) => setStageNumber(parseInt(e.target.value))}
              placeholder={loadedStageNumber?.toString()}
              InputProps={{
                type: "number",
              }}
              label="Numero do Stage"
              fullWidth
            />
          </Stack>
          <Stack
            direction={"row"}
            alignItems={"center"}
            gap={"30px"}
            mt={"30px"}
          >
            <Autocomplete
  multiple
  fullWidth
  value={selectedHierarchie}
  options={hierarchies}
  getOptionLabel={(option) => option.levelName}
  onChange={(event, value) => {
    setSelectedHierachie(value);
  }}
  filterSelectedOptions
  renderInput={(params) => (
    <TextField
      {...params}
      label={"Hierarquias"}
      placeholder={selectedHierarchie.length > 0 ? "" : hierarchyName?.toString()}
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
  sx={{ m: 0 }}
/>
          </Stack>
          <Stack direction={"row"} mt={"40px"}>
            <Button variant="contained" onClick={handleCreate}>
            {loadedStageNumber !== "0" ? "Confirmar e editar" : "Confirmar e criar"}
            </Button>
          </Stack>
        </Stack>
      </ContentArea>
    </ContentCard>
  );
}

CreateHistoryActionView.getLayout = getLayout("private");
