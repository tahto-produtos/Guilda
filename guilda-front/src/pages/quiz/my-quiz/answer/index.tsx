import { FactCheckOutlined, HomeOutlined } from "@mui/icons-material";
import HelpOutline from "@mui/icons-material/HelpOutline";
import TimerOutlined from "@mui/icons-material/TimerOutlined";
import {
  Box,
  Breadcrumbs,
  Button,
  Checkbox,
  Chip,
  Divider,
  FormControlLabel,
  IconButton,
  Link,
  Stack,
  TextField,
  Typography,
  selectClasses,
  useTheme,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { BaseModal } from "src/components/feedback";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { QuizContext } from "src/contexts/quiz-provider/quiz.context";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListHobbyUseCase } from "src/modules/personas/use-cases/list-hobby.use-case";
import { SendAnswerUseCase } from "src/modules/quiz/use-cases/answers/send-answer.use-case";
import { LoadMyQuizUseCase } from "src/modules/quiz/use-cases/load-my-quiz.use-case";
import { ListQuizUseCase } from "src/modules/quiz/use-cases/show-quiz.use-case";
import { Hobby } from "src/typings/models/hobby.model";
import { MyQuizModel } from "src/typings/models/my-quiz.model";
import { QuizDetail } from "src/typings/models/quiz-detail.model";
import { getLayout } from "src/utils";

