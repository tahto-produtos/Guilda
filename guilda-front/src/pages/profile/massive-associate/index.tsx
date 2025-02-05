import InsertDriveFile from "@mui/icons-material/InsertDriveFile";
import CheckCircle from "@mui/icons-material/CheckCircle";
import ManageAccounts from "@mui/icons-material/ManageAccounts";
import { LoadingButton } from "@mui/lab";
import { Box, Button, Stack, Typography, useTheme } from "@mui/material";
import { Card, PageHeader } from "src/components";
import { SheetBuilder, getLayout } from "src/utils";
import { useLoadingState } from "src/hooks";
import { useState } from "react";
import { grey } from "@mui/material/colors";
import { MassiveCreateCollaboratorUseCase } from "src/modules/collaborators/use-cases/massive-create-collaborators.use-case";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { MassiveAssociateCollaboratorProfileUseCase } from "src/modules/collaborators/use-cases/massive-associate-profiles.use-case";

export default function MassiveAssociateProfiles() {
    const { isLoading, startLoading, finishLoading } = useLoadingState();
    const [file, setFile] = useState<any>(null);
    const [success, setSuccess] = useState<boolean>(false);
    const theme = useTheme();
    const [hasErrors, setHasErrors] = useState<{ failedReason: string }[]>([]);

    const handleUpload = (event: any) => {
        const file = event.target.files[0];
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onloadend = () => {
            setFile(file);
        };
    };

    function downloadExampleFile() {
        try {
            const docRows = [
                ["administrador", "BC787450", "Nome do funcionário "],
                ["administrador", "BC787450", "Nome do funcionário "],
            ];
            let indicatorSheetBuilder = new SheetBuilder();
            indicatorSheetBuilder
                .setHeader(["PERFIL", "IDUSUARIO", "NOMEUSUARIO"])
                .append(docRows)
                .exportAs(`Exemplo_associação_massiva_perfis`);
            toast.success("Exemplo exportado com sucesso!");
        } catch (error) {
            toast.error("Falha na exportação do exemplo!");
        }
    }

    const sendFile = async () => {
        startLoading();

        new MassiveAssociateCollaboratorProfileUseCase()
            .handle(file)
            .then((data) => {
                toast.success("Importação concluida com sucesso!");
                setFile(null);
                setSuccess(true);
            })
            .catch((e) => {
                toast.error(e);
            })
            .finally(() => {
                finishLoading();
            });
    };

    return (
        <Card
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"space-between"}
        >
            <PageHeader
                title={`Associação massiva de perfis`}
                headerIcon={<ManageAccounts />}
                addButtonAction={downloadExampleFile}
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
                            <CheckCircle color="success" />
                            <Typography fontWeight={"600"}>
                                Importação concluida com sucesso!
                            </Typography>
                        </Box>

                        <Button
                            sx={{ px: "40px", mt: 2 }}
                            variant="contained"
                            onClick={() => {
                                setSuccess(false);
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
                            >
                                <Typography>Motivo das falhas</Typography>
                                {hasErrors.map((error, i) => (
                                    <Box
                                        key={i}
                                        display={"flex"}
                                        p={"5px 20px"}
                                        gap={"20px"}
                                        bgcolor={grey[300]}
                                    >
                                        {i} - {error.failedReason}
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
                            <Typography fontSize={"14px"} fontWeight={"500"}>
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
    );
}

MassiveAssociateProfiles.getLayout = getLayout("private");
