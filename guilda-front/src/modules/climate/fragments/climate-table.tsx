import {
  Autocomplete,
  Box,
  LinearProgress,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { useState } from "react";
import { useLoadingState } from "src/hooks";
import { uuid } from "uuidv4";
import { SendFeedbackClimateUseCase } from "../send-feedback.use-case";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { BaseModal } from "src/components/feedback";
import { ActionButton } from "src/components/inputs/action-button/action-button";
import { ProfileImage } from "src/components/data-display/profile-image/profile-image";
import { capitalizeText } from "src/utils/capitalizeText";

interface ClimateTableProps {
  tableData: any;
  refreshHandle: () => void;
}

export function ClimateTable(props: ClimateTableProps) {
  const { tableData, refreshHandle } = props;

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
        toast.success("Apoio supervisor aplicado com sucesso!");
      })
      .catch(() => {
        toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
      })
      .finally(() => {
        finishLoading();
      });
  };

  const columns: GridColDef[] = [
    {
      field: "name",
      headerName: "Nome",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString();
      },
      minWidth: 400,
      renderCell: (params) => {
        return (
          <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
            <ProfileImage height="40px" width="40px" name={params.value} />
            <Typography fontSize={"14px"} fontWeight={"600"}>
              {capitalizeText(params.value || "")}
            </Typography>
          </Stack>
        );
      },
    },
    /* {
      field: "idUserClimate",
      headerName: "Id",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString();
      },
      minWidth: 200,
    }, */
    {
      field: "data",
      headerName: "Data",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        const date = new Date(value);
        return new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' }).format(date).replace(",", "");
        //return value.toString();
      },
      minWidth: 200,
    },

    {
      field: "BC",
      headerName: "BC",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString();
      },
      minWidth: 200,
    },
    {
      field: "climate",
      headerName: "Clima",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString();
      },
      minWidth: 300,
    },
    {
      field: "reason",
      headerName: "Motivo",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString() || "-";
      },
      minWidth: 450,
    },
    {
      field: "applyType",
      headerName: "Apoio Supervisor",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      renderCell: (params) => {
        return (
          <Box width={"85px"}>
            {params.row.canApply === true && params.row.applyType == "" ? (
              <ActionButton
                title={"Apoio Supervisor"}
                loading={false}
                isActive={false}
                onClick={() => handleOnSelectApply(params.row.idUserClimate)}
              />
            ) : (
              <Typography variant="body1" fontSize={"13px"}>
                {params.row.applyType}
              </Typography>
            )}
          </Box>
        );
      },
      minWidth: 200,
    },
    {
      field: "nomeSupervisor",
      headerName: "Supervisor",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString() || "-";
      },
      minWidth: 400,
    },

    {
      field: "nomeCoordenador",
      headerName: "Coordenador",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString() || "-";
      },
      minWidth: 400,
    },
    {
      field: "nomeGerenteII",
      headerName: "Gerente II",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString() || "-";
      },
      minWidth: 400,
    },

    {
      field: "nomeGerenteI",
      headerName: "Gerente I",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString() || "-";
      },
      minWidth: 400,
    },
    {
      field: "nomeDiretor",
      headerName: "Diretor",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString() || "-";
      },
      minWidth: 400,
    },
    {
      field: "nomeCeo",
      headerName: "CEO",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString() || "-";
      },
      minWidth: 400,
    },
  ];

  return (
    <>
      <DataGrid
        columns={columns}
        rows={tableData?.length > 0 ? tableData : []}
        hideFooter
        disableColumnFilter
        disableRowSelectionOnClick
        autoHeight
        // rowCount={20}
        localeText={{
          noResultsOverlayLabel: "Nenhum resultado encontrado",
        }}
        paginationMode="server"
        loading={isLoading}
        slots={{
          loadingOverlay: LinearProgress,
        }}
        getRowId={(row) => {
          const uuidv4 = uuid();
          return uuidv4;
        }}
        sx={{ width: "100%" }}
      />
      <BaseModal
        width={"540px"}
        open={isOpenModalFeedBack}
        title={`Apoio Supervisor`}
        onClose={() => setIsOpenModalFeedBack(!isOpenModalFeedBack)}
      >
        <Box width={"100%"} display={"flex"} flexDirection={"column"} gap={1}>
          <Autocomplete
            placeholder={"Apoio Supervisor"}
            onChange={(_, type) => setTypeFeedbackSelected(type)}
            isOptionEqualToValue={(option, value) => option.type === value.type}
            renderInput={(props) => (
              <TextField {...props} size={"small"} label={"Apoio Supervisor"} />
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
    </>
  );
}
