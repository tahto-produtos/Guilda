import { LoginForm, LoginUseCase } from "../modules";
import {
    Box,
    Button,
    CardMedia,
    Divider,
    Stack,
    Typography,
    useTheme,
} from "@mui/material";
import Person from "@mui/icons-material/Person";
import { LoginFormData } from "../modules/auth/forms/login/login.schema";
import { useRouter } from "next/router";
import { useLoadingState } from "../hooks";
import { LoadingButton } from "@mui/lab";
import Logo from "../assets/logo/logo.png";
import Image from "next/image";
import Cookies from "js-cookie";
import { jwtTokenKey } from "src/constants";
import logo from "../assets/logo/logo-branca.png";
import { useUserPersona } from "src/contexts/user-persona/user-persona.provider";


export default function Login() {
    const { LoggedUserAccounts } = useUserPersona();
    const theme = useTheme();
    const router = useRouter();
    const { isLoading, startLoading, finishLoading } = useLoadingState();

    const handleLogin = async ({ username, password }: LoginFormData) => {
        startLoading();

        try {
            // Limpar LocalStorage
            localStorage.clear();
    
            // Limpar SessionStorage
            sessionStorage.clear();
    
            // Limpar Cookies usando js-cookie
            Object.keys(Cookies.get()).forEach(function (cookieName) {
                Cookies.remove(cookieName);
            });
    
            // Opcional: Limpar IndexedDB
            const databases = await indexedDB.databases();
            databases.forEach((db) => {
                if (db.name) {
                    indexedDB.deleteDatabase(db.name);
                }
            });
    
            Cookies.remove(jwtTokenKey);
    
            // Realizar login e obter dados
            const data = await new LoginUseCase().handle({ Username: username, Password: password });
    
            console.log("[LOG]", data);
            const userToken = Cookies.get("jwtToken");
    
            if (data?.token) {
                const firstLogin = data?.fisrtLogin;
    
                // Chame LoggedUserAccounts aqui após o login bem-sucedido
                await LoggedUserAccounts();
    
                if (firstLogin) {
                    router.push("/reset-password");
                } else {
                    router.push("/home");
                }
            }
        } catch (error) {
            console.error("Erro durante o login:", error);
        } finally {
            finishLoading();
        }
    }
    /* 
        const handleLogin = async ({ username, password }: LoginFormData) => {
        startLoading();
    
        Cookies.remove(jwtTokenKey);
    
        try {
            const data = await new LoginUseCase().handle({ Username: username, Password: password });
    
            console.log("[LOG] Data returned from LoginUseCase:", data);
            const userToken = Cookies.get(jwtTokenKey);
            console.log("[LOG] JWT Token from Cookies:", userToken);
    
            if (data?.token) {
                const firstLogin = data?.fisrtLogin;
    
                if (firstLogin) {
                    await router.push("/reset-password");
                } else {
                    await router.push("/home");
                }
            } else {
                console.error("[ERROR] No token received in response");
            }
        } catch (error) {
            console.error("[ERROR] Error during login:", error);
        } finally {
            finishLoading();
        }
    }; */

    return (
        <Box
            display={"flex"}
            justifyContent={"center"}
            flexDirection={"column"}
            alignItems={"center"}
            gap={"50px"}
            minHeight={"100vh"}
            sx={{
                background:
                    "linear-gradient(90deg, rgba(91, 68, 146, 1) 0%, rgba(48, 166, 152, 1) 100%)",
            }}
        >
            <CardMedia
                component="img"
                image={logo.src}
                sx={{
                    width: "153px",
                    objectFit: "contain",
                }}
            />
            <Box
                sx={{
                    width: "520px",
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
                    py={"60px"}
                    px={"80px"}
                    borderRadius={"16px"}
                    bgcolor={"white"}
                    display={"flex"}
                    alignItems={"center"}
                    width={"100%"}
                >
                    <Stack width={"100%"} gap={"16px"} mb={"30px"}>
                        <Typography
                            color={"primary"}
                            variant="h1"
                            fontSize={"24px"}
                            fontWeight={"500"}
                        >
                            Bem vindo ao GUILDA
                        </Typography>
                        <Typography
                            color={"text.secondary"}
                            variant="h1"
                            fontSize={"15px"}
                            fontWeight={"300"}
                        >
                            Para acessar a plataforma faca o login com sua
                            matricula e senha, caso tenha duvidas entre em
                            contato com o suporte.
                        </Typography>
                        <Divider
                            sx={{
                                bgcolor: theme.palette.primary.main,
                                height: "2px",
                            }}
                        />
                    </Stack>
                    <LoginForm id={"login-form"} onSubmit={handleLogin} />
                    <Box
                        width={"100%"}
                        display={"flex"}
                        justifyContent={"center"}
                        alignItems={"center"}
                        flexDirection={"column"}
                        mt={"30px"}
                    >
                        {/* <Typography variant={"caption"} fontSize={"0.8rem"}>
                            Don't have an account?
                            <Button
                                variant={"text"}
                                size={"small"}
                                sx={{ fontSize: ".75rem", mb: "0.2rem" }}
                            >
                                Register
                            </Button>
                        </Typography> */}
                        <LoadingButton
                            loading={isLoading}
                            type={"submit"}
                            form={"login-form"}
                            variant={"contained"}
                            size="large"
                            sx={{
                                height: "60px",
                                fontSize: "18px",
                                minWidth: "150px",
                            }}
                        >
                            Login
                        </LoadingButton>
                    </Box>
                </Stack>
            </Box>
        </Box>
    );
}
