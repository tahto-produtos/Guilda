import BarChart from "@mui/icons-material/BarChart";
import {
  Box,
  Chip,
  CircularProgress,
  LinearProgress,
  Stack,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format, isValid } from "date-fns";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useLoadingState } from "src/hooks";
import { IntegracaoAPIResultUseCase } from "src/modules/panel/use-cases/IntegracaoAPIResult.use-case";
import { getLayout } from "src/utils";
import { formatCurrency } from "src/utils/format-currency";

interface IntegrationData {
  DATA: string;
  TYPE: string;
  STATUS: string;
  QTD: number;
}

export default function IntegrationsPanel() {
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [startDatePicker, setStartDatePicker] = useState<dateFns | Date | null>(
    null
  );
  const [endDatePicker, setEndDatePicker] = useState<dateFns | Date | null>(
    null
  );
  const [rowCountState, setRowCountState] = useState(100);
  const [data, setData] = useState<IntegrationData[]>([]);

  const getResults = async () => {
    if (
      !startDatePicker ||
      !endDatePicker ||
      !isValid(startDatePicker) ||
      !isValid(endDatePicker)
    ) {
      return toast.warning("Selecione as datas");
    }

    startLoading();

    const payload = {
      dtInicial: format(new Date(startDatePicker.toString()), "yyyy-MM-dd"),
      dtFinal: format(new Date(endDatePicker.toString()), "yyyy-MM-dd"),
    };

    new IntegracaoAPIResultUseCase()
      .handle(payload)
      .then((data) => {
        setData(data);
      })
      .catch(() => {
        toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    startDatePicker && endDatePicker && getResults();
  }, [startDatePicker, endDatePicker]);

  return (
    <Box width={"100%"} display={"flex"} flexDirection={"column"} gap={2}>
      <Card
        width={"100%"}
        display={"flex"}
        flexDirection={"column"}
        justifyContent={"space-between"}
      >
        <PageHeader title={`Painel de Integrações`} headerIcon={<BarChart />} />
        <Stack px={2} py={3} width={"100%"} gap={1} flexDirection={"row"}>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="De"
              value={startDatePicker}
              onChange={(newValue) => setStartDatePicker(newValue)}
              slotProps={{
                textField: {
                  size: "small",
                  sx: {
                    minWidth: "180px",
                    svg: {
                      color: grey[500],
                    },
                    width: "100%",
                  },
                },
              }}
            />
          </LocalizationProvider>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Até"
              value={endDatePicker}
              onChange={(newValue) => setEndDatePicker(newValue)}
              slotProps={{
                textField: {
                  size: "small",
                  sx: {
                    minWidth: "180px",
                    svg: {
                      color: grey[500],
                    },
                    width: "100%",
                  },
                },
              }}
            />
          </LocalizationProvider>
        </Stack>
        {!isLoading ? (
          <DataGrid
            columns={columns}
            rows={data}
            hideFooter
            disableColumnFilter
            disableRowSelectionOnClick
            autoHeight
            rowCount={rowCountState}
            paginationMode="server"
            loading={isLoading}
            slots={{
              loadingOverlay: LinearProgress,
            }}
            getRowId={(row) =>
              `${row.QTD_IMPORTACAO}-${
                Math.floor(Math.random() * (1000 - 1 + 1)) + 1
              }`
            }
          />
        ) : (
          <Stack
            sx={{
              width: "100%",
              justifyContent: "center",
              alignItems: "center",
            }}
          >
            <CircularProgress />
          </Stack>
        )}
      </Card>
    </Box>
  );
}

IntegrationsPanel.getLayout = getLayout("private");

export const columns: GridColDef[] = [
  {
    field: "DATA",
    headerName: "Data",
    flex: 5,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
  },
  {
    field: "TYPE",
    headerName: "Rota",
    flex: 5,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
  },
  {
    field: "STATUS",
    headerName: "Status",
    flex: 5,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Box>
          {params?.row?.STATUS == "COMPLETO" ? (
            <Chip
              label={params?.row?.STATUS}
              color="success"
              variant="outlined"
              size="small"
            />
          ) : (
            <Chip
              label={params?.row?.STATUS}
              color="error"
              variant="outlined"
              size="small"
            />
          )}
        </Box>
      );
    },
  },
  {
    field: "QTD",
    headerName: "Quantidade Informação",
    flex: 3,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    valueFormatter: (params) => {
      return formatCurrency(params.value);
    },
  },
];
