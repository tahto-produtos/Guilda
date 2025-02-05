import { FactCheckOutlined, HomeOutlined } from "@mui/icons-material";
import {
    Box,
    Breadcrumbs,
    Button,
    Chip,
    Link,
    Stack,
    Typography,
    useTheme,
} from "@mui/material";
import { format } from "date-fns";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ProfileImage } from "src/components/data-display/profile-image/profile-image";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { QuizContext } from "src/contexts/quiz-provider/quiz.context";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListHobbyUseCase } from "src/modules/personas/use-cases/list-hobby.use-case";
import { LoadMyQuizUseCase } from "src/modules/quiz/use-cases/load-my-quiz.use-case";
import { Hobby } from "src/typings/models/hobby.model";
import { MyQuizModel } from "src/typings/models/my-quiz.model";
import { getLayout, DateUtils } from "src/utils";
import { capitalizeText } from "src/utils/capitalizeText";

export default function MyQuizView() {
    const { quizzes, refreshQuizzes, setSelectedQuiz } =
        useContext(QuizContext);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [searchText, setSearchText] = useState<string>("");
    const debouncedSearchText: string = useDebounce<string>(searchText, 400);
    const router = useRouter();

    const theme = useTheme();

    const handleClickAnswerQuiz = (quizData: MyQuizModel) => {
        var dataEndQuiz = new Date(quizData.ENDED_AT);
        var curDate = new Date();
        if(quizData.STATUS != "Concluido" && dataEndQuiz > curDate) {
            setSelectedQuiz(quizData);
        } else {
            return;
        }
    };

    useEffect(() => {
        refreshQuizzes();
    }, []);

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
                        <Typography fontWeight={"700"}>Meus Quiz</Typography>
                    </Link>
                </Breadcrumbs>
            </Stack>
            <ContentArea sx={{ py: " 40px" }}>
                <Stack px={"40px"}>
                    <PageTitle
                        icon={<FactCheckOutlined sx={{ fontSize: "40px" }} />}
                        title="Quiz"
                        loading={isLoading}
                    ></PageTitle>
                    <Stack direction={"column"} gap={"20px"} mt={"20px"}>
                        {quizzes.map((item, index) => (
                            <Box
                                key={index}
                                border={`solid 1px ${
                                    item.STATUS == "Concluido"
                                        ? theme.palette.secondary.main
                                        : theme.palette.primary.main
                                }`}
                                display={"flex"}
                                width={"100%"}
                                borderRadius={"16px"}
                                p={"24px"}
                                alignItems={"center"}
                                justifyContent={"space-between"}
                                onClick={() => handleClickAnswerQuiz(item)}
                                sx={{ cursor: (item.STATUS != "Concluido" ? (new Date(item.ENDED_AT) <= new Date() ? "inherit" : "pointer" ) : "inherit") }}
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
                                                    {capitalizeText(
                                                        item.TITLE || ""
                                                    )}
                                                    <Typography
                                                        variant="body2"
                                                        component={"span"}
                                                        fontSize={"16px"}
                                                        fontWeight={"600"}
                                                        fontFamily={
                                                            "Montserrat"
                                                        }
                                                        color={
                                                            item.REQUIRED
                                                                ? "secondary"
                                                                : "primary"
                                                        }
                                                        ml={1}
                                                    >
                                                        {" "}
                                                        -{" "}
                                                        {item.REQUIRED
                                                            ? "Obrigatório"
                                                            : ""}
                                                    </Typography>
                                                    {/* <Typography
                                                  variant="body2"
                                                  fontSize={"16px"}
                                                  component={"span"}
                                                  fontWeight={"600"}
                                                  fontFamily={
                                                      "Montserrat"
                                                  }
                                                  color={"text.secondary"}
                                              >
                                                  {" "}
                                                  {quiz.description &&
                                                      `- ${quiz.description}`}
                                              </Typography> */}
                                                </Typography>

                                                <Typography
                                                    variant="body2"
                                                    fontSize={"14px"}
                                                    mt={"8px"}
                                                >
                                                    {item.STARTED_AT ? `Iniciado em ${DateUtils.formatDateTimePtBR(item.STARTED_AT)}` : ``}
                                                    {item.ENDED_AT ? ` - Finalizado em ${DateUtils.formatDateTimePtBR(item.ENDED_AT)}` : ``}
                                                </Typography>
                                            </Stack>
                                        </Stack>
                                        <Stack
                                            direction={"row"}
                                            alignItems={"center"}
                                            mt={"30px"}
                                            gap={"83px"}
                                        >
                                            <Stack
                                                direction={"column"}
                                                gap={"9px"}
                                            >
                                                <Typography
                                                    fontSize={"14px"}
                                                    fontWeight={"400"}
                                                >
                                                    Status
                                                </Typography>
                                                <Typography
                                                    fontSize={"14px"}
                                                    fontWeight={"700"}
                                                    color={
                                                        item.STATUS ==
                                                        "Concluido"
                                                            ? "secondary"
                                                            : "primary"
                                                    }
                                                >
                                                    {item.STATUS}
                                                </Typography>
                                            </Stack>
                                            <Stack
                                                direction={"column"}
                                                gap={"9px"}
                                            >
                                                <Typography
                                                    fontSize={"14px"}
                                                    fontWeight={"400"}
                                                >
                                                    Concluido
                                                </Typography>
                                                <Typography
                                                    fontSize={"14px"}
                                                    fontWeight={"700"}
                                                    color={
                                                        item.STATUS ==
                                                        "Concluido"
                                                            ? "secondary"
                                                            : "primary"
                                                    }
                                                >
                                                    {item.CONCLUED}
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
                            // <Stack
                            //     key={index}
                            //     width={"100%"}
                            //     py={"20px"}
                            //     px={"30px"}
                            //     border={`solid 1px ${theme.palette.grey[300]}`}
                            //     borderRadius={"8px"}
                            //     direction={"row"}
                            //     alignItems={"center"}
                            //     justifyContent={"space-between"}
                            //     borderLeft={`solid 18px ${theme.palette.secondary.main}`}
                            //     sx={{ cursor: "pointer" }}
                            //     onClick={() => setSelectedQuiz(item)}
                            // >
                            //     <Stack>
                            //         <Typography
                            //             variant="h1"
                            //             fontSize={"16px"}
                            //             fontWeight={"600"}
                            //         >
                            //             {item.TITLE}
                            //         </Typography>
                            //         <Typography
                            //             variant="body1"
                            //             color={"text.secondary"}
                            //         >
                            //             Iniciado em: {item.STARTED_AT} -
                            //             Finalizado em: {item.ENDED_AT}
                            //         </Typography>
                            //     </Stack>
                            //     <Stack>
                            //         <Typography>Status</Typography>
                            //         <Typography variant="h3" fontSize={"16px"}>
                            //             {item.CONCLUED}
                            //         </Typography>
                            //         <Chip
                            //             size="small"
                            //             sx={{ mt: "10px" }}
                            //             color={
                            //                 item.STATUS == "Concluido"
                            //                     ? "success"
                            //                     : "warning"
                            //             }
                            //             label={item.STATUS}
                            //         />
                            //     </Stack>
                            // </Stack>
                        ))}
                    </Stack>
                </Stack>
            </ContentArea>
        </ContentCard>
    );
}

MyQuizView.getLayout = getLayout("private");
