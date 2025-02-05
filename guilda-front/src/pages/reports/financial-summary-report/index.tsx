import FileOpen from "@mui/icons-material/FileOpen";
import { Card, PageHeader } from "src/components";
import { getLayout } from "src/utils";
import {
  Autocomplete,
  Box,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  TextField,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format } from "date-fns";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListGroupsUseCase } from "src/modules/groups";
import { ListHierarchiesUseCase } from "src/modules/hierarchies/use-cases/list-hierarchies.use-case";
import { Group, Indicator, Sector } from "src/typings";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { PaginationModel } from "src/typings/models/pagination.model";
import { LoadingButton } from "@mui/lab";
import { SheetBuilder } from "src/utils";
import { IndicatorsBySectorsUseCase } from "src/modules/home/use-cases/IndicatorsBySectors/IndicatorsBySectors.use-case";
import { SectorsByHierachyUseCase } from "src/modules/home/use-cases/SectorsByHierarchy/SectorsByHierarchy.use-case";
import { ExportResultsReportUseCase } from "src/modules/home/use-cases/export-results-report";
import { ListFinancialSummaryUseCase } from "src/modules/reports/use-cases/list-financial-summary.use-case";
import { UserInfoContext } from "src/contexts/user-context/user.context";

export interface FinancialSummaryItem {
  DATA_FECHAMENTO: string;
  CODIGO_PRODUTO: string;
  PRODUTO: string;
  ESTOQUE: string;
  SAIDA_ATUAL: string;
  QTD_ESTOQUE: string;
  MOVIMENTACAO_ESTOQUE: string;
  ENTRADA: string;
  LIBERADO_ENTREGA: string;
  PEDIDO: string;
  CANCELADO: string;
  EXPIRADO: string;
  PRATELEIRA: string;
  FECHAMENTO_ESTOQUE: string;
  VALIDADOR: string;
}

export default function FinancialSummaryReportView() {
  const { myUser } = useContext(UserInfoContext);

  const { finishLoading, isLoading, startLoading } = useLoadingState();

  const [startDatePicker, setStartDatePicker] = useState<dateFns | Date | null>(
    null
  );
  const [endDatePicker, setEndDatePicker] = useState<dateFns | Date | null>(
    null
  );

  async function handleReportExtract() {
    startLoading();

    if (!startDatePicker || !endDatePicker) {
      return toast.warning("Selecione as datas");
    }

    const payload = {
      startDate: format(new Date(startDatePicker.toString()), "yyyy-MM-dd"),
      endDate: format(new Date(endDatePicker.toString()), "yyyy-MM-dd"),
    };

    new ListFinancialSummaryUseCase()
      .handle(payload)
      .then((data: FinancialSummaryItem[]) => {
        if (data.length <= 0) {
          return toast.warning("Sem dados para exportar.");
        }
        const docRows = data.map((item) => {
          return [
            item.DATA_FECHAMENTO &&
              `${item.DATA_FECHAMENTO.split(" ")[0].split("/")[1]}/${
                item.DATA_FECHAMENTO.split(" ")[0].split("/")[0]
              }${item.DATA_FECHAMENTO.split(" ")[0].split("/")[2]}`,
            item.CODIGO_PRODUTO,
            item.PRODUTO,
            item.ESTOQUE,
            item.SAIDA_ATUAL,
            item.QTD_ESTOQUE,
            item.MOVIMENTACAO_ESTOQUE,
            item.ENTRADA,
            item.LIBERADO_ENTREGA,
            item.PEDIDO,
            item.CANCELADO,
            item.EXPIRADO,
            item.PRATELEIRA,
            item.FECHAMENTO_ESTOQUE,
            item.VALIDADOR,
          ];
        });
        let indicatorSheetBuilder = new SheetBuilder();
        indicatorSheetBuilder
          .setHeader([
            "Data Fechamento",
            "Código do Produto",
            "Produto",
            "Estoque",
            "Saida Atual",
            "Quantidade Estoque",
            "Movimentação Estoque",
            "Entrada",
            "Liberado Entrega",
            "Pedido",
            "Cancelado",
            "Expirado",
            "Prateleira",
            "Fechamento Estoque",
            "Validador"
          ])
          .append(docRows)
          .exportAs(`Relatório_fechamento_caixa`);
        toast.success("Relatório exportado com sucesso!");
      })
      .catch(() => {
        toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
      })
      .finally(() => {
        finishLoading();
      });
  }

  return (
    <Box display="flex" flexDirection={"column"} width={"100%"}>
      <Card width={"100%"} display={"flex"} flexDirection={"column"}>
        <PageHeader
          title={"Relatório de fechamento de caixa"}
          headerIcon={<FileOpen />}
        />
        <Stack px={2} py={4} width={"100%"} gap={2}>
          <Box width={"100%"} display={"flex"} flexDirection={"column"}>
            <Box display={"flex"} flexDirection={"row"} gap={"10px"} mb={2}>
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
            </Box>
            <LoadingButton
              variant="contained"
              onClick={handleReportExtract}
              sx={{ mt: 2 }}
              loading={isLoading}
            >
              {isLoading ? "Aguarde..." : "Gerar Relatório"}
            </LoadingButton>
          </Box>
        </Stack>
      </Card>
    </Box>
  );
}

FinancialSummaryReportView.getLayout = getLayout("private");
