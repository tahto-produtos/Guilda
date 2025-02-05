import {
  CheckCircle,
  CheckCircleOutline,
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
import DocumentScannerOutlinedIcon from "@mui/icons-material/DocumentScannerOutlined";
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
import { ProfileImage } from "src/components/data-display/profile-image/profile-image";
import { ActionButton } from "src/components/inputs/action-button/action-button";
import { TextFieldTitle } from "src/components/inputs/title-text-field/title-text-field";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
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
import {
  DetailsFeedBackUseCase,
  FeedbackDetail,
} from "src/modules/feedback/use-cases/details-feedback.use-case";
import { SignFeedBackUseCase } from "src/modules/feedback/use-cases/sign-feedback.use-case";
import { ActionDetails } from "src/typings/models/action-details.model";
import { getLayout } from "src/utils";
import { capitalizeText } from "src/utils/capitalizeText";
import { uuid } from "uuidv4";

export default function FeedbackDetails() {
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const { listFeedbackMandatories } = useContext(QuizContext);
  const [searchText, setSearchText] = useState<string>("");
  const debouncedSearchText: string = useDebounce<string>(searchText, 400);
  const router = useRouter();
  const { id, sign } = router.query;
  const theme = useTheme();

  const [data, setData] = useState<FeedbackDetail | null>(null);

  async function getDetails() {
    if (!id) return;

    startLoading();

    const actionId = id.toString().split("-")[0];
    const automatic = id.toString().split("-")[1];

    await new DetailsFeedBackUseCase()
      .handle({
        id: parseInt(id as string),
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

  async function handleSign() {
    if (!data) return;

    await new SignFeedBackUseCase()
      .handle({
        id: data?.idFeedbackUser,
      })
      .then((data) => {
        toast.success("Assinado com sucesso!");
        listFeedbackMandatories();
      })
      .catch(() => {
        toast.error("Erro ao assinar feedback");
      })
      .finally(() => {
        finishLoading();
        router.push("/feedback/my-feedbacks");
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
            onClick={() => {
              if (sign && sign == "1") {
                router.push("/feedback/my-feedbacks");
              } else {
                router.push("/feedback");
              }
            }}
          >
            <Typography fontWeight={"400"}>
              {sign && sign == "1" ? "Meus feedbacks" : "Feedback"}
            </Typography>
          </Link>
          <Link
            sx={{
              display: "flex",
              alignItems: "center",

              textDecoration: "none",
            }}
            color={theme.palette.background.default}
          >
            <Typography fontWeight={"700"}>Detalhes do feedback</Typography>
          </Link>
        </Breadcrumbs>
      </Stack>
      <ContentArea sx={{ py: " 40px" }}>
        <Stack px={"40px"}>
          <PageTitle
            icon={<ListAltOutlined sx={{ fontSize: "40px" }} />}
            title="Informações sobre feedback"
            loading={isLoading}
          ></PageTitle>
          <Divider />
          <Stack direction={"column"} gap={"16px"} mt={"30px"}>
            <Stack
              width={"100%"}
              borderRadius={"16px"}
              border={`solid 1px #e1e1e1`}
              flexDirection={"row"}
              gap={"30px"}
              alignItems={"center"}
              padding={"20px"}
            >
              <Stack flexDirection={"row"} gap={"10px"} alignItems={"center"}>
                <CheckCircleOutline style={{ fontSize: 30 }} color="success" />
                <Typography fontSize={"20px"}>Enviado para</Typography>
              </Stack>
              <Stack flexDirection={"row"} gap={"10px"} alignItems={"center"}>
                <ProfileImage height="50px" width="50px" name={data?.sendFor} />
                <Stack gap={"5px"}>
                  <Typography fontSize={"16px"}>{data?.sendFor}</Typography>
                  <Typography fontSize={"16px"}>
                    {data?.sendForHierarchy}
                  </Typography>
                </Stack>
              </Stack>
            </Stack>
            <Stack direction={"row"} gap={"16px"}>
              <TextFieldTitle title="Título">
                <TextField value={data?.title} placeholder="Título" fullWidth />
              </TextFieldTitle>
              <TextFieldTitle title="Protocolo">
                <TextField
                  value={data?.protocol}
                  placeholder="Protocolo"
                  fullWidth
                />
              </TextFieldTitle>
            </Stack>
            <Stack direction={"row"} gap={"16px"}>
              <TextFieldTitle title="Status da penalidade">
                <TextField
                  value={data?.status}
                  placeholder="Status da penalidade"
                  fullWidth
                />
              </TextFieldTitle>
              <TextFieldTitle title="Infração associada">
                <TextField
                  value={data?.nameInfraction}
                  placeholder="Infração associada"
                  fullWidth
                />
              </TextFieldTitle>
              <TextFieldTitle title="Data da criação">
                <TextField
                  value={
                    data?.createdAt
                      ? format(new Date(data?.createdAt), "dd/MM/yyyy")
                      : ""
                  }
                  placeholder="Data da criação"
                  fullWidth
                />
              </TextFieldTitle>
            </Stack>
            <Stack direction={"row"} gap={"16px"}>
              <TextFieldTitle title="Detalhes">
                <TextField
                    multiline
                    rows={4}
                    placeholder="Detalhes"
                    fullWidth
                    value={data?.content}
                  />
              </TextFieldTitle>
            </Stack>
            {data?.files && data?.files.length > 0 && (
              <Stack direction={"column"} gap={2}>
                <TextFieldTitle title="Documentos">
                  {data?.files.map((item, index) => (
                    <Link
                      underline="hover"
                      sx={{ display: "flex", alignItems: "center" }}
                      href={item.url}
                      key={index}
                      target="_blank"
                    >
                      <DocumentScannerOutlinedIcon
                        sx={{
                          mr: 0.5,
                        }}
                      />
                      Documento {index + 1}
                    </Link>
                  ))}
                </TextFieldTitle>
              </Stack>
            )}

            {sign && sign == "1" && (
              <Stack direction={"row"} justifyContent={"flex-end"}>
                <Button
                  color="primary"
                  variant="contained"
                  onClick={handleSign}
                >
                  Assinar feedback
                </Button>
              </Stack>
            )}
          </Stack>
        </Stack>
      </ContentArea>
    </ContentCard>
  );
}

FeedbackDetails.getLayout = getLayout("private");
