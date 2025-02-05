import { Box, Button, Typography } from "@mui/material";
import { grey } from "@mui/material/colors";
import React from "react";
import {
  ListAnswerResponseModel,
  ListQuestionsResponseModel,
} from "src/typings";

interface ListItemAnswerProps {
  answer: ListAnswerResponseModel;
  disableButtons: boolean;
  onViewAnswers: () => void;
  onEdit: () => void;
  onDelete: () => void;
}

export function ListItemAnswer(props: ListItemAnswerProps) {
  const { answer, disableButtons, onDelete, onEdit, onViewAnswers } = props;
  return (
    <Box
      border={`solid 1px ${grey[200]}`}
      display={"flex"}
      width={"100%"}
      borderRadius={2}
      p={2}
      alignItems={"center"}
      justifyContent={"space-between"}
      gap={"40px"}
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
          {answer.URL && answer.URL !== undefined && (
            <img
              alt="imagem"
              src={answer.URL}
              style={{ width: "100%", height: "100%" }}
            />
          )}
        </Box>
        <Box>
          <Typography variant="body2" fontSize={"16px"}>
            {answer.QUESTION}
            <Typography
              variant="body2"
              component={"span"}
              fontSize={"12px"}
              color={"primary"}
              ml={1}
              fontWeight={"500"}
            >
              #{answer.IDGDA_QUIZ_ANSWER}
            </Typography>
          </Typography>
          <Typography variant="body2" fontSize={"12px"} color={grey[600]}>
            Resposta: certa? {answer.RIGHT_ANSWER == 1 ? "Sim" : "Não"}
          </Typography>
          <Typography
            variant="body2"
            fontSize={"13px"}
            mt={1}
            fontWeight={"500"}
          >
            URL: {answer.URL}
          </Typography>
        </Box>
      </Box>
      <Box display={"flex"} gap={1}>
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
