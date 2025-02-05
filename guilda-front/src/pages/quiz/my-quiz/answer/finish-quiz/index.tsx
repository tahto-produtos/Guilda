import { HomeOutlined } from "@mui/icons-material";
import DoneAllOutlinedIcon from "@mui/icons-material/DoneAllOutlined";
import {
  Breadcrumbs,
  Divider,
  Link,
  Stack,
  Typography,
  useTheme,
  Button,
} from "@mui/material";
import { useRouter } from "next/router";
import { useContext } from "react";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { getLayout } from "src/utils";
import { QuizContext } from "src/contexts/quiz-provider/quiz.context";

export default function FinishQuiz() {
  const router = useRouter();

  const theme = useTheme();
  const { refreshQuizzes } = useContext(QuizContext);

  const finishQuiz = async () => {
    refreshQuizzes();
    //router.push("/");
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
            <Typography fontWeight={"700"}>Quiz Finalizado</Typography>
          </Link>
        </Breadcrumbs>
      </Stack>
      <ContentArea sx={{ py: " 40px" }}>
        <Stack px={"40px"}>
          <PageTitle
            icon={
              <DoneAllOutlinedIcon
                color={"primary"}
                sx={{ fontSize: "40px" }}
              />
            }
            title="Quiz Finalizado"
            loading={false}
          ></PageTitle>
          <Divider />
          <Stack direction={"column"} gap={"20px"} mt={"20px"}>
            <Typography fontWeight={"700"}>
              Parabéns você concluiu o quiz!
            </Typography>
            <Button variant="contained" onClick={finishQuiz}>
              Finalizar
            </Button>
          </Stack>
        </Stack>
      </ContentArea>
    </ContentCard>
  );
}

FinishQuiz.getLayout = getLayout("private");
