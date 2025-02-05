import {
  Autocomplete,
  Box,
  Checkbox,
  TextField,
  Typography,
} from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { BaseModal, ConfirmModal } from "../feedback";
import { ActionButton } from "../inputs/action-button/action-button";
import { SendFeedbackClimateUseCase } from "src/modules/climate/send-feedback.use-case";
import { toast } from "react-toastify";
import { useLoadingState } from "src/hooks";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";

interface ClimateProps {
  items: any[];
  refreshHandle: () => void;
}

export function Climate(props: ClimateProps) {
  const { items, refreshHandle } = props;
  const { finishLoading, isLoading, startLoading } = useLoadingState();

  const [isOpenModalFeedBack, setIsOpenModalFeedBack] =
    useState<boolean>(false);
  const [feedbackTypes, setFeedbackTypes] = useState<
    {
      id: number;
      type: string;
    }[]
  >([
    {
      id: 1,
      type: "Acompanhamento Realizado",
    },
    {
      id: 2,
      type: "Apoio Supervisor Aplicado",
    },
  ]);
  const [typeFeedbackSelected, setTypeFeedbackSelected] = useState<{
    id: number;
    type: string;
  } | null>(null);
  const [idClimateSelected, setIdClimateSelected] = useState<number | null>(
    null
  );

  const handleOnSelectApply = async (idClimateUser: number) => {
    setIsOpenModalFeedBack(true);
    setIdClimateSelected(idClimateUser);
  };

  const handleConfirmFeedback = async () => {
    if (typeFeedbackSelected == null || idClimateSelected == null) {
      return false;
    }

    await new SendFeedbackClimateUseCase()
      .handle({
        idClimateUser: idClimateSelected,
        idClimateApplyType: typeFeedbackSelected.id,
      })
      .then((data) => {
        setIsOpenModalFeedBack(false);
        setIdClimateSelected(null);
        setTypeFeedbackSelected(null);
        refreshHandle();
        toast.success("Apoio Supervisor Aplicado com sucesso!");
      })
      .catch(() => {
        toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
      })
      .finally(() => {
        finishLoading();
      });
  };

  return (
    <Box
      display={"block"}
      maxWidth={"1200px"}
      overflow={"scroll"}
      flexDirection={"column"}
    >
      <Box
        border={`solid 2px #D9D9D9`}
        display={"flex"}
        width={"1500px"}
        borderRadius={2}
        p={2}
        alignItems={"center"}
        justifyContent={"space-between"}
        marginTop={2}
        sx={{
          backgroundColor: "#FAFAFA",
        }}
      >
        <Box display={"flex"} alignItems={"center"} gap={4} width={"100%"}>
          <Box width={"35px"}>
            <Typography variant="body1" fontSize={"13px"} fontWeight={900}>
              Id
            </Typography>
          </Box>
          <Box width={"85px"}>
            <Typography variant="body1" fontSize={"13px"} fontWeight={900}>
              Data
            </Typography>
          </Box>
          <Box width={"85px"}>
            <Typography variant="body1" fontSize={"13px"} fontWeight={900}>
              Nome
            </Typography>
          </Box>
          <Box width={"85px"}>
            <Typography variant="body1" fontSize={"13px"} fontWeight={900}>
              BC
            </Typography>
          </Box>
          <Box width={"85px"}>
            <Typography variant="body1" fontSize={"13px"} fontWeight={900}>
              Clima
            </Typography>
          </Box>
          <Box width={"85px"}>
            <Typography variant="body1" fontSize={"13px"} fontWeight={900}>
              Motivo
            </Typography>
          </Box>
          <Box width={"85px"}>
            <Typography variant="body1" fontSize={"13px"} fontWeight={900}>
              Feedback
            </Typography>
          </Box>
          <Box width={"85px"}>
            <Typography variant="body1" fontSize={"13px"} fontWeight={900}>
              Supervisor
            </Typography>
          </Box>
          <Box width={"85px"}>
            <Typography variant="body1" fontSize={"13px"} fontWeight={900}>
              Coordenador
            </Typography>
          </Box>
          <Box width={"85px"}>
            <Typography variant="body1" fontSize={"13px"} fontWeight={900}>
              Gerente II
            </Typography>
          </Box>
          <Box width={"85px"}>
            <Typography variant="body1" fontSize={"13px"} fontWeight={900}>
              Gerente I
            </Typography>
          </Box>
          <Box width={"85px"}>
            <Typography variant="body1" fontSize={"13px"} fontWeight={900}>
              Diretor
            </Typography>
          </Box>
          <Box width={"85px"}>
            <Typography variant="body1" fontSize={"13px"} fontWeight={900}>
              CEO
            </Typography>
          </Box>
        </Box>
      </Box>
      {items.length > 0
        ? items.map((item, index) => (
            <Box
              key={index}
              border={`solid 2px #D9D9D9`}
              display={"flex"}
              width={"100%"}
              borderRadius={2}
              p={2}
              alignItems={"center"}
              justifyContent={"space-between"}
              marginTop={2}
            >
              <Box
                display={"flex"}
                alignItems={"center"}
                gap={4}
                width={"100%"}
              >
                <Box width={"35px"}>
                  <Typography variant="body1" fontSize={"13px"}>
                    {item.idUserClimate}
                  </Typography>
                </Box>
                <Box width={"85px"}>
                  <Typography variant="body1" fontSize={"13px"}>
                    {item.data}
                  </Typography>
                </Box>
                <Box width={"85px"}>
                  <Typography
                    variant="body1"
                    fontSize={"13px"}
                    fontWeight={900}
                  >
                    {item.name}
                  </Typography>
                </Box>
                <Box width={"85px"}>
                  <Typography variant="body1" fontSize={"13px"}>
                    {item.BC}
                  </Typography>
                </Box>
                <Box width={"85px"}>
                  <Typography variant="body1" fontSize={"13px"}>
                    {item.climate}
                  </Typography>
                </Box>
                <Box width={"85px"}>
                  <Typography
                    variant="body1"
                    fontSize={"13px"}
                    fontWeight={900}
                    color={"#2FAC9F"}
                  >
                    {item.reason}
                  </Typography>
                </Box>
                <Box width={"85px"}>
                  {item.canApply === true && item.applyType == "" ? (
                    <ActionButton
                      title={"Feddback"}
                      loading={false}
                      isActive={false}
                      onClick={() => handleOnSelectApply(item.idUserClimate)}
                    />
                  ) : (
                    <Typography variant="body1" fontSize={"13px"}>
                      {item.applyType}
                    </Typography>
                  )}
                </Box>
                <Box width={"85px"}>
                  <Typography variant="body1" fontSize={"13px"}>
                    {item.nomeSupervisor}
                  </Typography>
                </Box>
                <Box width={"85px"}>
                  <Typography variant="body1" fontSize={"13px"}>
                    {item.nomeCoordenador}
                  </Typography>
                </Box>
                <Box width={"85px"}>
                  <Typography variant="body1" fontSize={"13px"}>
                    {item.nomeGerenteII}
                  </Typography>
                </Box>
                <Box width={"85px"}>
                  <Typography variant="body1" fontSize={"13px"}>
                    {item.nomeGerenteI}
                  </Typography>
                </Box>
                <Box width={"85px"}>
                  <Typography variant="body1" fontSize={"13px"}>
                    {item.nomeDiretor}
                  </Typography>
                </Box>
                <Box width={"50px"}>
                  <Typography variant="body1" fontSize={"13px"}>
                    {item.nomeCeo}
                  </Typography>
                </Box>
              </Box>
            </Box>
          ))
        : null}
      {/* <ConfirmModal
                onClose={() => setIsOpenModalFeedBack(false)}
                onConfirm={() => null}
                open={isOpenModalFeedBack}
            /> */}
      <BaseModal
        width={"540px"}
        open={isOpenModalFeedBack}
        title={`Feedback`}
        onClose={() => setIsOpenModalFeedBack(!isOpenModalFeedBack)}
      >
        <Box width={"100%"} display={"flex"} flexDirection={"column"} gap={1}>
          <Autocomplete
            placeholder={"Feedback"}
            onChange={(_, type) => setTypeFeedbackSelected(type)}
            isOptionEqualToValue={(option, value) => option.type === value.type}
            renderInput={(props) => (
              <TextField {...props} size={"small"} label={"Feedback"} />
            )}
            renderOption={(props, option) => {
              return (
                <li {...props} key={option.id}>
                  {option.type}
                </li>
              );
            }}
            renderTags={() => null}
            getOptionLabel={(option) => option.type}
            options={feedbackTypes || []}
            sx={{ mb: 0 }}
          />
          <ActionButton
            title={"Salvar"}
            loading={false}
            isActive={false}
            onClick={() => handleConfirmFeedback()}
          />
        </Box>
      </BaseModal>
    </Box>
  );
}
