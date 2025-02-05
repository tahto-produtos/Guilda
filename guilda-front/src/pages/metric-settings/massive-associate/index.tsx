import { useContext, useState } from "react";
import { Box, Button, Stack, Typography, useTheme } from "@mui/material";
import PermDataSetting from "@mui/icons-material/PermDataSetting";
import { SheetBuilder, getLayout } from "src/utils";
import { Card, PageHeader } from "src/components";
import { useLoadingState } from "src/hooks";
import CheckCircle from "@mui/icons-material/CheckCircle";
import InsertDriveFile from "@mui/icons-material/InsertDriveFile";
import { grey } from "@mui/material/colors";
import { MassiveAssociateUseCase } from "src/modules/sectors/use-cases/massive-associate.use-case";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { LoadingButton } from "@mui/lab";
import Download from "@mui/icons-material/Download";
import DownloadRounded from "@mui/icons-material/DownloadRounded";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { MetricsHistoryReport } from "../../../modules/sectors/components/metrics-history-report";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export default function MassiveAssociate() {
    const { isLoading, startLoading, finishLoading } = useLoadingState();
    const { myPermissions } = useContext(PermissionsContext);
    const [file, setFile] = useState<any>(null);
    const [success, setSuccess] = useState<{
        totalCreated: number;
        totalUpdated: number;
    } | null>(null);
    const theme = useTheme();
    const [hasErrors, setHasErrors] = useState<[]>([]);
    const handleUpload = (event: any) => {
        const file = event.target.files[0];
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onloadend = () => {
            setFile(file);
        };
    };

    const sendFile = async () => {
        startLoading();

        new MassiveAssociateUseCase()
            .handle(file)
            .then((data) => {


                if (data.failed.length > 0) {
                    toast.warning("Importação não concluida!")
                }
                else {
                    toast.success("Importação concluida com sucesso!");
                }

                setSuccess({
                    totalCreated: data.totalCreated,
                    totalUpdated: data.totalUpdated,
                });
                setFile(null)
                data.failed && setHasErrors(data.failed);
                console.log(hasErrors);
            }) 
            .catch((e) => {
                const msg = e?.response?.data?.message || null;

                if (msg) {
                    toast.warning(msg);
                } else {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                }
            })
            .finally(() => {
                finishLoading();
            });
    };

    function downloadExampleFile() {
        try {
            const docRows = [
                [
                    "Teste",
                    "15000",
                    "########",
                    "",
                    "120",
                    "9999",
                    "1",
                    "10",
                    "8",
                    "6",
                    "4",
                    "1",
                ],
            ];
            let indicatorSheetBuilder = new SheetBuilder();
            indicatorSheetBuilder
                .setHeader([
                    "NOME INDICADOR",
                    "INDICADOR",
                    "DT INICIO",
                    "DT FIM",
                    "VALOR DA META",
                    "CODIGO GIP",
                    "STATUS",
                    "MOEDA GRUPO 1",
                    "MOEDA GRUPO 2",
                    "MOEDA GRUPO 3",
                    "MOEDA GRUPO 4",
                    "PESO",
                ])
                .append(docRows)
                .exportAs(`Exemplo_importação_associação_massiva_1`);
            toast.success("Exemplo exportado com sucesso!");
        } catch (error) {
            toast.error("Falha na exportação do exemplo!");
        }
    }

    function downloadExampleFile2() {
        try {
            const docRows = [
                [
                    "Abs Humano",
                    "2",
                    "100",
                    "1889",
                    "SKY - PRE PAGO",
                    "",
                    "",
                    "Diurno",
                    "1",
                    "4",
                    "3",
                    "2",
                    "1",
                    "1",
                    "110% à 999999%",
                    "100% à 109.99%",
                    "90% à 99.99%",
                    "0% à 89.99%",
                    "01/10/2024",
                    "31/10/2024",
                ],
            ];
            let indicatorSheetBuilder = new SheetBuilder();
            indicatorSheetBuilder
                .setHeader([
                    "NOME INDICADOR",
                    "COD INDICADOR",
                    "VALOR DA META",
                    "COD GIP SETOR",
                    "SETOR",
                    "COD GIP SUBSETOR",
                    "SUBSETOR",
                    "TURNO",
                    "STATUS",
                    "MOEDA GRUPO 1",
                    "MOEDA GRUPO 2",
                    "MOEDA GRUPO 3",
                    "MOEDA GRUPO 4",
                    "PESO",
                    "G1",
                    "G2",
                    "G3",
                    "G4",
                    "DATA INICIO",
                    "DATA FIM",
                ])
                .append(docRows)
                .exportAs(`Exemplo_importação_associação_massiva_2`);
            toast.success("Exemplo exportado com sucesso!");
        } catch (error) {
            toast.error("Falha na exportação do exemplo!");
        }
    }

    return (
        <Box width={"100%"} gap={2} display={"flex"} flexDirection={"column"}>
            <Card display={"flex"} width={"100%"} flexDirection={"column"}>
                <PageHeader
                    title={"Input massivo de associação de indicador por setor"}
                    headerIcon={<PermDataSetting />}
                    // secondaryButtonIcon={<Download />}
                    // secondaryButtonTitle="Baixar exemplo"
                    // secondayButtonAction={() => downloadExampleFile()}
                    addButtonAction={() => downloadExampleFile2()}
                    addButtonTitle="Baixar exemplo"
                />

                <Stack width={"100%"}>
                    {success && (
                        <Box
                            sx={{
                                width: "100%",
                                display: "flex",
                                justifyContent: "center",
                                alignItems: "center",
                                flexDirection: "column",

                            }}
                        >
                            <Box display={"flex"} alignItems={"center"} gap={1}>

                                {

                                    hasErrors.length > 0 ? (
                                        <>
                                            <CheckCircle color="error" />
                                            <Typography fontWeight={"600"}>
                                                Importação não concluida!
                                            </Typography>
                                        </>
                                    ) : (

                                        <>
                                            <CheckCircle color="success" />
                                            <Typography fontWeight={"600"}>
                                                Importação concluida com sucesso!
                                            </Typography>
                                        </>
                                    )}
                            </Box>

                            <Box display={"flex"} gap={1}>
                                <Typography fontSize={"14px"}>
                                    Total criado:{" "}
                                    <Typography
                                        fontWeight={"500"}
                                        color={"primary"}
                                        fontSize={"14px"}
                                        component={"span"}
                                    >
                                        {success.totalCreated}
                                    </Typography>
                                </Typography>
                                <Typography fontSize={"14px"}>
                                    Total atualizado:{" "}
                                    <Typography
                                        fontWeight={"500"}
                                        color={"primary"}
                                        fontSize={"14px"}
                                        component={"span"}
                                    >
                                        {success.totalUpdated}
                                    </Typography>
                                </Typography>
                            </Box>
                            <Button
                                sx={{ px: "40px", mt: 2 }}
                                variant="contained"
                                onClick={() => {
                                    setSuccess(null);
                                    setHasErrors([]);
                                }}
                            >
                                Importar outro arquivo
                            </Button>
                            {hasErrors.length > 0 && (
                                <Box
                                    display={"flex"}
                                    flexDirection={"column"}
                                    gap={"5px"}
                                    justifyContent={"center"}
                                    alignItems={"center"}
                                    mt={"20px"}
                                    sx={{  overflowY: "auto", width: "80%" }}
                                >
                                    <Typography>Motivo das falhas</Typography>
                                    {hasErrors.map((error, i) => (
                                        <Box
                                            key={i + 1}
                                            display={"flex"}
                                            p={"5px 20px"}
                                            gap={"20px"}
                                            bgcolor={grey[300]}
                                            width="100%"
                                        >
                                            {i + 1} - {error}
                                        </Box>
                                    ))}
                                </Box>
                            )}
                        </Box>
                    )}
                    {file && !success && (
                        <Box
                            sx={{
                                display: "flex",
                                justifyContent: "center",
                                alignItems: "center",
                                gap: 2,
                                px: "20px",
                                mt: "20px",
                            }}
                        >
                            <Box
                                sx={{
                                    backgroundColor: theme.palette.primary.main,
                                    minWidth: "50px",
                                    height: "50px",
                                    borderRadius: "20px",
                                    alignItems: "center",
                                    justifyContent: "center",
                                    display: "flex",
                                }}
                            >
                                <InsertDriveFile
                                    sx={{ fontSize: "30px", color: "#fff" }}
                                />
                            </Box>
                            <Box>
                                <Typography
                                    fontSize={"12px"}
                                    sx={{ color: grey[600] }}
                                >
                                    Arquivo selecionado
                                </Typography>
                                <Typography
                                    fontSize={"14px"}
                                    fontWeight={"500"}
                                >
                                    {file.name}
                                </Typography>
                                <Typography fontSize={"12px"}>
                                    Tamanho: {file.size}k
                                </Typography>
                            </Box>
                        </Box>
                    )}
                    {!success && (
                        <Box width={"100%"} p={"20px 20px"}>
                            <input
                                accept=".xls, .xlsx, .csv"
                                style={{ display: "none" }}
                                id="image-upload"
                                type="file"
                                onChange={(e) => handleUpload(e)}
                            />
                            <label htmlFor="image-upload">
                                <Button
                                    variant="outlined"
                                    color="primary"
                                    component="span"
                                    fullWidth
                                    disabled={isLoading}
                                >
                                    Selecione um arquivo
                                </Button>
                            </label>
                            <LoadingButton
                                onClick={sendFile}
                                fullWidth
                                loading={isLoading}
                                variant="contained"
                                sx={{ mt: 1 }}
                                disabled={!file}
                            >
                                Enviar arquivo
                            </LoadingButton>
                        </Box>
                    )}
                </Stack>
            </Card>
            <MetricsHistoryReport />
        </Box>
    );
}

MassiveAssociate.getLayout = getLayout("private");
