import {
  FeedbackOutlined,
  HomeOutlined,
  Rocket,
  Stars,
  WorkspacePremium,
} from "@mui/icons-material";
import {
  Breadcrumbs,
  Button,
  CardMedia,
  Divider,
  LinearProgress,
  Link,
  Stack,
  Typography,
  useTheme,
} from "@mui/material";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { format } from "date-fns";
import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { useDebounce, useLoadingState } from "src/hooks";
import { AvailableCampaigns } from "src/modules/campaign/fragments/available-campaigns";
import { MyCampaignCard } from "src/modules/campaign/fragments/my-campaign-card";
import {
  DetailsInformationOperationalCampaign,
  DetailsInformationOperationalCampaignUseCase,
  DetailsOperationalCampaignUseCase,
} from "src/modules/campaign/use-cases/DetailsOperationalCampaign.use-case";
import { PayCampaignUseCase } from "src/modules/campaign/use-cases/pay-campaing.use-case";
import {
  OperationalCampaign,
  OperationalCampaignDetails,
} from "src/typings/models/operational-campaign.model";
import { getLayout } from "src/utils";
import { uuid } from "uuidv4";

export default function CampaignDetailsView() {
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [searchText, setSearchText] = useState<string>("");
  const debouncedSearchText: string = useDebounce<string>(searchText, 400);
  const router = useRouter();
  const compaignId = parseInt(router.query.id as string);
  const [myOperationalCampaign, setMyOperationalCampaign] = useState<
    OperationalCampaign[]
  >([]);
  const [data, setData] =
    useState<DetailsInformationOperationalCampaign | null>(null);

  const getMyCampaigns = async () => {
    await new DetailsInformationOperationalCampaignUseCase()
      .handle({
        IDGDA_OPERATIONAL_CAMPAIGN: compaignId,
        ISIMPORTANT: false,
      })
      .then((data) => {
        setData(data);
      })
      .catch(() => {})
      .finally(() => {});
  };

  useEffect(() => {
    getMyCampaigns();
  }, []);

  const handleClickPayCampaing = async () => {
    await new PayCampaignUseCase()
      .handle({
        idCampaign: compaignId,
      })
      .then((data) => {
        console.log('[LOG]', data);
        toast.success("Pagamento com sucesso!");
        getMyCampaigns();
      })
      .catch(() => {})
      .finally(() => {});
  };

  const theme = useTheme();

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
          <Stack direction={"row"}>
            <PageTitle
              icon={<Stars sx={{ fontSize: "40px" }} />}
              title={`Campanha: ${data?.name}`}
              loading={isLoading}
            ></PageTitle>
            <CardMedia
              component="img"
              image={data?.image}
              sx={{
                width: "449px",
                objectFit: "cover",
                height: "110px",
                borderRadius: "24px",
                backgroundColor: "#f1f1f1",
              }}
            />
          </Stack>
          <Divider />
          <Stack direction={"column"} gap={"40px"} mt={"40px"}>
            <Stack
              direction={"row"}
              width={"100%"}
              justifyContent={"space-between"}
            >
              <Stack gap={"26px"} paddingRight={"70px"}>
                <LinearProgress
                  value={data?.position}
                  variant="determinate"
                  sx={{
                    minHeight: "16px",
                    backgroundColor: "#E5E5E5",
                  }}
                />
                <Stack direction={"row"} gap={"10px"} alignItems={"center"}>
                  <Typography>Pontos</Typography>
                  <Typography fontSize={"20px"} fontWeight={"600"}>
                    {data?.pontuation || "0"}
                  </Typography>
                </Stack>
                <Stack direction={"row"} gap={"10px"} alignItems={"center"}>
                  <Typography>Posição no ranking</Typography>
                  <Typography fontSize={"20px"} fontWeight={"600"}>
                    {data?.position}
                  </Typography>
                </Stack>
              </Stack>
              <Stack gap={"0px"}>
                <Typography fontSize={"13px"} fontWeight={"400"}>
                  Data de inicio
                </Typography>
                <Typography fontSize={"16px"} fontWeight={"600"}>
                  {data?.dtInicio &&
                    format(new Date(data.dtInicio), "dd/MM/yyyy")}
                </Typography>
                <Typography fontSize={"13px"} mt={"10px"} fontWeight={"400"}>
                  Data de finalização
                </Typography>
                <Typography fontSize={"16px"} fontWeight={"600"}>
                  {data?.dtFim && format(new Date(data.dtFim), "dd/MM/yyyy")}
                </Typography>
              </Stack>
            </Stack>
            <Stack gap={"10px"}>
              <PageTitle
                icon={<Rocket sx={{ fontSize: "40px" }} />}
                title={`Missões da campanha`}
                loading={isLoading}
              />
              <Stack direction={"row"} gap={"20px"} flexWrap={"wrap"}>
                {data?.missions.map((item, index) => (
                  <Stack
                    direction={"column"}
                    gap={"10px"}
                    p={"20px"}
                    borderRadius={"24px"}
                    border={`solid 1px #e1e1e1`}
                    key={index}
                  >
                    <Typography
                      fontSize={"16px"}
                      fontWeight={"600"}
                      sx={{ color: "#7D7D7D" }}
                    >
                      {item.mission_type}
                    </Typography>
                    <Stack>
                      <Typography
                        fontSize={"24px"}
                        fontWeight={"600"}
                        sx={{ color: theme.palette.secondary.main }}
                      >
                        {item.mission_indicator}
                      </Typography>
                      <Typography
                        fontSize={"16px"}
                        fontWeight={"500"}
                        sx={{ color: theme.palette.primary.main }}
                      >
                        {item.mission_text}
                      </Typography>
                    </Stack>
                  </Stack>
                ))}
              </Stack>
            </Stack>
            <Stack gap={"10px"}>
              <PageTitle
                icon={<WorkspacePremium sx={{ fontSize: "40px" }} />}
                title={`Ranking de pontuação`}
                loading={isLoading}
              />
              <Stack direction={"row"} gap={"20px"} flexWrap={"wrap"}>
                {data && (
                  <DataGrid
                    columns={columns}
                    rows={data.rankings}
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
                    getRowId={(row) => {
                      const uuidv4 = uuid();
                      return uuidv4;
                    }}
                    sx={{ width: "100%" }}
                  />
                )}
              </Stack>
            </Stack>
            {data?.showButtonPay == 1 && (
              <Stack gap={"10px"}>
              <Button
                variant="contained"
                color="primary"
                onClick={() => handleClickPayCampaing()}
              >
                Pagamento manual
              </Button>
            </Stack>
            )}
            
          </Stack>
        </Stack>
      </ContentArea>
    </ContentCard>
  );
}

CampaignDetailsView.getLayout = getLayout("private");

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
    field: "position",
    headerName: "Posição",
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
    field: "name",
    headerName: "Nome do usuário",
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
    field: "status",
    headerName: "Status",
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
    field: "pontuation",
    headerName: "Pontos totais obtidos",
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
    field: "award",
    headerName: "Prêmios",
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
];
