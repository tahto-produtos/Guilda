import {
  FactCheckOutlined,
  Feedback,
  FeedbackOutlined,
  HomeOutlined,
  LinearScale,
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
  useTheme,
} from "@mui/material";
import { format } from "date-fns";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ActionButton } from "src/components/inputs/action-button/action-button";
import { TextFieldTitle } from "src/components/inputs/title-text-field/title-text-field";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { QuizContext } from "src/contexts/quiz-provider/quiz.context";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListTypePedagogicalScaleUseCase } from "src/modules/feedback/use-cases/ListTypePedagogicalScale.use-case";
import { CreateFeedBackUseCase } from "src/modules/feedback/use-cases/create-feedback.use-case";
import { ListGravityPedagogicalEscale } from "src/modules/feedback/use-cases/list-pedagogical-scale-gravity.use-case";
import { ListPedagogicalEscaleUseCase } from "src/modules/feedback/use-cases/listPedagogicalScale.use-case";
import { SearchAccountsUseCase } from "src/modules/personas/use-cases/search-accounts.use-case";
import { PedagogicalGravity } from "src/typings/models/pedagogical-gravity.model";
import { PedagogicalScale } from "src/typings/models/pedagogical-scale.model";
import { TypePedagogicalScale } from "src/typings/models/type-pedagogical-scale.model";
import { getLayout } from "src/utils";
import { capitalizeText } from "src/utils/capitalizeText";

