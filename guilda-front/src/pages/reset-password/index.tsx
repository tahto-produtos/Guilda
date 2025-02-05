import {
    Box,
    Button,
    Stack,
    TextField,
    Typography,
    useTheme,
} from "@mui/material";
import Person from "@mui/icons-material/Person";
import { useRouter } from "next/router";
import { LoadingButton } from "@mui/lab";
import Logo from "../../assets/logo/logo.png";
import Image from "next/image";
import Cookies from "js-cookie";
import { INTERNAL_SERVER_ERROR_MESSAGE, jwtTokenKey } from "src/constants";
import { useLoadingState } from "src/hooks";
import { grey } from "@mui/material/colors";
import { ResetPasswordUseCase } from "src/modules/auth/use-cases/reset-password/reset-password.use-case";
import { toast } from "react-toastify";
import { useState } from "react";
import { LogoutUseCase } from "src/modules/auth/use-cases";

export default function ResetPassword() {
    const router = useRouter();
    const theme = useTheme();
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [password, setPassword] = useState<string>("");
    const [newPassword, setNewPassword] = useState<string>("");
    const [confirmNewPassword, setConfirmNewPassword] = useState<string>("");

    const handleResetPassword = async () => {
        if (!password || !newPassword || !confirmNewPassword) {
            return toast.warning("Preencha todos os campos");
        }

        startLoading();

        const payload = {
            currentPassword: password,
            newPassword: newPassword,
            confirmNewPassword: confirmNewPassword,
        };

        new ResetPasswordUseCase()
            .handle(payload)
            .then((data) => {
                toast.success("Senha alterada com sucesso!");
                handleLogout();
            })
            .catch((e) => {
                if (e?.response?.data?.message == "collaborator not found") {
                    return toast.error("Colaborador não encontrado");
                }
                if (
                    e?.response?.data?.message ==
                    "new password must be different than the current"
                ) {
                    return toast.error(
                        "A nova senha deve ser diferente da atual"
                    );
                }
                if (
                    e?.response?.data?.message == "current password not match"
                ) {
                    return toast.error("Senha atual inválida");
                }
                if (
                    e?.response?.data?.message ==
                    "new password not match confirm password"
                ) {
                    return toast.error(
                        "Nova senha não combina com a confirmação de senha."
                    );
                }
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    const handleLogout = async () => {
        startLoading();

        new LogoutUseCase()
            .handle()
            .then(() => {})
            .catch((e) => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
                return router.push("/login");
            });
    };

    return (
        <Box
            display={"flex"}
            justifyContent={"center"}
            alignItems={"center"}
            minHeight={"100vh"}
            sx={{
                background:
                    "linear-gradient(90deg, rgba(91, 68, 146, 1) 0%, rgba(48, 166, 152, 1) 100%)",
            }}
        >
            <Box
                sx={{
                    width: "400px",
                    [theme.breakpoints.only("xs")]: {
                        width: "100%",
                        px: 1,
                    },
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center",
                    flexDirection: "column",
                    gap: 4,
                }}
            >
                <Stack
                    py={3}
                    px={2}
                    borderRadius={"0.3rem"}
                    bgcolor={"white"}
                    border={"1px solid #f5f5f5"}
                    display={"flex"}
                    alignItems={"center"}
                    width={"100%"}
                >
                    <Image
                        src={Logo}
                        alt="Tahto"
                        width={150}
                        height={80}
                        style={{ marginBottom: 30 }}
                    />
                    <Box display={"flex"} flexDirection={"column"} gap={2}>
                        <Box
                            display={"flex"}
                            flexDirection={"column"}
                            gap={"5px"}
                        >
                            <Typography
                                textAlign={"center"}
                                fontSize={"14px"}
                                fontWeight={"600"}
                            >
                                Bem vindo(a), este é o seu primeiro login!
                            </Typography>
                            <Typography
                                textAlign={"center"}
                                fontSize={"13px"}
                                color={grey[700]}
                                lineHeight={"17px"}
                            >
                                Por favor preencha os campos abaixo para
                                realizar sua primeira troca de senha.
                            </Typography>
                            <Typography
                                textAlign={"center"}
                                fontSize={"13px"}
                                color={grey[700]}
                                lineHeight={"17px"}
                            >
                                Mínimo de 6 (seis) caracteres, sendo, 
                                obrigatoriamente, 2 (duas) letras.
                            </Typography>
                        </Box>
                        <Box display={"flex"} flexDirection={"column"} gap={1}>
                            <TextField
                                label="Senha atual"
                                fullWidth
                                size="small"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                            />
                            <TextField
                                label="Nova senha"
                                fullWidth
                                size="small"
                                value={newPassword}
                                onChange={(e) => setNewPassword(e.target.value)}
                            />
                            <TextField
                                label="Confirme a nova senha"
                                fullWidth
                                size="small"
                                value={confirmNewPassword}
                                onChange={(e) =>
                                    setConfirmNewPassword(e.target.value)
                                }
                            />
                        </Box>
                    </Box>

                    <Box
                        width={"100%"}
                        display={"flex"}
                        justifyContent={"space-between"}
                        alignItems={"center"}
                        mt={"20px"}
                        flexDirection={"column"}
                        gap={"5px"}
                    >
                        <LoadingButton
                            loading={isLoading}
                            variant="contained"
                            fullWidth
                            disabled={
                                isLoading ||
                                !password ||
                                !newPassword ||
                                !confirmNewPassword
                            }
                            onClick={handleResetPassword}
                        >
                            Confirmar nova senha
                        </LoadingButton>
                        <LoadingButton
                            variant="outlined"
                            color="error"
                            fullWidth
                            onClick={handleLogout}
                            loading={isLoading}
                        >
                            Fazer logout
                        </LoadingButton>
                    </Box>
                </Stack>
            </Box>
        </Box>
    );
}

// ResetPassword.getLayout = getLayout("private");
