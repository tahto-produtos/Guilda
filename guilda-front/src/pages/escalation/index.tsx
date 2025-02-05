import {
  HomeOutlined,
  ListAltOutlined,
  PageviewOutlined,
} from "@mui/icons-material";
import {
  Breadcrumbs,
  Button,
  Link,
  Stack,
  Typography,
  lighten,
  useTheme,
} from "@mui/material";
import { useRouter } from "next/router";
import { useContext, useState } from "react";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { useLoadingState } from "src/hooks";
import { ActionPlanTable } from "src/modules/escalation/fragments/action-plan-table";
import { PerformanceIndicator } from "src/modules/escalation/fragments/performance-indicator";
import { getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";

export default function EscalationView() {
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [selectedTab, setSelectedTab] = useState<"plans" | "performance">("plans");
  const { myPermissions } = useContext(PermissionsContext);
  const router = useRouter();
  const theme = useTheme();

  const renderTabContent = () => {
    if (selectedTab === "plans") {
      return <ActionPlanTable />;
    }
    if (selectedTab === "performance") {
      return <PerformanceIndicator />;
    }
    return null;
  };

  const handleCreateAutoAction = () => {
    router.push("/escalation/create-auto-action");
  };

  const handleCreateActionPlan = () => {
    router.push("/escalation/create-action");
  };

  return (
    <ContentCard sx={{ p: 0 }}>
      <Stack
        width={"100%"}
        height={"80px"}
        sx={{
          borderTopLeftRadius: "16px",
          borderTopRightRadius: "16px",
        }}
        bgcolor={theme.palette.secondary.main}
        pl={"80px"}
        justifyContent={"center"}
      >
        <Breadcrumbs
          aria-label="breadcrumb"
          sx={{
            color: theme.palette.background.default,
          }}
        >
          <Link
            underline="hover"
            sx={{ display: "flex", alignItems: "center" }}
            color={theme.palette.background.default}
            href="/"
          >
            <HomeOutlined
              sx={{
                mr: 0.5,
                color: theme.palette.background.default,
              }}
            />
          </Link>
          <Link
            sx={{
              display: "flex",
              alignItems: "center",
              textDecoration: "none",
            }}
            color={theme.palette.background.default}
          >
            <Typography fontWeight={"700"}>Escalation</Typography>
          </Link>
        </Breadcrumbs>
      </Stack>
      <ContentArea sx={{ py: "40px" }}>
        <Stack px={"40px"}>
          <PageTitle
            icon={<ListAltOutlined sx={{ fontSize: "40px" }} />}
            title="Escalation"
            loading={isLoading}
          >
            <Stack direction={"row"} gap={"16px"}>
              <Button
                variant="contained"
                onClick={handleCreateAutoAction}
                disabled={abilityFor(myPermissions).cannot(
                  "Criar Ação automatica",
                  "Escalation"
                )}
              >
                Criar ação automática
              </Button>
              <Button
                variant="contained"
                onClick={handleCreateActionPlan}
                disabled={abilityFor(myPermissions).cannot(
                  "Criar Plano de ação",
                  "Escalation"
                )}
              >
                Criar plano de ação
              </Button>
            </Stack>
          </PageTitle>

          <Stack direction={"row"} alignItems={"center"} gap={"20px"} mt={"20px"}>
            <Button
              variant={selectedTab === "plans" ? "contained" : "text"}
              sx={{
                borderRadius: "16px",
                bgcolor:
                  selectedTab === "plans"
                    ? lighten(theme.palette.secondary.main, 0.7)
                    : undefined,
                color: theme.palette.text.primary,
              }}
              startIcon={<ListAltOutlined />}
              onClick={() => setSelectedTab("plans")}
            >
              Planos de ação
            </Button>
            <Button
              variant={selectedTab === "performance" ? "contained" : "text"}
              sx={{
                borderRadius: "16px",
                bgcolor:
                  selectedTab === "performance"
                    ? lighten(theme.palette.secondary.main, 0.7)
                    : undefined,
                color: theme.palette.text.primary,
              }}
              startIcon={<PageviewOutlined />}
              onClick={() => setSelectedTab("performance")}
            >
              Análise de desempenho
            </Button>
          </Stack>

          <Stack mt={"20px"}>
            {renderTabContent()}
          </Stack>
        </Stack>
      </ContentArea>
    </ContentCard>
  );
}

EscalationView.getLayout = getLayout("private");