import {
  Autocomplete,
  Box,
  Button,
  Checkbox,
  Drawer,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { DateTimePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { format, set } from "date-fns";
import React, { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { BaseModal } from "src/components/feedback";
import { useLoadingState } from "src/hooks";
import { ActionButton } from "src/components/inputs/action-button/action-button";
import {
  ListQuestionsResponseModel,
  ListTypeQuestionResponseModel,
  TypeQuestionModel,
} from "src/typings";
import { EditQuestionsUseCase, ListTypeQuestionsUseCase } from "../use-cases";

interface ModalEditQuestionProps {
  idQuestion: number;
  question: ListQuestionsResponseModel | undefined;
  isOpenModal: boolean;
  onClose: () => void;
}

export function ModalEditQuestion(props: ModalEditQuestionProps) {
  const { idQuestion, question, isOpenModal, onClose } = props;

  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [questionData, setQuestionData] =
    useState<ListQuestionsResponseModel>();
  const [typesQuestions, setTypesQuestions] = useState<
    ListTypeQuestionResponseModel[] | null
  >(null);
  const [typeQuestionSelected, setTypeQuestionSelected] =
    useState<TypeQuestionModel | null>(null);
  const [questionDescription, setQuestionDescription] = useState<string>("");
  const [timeQuestion, setTimeQuestion] = useState<number>(0);

  useEffect(() => {
    if (idQuestion && idQuestion !== undefined && idQuestion !== null) {
      // TODO: VERIFICAR ENDPOINT PARA DAR GET EM UM QUIZ
    } else {
      if (question) {
        setQuestionDescription(question.QUESTION);
        setTimeQuestion(question.TIME_ANSWER);
        if (typesQuestions) {
          typesQuestions.forEach((type) => {
            if (type.TYPE == question.TYPE) {
              setTypeQuestionSelected(type);
            }
          });
        }
        setQuestionData(question);
      }
    }
  }, [question, idQuestion]);

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

  const handleEditQuestion = async () => {
    startLoading();

    await new EditQuestionsUseCase()
      .handle({
        IDGDA_QUIZ_QUESTION: questionData?.IDGDA_QUIZ_QUESTION ?? 0,
        IDGDA_QUIZ_QUESTION_TYPE:
          typeQuestionSelected?.IDGDA_QUIZ_QUESTION_TYPE ?? 0,
        QUESTION: questionDescription,
        TIME_ANSWER: timeQuestion,
      })
      .then((data) => {
        setTypeQuestionSelected(null);
        setQuestionDescription("");
        setTimeQuestion(0);
        onClose();
        toast.success(data);
      })
      .catch(() => {
        toast.error("Falha ao editar quiz.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  return (
    <Drawer
      open={isOpenModal}
      anchor={"right"}
      PaperProps={{
        sx: {
          borderTopLeftRadius: "16px",
          borderBottomLeftRadius: "16px",
        },
      }}
      title={`Editar pergunta #${questionData?.IDGDA_QUIZ_QUESTION}`}
      onClose={onClose}
    >
      <Box
        width={"100%"}
        display={"flex"}
        flexDirection={"column"}
        gap={1}
        p={"24px"}
        minWidth={"400px"}
      >
        <Typography fontSize={"18px"} fontWeight={"700"} mb={"20px"}>
          Editar pergunta #{questionData?.IDGDA_QUIZ_QUESTION}
        </Typography>
        <Autocomplete
          value={typeQuestionSelected}
          placeholder={"Tipo"}
          disabled={questionData?.TYPE === "DISSERTATIVA" ? true : false}
          disableClearable={false}
          onChange={(e, value) => {
            setTypeQuestionSelected(value);
          }}
          isOptionEqualToValue={(option, value) =>
            option.IDGDA_QUIZ_QUESTION_TYPE == value.IDGDA_QUIZ_QUESTION_TYPE
          }
          disableCloseOnSelect
          renderInput={(props) => <TextField {...props} label={"Tipo"} />}
          getOptionLabel={(option) => option.TYPE}
          options={typesQuestions ?? []}
          fullWidth
          renderOption={(props, option) => {
            return (
              <li {...props} key={option.IDGDA_QUIZ_QUESTION_TYPE}>
                {option.TYPE}
              </li>
            );
          }}
          sx={{ m: 0 }}
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

        <ActionButton
          title={"Salvar"}
          loading={isLoading}
          isActive={false}
          onClick={() => handleEditQuestion()}
        />
      </Box>
    </Drawer>
  );
}
