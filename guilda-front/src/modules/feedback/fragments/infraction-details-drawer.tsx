import { Close } from "@mui/icons-material";
import {
  Button,
  Divider,
  Drawer,
  IconButton,
  Stack,
  TextField,
  Typography,
  useTheme,
} from "@mui/material";
import { TextFieldTitle } from "src/components/inputs/title-text-field/title-text-field";
import { useLoadingState } from "src/hooks";
import { DeleteIngractionUseCase } from "../use-cases/delete-infraction.use-case";
import { Infraction } from "src/typings/models/infraction.model";
import { toast } from "react-toastify";

interface InfractionDetailsDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  infraction: Infraction;
  refreshList?: () => void;
}

export function InfractionDetailsDrawer(props: InfractionDetailsDrawerProps) {
  const { isOpen, onClose, infraction, refreshList } = props;
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const theme = useTheme();

  async function handleDelete() {
    startLoading();

    await new DeleteIngractionUseCase()
      .handle({
        id: infraction.IDGDA_PEDAGOGICAL_SCALE,
      })
      .then((data) => {
        toast.success("Infração apagada com sucesso!");
        refreshList && refreshList();
      })
      .catch(() => {})
      .finally(() => {
        finishLoading();
      });
  }

  return (
    <Drawer
      open={isOpen}
      anchor={"right"}
      onClose={onClose}
      PaperProps={{
        sx: {
          borderRadius: "16px 0px 0px 16px",
        },
      }}
    >
      <Stack
        width={"100%"}
        minWidth={"430px"}
        maxWidth={"430px"}
        direction={"column"}
        height={"100vh"}
      >
        <Stack
          width={"100%"}
          py={"24px"}
          px={"24px"}
          justifyContent={"space-between"}
          direction={"row"}
          alignItems={"center"}
        >
          <Stack direction={"row"} gap={"10px"}>
            <Typography variant="h2">Criação de infração</Typography>
          </Stack>
          <IconButton onClick={onClose}>
            <Close sx={{ color: theme.palette.text.primary }} />
          </IconButton>
        </Stack>
        <Stack px={"24px"}>
          <Divider />
        </Stack>
        <Stack
          direction={"column"}
          py={"24px"}
          width={"100%"}
          px={"24px"}
          gap={"24px"}
        >
          <TextFieldTitle title="Nome da infração">
            <TextField
              value={infraction.NAME}
              InputProps={{
                readOnly: true,
              }}
            />
          </TextFieldTitle>
          <TextFieldTitle title="Colaboradores associados">
            <TextField
              value={infraction.AMOUNT_COLLABORATORS}
              InputProps={{
                readOnly: true,
              }}
            />
          </TextFieldTitle>
          <TextFieldTitle title="Criado em">
            <TextField
              value={infraction.CREATED_AT}
              InputProps={{
                readOnly: true,
              }}
            />
          </TextFieldTitle>
          <TextFieldTitle title="Tipo">
            <TextField
              value={infraction.TYPE}
              InputProps={{
                readOnly: true,
              }}
            />
          </TextFieldTitle>
          <TextFieldTitle title="Tipo infração">
            <TextField
              value={infraction.TYPE_INFRACTION}
              InputProps={{
                readOnly: true,
              }}
            />
          </TextFieldTitle>
          <TextFieldTitle title="Ordem">
            <TextField
              value={infraction.ORDER}
              InputProps={{
                readOnly: true,
              }}
            />
          </TextFieldTitle>
          <TextFieldTitle title="Cod.">
            <TextField
              value={infraction.CODE}
              InputProps={{
                readOnly: true,
              }}
            />
          </TextFieldTitle>
          <TextFieldTitle title="Período">
            <TextField
              value={infraction.PERIOD}
              InputProps={{
                readOnly: true,
              }}
            />
          </TextFieldTitle>
        </Stack>
        <Stack
          direction={"row"}
          width={"100%"}
          px={"24px"}
          justifyContent={"flex-end"}
        >
          <Button variant="contained" color="error" onClick={handleDelete}>
            Apagar infração
          </Button>
        </Stack>
      </Stack>
    </Drawer>
  );
}
