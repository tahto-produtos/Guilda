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
import { useLoadingState } from "src/hooks";
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
import { SearchAccountsUseCase } from "src/modules/personas/use-cases/search-accounts.use-case";
import ConnectWithoutContactOutlined from "@mui/icons-material/ConnectWithoutContactOutlined";
import FilterList from "@mui/icons-material/FilterList";
import HomeOutlined from "@mui/icons-material/HomeOutlined";
import { useRouter } from "next/router";
import { ClimateTable } from "src/modules/climate/fragments/climate-table";
import { SectorIndicators } from "src/modules/home/use-cases/get-sectors-indicators";
import { usePagination } from "src/hooks/use-pagination/use-pagination";
import { SectorsByHierachyUseCase } from "src/modules/home/use-cases/SectorsByHierarchy/SectorsByHierarchy.use-case";

interface BalanceReportModel {
  hierarchy: string;
  sectors: Array<{
    sectorId: number;
    sector: string;
    monetization: number;
  }>;
}

function formatDetails(
  order: any,
  indicator: any,
  reason: any,
  observation: any
) {
  if (reason) {
    return `Motivo: ${reason}`;
  } else if (observation) {
    return `${observation}`;
  } else if (indicator) {
    return `Indicador: ${indicator.name}`;
  } else if (order) {
    return `Compra na loja - COD: ${order.id}`;
  }
}

