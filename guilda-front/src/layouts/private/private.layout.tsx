import { ReactNode } from "react";
import { Box, Container, Stack } from "@mui/material";
import { UserWebsocketProvider } from "src/contexts/user-websocket-context/user-websocket-provider";
import { Sidebar } from "src/components/navigation/sidebar/sidebar";
import { Header } from "src/components/data-display/header/header";

interface PrivateLayoutProps {
  children: ReactNode;
}

export function PrivateLayout({ children }: PrivateLayoutProps) {
  return (
    <UserWebsocketProvider>
      <Box
        minHeight={"100vh"}
        sx={{
          background: "linear-gradient(117.06deg, #692FAE 0%, #379D92 99.78%)",
          backgroundAttachment: "fixed",
        }}
      >
        <Box
          width={"100%"}
          bgcolor={"#fff"}
          height={"106px"}
          display={"flex"}
          alignItems={"center"}
          mb={"24px"}
          sx={{
            position: "fixed",
            top: 0,
            zIndex: 9,
          }}
          borderBottom={"solid 1px #e1e1e1"}
        >
          <Container maxWidth={"xl"}>
            <Header />
          </Container>
        </Box>
        <Stack
          position={"fixed"}
          width={"100%"}
          height={"38px"}
          top={"106px"}
          sx={{
            background:
              "linear-gradient(117.06deg, #692FAE 0%, #379D92 99.78%)",
          }}
          zIndex={9}
        ></Stack>
        <Container
          maxWidth={"xl"}
          sx={{
            flexDirection: "row",
            display: "flex",
            width: "100%",
            gap: "33px",
            paddingBottom: "24px",
            minHeight: "100vh",
          }}
        >
          <Sidebar />
          <Stack
            display={"flex"}
            width={"100%"}
            position={"relative"}
            mt={"144px"}
          >
            {children}
          </Stack>
        </Container>
      </Box>
    </UserWebsocketProvider>
  );
}
