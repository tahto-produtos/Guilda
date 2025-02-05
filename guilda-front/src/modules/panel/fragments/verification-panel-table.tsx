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
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { BaseModal } from "src/components/feedback";
import { ActionButton } from "src/components/inputs/action-button/action-button";
import { ProfileImage } from "src/components/data-display/profile-image/profile-image";
import { capitalizeText } from "src/utils/capitalizeText";

interface VerificationPanelTableProps {
  tableData: any;
  refreshHandle: () => void;
}

export function VerificationPanelTable(props: VerificationPanelTableProps) {
  const { tableData, refreshHandle } = props;

  const { finishLoading, isLoading, startLoading } = useLoadingState();

  const columns: GridColDef[] = [
    {
      field: "ID_COLLABORATOR",
      headerName: "BC",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString();
      },
      minWidth: 100,
    },
    {
      field: "NAME",
      headerName: "Nome",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString().toUpperCase();
      },
      minWidth: 300,
    },
    {
      field: "HIERARQUIA",
      headerName: "Hierarquia",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString();
      },
      minWidth: 150,
    },
    {
      field: "ATIVO",
      headerName: "Ativo",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString();
      },
      minWidth: 100,
    },
    {
      field: "REPROCESSAMENTO",
      headerName: "Processamento",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString();
      },
      minWidth: 150,
    },
    {
      field: "MOEDAS_GANHAS",
      headerName: "Moedas Ganhas",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString() || "-";
      },
      minWidth: 150,
    },
    {
      field: "FATOR0",
      headerName: "Fator 0",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString() || "-";
      },
      minWidth: 100,
    },
    {
      field: "FATOR1",
      headerName: "Fator 1",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString() || "-";
      },
      minWidth: 100,
    },
    {
      field: "DATA_RECEBIMENTO_RESULTADO",
      headerName: "Data Recebimento Resultado",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        if(value) {
          const date = new Date(value);
          return new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' }).format(date).replace(",", "");
        } else {
          return value.toString()
        }
      },
      minWidth: 300,
    },

    {
      field: "META_UTILIZADA_MOEDA",
      headerName: "Meta Utilizada Moeda",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString() || "-";
      },
      minWidth: 250,
    },
    {
      field: "METRICA_MINIMA",
      headerName: "Métrica Mínima",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString() || "-";
      },
      minWidth: 150,
    },

    {
      field: "METRICA_GRUPO",
      headerName: "Métrica Grupo",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString() || "-";
      },
      minWidth: 150,
    },
    {
      field: "ALTEROU_METRICA",
      headerName: "Alterou Métrica",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString() || "-";
      },
      minWidth: 150,
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
    </>
  );
}