export default function ListClimate() {
  const theme = useTheme();
  const [startDate, setStartDate] = useState<Date | null>(null);
  const [endDate, setEndDate] = useState<Date | null>(null);
  const [data, setData] = useState<any>(null);
  const router = useRouter();
  const { handleChange, page, setPage, setTotalPages, totalPages } =
    usePagination();
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const { myUser } = useContext(UserInfoContext);
  const [sectorSearch, setSectorSearch] = useState<string>("");
  const [sectors, setSectors] = useState<SectorAndSubsector[]>([]);
  const [selectedSectors, setSelectedSectors] = useState<SectorAndSubsector[]>([]);
  const [isOpenFilters, setIsOpenFilters] = useState(false);
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
    }[]
  >([]);
  const [collaboratorSearch, setCollaboratorSearch] = useState<string>("");
  const [answered, setAnswered] = useState<boolean>(false);
  const [unanswered, setUnanswered] = useState<boolean>(false);
  const [feedback, setFeedback] = useState<boolean>(false);
  const [shouldExecute, setShouldExecute] = useState(false);

  async function handleReportExtractExport(newFilter = false) {

    let formatedStartDate = null;
    let formatedEndDate = null;

    if (startDate && endDate && isValid(startDate) && isValid(endDate)) {
      formatedStartDate = format(new Date(startDate.toString()), "yyyy-MM-dd");
      formatedEndDate = format(new Date(endDate.toString()), "yyyy-MM-dd");
    } else {
      return;
    }

    setData([]);

    startLoading();

    const payload = {
      STARTEDATFROM: formatedStartDate,
      STARTEDATTO: formatedEndDate,
      PERSONASID: selectedCollaborator.map((collaborator) =>
        Number(collaborator.id)
      ),
      SECTORSID: selectedSectors.map((sector) => Number(sector.id)),
      FLAGRESPONSE: answered ? 1 : 0,
      FLAGNORESPONSE: unanswered ? 1 : 0,
      FLAGCANFEEDBACK: feedback ? 1 : 0,
      limit: 10,
      page: page,
    };

    await new ListClimateUseCase()
      .handle(payload)
      .then((data) => {
        setData(data.resposts);
        setTotalPages(data.TOTALPAGES);
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

  useEffect(() => {
    handleReportExtractExport(true);
  }, [feedback]);

  const filterClimate = async () => {
    setIsOpenFilters(false);
    handleReportExtractExport(true);
  };

  const getSectorsAndSubSectors = async (isSubsector = false, sector = "") => {
    //startLoading();

    await new ListSectorsAndSubsectrosUseCase()
      .handle({
        isSubsector,
        sector,
      })
      .then((data) => {
        setSectors(data);
      })
      .catch(() => {
        toast.error(
          `Falha ao carregar ${isSubsector ? "subsetores" : "setores"}.`
        );
      });
  };

  useEffect(() => {
    getSectorsAndSubSectors(false, sectorSearch);
  }, [sectorSearch]);

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

  function handleOnSelectAnswered() {
    if (!answered) {
      setAnswered(true);
      setUnanswered(false);
    } else {
      setAnswered(false);
    }
  }

  function handleOnSelectUnanswered() {
    if (!unanswered) {
      setUnanswered(true);
      setAnswered(false);
    } else {
      setUnanswered(false);
    }
  }

  useEffect(() => {
      setShouldExecute(true);
  }, [answered, unanswered]);

  useEffect(() => {
    if (shouldExecute) {
      handleReportExtractExport(true);
      setShouldExecute(false);
    }
  }, [shouldExecute]);

  useEffect(() => {
    getSectorsAndSubSectors(false, sectorSearch);
    const currentDate = new Date();
    // Subtrai 2 dias da data atual usando date-fns
    const modifiedStartDate = subDays(currentDate, 2);
    setStartDate(modifiedStartDate);
    setEndDate(new Date());
    handleReportExtractExport(true);
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
            onClick={() => router.push("/climate")}
          >
            <Typography fontWeight={"500"}>Clima</Typography>
          </Link>
          <Link
            sx={{
              display: "flex",
              alignItems: "center",

              textDecoration: "none",
            }}
            color={theme.palette.background.default}
          >
            <Typography fontWeight={"700"}>Histórico de respostas</Typography>
          </Link>
        </Breadcrumbs>
      </Stack>
      <ContentArea sx={{ py: "40px" }}>
        <PageTitle
          icon={<ConnectWithoutContactOutlined sx={{ fontSize: "30px" }} />}
          title="Histórico de resposta - Clima"
          loading={isLoading}
        >
          <Button variant="contained" onClick={() => setIsOpenFilters(true)}>
            Filtrar
          </Button>
        </PageTitle>

        <Box
          display={"flex"}
          gap={2}
          width={"100%"}
          marginTop={2}
          bgcolor={"#F4F4F4"}
          p={"12px"}
          borderRadius={"16px"}
        >
          <Stack direction={"row"} alignItems={"center"} gap={"0px"}>
            <Button
              variant={answered ? "contained" : "outlined"}
              sx={{
                border: !answered
                  ? `solid 1px ${theme.palette.secondary.main}`
                  : undefined,
                color: !answered
                  ? `${theme.palette.secondary.main}`
                  : undefined,
              }}
              onClick={(event) => handleOnSelectAnswered()}
            >
              Só Respondidas
            </Button>
          </Stack>
          <Stack direction={"row"} alignItems={"center"} gap={"0px"}>
            <Button
              variant={unanswered ? "contained" : "outlined"}
              sx={{
                border: !unanswered
                  ? `solid 1px ${theme.palette.secondary.main}`
                  : undefined,
                color: !unanswered
                  ? `${theme.palette.secondary.main}`
                  : undefined,
              }}
              onClick={(event) => handleOnSelectUnanswered()}
            >
              Só Não Respondidas
            </Button>
          </Stack>
          <Stack direction={"row"} alignItems={"center"} gap={"0px"}>
            <Button
              variant={feedback ? "contained" : "outlined"}
              sx={{
                border: !feedback
                  ? `solid 1px ${theme.palette.secondary.main}`
                  : undefined,
                color: !feedback
                  ? `${theme.palette.secondary.main}`
                  : undefined,
              }}
              onClick={() => setFeedback(!feedback)}
            >
              Sem Apoio Supervisor
            </Button>
          </Stack>
        </Box>
        {/* <Box display={"flex"} gap={2} width={"100%"} marginTop={2}>
                    <Button variant="contained" onClick={handleReportExtractExport}>
                        Ver relatório
                    </Button>
                </Box> */}
        <Stack mt={"20px"}>
          {data && data.length > 0 && (
            <ClimateTable
              tableData={data}
              refreshHandle={handleReportExtractExport}
            />
            //   <Climate items={data} refreshHandle={handleReportExtractExport} />
          )}
          <Pagination
            count={totalPages || 0}
            page={page}
            onChange={handleChange}
            disabled={isLoading}
          />
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
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Até"
              value={endDate}
              onChange={(newValue) => setEndDate(newValue)}
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
            multiple
            fullWidth
            value={selectedSectors}
            options={sectors}
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
              setSelectedSectors(value);
            }}
            onInputChange={(e, text) => setSectorSearch(text)}
            filterOptions={(options, { inputValue }) =>
              options.filter(
                (item) =>
                  item.id.toString().includes(inputValue.toString()) ||
                  item.name
                    .toString()
                    .toLowerCase()
                    .includes(inputValue.toString().toLowerCase())
              )
            }
            filterSelectedOptions
            renderInput={(props) => (
              <TextField {...props} label={"Setor"} sx={{ height: "auto" }} />
            )}
            renderOption={(props, option) => {
              return (
                <li {...props} key={option.id}>
                 {option.id} - {option.name}
                </li>
              );
            }}
            limitTags={1}
            isOptionEqualToValue={(option, value) => option.name === value.name}
            sx={{ m: 0, height: "auto" }}
          />
          <Autocomplete
            multiple
            fullWidth
            disableClearable={false}
            value={selectedCollaborator}
            options={collaborators}
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
              setSelectedCollaborator(value);
            }}
            onInputChange={(e, text) => setCollaboratorSearch(text)}
            filterSelectedOptions
            renderInput={(props) => (
              <TextField {...props} label={"Colaboradores"} />
            )}
            renderOption={(props, option) => {
              return (
                <li {...props} key={option.id}>
                  {option.id} - {option.name}
                </li>
              );
            }}
            isOptionEqualToValue={(option, value) => option.name === value.name}
            sx={{ m: 0 }}
          />
          <Button variant="contained" onClick={() => filterClimate()}>
            Filtrar
          </Button>
        </Stack>
      </Drawer>
    </ContentCard>
  );
}

ListClimate.getLayout = getLayout("private");