export default function CreateFeedbackView() {
  const { quizzes, refreshQuizzes, setSelectedQuiz } = useContext(QuizContext);
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [pedagogicalScalesGravity, setPedagogicalScalesGravity] = useState<
    PedagogicalGravity[]
  >([]);
  const [
    selectedTypePedagogicalScalesGravity,
    setSelectedTypePedagogicalScalesGravity,
  ] = useState<PedagogicalGravity | null>(null);
  const [pedagogicalScales, setPedagogicalScales] = useState<
    PedagogicalScale[]
  >([]);
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
  const [selectedTypePedagogicalScales, setSelectedTypePedagogicalScales] =
    useState<PedagogicalScale | null>(null);
  const [selectedTab, setSelectedTab] = useState<"status" | "scale">("status");
  const [collaboratorSearch, setCollaboratorSearch] = useState<string>("");
  const router = useRouter();
  const [reason, setReason] = useState<string>("");
  const [details, setDetails] = useState<string>("");
  const [files, setFiles] = useState<File[]>([]);

  const theme = useTheme();

  useEffect(() => {
    refreshQuizzes();
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
      .catch(() => {})
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    getCollaborators(collaboratorSearch);
  }, [collaboratorSearch]);

  async function createFeedback() {
    if (
      !selectedTypePedagogicalScales ||
      !selectedTypePedagogicalScalesGravity ||
      !selectedCollaborator
    ) {
      return;
    }

    await new CreateFeedBackUseCase()
      .handle({
        IDGDA_PEDAGOGICAL_SCALE:
          selectedTypePedagogicalScales?.IDGDA_PEDAGOGICAL_SCALE,
        IDGDA_PEDAGOGICAL_SCALE_GRAVITY:
          selectedTypePedagogicalScalesGravity?.IDGDA_PEDAGOGICAL_SCALE_GRAVITY,
        IDPERSONA_RECEIVED_BY: selectedCollaborator?.id,
        REASON: reason,
        DETAILS: details,
        FILES: files,
      })
      .then((data) => {
        toast.success("Feedback enviado com sucesso!");
        setReason("");
        setSelectedTypePedagogicalScales(null);
        setSelectedTypePedagogicalScalesGravity(null);
        setSelectedCollaborator(null);
        setDetails("");
        setFiles([]);
      })
      .catch(() => {})
      .finally(() => {
        finishLoading();
      });
  }

  async function listTypePedagogical() {
    await new ListPedagogicalEscaleUseCase()
      .handle()
      .then((data) => {
        setPedagogicalScales(data);
      })
      .catch(() => {})
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    listTypePedagogical();
  }, []);

  async function listTypePedagogicalGravity() {
    await new ListGravityPedagogicalEscale()
      .handle()
      .then((data) => {
        setPedagogicalScalesGravity(data);
      })
      .catch(() => {})
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    listTypePedagogicalGravity();
  }, []);

  const handleUploadFiles = (event: any) => {
    const files = event.target.files;

    if (files.length > 0) {
      const filesArray = Array.from(files);

      filesArray.forEach((file, index) => {
        const reader = new FileReader();
        reader.readAsDataURL(file as File);
        reader.onloadend = () => {};
      });

      setFiles(filesArray as File[]);
    }
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
            href="/feedback"
          >
            <Typography fontWeight={"400"}>Feedback</Typography>
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
              Criação de formulário de feedback
            </Typography>
          </Link>
        </Breadcrumbs>
      </Stack>
      <ContentArea sx={{ py: " 40px" }}>
        <Stack px={"40px"}>
          <PageTitle
            title="Criação de formulário de feedback"
            loading={isLoading}
          ></PageTitle>
          <Divider />
          <Stack direction={"column"} gap={"16px"} mt={"40px"}>
            <Stack direction={"row"} width={"100%"} gap={"20px"}>
              <TextFieldTitle title="Título do feedback">
                <TextField
                  placeholder="Defina o título"
                  fullWidth
                  onChange={(e) => setReason(e.target.value)}
                  value={reason}
                  sx={{ marginRight: "10px" }}
                />
              </TextFieldTitle>
              <TextFieldTitle title="Anexar documentos ao feedback">
                  <input
                    accept="image/*, .pdf, .xls, .xlsx, .doc, .docx"
                    style={{ display: "none" }}
                    id="files-upload"
                    type="file"
                    onChange={handleUploadFiles}
                    multiple
                  />
                  <label htmlFor="files-upload">
                    <Button
                      variant="contained"
                      color="primary"
                      component="span"
                      fullWidth
                      sx={{ height: "52px" }}
                    >
                      {files.length > 0
                      ? `${files.length} ${
                          files.length == 1
                            ? "documento selecionado"
                            : "documentos selecionados"
                        }`
                      : "Selecionar documentos"}
                    </Button>
                  </label>
              </TextFieldTitle>
            </Stack>
            <Stack direction={"row"} width={"100%"} gap={"20px"}>
              <TextFieldTitle title="Escala pedagógica">
                <Autocomplete
                  fullWidth
                  value={selectedTypePedagogicalScales}
                  options={pedagogicalScales}
                  disableClearable={false}
                  getOptionLabel={(option) => option.PEDAGOGICAL_SCALE}
                  onChange={(event, value) => {
                    setSelectedTypePedagogicalScales(value);
                  }}
                  filterSelectedOptions
                  renderInput={(props) => (
                    <TextField {...props} label={"Selecione"} />
                  )}
                  renderOption={(props, option) => {
                    return (
                      <li {...props} key={option.IDGDA_PEDAGOGICAL_SCALE}>
                        {option.PEDAGOGICAL_SCALE}
                      </li>
                    );
                  }}
                  isOptionEqualToValue={(option, value) =>
                    option.PEDAGOGICAL_SCALE === value.PEDAGOGICAL_SCALE
                  }
                  sx={{ m: 0 }}
                />
              </TextFieldTitle>
              <TextFieldTitle title="Gravidade">
                <Autocomplete
                  fullWidth
                  value={selectedTypePedagogicalScalesGravity}
                  options={pedagogicalScalesGravity}
                  disableClearable={false}
                  getOptionLabel={(option) => option.PEDAGOGICAL_SCALE_GRAVITY}
                  onChange={(event, value) => {
                    setSelectedTypePedagogicalScalesGravity(value);
                  }}
                  filterSelectedOptions
                  renderInput={(props) => (
                    <TextField {...props} label={"Selecione"} />
                  )}
                  renderOption={(props, option) => {
                    return (
                      <li {...props} key={option.IDGDA_PEDAGOGICAL_SCALE_GRAVITY}>
                        {option.PEDAGOGICAL_SCALE_GRAVITY}
                      </li>
                    );
                  }}
                  isOptionEqualToValue={(option, value) =>
                    option.PEDAGOGICAL_SCALE_GRAVITY === value.PEDAGOGICAL_SCALE_GRAVITY
                  }
                  sx={{ m: 0 }}
                />
              </TextFieldTitle>
              <TextFieldTitle title="Enviar para">
                <Autocomplete
                  fullWidth
                  value={selectedCollaborator}
                  options={collaborators}
                  onInputChange={(e, text) => setCollaboratorSearch(text)}
                  disableClearable={false}
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
                        {option.id} - {option.name}
                      </li>
                    );
                  }}
                  isOptionEqualToValue={(option, value) =>
                    option.name === value.name
                  }
                  sx={{ m: 0 }}
                />
              </TextFieldTitle>
            </Stack>
            <Stack direction={"row"} width={"100%"}>
              <TextFieldTitle title="Detalhes do feedback">
                <TextField
                  multiline
                  rows={4}
                  placeholder="Defina os detalhes"
                  fullWidth
                  onChange={(e) => setDetails(e.target.value)}
                  value={details}
                />
              </TextFieldTitle>
            </Stack>
            <Stack direction={"row"}>
              <Button variant="contained" onClick={createFeedback}>
                Enviar
              </Button>
            </Stack>
          </Stack>
        </Stack>
      </ContentArea>
    </ContentCard>
  );
}

CreateFeedbackView.getLayout = getLayout("private");
