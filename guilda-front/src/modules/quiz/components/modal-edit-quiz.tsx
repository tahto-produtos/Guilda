import {
  Autocomplete,
  Box,
  Button,
  Checkbox,
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
import { ListPersonasV2UseCase } from "src/modules/personas/use-cases/list-personas-v2.use-case";
import { PersonaAccountModel, QuizModel } from "src/typings";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { EditQuizUseCase } from "../use-cases";
import { ActionButton } from "src/components/inputs/action-button/action-button";
import { DateUtils } from "src/utils";

interface ModalEditQuizProps {
  idQuiz: number;
  quiz: QuizModel | undefined;
  isOpenModal: boolean;
  onClose: () => void;
}

export function ModalEditQuiz(props: ModalEditQuizProps) {
  const { idQuiz, quiz, isOpenModal, onClose } = props;

  const { finishLoading, isLoading, startLoading } = useLoadingState();

  const [quizData, setQuizData] = useState<QuizModel>();
  const [searchTextPersonaDemandant, setSearchTextPersonaDemandant] =
    useState<string>("");
  const [demandantSelected, setDemandantSelected] =
    useState<PersonaAccountModel | null>(null);
  const [personasDemant, setPersonasDemandant] = useState<
    PersonaAccountModel[]
  >([]);
  const [searchTextPersonaResponsible, setSearchTextPersonaResponsible] =
    useState<string>("");
  const [responsibleSelected, setResponsibleSelected] =
    useState<PersonaAccountModel | null>(null);
  const [personasResponsibles, setPersonasResponsible] = useState<
    PersonaAccountModel[]
  >([]);
  const [titleQuiz, setTitleQuiz] = useState<string>("");
  const [descriptionQuiz, setDescriptionQuiz] = useState<string>("");
  const [dateStartQuiz, setDateStartQuiz] = useState<string | null>("");
  const [dateEndQuiz, setDateEndQuiz] = useState<string | null>("");
  const [monetizatioValue, setMonetizationValue] = useState<number>(0);
  const [percentMonetizatioValue, setPercentMonetizationValue] =
    useState<number>(0);
  const [requiredQuiz, setRequiredQuiz] = useState<boolean>(false);

  useEffect(() => {
    if (idQuiz && idQuiz !== undefined && idQuiz !== null) {
      // TODO: VERIFICAR ENDPOINT PARA DAR GET EM UM QUIZ
    } else {
      if (quiz) {
        setTitleQuiz(quiz.title);
        setDescriptionQuiz(quiz.description);
        setRequiredQuiz(quiz.required === 1 ? true : false);
        setMonetizationValue(quiz.Monetization);
        setPercentMonetizationValue(quiz.PercentMonetization);
        setDateStartQuiz(quiz?.startedAt);
        setDateEndQuiz(quiz.endedAt);
        setDemandantSelected({
          FOLLOWED_BY_ME: false,
          FOTO: "",
          IDGDA_PERSONA_USER: quiz.idCreator,
          NOME: quiz.nameCreator,
          TIPO: "",
        });
        setResponsibleSelected({
          FOLLOWED_BY_ME: false,
          FOTO: "",
          IDGDA_PERSONA_USER: quiz.idResponsible,
          NOME: quiz.nameResponsible,
          TIPO: "",
        });
      }
      setQuizData(quiz);
    }
  }, [quiz, idQuiz]);

  async function ListPersonasDemandant() {
    startLoading();

    const payload = {
      accountPersona: searchTextPersonaDemandant,
      limit: 10,
      page: 1,
    };

    new ListPersonasV2UseCase()
      .handle(payload)
      .then((data) => {
        setPersonasDemandant(data.ACCOUNTS);
      })
      .catch((e) => {
        toast.error("Erro ao listar os demandantes. ");
      })
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    ListPersonasDemandant();
  }, [searchTextPersonaDemandant]);

  async function ListPersonasResponsibles() {
    startLoading();

    const payload = {
      accountPersona: searchTextPersonaResponsible,
      limit: 10,
      page: 1,
    };

    new ListPersonasV2UseCase()
      .handle(payload)
      .then((data) => {
        setPersonasResponsible(data.ACCOUNTS);
      })
      .catch((e) => {
        toast.error("Erro ao listar os responsáveis.");
      })
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    ListPersonasResponsibles();
  }, [searchTextPersonaResponsible]);

  const handleEditQuiz = async () => {
    startLoading();

    await new EditQuizUseCase()
      .handle({
        IDGDA_QUIZ: quizData?.codQuiz,
        TITLE: titleQuiz,
        DESCRIPTION: descriptionQuiz,
        REQUIRED: requiredQuiz ? 1 : 0,
        IDGDA_COLLABORATOR_DEMANDANT:
          demandantSelected == null ? 0 : demandantSelected?.IDGDA_PERSONA_USER,
        IDGDA_COLLABORATOR_RESPONSIBLE:
          responsibleSelected == null
            ? 0
            : responsibleSelected?.IDGDA_PERSONA_USER,
        MONETIZATION: monetizatioValue,
        PERCENT_MONETIZATION: percentMonetizatioValue,
        /*          STARTED_AT: dateStartQuiz
                  ? DateUtils.formatDatePtBRWithTimeAndSecondsToUS(dateStartQuiz)
                  : "",
                ENDED_AT: dateEndQuiz
                  ? DateUtils.formatDatePtBRWithTimeAndSecondsToUS(dateEndQuiz)
                  : "",  */
        STARTED_AT: dateStartQuiz
          ? dateStartQuiz
          : "",
        ENDED_AT: dateEndQuiz
          ? dateEndQuiz
          : "",
      })
      .then((data) => {
        setDemandantSelected(null);
        setResponsibleSelected(null);
        setTitleQuiz("");
        setDescriptionQuiz("");
        setDateStartQuiz("");
        setDateEndQuiz("");
        setRequiredQuiz(false);
        setMonetizationValue(0);
        setPercentMonetizationValue(0);
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
    <BaseModal
      width={"540px"}
      open={isOpenModal}
      title={`Editar Quiz #${quizData?.codQuiz}`}
      onClose={onClose}
    >
      <Box width={"100%"} display={"flex"} flexDirection={"column"} gap={1}>
        <Autocomplete
          value={demandantSelected}
          placeholder={"Demandante"}
          disableClearable={false}
          onChange={(e, value) => {
            setDemandantSelected(value);
          }}
          onInputChange={(e, value) => {
            setSearchTextPersonaDemandant(value);
          }}
          isOptionEqualToValue={(option, value) =>
            option.IDGDA_PERSONA_USER == value.IDGDA_PERSONA_USER
          }
          disableCloseOnSelect
          renderInput={(props) => <TextField {...props} label={"Demandante"} />}
          getOptionLabel={(option) => option.NOME}
          options={personasDemant}
          fullWidth
          renderOption={(props, option) => {
            return (
              <li {...props} key={option.IDGDA_PERSONA_USER}>
                {option.NOME}
              </li>
            );
          }}
          sx={{ m: 0 }}
        />
        <Autocomplete
          value={responsibleSelected}
          placeholder={"Responsável"}
          disableClearable={false}
          onChange={(e, value) => {
            setResponsibleSelected(value);
          }}
          onInputChange={(e, value) => {
            setSearchTextPersonaResponsible(value);
          }}
          isOptionEqualToValue={(option, value) =>
            option.IDGDA_PERSONA_USER == value.IDGDA_PERSONA_USER
          }
          disableCloseOnSelect
          renderInput={(props) => (
            <TextField {...props} label={"Responsável"} />
          )}
          getOptionLabel={(option) => option.NOME}
          options={personasResponsibles}
          fullWidth
          renderOption={(props, option) => {
            return (
              <li {...props} key={option.IDGDA_PERSONA_USER}>
                {option.NOME}
              </li>
            );
          }}
          sx={{ m: 0 }}
        />
        <TextField
          value={titleQuiz}
          label="Título"
          onChange={(e) => setTitleQuiz(e.target.value)}
        />
        <TextField
          multiline
          value={descriptionQuiz}
          onChange={(e) => setDescriptionQuiz(e.target.value)}
          rows={4}
          placeholder="Descrição"
        />
        <LocalizationProvider dateAdapter={AdapterDateFns}>
          <DateTimePicker
            label="Data e hora de início"
            value={dateStartQuiz ? new Date(dateStartQuiz) : null} // Certifique-se de passar um objeto Date válido
            onChange={(value) => {
              if (value) {
                // Remove o deslocamento de fuso horário para manter o horário local
                const localDate = new Date(value.getTime() - value.getTimezoneOffset() * 60000);
                const formattedDate = localDate.toISOString().replace('T', ' ').slice(0, 19); // Formato 'yyyy-MM-dd HH:mm:ss'
                setDateStartQuiz(formattedDate);
              } else {
                setDateStartQuiz(null);
              }
            }}
          />
        </LocalizationProvider>
        <LocalizationProvider dateAdapter={AdapterDateFns}>
          <DateTimePicker
            label="Data e hora de fim"
            value={dateEndQuiz ? new Date(dateEndQuiz) : null}
            onChange={(value) => {
              if (value) {
                // Remove o deslocamento de fuso horário para manter o horário local
                const localDate = new Date(value.getTime() - value.getTimezoneOffset() * 60000);
                const formattedDate = localDate.toISOString().replace('T', ' ').slice(0, 19); // Formato 'yyyy-MM-dd HH:mm:ss'
                setDateEndQuiz(formattedDate);
              } else {
                setDateEndQuiz(null);
              }
            }}
          
          />
        </LocalizationProvider>
        <Stack direction={"row"} alignItems={"center"} gap={"30px"}>
          <Checkbox
            checked={quiz?.required === 1 ? true : false}
            onChange={(e) => setRequiredQuiz(e.target.checked)}
          />
          <Typography variant="caption" sx={{ textWrap: "nowrap" }}>
            Quiz Obrigatório?
          </Typography>
        </Stack>
        <TextField
          value={monetizatioValue}
          label="Moedas monetização"
          type="number"
          onChange={(e) => setMonetizationValue(Number(e.target.value))}
        />
        <TextField
          value={percentMonetizatioValue}
          label="Percentual de resposta"
          type="number"
          onChange={(e) => setPercentMonetizationValue(Number(e.target.value))}
        />
        <ActionButton
          title={"Salvar"}
          loading={isLoading}
          isActive={false}
          onClick={() => handleEditQuiz()}
        />
      </Box>
    </BaseModal>
  );
}