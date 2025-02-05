import Search from "@mui/icons-material/Search";
import InsertDriveFile from "@mui/icons-material/InsertDriveFile";
import {
  Box,
  Button,
  FormControl,
  InputAdornment,
  InputLabel,
  MenuItem,
  Select,
  TextField,
  Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { Stack } from "@mui/system";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format, isValid, set } from "date-fns";
import { useContext, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useLoadingState } from "src/hooks";
import { ListMonetizationHierarchyUseCase } from "src/modules/monetization/use-cases/list-monetization-hierarchy";
import { DateUtils, SheetBuilder, getLayout } from "src/utils";
import { ListTransactions } from "src/modules/monetization/use-cases/list-transactions.use-case";
import { ModalMonetizationReportAdm } from "src/modules/monetization/fragments/modal-monetization-report-adm";
import { ModalMonetizationReportAdmDay } from "src/modules/monetization/fragments/modal-monetization-report-adm-day";
import { ModalMonetizationReportConsolidado } from "src/modules/monetization/fragments/modal-monetization-report-consolidado";
import { formatCurrency } from "src/utils/format-currency";
import abilityFor from "src/utils/ability-for";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { ReportQuizUseCase } from "src/modules/quiz/use-cases/report-quiz.use-case";
import { FeedbackReportUseCase } from "src/modules/feedback/use-cases/feedback-report.use-case";

interface BalanceReportModel {
  hierarchy: string;
  sectors: Array<{
    sectorId: number;
    sector: string;
    monetization: number;
  }>;
}

function formatDetails(
  order: any,
  indicator: any,
  reason: any,
  observation: any
) {
  if (reason) {
    return `Motivo: ${reason}`;
  } else if (observation) {
    return `${observation}`;
  } else if (indicator) {
    return `Indicador: ${indicator.name}`;
  } else if (order) {
    return `Compra na loja - COD: ${order.id}`;
  }
}

export default function FeedbackReport() {
  const { myPermissions } = useContext(PermissionsContext);
  const [startDate, setStartDate] = useState<dateFns | null>(null);
  const [endDate, setEndDate] = useState<dateFns | null>(null);

  const [startDateC, setStartDateC] = useState<dateFns | null>(null);
  const [endDateC, setEndDateC] = useState<dateFns | null>(null);

  const [startDateP, setStartDateP] = useState<dateFns | null>(null);
  const [endDateP, setEndDateP] = useState<dateFns | null>(null);

  const [startDateR, setStartDateR] = useState<dateFns | null>(null);
  const [endDateR, setEndDateR] = useState<dateFns | null>(null);

  const [data, setData] = useState<any>(null);
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [reportList, setReportList] = useState<BalanceReportModel[]>([]);
  const [searchText, setSearchText] = useState<string>("");
  const [filteredBy, setFilteredBy] = useState<string>("");
  const [filterValue, setFilterValue] = useState<string>("");
  const [limit, setLimit] = useState<number>(100);
  const { myUser } = useContext(UserInfoContext);
  const [isOpen, setIsOpen] = useState<boolean>(false);
  const [isOpenDay, setIsOpenDay] = useState<boolean>(false);

  const [isOpenConsolidado, setIsOpenConsolidado] = useState<boolean>(false);

  function handleReportExtractExport() {
    if (!myUser || !startDateC || !endDateC) return;
    startLoading();

    new FeedbackReportUseCase()
      .handle({
        DATE_START: format(new Date(startDateC.toString()), "yyyy-MM-dd"),
        DATE_END: format(new Date(endDateC.toString()), "yyyy-MM-dd"),
      })
      .then((data) => {
        if (data.length <= 0) {
          return toast.warning("Sem dados para exportar.");
        }

        const docRows = data.map((item: any) => {
          return [
            item.PROTOCOLO,
            item.RESPONSAVEL,
            item.COLABORADOR,
            item.MOTIVO_FEEDBACK,
            item.DETAIL,
            item.CREATED_AT,
            item.SIGNED_AT,
          ];
        });
        let indicatorSheetBuilder = new SheetBuilder();
        indicatorSheetBuilder
          .setHeader([
            "PROTOCOLO",
            "RESPONSAVEL",
            "COLABORADOR",
            "MOTIVO_FEEDBACK",
            "DETALHE",
            "DATA_DO_FEEDBACK",
            "DATA_ASSINATURA",
          ]) 
          .append(docRows)
          .exportAs(`Relatório_feedback`);
      })
      .catch(() => {
        toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
      })
      .finally(() => {
        finishLoading();
      });
  }

  function formatResultDate(dateString: string) {
    if (dateString) {
      const date = dateString.split("T")[0];
      const dateSplited = date.split("-");
      return `${dateSplited[2]}/${dateSplited[1]}/${dateSplited[0]}`;
    } else {
      return "";
    }
  }

  return (
    <Card
      width={"100%"}
      display={"flex"}
      flexDirection={"column"}
      justifyContent={"space-between"}
    >
      <PageHeader
        title={`Relatório de Feedback`}
        headerIcon={<InsertDriveFile />}
      />
      <Stack px={2} py={3} width={"100%"} gap={2}>
        {/* <Box display={"flex"} gap={2} width={"100%"}>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Data Inicial"
              value={startDate}
              onChange={(newValue) => setStartDate(newValue)}
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
              label="Data Final"
              value={endDate}
              onChange={(newValue) => setEndDate(newValue)}
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
        </Box> */}

        <Box display={"flex"} gap={2} width={"100%"}>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Data inicial"
              value={startDateC}
              onChange={(newValue) => setStartDateC(newValue)}
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
              label="Data final"
              value={endDateC}
              onChange={(newValue) => setEndDateC(newValue)}
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
        </Box>
        {/* 
        <Box display={"flex"} gap={2} width={"100%"}>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Data Publicação (início)"
              value={startDateP}
              onChange={(newValue) => setStartDateP(newValue)}
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
              label="Data Publicação (final)"
              value={endDateP}
              onChange={(newValue) => setEndDateP(newValue)}
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
        </Box>

        <Box display={"flex"} gap={2} width={"100%"}>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Data Resposta (início)"
              value={startDateR}
              onChange={(newValue) => setStartDateR(newValue)}
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
              label="Data Resposta (final)"
              value={endDateR}
              onChange={(newValue) => setEndDateR(newValue)}
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
        </Box> */}
        <Button
          onClick={handleReportExtractExport}
          sx={{ mt: "20px" }}
          variant="contained"
        >
          Exportar Relatório
        </Button>
      </Stack>
    </Card>
  );
}

FeedbackReport.getLayout = getLayout("private");