export default function AnswerQuizView() {
  const router = useRouter();
  const { selectedQuiz, refreshQuizzes } = useContext(QuizContext);
  const [quizz, setQuiz] = useState<QuizDetail | null>(null);
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [checkedOptions, setCheckedOptions] = useState<number[]>([]);
  const [answer, setAnswer] = useState<string>("");
  const [timeLeft, setTimeLeft] = useState<number | null>(null);
  const [isOpenOrientantion, setIsOpenOrientation] = useState<boolean>(false);

  const theme = useTheme();

  useEffect(() => {
    if (quizz && quizz.QUESTION[0]?.TIME_ANSWER > 0) {
      setTimeLeft(quizz.QUESTION[0].TIME_ANSWER);
      const timer = setInterval(() => {
        setTimeLeft((prevTime) => {
          if (prevTime === 0) {
            clearInterval(timer);
            sendAnswer(true);
            return null;
          }
          return prevTime ? prevTime - 1 : null;
        });
      }, 1000);

      return () => clearInterval(timer);
    }
  }, [quizz]);

  useEffect(() => {
    if (!selectedQuiz) {
      router.push("/");
    }
  }, [selectedQuiz]);

  async function showQuiz() {
    if (!selectedQuiz) return;

    startLoading();
    setTimeLeft(null);

    new ListQuizUseCase()
      .handle({
        idQuiz: selectedQuiz.IDGDA_QUIZ,
        idQuizUser: selectedQuiz.IDGDA_QUIZ_USER,
      })
      .then((data) => {
        setQuiz(data[0]);
        if (!data[0]) {
          router.push("/quiz/my-quiz/answer/finish-quiz");
          //refreshQuizzes();
        }
      })
      .catch(() => {
        toast.error("Erro ao listar os hobbies.");
      })
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    selectedQuiz && showQuiz();
  }, [selectedQuiz]);

  function handleOnSelect(id: number) {
    let newArr = [...checkedOptions];

    if ([2, 3].includes(quizz?.QUESTION[0]?.IDTYPE || 0)) {
      return setCheckedOptions([id]);
    }

    if (newArr.includes(id)) {
      newArr = newArr.filter((item) => item !== id);
    } else {
      newArr.push(id);
    }

    setCheckedOptions(newArr);
  }

  async function sendAnswer(NO_ANSWER: boolean) {
    if (!selectedQuiz || !quizz) return;

    startLoading();

    new SendAnswerUseCase()
      .handle({
        ANSWER: answer,
        IDGDA_QUIZ_ANSWER: checkedOptions.length > 0 ? checkedOptions : [0],
        IDGDA_QUIZ_QUESTION: quizz?.QUESTION[0]?.IDGDA_QUIZ_QUESTION,
        IDGDA_QUIZ_USER: selectedQuiz.IDGDA_QUIZ_USER,
        NO_ANSWER: NO_ANSWER ? 1 : 0,
      })
      .then((data) => {
        showQuiz();
      })
      .catch(() => {
        toast.error("Erro ao reponder essa pergunta.");
      })
      .finally(() => {
        finishLoading();
        setCheckedOptions([]);
        setAnswer("");
      });
  }

  if (!quizz && !isLoading) {
    return (
      <ContentCard>
        <ContentArea>
          <PageTitle
            title={`Quiz - ${selectedQuiz?.TITLE}`}
            loading={isLoading}
          ></PageTitle>
          <Stack py={"50px"}>
            <Typography variant="h3">Você já finalizou esse quiz!</Typography>
          </Stack>
        </ContentArea>
      </ContentCard>
    );
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
            underline="hover"
            sx={{ display: "flex", alignItems: "center" }}
            color={theme.palette.background.default}
            href="/quiz/my-quiz"
          >
            <Typography fontWeight={"500"}>Quiz</Typography>
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
              Quiz - {selectedQuiz?.TITLE}
            </Typography>
          </Link>
        </Breadcrumbs>
      </Stack>
      <ContentArea sx={{ py: "40px" }}>
        <Stack px={"40px"}>
          <PageTitle
            icon={<FactCheckOutlined sx={{ fontSize: "40px" }} />}
            title={`Quiz - ${selectedQuiz?.TITLE}`}
            loading={isLoading}
          >
            <Stack direction={"row"} alignItems={"center"} gap={"24px"}>
              <Typography>
                Total respondido: {quizz?.QTD_ANSWER} / {quizz?.QTD_QUESTION}
              </Typography>
            </Stack>
          </PageTitle>
          <Divider />

          <Stack direction={"column"} gap={"20px"} mt={"40px"}>
            {/* <Stack width={"100%"} direction={"row"} gap={"16px"}>
              <Typography fontSize={"24px"} fontWeight={"600"}>
                Pergunta
              </Typography>
            </Stack> */}
            <Stack
              direction={"row"}
              justifyContent={"space-between"}
              alignItems={"center"}
            >
              <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
                {quizz?.QUESTION[0] && (
                  <Chip color="primary" label={quizz?.QUESTION[0]?.TYPE} />
                )}
                {quizz?.QUESTION[0]?.ORIENTATION && (
                  <IconButton
                    size="small"
                    color="secondary"
                    onClick={() => setIsOpenOrientation(true)}
                  >
                    <HelpOutline />
                  </IconButton>
                )}
              </Stack>
              {timeLeft !== null && (
                <Stack direction={"row"} gap={"10px"} alignItems={"center"}>
                  <TimerOutlined fontSize="large" color="primary" />
                  <Typography
                    fontWeight={"800"}
                    fontSize={"24px"}
                    color={"primary"}
                  >
                    {timeLeft}
                  </Typography>
                </Stack>
              )}
            </Stack>
            <Typography variant="h3" fontSize={"26px"} fontWeight={"700"}>
              {quizz?.QUESTION[0]?.QUESTION}
            </Typography>
            {quizz?.QUESTION[0]?.URL_QUESTION &&
            quizz?.QUESTION[0]?.URL_QUESTION !== undefined ? (
              <Box
                sx={{
                  width: "350px",
                  height: "350px",
                  backgroundColor: grey[100],
                  borderRadius: 2,
                }}
              >
                <img
                  alt="imagem"
                  src={quizz?.QUESTION[0]?.URL_QUESTION}
                  style={{
                    width: "100%",
                    height: "100%",
                    borderRadius: 2,
                  }}
                />
              </Box>
            ) : null}

            <Stack mt={"20px"} direction={"row"} gap={"32px"} flexWrap={"wrap"}>
              {quizz?.QUESTION[0]?.TYPE == "DISSERTATIVA" && (
                <TextField
                  value={answer}
                  onChange={(e) => setAnswer(e.target.value)}
                  rows={4}
                  multiline
                  placeholder="Resposta"
                />
              )}
              {quizz?.QUESTION[0]?.ANSWER.map((ans, index) => {
                const isSelected = checkedOptions.includes(
                  ans.IDGDA_QUIZ_ANSWER
                );

                return (
                  <Stack
                    key={index}
                    direction={"row"}
                    alignItems={"center"}
                    gap={"10px"}
                    onClick={() => handleOnSelect(ans.IDGDA_QUIZ_ANSWER)}
                  >
                    <Button
                      variant={isSelected ? "contained" : "outlined"}
                      sx={{
                        color: isSelected
                          ? "#fff"
                          : theme.palette.secondary.main,
                        borderColor: theme.palette.secondary.main,
                      }}
                    >
                      {ans.QUESTION}
                    </Button>
                  </Stack>
                );
              })}
            </Stack>
          </Stack>
          <Stack
            direction={"row"}
            width={"100%"}
            justifyContent={"flex-end"}
            alignItems={"center"}
            mt={"50px"}
            gap={"24px"}
          >
            <Button
              variant="contained"
              color="primary"
              onClick={() => sendAnswer(false)}
            >
              Avançar
            </Button>
            {!selectedQuiz?.REQUIRED && (
              <Button
                variant="contained"
                color="error"
                onClick={() => sendAnswer(true)}
              >
                Não responder
              </Button>
            )}
          </Stack>
        </Stack>
      </ContentArea>
      <BaseModal
        width={"540px"}
        open={isOpenOrientantion}
        title={`Orientações`}
        onClose={() => setIsOpenOrientation(false)}
      >
        <Box width={"100%"} display={"flex"} flexDirection={"column"} gap={1}>
          <Typography>{quizz?.QUESTION[0]?.ORIENTATION}</Typography>
        </Box>
      </BaseModal>
    </ContentCard>
  );
}

AnswerQuizView.getLayout = getLayout("private");
