import Search from "@mui/icons-material/Search";
import InsertDriveFile from "@mui/icons-material/InsertDriveFile";
import {
  Autocomplete,
  Box,
  Breadcrumbs,
  Button,
  Checkbox,
  Divider,
  Drawer,
  FormControl,
  InputAdornment,
  InputLabel,
  Link,
  MenuItem,
  Pagination,
  Select,
  TextField,
  Typography,
  useTheme,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { Stack } from "@mui/system";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format, isValid, set, subDays } from "date-fns";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useLoadingState, useDebounce } from "src/hooks";
import { ListMonetizationHierarchyUseCase } from "src/modules/monetization/use-cases/list-monetization-hierarchy";
import { DateUtils, SheetBuilder, getLayout } from "src/utils";
import { ListTransactions } from "src/modules/monetization/use-cases/list-transactions.use-case";
import { ModalMonetizationReportAdm } from "src/modules/monetization/fragments/modal-monetization-report-adm";
import { ModalMonetizationReportAdmDay } from "src/modules/monetization/fragments/modal-monetization-report-adm-day";
import { ModalMonetizationReportConsolidado } from "src/modules/monetization/fragments/modal-monetization-report-consolidado";
import { formatCurrency } from "src/utils/format-currency";
import abilityFor from "src/utils/ability-for";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { ListClimateUseCase } from "src/modules/climate/list-climate.use-case";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { Climate } from "src/components/climate";
import { ListSectorsAndSubsectrosUseCase } from "src/modules";
import { Sector, SectorAndSubsector } from "src/typings";
import { ListIndicatorsUseCase } from "src/modules/indicators/use-cases/list-indicators/list-indicators.use-case";
import { SearchAccountsUseCase } from "src/modules/personas/use-cases/search-accounts.use-case";
import ConnectWithoutContactOutlined from "@mui/icons-material/ConnectWithoutContactOutlined";
import FilterList from "@mui/icons-material/FilterList";
import HomeOutlined from "@mui/icons-material/HomeOutlined";
import { Indicator } from "src/typings/models/indicator.model";
import { useRouter } from "next/router";
import { VerificationPanelTable } from "src/modules/panel/fragments/verification-panel-table";
import { SectorIndicators } from "src/modules/home/use-cases/get-sectors-indicators";
import { usePagination } from "src/hooks/use-pagination/use-pagination";
import { SectorsByHierachyUseCase } from "src/modules/home/use-cases/SectorsByHierarchy/SectorsByHierarchy.use-case";
import { ListVerificationPanelUseCase } from "src/modules/panel/use-cases/list-verification-panel.use-case";


