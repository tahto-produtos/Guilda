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
import React, { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { BaseModal } from "src/components/feedback";
import { useLoadingState } from "src/hooks";
import { ActionButton } from "src/components/inputs/action-button/action-button";
import { ListAnswerResponseModel } from "src/typings";
import { EditAnswerUseCase } from "../use-cases";

interface ModalEditAnswerProps {
  idAnswer: number;
  answer: ListAnswerResponseModel | undefined;
  isOpenModal: boolean;
  onClose: () => void;
}

export function ModalEditAnswer(props: ModalEditAnswerProps) {
  const { idAnswer, answer, isOpenModal, onClose } = props;

  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [answerData, setAnswerData] = useState<ListAnswerResponseModel>();
  const [answerDescription, setAnswerDescription] = useState<string>("");
  const [answerCorrect, setAnswerCorrect] = useState<boolean>(false);

  useEffect(() => {
    if (idAnswer && idAnswer !== undefined && idAnswer !== null) {
      // TODO: VERIFICAR ENDPOINT PARA DAR GET EM UM QUIZ
    } else {
      if (answer) {
        console.log("answer", answer);
        setAnswerDescription(answer.QUESTION);
        setAnswerCorrect(answer.RIGHT_ANSWER === 1 ? true : false);
        setAnswerData(answer);
      }
    }
  }, [answer, idAnswer]);

  const handleEditAnswer = async () => {
    startLoading();

    await new EditAnswerUseCase()
      .handle({
        IDGDA_QUIZ_ANSWER: answerData?.IDGDA_QUIZ_ANSWER ?? 0,
        QUESTION: answerDescription,
        RIGHT_ANSWER: answerCorrect ? 1 : 0,
      })
      .then((data) => {
        setAnswerDescription("");
        setAnswerCorrect(false);
        onClose();
        toast.success(data);
      })
      .catch(() => {
        toast.error("Falha ao editar resposta.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  return (
    <Drawer
      anchor={"right"}
      PaperProps={{
        sx: {
          borderTopLeftRadius: "16px",
          borderBottomLeftRadius: "16px",
        },
      }}
      open={isOpenModal}
      title={`Editar resposta #${answerData?.IDGDA_QUIZ_ANSWER}`}
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
          Editar resposta
        </Typography>
        <TextField
          value={answerDescription}
          onChange={(e) => setAnswerDescription(e.target.value)}
          rows={4}
          placeholder="Resposta"
        />
        <Stack direction={"row"} alignItems={"center"} gap={"30px"}>
          <Checkbox
            checked={answerCorrect}
            onChange={(e) => setAnswerCorrect(e.target.checked)}
          />
          <Typography variant="caption" sx={{ textWrap: "nowrap" }}>
            Resposta certa?
          </Typography>
        </Stack>

        <ActionButton
          title={"Salvar"}
          loading={isLoading}
          isActive={false}
          onClick={() => handleEditAnswer()}
        />
      </Box>
    </Drawer>
  );
}
