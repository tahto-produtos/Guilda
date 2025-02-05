import { ReactNode, useContext, useEffect, useState } from "react";
import { Box, Button, Stack, Typography, useTheme } from "@mui/material";
import AdminPanelSettings from "@mui/icons-material/AdminPanelSettings";
import Cookies from "js-cookie";
import { useRouter } from "next/router";
import { toast } from "react-toastify";
import { AdmViewContext } from "./adm-view-context";
import { LogoutUseCase } from "src/modules/auth/use-cases";
import { guildaApiClient, guildaApiClient2 } from "src/services";
import { MeUseCase } from "src/modules/collaborators/use-cases/me.use-case";
import { UserInfoContext } from "../user-context/user.context";

interface IProviderProps {
  children: ReactNode;
}

export function AdmViewProvider({ children }: IProviderProps) {
  const [activeUser, setActiveUser] = useState<{
    id: number;
    name: string;
    registry: string;
    token: string;
  } | null>(null);

  const [currentUserToken, setCurrentUserToken] = useState<string | null>(null);
  const { setMyUser } = useContext(UserInfoContext);
  const theme = useTheme();
  const isAdmViewActive = Boolean(activeUser);
  const router = useRouter();
  const [hover, setHover] = useState(false);

  useEffect(() => {
    if (activeUser) {
      if (!currentUserToken) {
        const currentToken = Cookies.get("jwtToken");
        setCurrentUserToken(currentToken || null);
      }
      Cookies.set("jwtToken", activeUser.token);
      guildaApiClient.defaults.headers.common.Authorization = `Bearer ${activeUser.token}`;
      guildaApiClient2.defaults.headers.common.Authorization = `Bearer ${activeUser.token}`;

      new MeUseCase()
        .handle()
        .then((data) => {
          setMyUser(data);
        })
        .catch(() => {
          toast.error("Erro ao carregar suas permissões");
        });
    }
  }, [activeUser]);

  useEffect(() => {
    checkAdmView();
  }, [isAdmViewActive]);

  async function checkAdmView() {
    const isActive = Cookies.get("admViewActive") == "true";

    if (!isAdmViewActive && isActive) {
      await new LogoutUseCase()
        .handle()
        .then(() => {})
        .finally(() => {
          return router.push("/login");
        });
    }

    Cookies.set("admViewActive", isAdmViewActive.toString());
  }

  function stopAdmView() {
    if (!currentUserToken) {
      return;
    }

    Cookies.set("jwtToken", currentUserToken);
    Cookies.set("admViewActive", "false");
    guildaApiClient.defaults.headers.common.Authorization = `Bearer ${currentUserToken}`;
    guildaApiClient2.defaults.headers.common.Authorization = `Bearer ${currentUserToken}`;
    setActiveUser(null);
    router.reload();
  }

  return (
    <AdmViewContext.Provider value={{ isAdmViewActive, setActiveUser }}>
      <Box sx={{ position: "relative" }}>
        <Box
          onMouseEnter={() => setHover(true)}
          onMouseLeave={() => setHover(false)}
          sx={{
            position: "fixed",
            top: 0,
            left: 0,
            right: 0,
            height: "20px",
            zIndex: 999,
          }}
        />
        {isAdmViewActive && (
          <Box
            onMouseEnter={() => setHover(true)}
            onMouseLeave={() => setHover(false)}
            sx={{
              position: "fixed",
              top: 0,
              left: 0,
              right: 0,
              transform: hover ? "translateY(0)" : "translateY(-100%)",
              transition: "transform 0.3s ease-in-out",
              zIndex: 1000,
              bgcolor: "#fff",
              borderBottom: `solid 4px ${theme.palette.secondary.main}`,
            }}
          >
            <Stack
              width={"100%"}
              height={"60px"}
              alignItems={"center"}
              direction={"row"}
              justifyContent={"space-between"}
              px={"50px"}
            >
              <Box display={"flex"} alignItems={"center"} gap={"10px"}>
                <AdminPanelSettings color="success" />
                <Box display={"flex"} flexDirection={"column"} gap={"5px"}>
                  <Typography
                    variant="body2"
                    lineHeight={"10px"}
                    sx={{
                      color: "#000",
                      fontWeight: "900",
                      fontSize: "12px",
                    }}
                  >
                    Visão de administrador ativada no operador:
                  </Typography>
                  <Typography
                    variant="body2"
                    lineHeight={"10px"}
                    sx={{ color: "#000", fontSize: "12px" }}
                  >
                    {activeUser?.name} - {activeUser?.registry}
                  </Typography>
                </Box>
              </Box>
              <Button
                variant="contained"
                color="error"
                size="small"
                onClick={stopAdmView}
              >
                Parar espelhamento
              </Button>
            </Stack>
            {/* Add arrow indicator below the header */}
            <Box
              sx={{
                position: "absolute",
                top: "100%",
                width: "100%",
                borderBottom: "10px solid #663a82", // Adjust the color and thickness as needed
                marginBottom: "10px", // Adjust spacing between line and arrow as needed
              }}
            />
            <Box
              sx={{
                position: "absolute",
                top: "100%",
                left: "50%",
                transform: "translateX(-50%)",
                width: 0,
                height: 0,
                borderTop: "30px solid #663a82",
                borderBottom: "30px solid transparent",
                borderLeft: "30px solid transparent",
                borderRight: "30px solid transparent",
              }}
            />
          </Box>
        )}
        {children}
      </Box>
    </AdmViewContext.Provider>
  );
}
