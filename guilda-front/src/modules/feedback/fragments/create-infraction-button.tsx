import { Close } from "@mui/icons-material";
import FilterAltOutlined from "@mui/icons-material/FilterAltOutlined";
import {
  Autocomplete,
  Button,
  Divider,
  Drawer,
  IconButton,
  Stack,
  TextField,
  Typography,
  useTheme,
} from "@mui/material";
import { useEffect, useState } from "react";
import { TextFieldTitle } from "src/components/inputs/title-text-field/title-text-field";
import { ListPedagogicalEscaleUseCase } from "../use-cases/listPedagogicalScale.use-case";
import { ListGravityPedagogicalEscale } from "../use-cases/list-pedagogical-scale-gravity.use-case";
import { PedagogicalScale } from "src/typings/models/pedagogical-scale.model";
import { PedagogicalGravity } from "src/typings/models/pedagogical-gravity.model";
import { ListTypePedagogicalScaleUseCase } from "../use-cases/ListTypePedagogicalScale.use-case";
import { TypePedagogicalScale } from "src/typings/models/type-pedagogical-scale.model";
import { CreatePedagogicalScaleUseCase } from "../use-cases/CreatedPedagogicalScale.use.case";
import { toast } from "react-toastify";

interface CreateInfractionButtonProps {
  refreshList?: () => void;
}

