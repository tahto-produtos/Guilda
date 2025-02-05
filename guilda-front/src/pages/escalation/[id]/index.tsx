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
  LinearProgress,
  Link,
  Stack,
  TextField,
  Typography,
  lighten,
  useTheme,
} from "@mui/material";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { format } from "date-fns";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ActionButton } from "src/components/inputs/action-button/action-button";
import { TextFieldTitle } from "src/components/inputs/title-text-field/title-text-field";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { QuizContext } from "src/contexts/quiz-provider/quiz.context";
import { useDebounce, useLoadingState } from "src/hooks";
import { ActionPlanTable } from "src/modules/escalation/fragments/action-plan-table";
import { PerformanceIndicator } from "src/modules/escalation/fragments/performance-indicator";
import { CreateHistoryActionUseCase } from "src/modules/escalation/use-cases/create-history-action.use-case";
import { DeleteActionUseCase } from "src/modules/escalation/use-cases/delete-action.use-case";
import { DeleteStageUseCase } from "src/modules/escalation/use-cases/delete-stage.use-case";
import {
  LoadActionEscalation,
  LoadLibraryActionEscalationUseCase,
} from "src/modules/escalation/use-cases/load-library-action-escalation";
import { ShowActionDetailsUseCase } from "src/modules/escalation/use-cases/show-action-details.use-case";
import { CreateInfractionButton } from "src/modules/feedback/fragments/create-infraction-button";
import { InfractionsTable } from "src/modules/feedback/fragments/infractions-table";
import { ActionDetails } from "src/typings/models/action-details.model";
import { getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";
import { capitalizeText } from "src/utils/capitalizeText";
import { uuid } from "uuidv4";

export default function CreateHistoryActionView() {
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [searchText, setSearchText] = useState<string>("");
  const debouncedSearchText: string = useDebounce<string>(searchText, 400);
  const router = useRouter();
  const { id } = router.query;
  const theme = useTheme();
  const { myPermissions } = useContext(PermissionsContext);

  const [data, setData] = useState<ActionDetails | null>(null);

  async function getDetails() {
    if (!id) return;

    startLoading();

    const actionId = id.toString().split("-")[0];
    const automatic = id.toString().split("-")[1];

    await new ShowActionDetailsUseCase()
      .handle({
        automatic: (parseInt(automatic) as 0 | 1) || 0,
        idAction: actionId,
      })
      .then((data) => {
        setData(data);
      })
      .catch(() => {})
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    id && getDetails();
  }, [id]);

  async function deleteAction() {
    if (!id) return;

    startLoading();

    const actionId = id.toString().split("-")[0];
    const automatic = id.toString().split("-")[1];

    await new DeleteActionUseCase()
      .handle({
        automatic: (parseInt(automatic) as 0 | 1) || 0,
        idAction: actionId,
      })
      .then((data) => {
        toast.success("Apagado com sucesso!");
      })
      .catch(() => {})
      .finally(() => {
        finishLoading();
      });
  }

  async function deleteStage(id: number) {
    if (!data) return;
    startLoading();

    await new DeleteStageUseCase()
      .handle({
        AUTOMATIC: data.AUTOMATIC,
        IDGDA_ESCALATION_ACTION_STAGE: id,
      })
      .then((data) => {
        toast.success("Apagado com sucesso!");
        getDetails();
      })
      .catch(() => {})
      .finally(() => {
        finishLoading();
      });
  }

  async function editStage(idAction: number, stageNumber: number, hierarchy: string) {
    if (!data || !id) return;

    startLoading();

    router.push({
      pathname: '/escalation/create-stage-action',
      query: {
        action: id.toString().split("-")[0],
        isAutomatic: id.toString().split("-")[1],
        stageNumber: stageNumber,
        deleteId: idAction,
        hierarchy: hierarchy
      }
    });
        finishLoading();
  }

  const columnsColab: GridColDef[] = [
    {
      field: "ID",
      headerName: "Id",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      renderCell: (params) => {
        return (
          <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
            <Typography fontSize={"14px"} fontWeight={"600"}>
              {params.value}
            </Typography>
          </Stack>
        );
      },
    },
    {
      field: "NAME",
      headerName: "Nome",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      minWidth: 300,
      renderCell: (params) => {
        return (
          <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
            <Typography
              fontSize={"16px"}
              fontWeight={"700"}
              color={"secondary"}
            >
              {params.value}
            </Typography>
          </Stack>
        );
      },
    },
  ];

  const columns: GridColDef[] = [
    {
      field: "HIERARCHY",
      headerName: "Hierarquia",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      renderCell: (params) => {
        return (
          <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
            <Typography fontSize={"14px"} fontWeight={"600"}>
              {capitalizeText(params.value || "Sem hierarquia")}
            </Typography>
          </Stack>
        );
      },
    },
    {
      field: "NUMBER_STAGE",
      headerName: "Número do Stage",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      minWidth: 300,
      renderCell: (params) => {
        return (
          <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
            <Typography
              fontSize={"16px"}
              fontWeight={"700"}
              color={"secondary"}
            >
              {params.value}
            </Typography>
          </Stack>
        );
      },
    },
    {
      field: "ID_STAGE",
      headerName: "Ação",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      renderCell: (params) => {
        return (
          <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
            <Button
              variant="text"
              onClick={() => deleteStage(params.value)}
              color={"error"}
              sx={{ color: theme.palette.error.main, fontWeight: 700 }}
            >
              Apagar Stage
            </Button>
            <Button
              variant="text"
              onClick={() => editStage(params.value, params.row.NUMBER_STAGE, params.row.HIERARCHY.toString())}
              color={"primary"}
              sx={{ color: theme.palette.primary.main, fontWeight: 700 }}
              >
                Editar Stage
                </Button>
          </Stack>
        );
      },
    },
  ];

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
              Detalhes do plano de ação
            </Typography>
          </Link>
        </Breadcrumbs>
      </Stack>
      <ContentArea sx={{ py: " 40px" }}>
        <Stack px={"40px"}>
          <PageTitle
            icon={<ListAltOutlined sx={{ fontSize: "40px" }} />}
            title="Detalhes do plano de ação"
            loading={isLoading}
          >
            {id && (
              <Stack direction={"row"} gap={"16px"}>
                <Button
                  variant="contained"
                  onClick={() => {
                    router.push({
                      pathname: '/escalation/create-stage-action',
                      query: {
                      action: id.toString().split("-")[0],
                      isAutomatic: id.toString().split("-")[1],
                      stageNumber: 0,
                      hierarchy: ''
                            }
                        });
                  }}
/*                   disabled={abilityFor(myPermissions).cannot(
                    "Criar Stage",
                    "Escalation")
                  } */
                >
                  Criar ação de Stage
                </Button>
                <Button
                  variant="contained"
                  onClick={() => {
                    router.push(
                      `/escalation/create-history-action?action=${
                        id.toString().split("-")[0]
                      }`
                    );
                  }}
                >
                  Criar histórico de ação
                </Button>
              </Stack>
            )}
          </PageTitle>
          <Divider />
          <Stack direction={"column"} gap={"16px"} mt={"30px"}>
            {data?.AUTOMATIC == 1 && (
              <>
                <Stack direction={"row"} gap={"16px"}>
                  <TextFieldTitle title="Setor">
                    <TextField
                      value={data?.IDGDA_SECTOR}
                      placeholder="Setor"
                      fullWidth
                    />
                  </TextFieldTitle>
                  <TextFieldTitle title="SubSetor">
                    <TextField
                      value={data?.IDGDA_SUBSECTOR}
                      placeholder="SubSetor"
                      fullWidth
                    />
                  </TextFieldTitle>
                </Stack>
                <Stack direction={"row"} gap={"16px"}>
                  <TextFieldTitle title="Grupo">
                    <TextField
                      value={data?.IDGDA_GROUP}
                      placeholder="Grupo"
                      fullWidth
                    />
                  </TextFieldTitle>
                  <TextFieldTitle title="Indicador">
                    <TextField
                      value={data?.IDGDA_INDICATOR}
                      placeholder="Indicador"
                      fullWidth
                    />
                  </TextFieldTitle>
                </Stack>
                <Stack direction={"row"} gap={"16px"}>
                  <TextFieldTitle title="Percentual">
                    <TextField
                      value={data?.PERCENT}
                      placeholder="Percentual"
                      fullWidth
                    />
                  </TextFieldTitle>
                  <TextFieldTitle title="Tolerância">
                    <TextField
                      value={data?.TOLERANCE}
                      placeholder="Tolerância"
                      fullWidth
                    />
                  </TextFieldTitle>
                  <TextFieldTitle title="Automático">
                    <TextField
                      value={"Sim"}
                      placeholder="Automático"
                      fullWidth
                    />
                  </TextFieldTitle>
                </Stack>
              </>
            )}
            {data?.AUTOMATIC == 0 && (
              <>
                <Stack direction={"row"} gap={"16px"}>
                  <TextFieldTitle title="Nome">
                    <TextField
                      value={data?.NAME}
                      placeholder="Nome"
                      fullWidth
                    />
                  </TextFieldTitle>
                  <TextFieldTitle title="Descrição">
                    <TextField
                      value={data?.DESCRIPTION}
                      placeholder="Descrição"
                      fullWidth
                    />
                  </TextFieldTitle>
                </Stack>
                <Stack direction={"row"} gap={"16px"}>
                  <TextFieldTitle title="Inicia em">
                    <TextField
                      value={
                        data?.STARTED_AT &&
                        format(new Date(data?.STARTED_AT), "dd/MM/yyyy")
                      }
                      placeholder="Inicia em"
                      fullWidth
                    />
                  </TextFieldTitle>
                  <TextFieldTitle title="Finaliza em">
                    {" "}
                    <TextField
                      value={
                        data?.ENDED_AT &&
                        format(new Date(data?.ENDED_AT), "dd/MM/yyyy")
                      }
                      placeholder="Finaliza em"
                      fullWidth
                    />
                  </TextFieldTitle>
                  <TextFieldTitle title="Automático">
                    <TextField
                      value={"Não"}
                      placeholder="Automático"
                      fullWidth
                    />
                  </TextFieldTitle>
                </Stack>
              </>
            )}

            <Stack direction={"row"} gap={"16px"}>
              <TextFieldTitle title="Ações realizadas">
                {data?.ACTION_REALIZED.split("#").map((i, index) => (
                  <TextField
                    key={index}
                    value={i}
                    fullWidth
                    style={{ marginBottom: 10 }}
                  />
                ))}
              </TextFieldTitle>
            </Stack>
            <Stack direction={"column"} gap={"16px"}>
              <Typography fontSize={"20px"}>Stages</Typography>
              <DataGrid
                columns={columns}
                rows={data?.STAGES && data.STAGES.length > 0 ? data.STAGES : []}
                hideFooter
                disableColumnFilter
                disableRowSelectionOnClick
                autoHeight
                localeText={{
                  noResultsOverlayLabel: "Nenhum resultado encontrado",
                }}
                loading={isLoading}
                slots={{
                  loadingOverlay: LinearProgress,
                }}
                getRowId={(row) => {
                  const uuidv4 = uuid();
                  return uuidv4;
                }}
                sx={{ width: "100%" }}
              />
              {/* {data?.STAGES.map((i, index) => (
                <Stack
                  key={index}
                  direction={"row"}
                  gap={"16px"}
                  alignItems={"center"}
                >
                  <Typography>Hierarquia: {i.HIERARCHY}</Typography>
                  <Typography>Numero do Stage: {i.NUMBER_STAGE}</Typography>
                  <Button
                    sx={{ color: theme.palette.error.main }}
                    onClick={() => deleteStage(i.ID_STAGE)}
                  >
                    Apagar stage
                  </Button>
                </Stack>
              ))} */}
            </Stack>
            <Stack direction={"column"} gap={"16px"}>
              <Typography fontSize={"20px"}>Colaboradores</Typography>
              <DataGrid
                columns={columnsColab}
                rows={data?.COLLABORATORS && data.COLLABORATORS.length > 0 ? data.COLLABORATORS : []}
                hideFooter
                disableColumnFilter
                disableRowSelectionOnClick
                autoHeight
                localeText={{
                  noResultsOverlayLabel: "Nenhum resultado encontrado",
                }}
                loading={isLoading}
                slots={{
                  loadingOverlay: LinearProgress,
                }}
                getRowId={(row) => {
                  const uuidv4 = uuid();
                  return uuidv4;
                }}
                sx={{ width: "100%" }}
              />
            </Stack>
            <Stack direction={"row"} justifyContent={"flex-end"}>
              <Button color="error" variant="contained" onClick={deleteAction}>
                Deletar Ação
              </Button>
            </Stack>
          </Stack>
        </Stack>
      </ContentArea>
    </ContentCard>
  );
}

CreateHistoryActionView.getLayout = getLayout("private");
