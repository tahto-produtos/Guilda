import { FeedbackOutlined, HomeOutlined, Stars } from "@mui/icons-material";
import {
  Breadcrumbs,
  Button,
  Divider,
  LinearProgress,
  Link,
  Stack,
  Typography,
  useTheme,
} from "@mui/material";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { useDebounce, useLoadingState } from "src/hooks";
import { AvailableCampaigns } from "src/modules/campaign/fragments/available-campaigns";
import { MyCampaignCard } from "src/modules/campaign/fragments/my-campaign-card";
import { LoadMyOperationalCampaignUseCase } from "src/modules/campaign/use-cases/LoadMyOperationalCampaign.use-case";

import { AlertImportantUseCase } from "src/modules/campaign/use-cases/do-important.use-case";
import { MyFeedbackTable } from "src/modules/feedback/fragments/my-feedback-table";
import { OperationalCampaign } from "src/typings/models/operational-campaign.model";
import { getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";
import { uuid } from "uuidv4";
import { toast } from "react-toastify";

export default function CampaignsView() {
  const { myPermissions } = useContext(PermissionsContext);
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [searchText, setSearchText] = useState<string>("");
  const debouncedSearchText: string = useDebounce<string>(searchText, 400);
  const router = useRouter();
  const [myOperationalCampaign, setMyOperationalCampaign] = useState<
    OperationalCampaign[]
  >([]);

  const theme = useTheme();

  const [refresh, setRefresh] = useState(false);

  const forceRefresh = () => {
    setRefresh(prev => !prev); // Alterna o valor de 'refresh' para forçar a atualização
  };


  async function getMyCampaigns() {
    startLoading();
    await new LoadMyOperationalCampaignUseCase()
      .handle({
        STARTEDATFROM: "",
        STARTEDATTO: "",
        ENDEDATFROM: "",
        ENDEDATTO: "",
        NAME: "",
        limit: 5,
        page: 1,
      })
      .then((data) => {
        setMyOperationalCampaign(data.MyOperationalCampaign);
      })
      .catch(() => {})
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    getMyCampaigns();
  }, []);

  

 async function alterImportant(id: number) {
  if (!myOperationalCampaign) return;

  startLoading();

  await new AlertImportantUseCase()
    .handle({
      idCampaign: id,
    })
    .then(() => {
      toast.success("Alterado com sucesso!");
      getMyCampaigns();
      forceRefresh();
    })
    .catch(() => {})
    .finally(() => {
      finishLoading();
    });
} 

  const columns: GridColDef[] = [
    /* {
      field: "idUserClimate",
      headerName: "Id",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueFormatter({ value }) {
        return value.toString();
      },
      minWidth: 200,
    }, */
  
    { 
      field: "NAME",
      headerName: "Minhas campanhas",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      renderCell: (params) => {
        return (
          <Typography variant="body1" fontSize={"13px"}>
            {params.value}
          </Typography>
        );
      },
      minWidth: 200,
    },
    {
      field: "IDGDA_OPERATIONAL_CAMPAIGN",
      headerName: "Principal",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      renderCell: (params) => {
        return (
          <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
            <Button
              variant="text"
              onClick={() => alterImportant(params.value)}
              color={"error"}
              sx={{ color: theme.palette.error.main, fontWeight: 700 }}
            >
              Importante
            </Button>
          </Stack>
        );
      },
    },
    {
      field: "IDGDA_OPERATIONAL_CAMPAIGN_DETAIL",
      headerName: "Detalhe",
      flex: 2,
      disableColumnMenu: true,
      filterable: false,
      sortable: false,
      valueGetter: (params) => params.row.IDGDA_OPERATIONAL_CAMPAIGN,
      renderCell: (params) => {
        return (
          <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
            <Button
              variant="text"
              onClick={() => {
                router.push(
                  `/campaigns/campaign-details?id=${params.value}`
                );
              }}
              color={"error"}
              sx={{ color: theme.palette.info.main, fontWeight: 700 }}
            >
              Detalhe
            </Button>
          </Stack>
        );
      },
    },
  ];

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
            <Typography fontWeight={"700"}>Campanhas</Typography>
          </Link>
        </Breadcrumbs>
      </Stack>
      <ContentArea sx={{ py: " 40px" }}>
        <Stack px={"40px"}>
          <PageTitle
            icon={<Stars sx={{ fontSize: "40px" }} />}
            title="Campanhas"
            loading={isLoading}
          >
            <Stack gap={"10px"} direction={"row"}>
              {abilityFor(myPermissions).can("Gerenciar Campanha", "Campanha Operacional") && (
                <>
                    <Button
                      variant="outlined"
                      color="primary"
                      onClick={() => router.push("/campaigns/simulator")}
                    >
                      Simulador
                    </Button>
                    <Button
                    variant="contained"
                    color="primary"
                    onClick={() => router.push("/campaigns/create-campaign")}
                  >
                    Criar campanha
                  </Button>
                </>
              )}
            </Stack>
          </PageTitle>
          <Divider />
          <Stack direction={"column"} gap={"40px"} mt={"40px"}>
            {myOperationalCampaign[0] && (
              <MyCampaignCard
                campaignId={myOperationalCampaign[0].IDGDA_OPERATIONAL_CAMPAIGN}
                isImportant={true}
                refresh={refresh}
                forceRefresh={forceRefresh}
              />
            )}
            <DataGrid
              columns={columns}
              rows={myOperationalCampaign}
              hideFooter
              disableColumnFilter
              disableRowSelectionOnClick
              autoHeight
              // rowCount={20}
              localeText={{
                noResultsOverlayLabel: "Nenhum resultado encontrado",
              }}
              paginationMode="server"
              loading={isLoading}
              slots={{
                loadingOverlay: LinearProgress,
              }}
/*               onRowClick={(params) => {
                router.push(
                  `/campaigns/campaign-details?id=${params.row?.IDGDA_OPERATIONAL_CAMPAIGN}`
                );
              }} */
              getRowId={(row) => {
/*                 const uuidv4 = uuid();
                return uuidv4; */
                return row.IDGDA_OPERATIONAL_CAMPAIGN; // Use a unique field to identify rows
              }}
              sx={{ width: "100%" }}
            />
            <AvailableCampaigns />
          </Stack>
        </Stack>
      </ContentArea>
    </ContentCard>
  );
}

CampaignsView.getLayout = getLayout("private");