export function CreateInfractionButton(props: CreateInfractionButtonProps) {
  const { refreshList } = props;

  const [isOpen, setIsOpen] = useState<boolean>(false);
  const [name, setName] = useState<string>("");
  const [code, setCode] = useState<number | null>(null);
  const [order, setOrder] = useState<number | null>(null);
  const [timeoff, setTimeoff] = useState<number | null>(null);

  const [pedagogicalScalesGravity, setPedagogicalScalesGravity] = useState<
    PedagogicalGravity[]
  >([]);
  const [
    selectedTypePedagogicalScalesGravity,
    setSelectedTypePedagogicalScalesGravity,
  ] = useState<PedagogicalGravity | null>(null);

  const [pedagogicalScalesType, setPedagogicalScalesType] = useState<
    TypePedagogicalScale[]
  >([]);
  const [
    selectedTypePedagogicalScalesType,
    setSelectedTypePedagogicalScalesType,
  ] = useState<TypePedagogicalScale | null>(null);

  const [pedagogicalScales, setPedagogicalScales] = useState<
    PedagogicalScale[]
  >([]);

  const [selectedPedagogicalScales, setSelectedPedagogicalScales] =
    useState<PedagogicalScale | null>(null);

  const theme = useTheme();

  async function listPedagogical() {
    await new ListPedagogicalEscaleUseCase()
      .handle()
      .then((data) => {
        setPedagogicalScales(data);
      })
      .catch(() => {})
      .finally(() => {});
  }

  useEffect(() => {
    listPedagogical();
  }, []);

  async function listTypePedagogicalType() {
    await new ListTypePedagogicalScaleUseCase()
      .handle()
      .then((data) => {
        setPedagogicalScalesType(data);
      })
      .catch(() => {})
      .finally(() => {});
  }

  useEffect(() => {
    listTypePedagogicalType();
  }, []);

  async function listTypePedagogicalGravity() {
    await new ListGravityPedagogicalEscale()
      .handle()
      .then((data) => {
        setPedagogicalScalesGravity(data);
      })
      .catch(() => {})
      .finally(() => {});
  }

  useEffect(() => {
    listTypePedagogicalGravity();
  }, []);

  async function handleCreate() {
    if (
      !selectedTypePedagogicalScalesGravity ||
      !selectedTypePedagogicalScalesType ||
      !code ||
      !name ||
      !order
    )
      return;

    await new CreatePedagogicalScaleUseCase()
      .handle({
        NAME_INFRACTION: name,
        IDGDA_PEDAGOGICAL_SCALE_GRAVITY:
          selectedTypePedagogicalScalesGravity?.IDGDA_PEDAGOGICAL_SCALE_GRAVITY,
        IDGDA_PEDAGOGICAL_SCALE_TYPE:
          selectedTypePedagogicalScalesType?.IDGDA_PEDAGOGICAL_SCALE_TYPE,
        PEDAGOGICAL_ORDER: order,
        TIME_OFF: timeoff || 0,
        CODE: code,
      })
      .then(() => {
        toast.success("Infração criada com sucesso!");
        refreshList && refreshList();
      })
      .catch(() => {})
      .finally(() => {});
  }

  return (
    <>
      <Drawer
        open={isOpen}
        anchor={"right"}
        onClose={() => setIsOpen(false)}
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
            <IconButton onClick={() => setIsOpen(false)}>
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
                placeholder="Digite um nome"
                value={name}
                onChange={(e) => setName(e.target.value)}
              />
            </TextFieldTitle>
            <TextFieldTitle title="Código da infração">
              <TextField
                placeholder="Atribua um código"
                value={code}
                InputProps={{
                  type: "number",
                }}
                onChange={(e) => setCode(parseInt(e.target.value.toString()))}
              />
            </TextFieldTitle>
            <TextFieldTitle title="Ordem da infração">
              <TextField
                placeholder="Deifna a ordem"
                value={order}
                InputProps={{
                  type: "number",
                }}
                onChange={(e) => setOrder(parseInt(e.target.value.toString()))}
              />
            </TextFieldTitle>
            {/* <TextFieldTitle title="Escala pedagógica">
              <Autocomplete
                options={pedagogicalScales}
                getOptionLabel={(option) => option.PEDAGOGICAL_SCALE}
                onChange={(event, value) => {
                  setSelectedPedagogicalScales(value);
                }}
                sx={{ p: 0, m: 0 }}
                filterOptions={(x) => x}
                filterSelectedOptions
                fullWidth
                renderInput={(params) => (
                  <TextField
                    {...params}
                    variant="outlined"
                    placeholder="Selecione"
                  />
                )}
                renderOption={(props, option) => {
                  return (
                    <li {...props} key={option.IDGDA_PEDAGOGICAL_SCALE_TYPE}>
                      {option.PEDAGOGICAL_SCALE}
                    </li>
                  );
                }}
                isOptionEqualToValue={(option, value) =>
                  option.PEDAGOGICAL_SCALE === value.PEDAGOGICAL_SCALE
                }
              />
            </TextFieldTitle> */}
            <TextFieldTitle title="Tipo de infração">
              <Autocomplete
                value={selectedTypePedagogicalScalesType}
                options={pedagogicalScalesType}
                getOptionLabel={(option) => option.PEDAGOGICAL_SCALE_TYPE}
                onChange={(event, value) => {
                  setSelectedTypePedagogicalScalesType(value);
                }}
                filterOptions={(x) => x}
                filterSelectedOptions
                fullWidth
                sx={{ p: 0, m: 0 }}
                renderInput={(params) => (
                  <TextField
                    {...params}
                    variant="outlined"
                    placeholder="Selecione"
                  />
                )}
                renderOption={(props, option) => {
                  return (
                    <li {...props} key={option.PEDAGOGICAL_SCALE_TYPE}>
                      {option.PEDAGOGICAL_SCALE_TYPE}
                    </li>
                  );
                }}
                isOptionEqualToValue={(option, value) =>
                  option.PEDAGOGICAL_SCALE_TYPE === value.PEDAGOGICAL_SCALE_TYPE
                }
              />
            </TextFieldTitle>
            <TextFieldTitle title="Tipo de penalidade">
              <Autocomplete
                value={selectedTypePedagogicalScalesGravity}
                options={pedagogicalScalesGravity}
                getOptionLabel={(option) => option.PEDAGOGICAL_SCALE_GRAVITY}
                onChange={(event, value) => {
                  setSelectedTypePedagogicalScalesGravity(value);
                }}
                sx={{ p: 0, m: 0 }}
                filterOptions={(x) => x}
                filterSelectedOptions
                fullWidth
                renderInput={(params) => (
                  <TextField
                    {...params}
                    variant="outlined"
                    placeholder="Selecione"
                  />
                )}
                renderOption={(props, option) => {
                  return (
                    <li {...props} key={option.PEDAGOGICAL_SCALE_GRAVITY}>
                      {option.PEDAGOGICAL_SCALE_GRAVITY}
                    </li>
                  );
                }}
                isOptionEqualToValue={(option, value) =>
                  option.PEDAGOGICAL_SCALE_GRAVITY ===
                  value.PEDAGOGICAL_SCALE_GRAVITY
                }
              />
            </TextFieldTitle>
            <TextFieldTitle title="Tempo de penalidade">
              <TextField
                placeholder="Atribua um período"
                value={timeoff}
                InputProps={{
                  type: "number",
                }}
                onChange={(e) =>
                  setTimeoff(parseInt(e.target.value.toString()))
                }
              />
            </TextFieldTitle>
          </Stack>
          <Stack
            direction={"row"}
            width={"100%"}
            px={"24px"}
            justifyContent={"flex-end"}
          >
            <Button
              variant="contained"
              color="secondary"
              onClick={handleCreate}
            >
              Criar nova penalidade
            </Button>
          </Stack>
        </Stack>
      </Drawer>
      <Button
        variant="contained"
        color="secondary"
        endIcon={<FilterAltOutlined />}
        onClick={() => setIsOpen(true)}
      >
        Criar nova infração
      </Button>
    </>
  );
}
