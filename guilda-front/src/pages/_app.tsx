import { ReactElement, ReactNode, useEffect, useState } from "react";
import { NextPage } from "next";
import type { AppProps } from "next/app";
import { Box, CssBaseline, ThemeProvider, CircularProgress } from "@mui/material";
import Head from "next/head";
import { ToastContainer } from "react-toastify";

import { muiTheme } from "../theme";
import "../theme/custom-fonts.css";
import "react-toastify/dist/ReactToastify.css";
import Cookies from "js-cookie";
import LoadingApp from "src/utils/loading-app";
import { ShoppingCartProvider } from "src/contexts/shopping-cart-context/shopping-cart-provider";
import { AdmViewProvider } from "src/contexts/adm-view-context/adm-view-provider";
import { UserInfoProvider } from "src/contexts/user-context/user.provider";
import { PermissionsProvider } from "src/contexts/permissions-provider/permissions.provider";
import { UserPersonaProvider } from "src/contexts/user-persona/user-persona.provider";
import { NotificationsProvider } from "src/contexts/notifications-provider/notifications.provider";
import "../components/feedback/toast-custom/custom-toast-style.css";
import { QuizProvider } from "src/contexts/quiz-provider/quiz.provider";
import { ClimateProvider } from "src/contexts/climate-provider/climate.provider";

export type NextPageWithLayout<P = {}, IP = P> = NextPage<P, IP> & {
  getLayout?: (page: ReactElement) => ReactNode;
};

type AppPropsWithLayout = AppProps & {
  Component: NextPageWithLayout;
};

// Função de sanitização
const sanitizeInput = (input: any) => {
  if (typeof input === 'object' && input !== null) {
    if ('_proto_' in input) {
      delete (input as any)._proto_;
    }
    for (let key in input) {
      if (typeof input[key] === 'object') {
        sanitizeInput(input[key]);
      }
    }
  }
  return input;
};

export default function App({ Component, pageProps }: AppPropsWithLayout) {
  const sanitizedProps = sanitizeInput(pageProps);
  const [loading, setLoading] = useState(true);
  const [personaInitialized, setPersonaInitialized] = useState(false); // Estado para controlar a inicialização do UserPersona
  const [climateInitialized, setClimateInitialized] = useState(false); // Estado para controlar a inicialização do Climate

  const getLayout = Component.getLayout ?? ((page) => page);

  // Inicializar o UserPersona
  useEffect(() => {
    const initializeUserPersona = async () => {
      try {
        // Simulação de uma função de inicialização do UserPersona
        await new Promise((resolve) => setTimeout(resolve, 2000)); // Simulando tempo de inicialização
        setPersonaInitialized(true); // Marca como inicializado
      } catch (error) {
        console.error('Erro na inicialização do UserPersonaProvider', error);
      }
    };

    initializeUserPersona();
  }, []);

  // Inicializar o Climate somente após o UserPersona
  useEffect(() => {
    if (personaInitialized) {
      const initializeClimate = async () => {
        try {
          // Simulação de uma função de inicialização do Climate
          await new Promise((resolve) => setTimeout(resolve, 2000)); // Simulando tempo de inicialização
          setClimateInitialized(true); // Marca como inicializado
        } catch (error) {
          console.error('Erro na inicialização do ClimateProvider', error);
        }
      };

      initializeClimate();
    }
  }, [personaInitialized]);

  // Finaliza o loading somente quando tudo estiver inicializado
  useEffect(() => {
    if (personaInitialized && climateInitialized) {
      setLoading(false); // Tudo foi inicializado, pode parar o loading
    }
  }, [personaInitialized, climateInitialized]);

  return (
    <Box>
      <Head>
        <title>Guilda</title>
        <meta name="viewport" content="width=device-width, initial-scale=1" />
      </Head>
      <UserInfoProvider>
        <UserPersonaProvider>
          <PermissionsProvider>
            <AdmViewProvider>
              <ShoppingCartProvider>
                <NotificationsProvider>
                  <QuizProvider>
                    {personaInitialized ? ( // Renderiza Climate somente após o UserPersona
                      <ClimateProvider>
                        <ThemeProvider theme={muiTheme}>
                          <CssBaseline>
                            {loading ? (
                              <Box
                                display="flex"
                                justifyContent="center"
                                alignItems="center"
                                height="100vh"
                              >
                                <CircularProgress />
                              </Box>
                            ) : (
                              getLayout(<Component {...sanitizedProps} />)
                            )}
                          </CssBaseline>
                          <ToastContainer />
                        </ThemeProvider>
                      </ClimateProvider>
                    ) : (
                      <Box
                        display="flex"
                        justifyContent="center"
                        alignItems="center"
                        height="100vh"
                      >
                        <CircularProgress />
                      </Box>
                    )}
                  </QuizProvider>
                </NotificationsProvider>
              </ShoppingCartProvider>
            </AdmViewProvider>
          </PermissionsProvider>
        </UserPersonaProvider>
      </UserInfoProvider>
    </Box>
  );
}