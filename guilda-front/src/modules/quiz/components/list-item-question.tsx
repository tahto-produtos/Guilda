import { Box, Button, Typography } from "@mui/material";
import { grey } from "@mui/material/colors";
import React from "react";
import { ListQuestionsResponseModel } from "src/typings";

interface ListItemQuestionProps {
  question: ListQuestionsResponseModel;
  disableButtons: boolean;
  onViewAnswers: () => void;
  onEdit: () => void;
  onDelete: () => void;
}

export function ListItemQuestion(props: ListItemQuestionProps) {
  const { question, disableButtons, onDelete, onEdit, onViewAnswers } = props;
  return (
    <Box
      border={`solid 1px ${grey[200]}`}
      display={"flex"}
      width={"100%"}
      borderRadius={2}
      p={2}
      alignItems={"center"}
      gap={"40px"}
      justifyContent={"space-between"}
    >
      <Box display={"flex"} alignItems={"center"} gap={2}>
        <Box
          sx={{
            width: "70px",
            height: "70px",
            backgroundColor: grey[100],
            borderRadius: 2,
          }}
        >
          {question.URL_QUESTION && question.URL_QUESTION !== undefined && (
            <img
              alt="imagem"
              src={question.URL_QUESTION}
              style={{ width: "100%", height: "100%" }}
            />
          )}
        </Box>
        <Box>
          <Typography variant="body2" fontSize={"16px"}>
            {question.QUESTION}
            <Typography
              variant="body2"
              component={"span"}
              fontSize={"12px"}
              color={"primary"}
              ml={1}
              fontWeight={"500"}
            >
              #{question.IDGDA_QUIZ_QUESTION}
            </Typography>
          </Typography>
          <Typography variant="body2" fontSize={"12px"} color={grey[600]}>
            {question.TYPE}
            <Typography
              variant="body2"
              component={"span"}
              fontSize={"12px"}
              color={"primary"}
              ml={1}
              fontWeight={"500"}
            >
              #{question.IDGDA_TYPE}
            </Typography>
          </Typography>
          <Typography
            variant="body2"
            fontSize={"13px"}
            mt={1}
            fontWeight={"500"}
          >
            Tempo de resposta: {question.TIME_ANSWER} segundos
          </Typography>
          <Typography
            variant="body2"
            fontSize={"13px"}
            mt={1}
            fontWeight={"500"}
          >
            {question.ANSWER}
          </Typography>
          <Typography
            variant="body2"
            fontSize={"13px"}
            mt={1}
            fontWeight={"500"}
          >
            {question.URL_QUESTION}
          </Typography>
        </Box>
      </Box>
      <Box display={"flex"} gap={1}>
        <Button color="primary" variant="outlined" onClick={onViewAnswers} disabled={question.TYPE && question.TYPE === "DISSERTATIVA" ? true : false}>
          Respostas
        </Button>
        <Button color="primary" variant="outlined" onClick={onEdit} disabled={disableButtons}>
          Editar
        </Button>
        <Button
          color="error"
          disabled={disableButtons}
          variant="contained"
          onClick={onDelete}
        >
          Excluír
        </Button>
      </Box>
    </Box>
  );
}
