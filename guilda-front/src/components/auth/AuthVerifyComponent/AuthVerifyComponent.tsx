import Check from "@mui/icons-material/Check";
import {
    Box,
    Button,
    CircularProgress,
    TextField,
    Typography,
} from "@mui/material";
import { isAxiosError } from "axios";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { AuthVerify } from "src/modules/auth/use-cases/auth-verify/auth-verify.use-case";
import { EXCEPTION_CODES } from "src/typings";

interface IProps {
    isOpen: boolean;
    onClose: () => void;
    onVerified: () => void;
}

export function AuthVerifyComponent(props: IProps) {
    const { isOpen, onClose, onVerified } = props;
    const { myUser } = useContext(UserInfoContext);

    const [isAuthVerified, setIsAuthVerified] = useState<boolean>(false);
    const [authVerifyUser, setAuthVerifyUser] = useState<string>("");
    const [authVerifyPassword, setAuthVerifyPassword] = useState<string>("");
    const [authVerifyIsLoading, setAuthVerifyIsLoading] =
        useState<boolean>(false);

    useEffect(() => {
        setIsAuthVerified(false);
        setAuthVerifyPassword("");
    }, [isOpen]);

    function handleAuthVerify() {
        if (!myUser) {
            return;
        }

        setAuthVerifyIsLoading(true);

        const payload = {
            username: authVerifyUser,
            password: authVerifyPassword,
            currentUserId: myUser.id,
        };

        new AuthVerify()
            .handle(payload)
            .then((data) => {
                if (data == true) {
                    setIsAuthVerified(true);
                    onVerified();
                } else {
                    toast.warning("Usuário não autenticado");
                }
            })
            .catch((e) => {
                if (isAxiosError(e)) {
                    const errorCode = e?.response?.data?.code;
                    let message = INTERNAL_SERVER_ERROR_MESSAGE;

                    if (
                        errorCode === EXCEPTION_CODES.PASSWORD_NOT_MATCH ||
                        errorCode === EXCEPTION_CODES.NOT_FOUND
                    ) {
                        message = "Usuário e senha não combinam";
                    }

                    toast.error(message);
                }
            })
            .finally(() => {
                setAuthVerifyIsLoading(false);
            });
    }

    return (
        <BaseModal
            width={"540px"}
            open={isOpen}
            title={`Confirme seu login para continuar`}
            onClose={onClose}
        >
            <Box
                width={"100%"}
                display={"flex"}
                flexDirection={"column"}
                gap={1}
            >
                {authVerifyIsLoading && (
                    <Box
                        display={"flex"}
                        alignItems={"center"}
                        justifyContent={"center"}
                        gap={1}
                        py={4}
                    >
                        <CircularProgress />
                        <Typography>Aguarde...</Typography>
                    </Box>
                )}
                {!isAuthVerified && !authVerifyIsLoading && (
                    <Box
                        width={"100%"}
                        display={"flex"}
                        flexDirection={"column"}
                        gap={1}
                    >
                        <TextField
                            label="Nome de Usuário"
                            fullWidth
                            value={authVerifyUser}
                            onChange={(e) => setAuthVerifyUser(e.target.value)}
                        />
                        <TextField
                            label="Senha"
                            fullWidth
                            value={authVerifyPassword}
                            onChange={(e) =>
                                setAuthVerifyPassword(e.target.value)
                            }
                            type="password"
                        />
                        <Button
                            variant="contained"
                            onClick={handleAuthVerify}
                            disabled={authVerifyIsLoading}
                        >
                            Autenticar
                        </Button>
                    </Box>
                )}
                {isAuthVerified && (
                    <Box
                        display={"flex"}
                        alignItems={"center"}
                        justifyContent={"center"}
                        gap={1}
                        py={4}
                    >
                        <Check color="success" />
                        <Typography>Verificado!</Typography>
                    </Box>
                )}
            </Box>
        </BaseModal>
    );
}
