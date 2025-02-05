import {
    Box,
    Button,
    Stack,
    TextField,
    Typography,
    useTheme,
} from "@mui/material";
import { useState } from "react";
import { useLoadingState } from "src/hooks";
import { MassiveCreditUseCase } from "../use-cases/massive-credit.use-case";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import InsertDriveFile from "@mui/icons-material/InsertDriveFile";
import CheckCircle from "@mui/icons-material/CheckCircle";
import { grey } from "@mui/material/colors";
import { LoadingButton } from "@mui/lab";
import { MassiveDebitUseCase } from "../use-cases/massive-debit.use-case";

export function MassiveDebits() {
    const { isLoading, startLoading, finishLoading } = useLoadingState();
    const [file, setFile] = useState<any>(null);
    const [success, setSuccess] = useState<boolean>(false);
    const [collaboratorId, setCollaboratorId] = useState<string>("");
    const theme = useTheme();

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

        new MassiveDebitUseCase()
            .handle(file, collaboratorId)
            .then((data) => {
                toast.success("Importação concluida com sucesso!");
                setSuccess(true);
                setFile(null);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    return (
        <Stack width={"100%"}>
            {success && (
                <Box
                    sx={{
                        width: "100%",
                        display: "flex",
                        justifyContent: "center",
                        alignItems: "center",
                        flexDirection: "column",
                        height: "300px",
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
                        onClick={() => setSuccess(false)}
                    >
                        Importar outro arquivo
                    </Button>
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
                        <Typography fontSize={"12px"} sx={{ color: grey[600] }}>
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
                    <TextField
                        label="Código do Colaborador"
                        value={collaboratorId}
                        onChange={(e) => setCollaboratorId(e.target.value)}
                        fullWidth
                        sx={{ mb: 1 }}
                    />
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
    );
}
