import DownloadRounded from "@mui/icons-material/DownloadRounded";
import { LoadingButton } from "@mui/lab";
import { Box, Stack } from "@mui/material";
import { grey } from "@mui/material/colors";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format, isValid } from "date-fns";
import { useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useLoadingState } from "src/hooks";
import { MetricBySectorUseCase } from "src/modules/sectors/use-cases/MetricsBySector.use-case";
import { DateUtils, SheetBuilder } from "src/utils";

export function MetricsHistoryReport() {
    const { isLoading, startLoading, finishLoading } = useLoadingState();

    const [startDatePicker, setStartDatePicker] = useState<
        dateFns | Date | null
    >(null);
    const [endDatePicker, setEndDatePicker] = useState<dateFns | Date | null>(
        null
    );

    function exportReport() {
        let formatedStartDate = null;
        let formatedEndDate = null;

        if (
            startDatePicker &&
            endDatePicker &&
            isValid(startDatePicker) &&
            isValid(endDatePicker)
        ) {
            formatedStartDate = format(
                new Date(startDatePicker.toString()),
                "yyyy-MM-dd"
            );
            formatedEndDate = DateUtils.formatDateIsoEndOfDay(
                new Date(endDatePicker.toString())
            );
        } else {
            toast.warning("Selecione datas válidas");
            return;
        }

        startLoading();

        const payload = {
            datainicial: formatedStartDate,
            datafinal: formatedEndDate,
        };

        new MetricBySectorUseCase()
            .handle(payload)
            .then((data) => {
                if (data.length <= 0) {
                    return toast.warning("Sem dados para exportar.");
                }

                const docRows = data.map((item: any) => {
                    return [
                        item.SETOR,
                        { v: item.CODGIP, t: "n" },
                        item.SUBSETOR,
                        { v: item.CODGIPSUB, t: "n" },
                        item.INDICADOR,
                        { v: item.CODINDICADOR, t: "n" },
                        item.TIPO,
                        item.TURNO,
                        { v: item.META, t: "n" },
                        item.GRUPO,
                        { v: item.META_MINIMA, t: "n" },
                        { v: item.MOEDAS, t: "n" },
                        item.DATAINICIAL &&
                            `${item.DATAINICIAL.split(" ")[0].split("/")[1]}/${
                                item.DATAINICIAL.split(" ")[0].split("/")[0]
                            }/${item.DATAINICIAL.split(" ")[0].split("/")[2]}`,
                        item.DATAFINAL &&
                            `${item.DATAFINAL.split(" ")[0].split("/")[1]}/${
                                item.DATAFINAL.split(" ")[0].split("/")[0]
                            }/${item.DATAFINAL.split(" ")[0].split("/")[2]}`,
                        item.STATUS,
                    ];
                });
                let indicatorSheetBuilder = new SheetBuilder();
                indicatorSheetBuilder
                    .setHeader([
                        "Setor",
                        "Cod. GIP",
                        "Sub Setor",
                        "Cod. GIP Sub",
                        "Indicador",
                        "Cod. Indicador",
                        "Tipo",
                        "Turno",
                        "Meta",
                        "Grupo",
                        "Meta Min.",
                        "Moedas",
                        "Data Inicial",
                        "Data Final",
                        "Status",
                    ])
                    .append(docRows)
                    .exportAs(`Relatório de Histório de métricas`);
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
        <Card display={"flex"} width={"100%"} flexDirection={"column"}>
            <PageHeader
                title={"Extração histórico de métricas"}
                headerIcon={<DownloadRounded />}
            />

            <Stack width={"100%"}>
                <Box width={"100%"} p={"20px 20px"}>
                    <Box
                        display={"flex"}
                        flexDirection={"row"}
                        gap={"10px"}
                        mb={2}
                    >
                        <LocalizationProvider dateAdapter={AdapterDateFns}>
                            <DatePicker
                                label="De"
                                value={startDatePicker}
                                onChange={(newValue) =>
                                    setStartDatePicker(newValue)
                                }
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
                                onChange={(newValue) =>
                                    setEndDatePicker(newValue)
                                }
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
                        onClick={exportReport}
                        fullWidth
                        loading={isLoading}
                        variant="contained"
                        sx={{ mt: 1 }}
                        disabled={!startDatePicker || !endDatePicker}
                    >
                        Exportar relatório
                    </LoadingButton>
                </Box>
            </Stack>
        </Card>
    );
}