export default function ListClimate() {
  const theme = useTheme();
  const [startDate, setStartDate] = useState<Date | null>(null);
  const [data, setData] = useState<any>(null);
  const router = useRouter();
  const { handleChange, page, setPage, setTotalPages, totalPages } =
    usePagination();
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const { myUser } = useContext(UserInfoContext);
  const [indicator, setIndicator] = useState<Indicator | null>(null);
  const [indicatorSearchValue, setIndicatorSearchValue] = useState<string>("");
  const [indicators, setIndicators] = useState<Indicator[]>([]);
  const debouncedIndicatorsSearchTerm: string = useDebounce<string>(
    indicatorSearchValue,
    400
  );
  const [isOpenFilters, setIsOpenFilters] = useState(true);
  const [collaborators, setCollaborators] = useState<
    {
      id: number;
      name: string;
      registry: string;
    }[]
  >([]);
  const [selectedCollaborator, setSelectedCollaborator] = useState<
    {
      id: number;
      name: string;
      registry: string;
    } | null
  >(null);
  const [collaboratorSearch, setCollaboratorSearch] = useState<string>("");

  async function handleReportExtractExport(newFilter = false) {
    if(!startDate  || !indicator || !selectedCollaborator) return;
    let formatedStartDate = null;

    if (startDate && isValid(startDate)) {
      formatedStartDate = format(new Date(startDate.toString()), "yyyy-MM-dd");
    } else {
      return;
    }

    setData([]);

    startLoading();

    const payload = {
      indicatorId: indicator.id,
      dateSend: formatedStartDate,
      collaboratorId: selectedCollaborator.id,
    };

    await new ListVerificationPanelUseCase()
      .handle(payload)
      .then((data) => {
        setData(data);
        if(newFilter) setPage(1);
      })
      .catch(() => {
        toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
      })
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    handleReportExtractExport();
  }, [page]);

  const filterPanel = async () => {
    setIsOpenFilters(false);
    handleReportExtractExport(true);
  };

  const getIndicators = async (searchText: string) => {
    //startLoading();

    await new ListIndicatorsUseCase()
      .handle({
        limit: 10,
        offset: 1,
        searchText: searchText,
      })
      .then((data) => {
        setIndicators(data.items);
        console.log("indicators", data.items);
      })
      .catch(() => {
        toast.error("Falha ao carregar indicadores.");
      })
      .finally(() => {
        //finishLoading();
      });
  };

  useEffect(() => {
    getIndicators(indicatorSearchValue);
  }, [indicatorSearchValue]);

  const getCollaborators = async (searchText: string) => {
    //startLoading();

    await new SearchAccountsUseCase()
      .handle({
        limit: 10,
        page: 1,
        Collaborator: searchText,
      })
      .then((data) => {
        setCollaborators(data[0].account);
      })
      .catch(() => {
        toast.error("Falha ao carregar colaboradores.");
      })
      .finally(() => {
        //finishLoading();
      });
  };

  useEffect(() => {
    getCollaborators(collaboratorSearch);
  }, [collaboratorSearch]);

  useEffect(() => {
    getCollaborators("");
  }, []);

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
            onClick={() => router.push("/")}
          >
            <Typography fontWeight={"500"}>Painéis</Typography>
          </Link>
          <Link
            sx={{
              
              display: "flex",
              alignItems: "center",

              textDecoration: "none",
            }}
            color={theme.palette.background.default}
          >
            <Typography fontWeight={"700"}>Painel de verificação</Typography>
          </Link>
        </Breadcrumbs>
      </Stack>
      <ContentArea sx={{ py: "40px" }}>
        <PageTitle
          icon={<ConnectWithoutContactOutlined sx={{ fontSize: "30px" }} />}
          title="Painel de Verificação"
          loading={isLoading}
        >
          <Button variant="contained" onClick={() => setIsOpenFilters(true)}>
            Filtrar
          </Button>
        </PageTitle>

        <Stack mt={"20px"}>
          {data && data.length > 0 && (
            <VerificationPanelTable
              tableData={data}
              refreshHandle={handleReportExtractExport}
            />
            //   <Climate items={data} refreshHandle={handleReportExtractExport} />
          )}
          {/*<Pagination
            count={totalPages || 0}
            page={page}
            onChange={handleChange}
            disabled={isLoading}
          />*/}
        </Stack>
      </ContentArea>
      <Drawer
        open={isOpenFilters}
        anchor={"right"}
        onClose={() => setIsOpenFilters(false)}
        PaperProps={{
          sx: {
            borderTopLeftRadius: "16px",
            borderBottomLeftRadius: "16px",
          },
        }}
      >
        <Stack
          width={"100vw"}
          minWidth={"100%"}
          maxWidth={"430px"}
          direction={"column"}
          height={"100vh"}
          gap={"20px"}
          py={"40px"}
          px={"35px"}
        >
          <Typography
            fontSize={"20px"}
            fontWeight={"600"}
            alignItems={"center"}
            gap={"10px"}
            display={"flex"}
            flexDirection={"row"}
          >
            <FilterList />
            Filtrar
          </Typography>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="De"
              value={startDate}
              onChange={(newValue) => setStartDate(newValue)}
              slotProps={{
                textField: {
                  sx: {
                    minWidth: "180px",
                    svg: {
                      color: grey[500],
                    },
                    width: "100%",
                  },
                },
              }}
            />
          </LocalizationProvider>
          <Autocomplete
            fullWidth
            value={indicator}
            options={indicators}
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
              setIndicator(value);
            }}
            onInputChange={(e, text) => setIndicatorSearchValue(text)}
            filterOptions={(options, { inputValue }) =>
              options.filter((item) =>
                item.name
                  .toLocaleLowerCase()
                  .includes(inputValue.toLocaleLowerCase())
              )
            }
            filterSelectedOptions
            renderInput={(params) => (
              <TextField
                {...params}
                variant="outlined"
                label="Indicadores"
                placeholder="Buscar"
              />
            )}
            renderOption={(props, option) => {
              return (
                <li {...props} key={option.id}>
                  {option.name}
                </li>
              );
            }}
            isOptionEqualToValue={(option, value) => option.name === value.name}
            sx={{ p: 0, m: 0 }}
          />
          <Autocomplete
            fullWidth
            value={selectedCollaborator}
            options={collaborators}
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
              setSelectedCollaborator(value);
            }}
            onInputChange={(e, text) => setCollaboratorSearch(text)}
            filterOptions={(options, { inputValue }) =>
              options.filter((item) =>
                item.name
                  .toLocaleLowerCase()
                  .includes(inputValue.toLocaleLowerCase())
              )
            }
            filterSelectedOptions
            renderInput={(params) => (
              <TextField
                {...params}
                variant="outlined"
                label="Colaboradores"
                placeholder="Buscar"
              />
            )}
            renderOption={(props, option) => {
              return (
                <li {...props} key={option.id}>
                  {option.name}
                </li>
              );
            }}
            isOptionEqualToValue={(option, value) => option.name === value.name}
            sx={{ p: 0, m: 0 }}
          />
          <Button variant="contained" onClick={() => filterPanel()}>
            Filtrar
          </Button>
        </Stack>
      </Drawer>
    </ContentCard>
  );
}

ListClimate.getLayout = getLayout("private");
