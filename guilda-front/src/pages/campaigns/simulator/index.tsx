import {
  FeedbackOutlined,
  HomeOutlined,
  Rocket,
  Stars,
  WorkspacePremium,
} from "@mui/icons-material";
import {
  Autocomplete,
  Breadcrumbs,
  Button,
  CardMedia,
  Divider,
  LinearProgress,
  Link,
  Stack,
  TextField,
  Typography,
  useTheme,
} from "@mui/material";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { format } from "date-fns";
import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListSectorsAndSubsectrosUseCase } from "src/modules";
import { AvailableCampaigns } from "src/modules/campaign/fragments/available-campaigns";
import { MyCampaignCard } from "src/modules/campaign/fragments/my-campaign-card";
import {
  DetailsInformationOperationalCampaign,
  DetailsInformationOperationalCampaignUseCase,
  DetailsOperationalCampaignUseCase,
} from "src/modules/campaign/use-cases/DetailsOperationalCampaign.use-case";
import { LoadMyOperationalCampaignUseCase } from "src/modules/campaign/use-cases/LoadMyOperationalCampaign.use-case";
import {
  SimulatorIndice,
  SimulatorUseCase,
} from "src/modules/campaign/use-cases/Simulator.use-case";
import { SectorAndSubsector } from "src/typings";
import {
  OperationalCampaign,
  OperationalCampaignDetails,
} from "src/typings/models/operational-campaign.model";
import { getLayout } from "src/utils";
import { uuid } from "uuidv4";

export default function SimulatorView() {
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [searchText, setSearchText] = useState<string>("");
  const debouncedSearchText: string = useDebounce<string>(searchText, 400);
  const router = useRouter();
  const compaignId = parseInt(router.query.id as string);
  const [myOperationalCampaign, setMyOperationalCampaign] = useState<
    OperationalCampaign[]
  >([]);

  const [sectors, setSectors] = useState<SectorAndSubsector[]>([]);
  const [selectedSectors, setSelectedSectors] =
    useState<SectorAndSubsector | null>(null);
  const [sectorSearch, setSectorSearch] = useState<string>("");
  const theme = useTheme();

  const [cost, setCost] = useState<number | null>(null);
  const [data, setData] = useState<SimulatorIndice[]>([]);

  const getMyCampaigns = async () => {
    if (!selectedSectors) return;

    await new SimulatorUseCase()
      .handle({
        sector: selectedSectors.id,
      })
      .then((data) => {
        setCost(data.costCampaign);
        setData(data.indices);
      })
      .catch(() => {})
      .finally(() => {});
  };

  useEffect(() => {
    getMyCampaigns();
  }, [selectedSectors]);

  const getSectorsAndSubSectors = async (isSubsector = false, sector = "") => {
    startLoading();

    await new ListSectorsAndSubsectrosUseCase()
      .handle({
        isSubsector,
        sector,
      })
      .then((data) => {
        setSectors(data);
      })
      .catch(() => {})
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    getSectorsAndSubSectors(false, sectorSearch);
  }, [sectorSearch]);

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
              title={`Simulador`}
              loading={isLoading}
            ></PageTitle>
          </Stack>
          <Divider />
          <Stack direction={"column"} gap={"40px"} mt={"40px"}>
            <Autocomplete
              fullWidth
              value={selectedSectors}
              options={sectors}
              getOptionLabel={(option) => option.name}
              onChange={(event, value) => {
                setSelectedSectors(value);
              }}
              onInputChange={(e, text) => setSectorSearch(text)}
              filterOptions={(x) => x}
              filterSelectedOptions
              renderInput={(props) => (
                <TextField {...props} label={"Setores"} />
              )}
              renderOption={(props, option) => {
                return (
                  <li {...props} key={option.id}>
                    {option.name}
                  </li>
                );
              }}
              isOptionEqualToValue={(option, value) =>
                option.name === value.name
              }
              sx={{ m: 0 }}
            />
            <TextField value={cost} />
            <DataGrid
              columns={columns}
              rows={data.length > 0 ? data : []}
              hideFooter
              disableColumnFilter
              disableRowSelectionOnClick
              autoHeight
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
            />
          </Stack>
        </Stack>
      </ContentArea>
    </ContentCard>
  );
}

SimulatorView.getLayout = getLayout("private");

const columns: GridColDef[] = [
  {
    field: "indice",
    headerName: "Indice",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
          <Typography fontSize={"16px"} fontWeight={"400"}>
            {params.value}
          </Typography>
        </Stack>
      );
    },
  },
  {
    field: "hc",
    headerName: "HC",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
          <Typography fontSize={"16px"} fontWeight={"400"}>
            {params.value}
          </Typography>
        </Stack>
      );
    },
  },
  {
    field: "coinsMonth",
    headerName: "Moedas mensais",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
          <Typography fontSize={"16px"} fontWeight={"400"}>
            {params.value}
          </Typography>
        </Stack>
      );
    },
  },
  {
    field: "fullPotentialCoins",
    headerName: "fullPotentialCoins",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
          <Typography fontSize={"16px"} fontWeight={"400"}>
            {params.value}
          </Typography>
        </Stack>
      );
    },
  },
  {
    field: "fullPotentialTotal",
    headerName: "fullPotentialTotal",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
          <Typography fontSize={"16px"} fontWeight={"400"}>
            {params.value}
          </Typography>
        </Stack>
      );
    },
  },
  {
    field: "total60",
    headerName: "total60",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
          <Typography fontSize={"16px"} fontWeight={"400"}>
            {params.value}
          </Typography>
        </Stack>
      );
    },
  },
  {
    field: "evol",
    headerName: "evol",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
          <Typography fontSize={"16px"} fontWeight={"400"}>
            {params.value}
          </Typography>
        </Stack>
      );
    },
  },
  {
    field: "payMonth",
    headerName: "payMonth",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
          <Typography fontSize={"16px"} fontWeight={"400"}>
            {params.value}
          </Typography>
        </Stack>
      );
    },
  },
  {
    field: "range",
    headerName: "range",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
          <Typography fontSize={"16px"} fontWeight={"400"}>
            {params.value}
          </Typography>
        </Stack>
      );
    },
  },
];
