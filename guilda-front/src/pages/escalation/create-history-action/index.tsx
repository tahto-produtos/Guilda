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
import {
  LoadActionEscalation,
  LoadLibraryActionEscalationUseCase,
} from "src/modules/escalation/use-cases/load-library-action-escalation";
import { CreateInfractionButton } from "src/modules/feedback/fragments/create-infraction-button";
import { getLayout } from "src/utils";
import { capitalizeText } from "src/utils/capitalizeText";

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
  const [realize, setRealize] = useState<string>("");
  const actionId = router.query.action;
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

  async function handleCreate() {
    if (!actionId) return;

    startLoading();

    await new CreateHistoryActionUseCase()
      .handle({
        ACTION_REALIZE: realize,
        IDGDA_ESCALATION_ACTION: parseInt(actionId.toString()),
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
            <Typography fontWeight={"700"}>
              Criação de histórico de ação
            </Typography>
          </Link>
        </Breadcrumbs>
      </Stack>
      <ContentArea sx={{ py: " 40px" }}>
        <Stack px={"40px"}>
          <PageTitle
            icon={<ListAltOutlined sx={{ fontSize: "40px" }} />}
            title="Criação de histórico de ação"
            loading={isLoading}
          ></PageTitle>
          <Divider />
          <Stack py={"40px"} direction={"row"} gap="16px">
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
              onChange={(e) => setRealize(e.target.value)}
              value={realize}
              label="Realizar"
              fullWidth
            />
          </Stack>
          <Stack direction={"row"}>
            <Button
              variant="contained"
              disabled={!actionId}
              onClick={handleCreate}
            >
              Confirmar e criar
            </Button>
          </Stack>
        </Stack>
      </ContentArea>
    </ContentCard>
  );
}

CreateHistoryActionView.getLayout = getLayout("private");
