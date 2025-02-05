import {
    DangerousOutlined,
    FeedbackOutlined,
    FilterAlt,
    FilterAltOutlined,
    HomeOutlined,
    LinearScale,
    ListAltOutlined,
    PageviewOutlined,
    SendTimeExtensionOutlined,
  } from "@mui/icons-material";
  import {
    Autocomplete,
    Box,
    Breadcrumbs,
    Button,
    CircularProgress,
    Divider,
    Link,
    Stack,
    TextField,
    Typography,
    lighten,
    useTheme,
  } from "@mui/material";
  import { grey } from "@mui/material/colors";
  import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
  import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
  import { format } from "date-fns";
  import { useRouter } from "next/router";
  import { useContext, useEffect, useState } from "react";
  import { toast } from "react-toastify";
  import { PageTitle } from "src/components/data-display/page-title/page-title";
  import { ContentArea } from "src/components/surfaces/content-area/content-area";
  import { ContentCard } from "src/components/surfaces/content-card/content-card";
  import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
  import { QuizContext } from "src/contexts/quiz-provider/quiz.context";
  import { UserInfoContext } from "src/contexts/user-context/user.context";
  import { useDebounce, useLoadingState } from "src/hooks";
  import {
    ListPeriodUseCase,
    ListSectorsAndSubsectrosUseCase,
    ListSectorsUseCase,
  } from "src/modules";
import { ReportCampaignUseCase } from "src/modules/campaign/use-cases/report-campaing.use-case";
  import { ActionPlanTable } from "src/modules/escalation/fragments/action-plan-table";
  import { PerformanceIndicator } from "src/modules/escalation/fragments/performance-indicator";
  import { CreateAutoActionUseCase } from "src/modules/escalation/use-cases/create-auto-action.use-case";
  import { CreateHistoryActionUseCase } from "src/modules/escalation/use-cases/create-history-action.use-case";
  import { CreateStageActionUseCase } from "src/modules/escalation/use-cases/create-stage-action.use-case";
  import {
    LoadActionEscalation,
    LoadLibraryActionEscalationUseCase,
  } from "src/modules/escalation/use-cases/load-library-action-escalation";
  import { ReportActionEscalationUseCase } from "src/modules/escalation/use-cases/report-action-escalation.use-case";
  import { CreateInfractionButton } from "src/modules/feedback/fragments/create-infraction-button";
  import { ListGroupsNewUseCase } from "src/modules/groups/use-cases/list-groups-new";
  import { ListHierarchiesUseCase } from "src/modules/hierarchies/use-cases/list-hierarchies.use-case";
  import { ListIndicatorsUseCase } from "src/modules/indicators/use-cases";
  import { Indicator, Period, Sector, SectorAndSubsector } from "src/typings";
  import { GroupNew } from "src/typings/models/group-new.model";
  import { Hierarchie } from "src/typings/models/hierarchie.model";
  import { PaginationModel } from "src/typings/models/pagination.model";
  import { SheetBuilder, getLayout } from "src/utils";
  import { capitalizeText } from "src/utils/capitalizeText";
  
  export default function ActionEscalationReportView() {
    const { myUser } = useContext(UserInfoContext);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [searchText, setSearchText] = useState<string>("");
    const debouncedSearchText: string = useDebounce<string>(searchText, 400);
    const router = useRouter();
    const theme = useTheme();
  
    const [searchAction, setSearchAction] = useState<string>("");
    const [sector, setSector] = useState<SectorAndSubsector[]>([]);
    const [sectors, setSectors] = useState<SectorAndSubsector[]>([]);
    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const debouncedSectorSearchTerm: string = useDebounce<string>(
      sectorsSearchValue,
      400
    );
  
    const [sectorSub, setSectorSub] = useState<SectorAndSubsector[]>([]);
    const [sectorsSub, setSectorsSub] = useState<SectorAndSubsector[]>([]);
    const [sectorsSearchValueSub, setSectorsSearchValueSub] =
      useState<string>("");
    const debouncedSectorSearchTermSub: string = useDebounce<string>(
      sectorsSearchValue,
      400
    );
  
    const [indicatorsSearchValue, setIndicatorsSearchValue] =
      useState<string>("");
    const [selectedIndicators, setSelectedIndicators] = useState<Indicator[]>([]);
    const [indicators, setIndicators] = useState<Indicator[]>([]);
  
    const [startDate, setStartDate] = useState<dateFns | null>(null);
    const [endDate, setEndDate] = useState<dateFns | null>(null);

    const [startDateI, setStartDateI] = useState<dateFns | null>(null);
    const [endDateI, setEndDateI] = useState<dateFns | null>(null);
  
    const getIndicatorsList = async () => {
      const pagination: PaginationModel = {
        limit: 20,
        offset: 0,
        searchText: indicatorsSearchValue,
      };
  
      new ListIndicatorsUseCase()
        .handle(pagination)
        .then((data) => {
          setIndicators(data.items);
        })
        .catch(() => {
          toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
        })
        .finally(() => {});
    };
  
    useEffect(() => {
      getIndicatorsList();
    }, [indicatorsSearchValue]);
  
    async function handleCreate() {
      if (!startDate || !endDate || !startDateI || !endDateI) return;
      startLoading();
      await new ReportCampaignUseCase()
        .handle({
          ENDEDATFROM: format(new Date(startDateI.toString()), "yyyy-MM-dd"),
          ENDEDATTO: format(new Date(endDateI.toString()), "yyyy-MM-dd"),
          INDICATORSID: selectedIndicators.map((i) => i.id),
          STARTEDATFROM: format(new Date(startDate.toString()), "yyyy-MM-dd"),
          STARTEDATTO: format(new Date(endDate.toString()), "yyyy-MM-dd"),
        })
        .then((data) => {
          if (data.length <= 0) {
            return toast.warning("Sem dados para exportar.");
          }
          const docRows = data.map((item: any) => {
            return [
              item.NOME_CAMPANHA,
              item.CRIADO_POR,
              item.DATA_INICIO,
              item.DATA_FIM,
              { v: item.IDGDA_INDICATOR, t: "n" },
              { v: item.IDGDA_COLLABORATOR, t: "n" },
              item.NOME_INDICADOR,
              item.NOME_COLABORADOR,
              item.RANKING_COLABORADOR,
              item.STATUS_CAMPANHA,
              item.RESULTADO_INICIAL,
              item.RESULTADO_ATUAL,
              item.PORCENTAGEM_EVOLUCAO,
            ];
          });
          let indicatorSheetBuilder = new SheetBuilder();
          indicatorSheetBuilder
            .setHeader([
              "NOME_CAMPANHA",
              "CRIADO_POR",
              "DATA_INICIO",
              "DATA_FIM",
              "IDGDA_INDICATOR",
              "IDGDA_COLLABORATOR",
              "NOME_INDICADOR",
              "NOME_COLABORADOR",
              "RANKING_COLABORADOR",
              "STATUS_CAMPANHA",
              "RESULTADO_INICIAL",
              "RESULTADO_ATUAL",
              "PORCENTAGEM_EVOLUCAO",
            ])
            .append(docRows)
            .exportAs(`Relatório_Plano_de_Campanha`);
          toast.success("Relatório exportado com sucesso!");
        })
        .catch(() => {
          toast.error("Falha ao exportar.");
        })
        .finally(() => {
          finishLoading();
        });
    }
  
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
              <Typography fontWeight={"700"}>
                Relatório de Plano de Ação
              </Typography>
            </Link>
          </Breadcrumbs>
        </Stack>
        <ContentArea sx={{ py: " 40px" }}>
          <Stack px={"40px"}>
            <PageTitle
              icon={<ListAltOutlined sx={{ fontSize: "40px" }} />}
              title="Relatório de Campanha"
              loading={isLoading}
            ></PageTitle>
            <Divider />
            <Stack mt={"40px"} direction={"row"} gap="16px">
              
            </Stack>

            <Stack direction={"row"} gap={"16px"} mb={"30px"}>
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <DatePicker
                  label="Data início"
                  value={startDateI}
                  onChange={(newValue) => setStartDateI(newValue)}
                  slotProps={{
                    textField: {
                      fullWidth: true,
                      sx: {
                        minWidth: "180px",
                        svg: {
                          color: grey[500],
                        },
                      },
                    },
                  }}
                />
              </LocalizationProvider>
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <DatePicker
                  label="Data fim"
                  value={endDateI}
                  onChange={(newValue) => setEndDateI(newValue)}
                  slotProps={{
                    textField: {
                      fullWidth: true,
                      sx: {
                        minWidth: "180px",
                        svg: {
                          color: grey[500],
                        },
                      },
                    },
                  }}
                />
              </LocalizationProvider>
            </Stack>

            <Stack direction={"row"} gap={"16px"} mb={"30px"}>
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <DatePicker
                  label="De"
                  value={startDate}
                  onChange={(newValue) => setStartDate(newValue)}
                  slotProps={{
                    textField: {
                      fullWidth: true,
                      sx: {
                        minWidth: "180px",
                        svg: {
                          color: grey[500],
                        },
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
                      fullWidth: true,
                      sx: {
                        minWidth: "180px",
                        svg: {
                          color: grey[500],
                        },
                      },
                    },
                  }}
                />
              </LocalizationProvider>
            </Stack>
            <Stack direction={"row"} gap="16px">
              
              <Autocomplete
                fullWidth
                multiple
                value={selectedIndicators}
                options={indicators}
                getOptionLabel={(option) => option.name}
                onChange={(event, value) => {
                  setSelectedIndicators(value);
                }}
                filterSelectedOptions
                renderInput={(props) => (
                  <TextField {...props} label={"Indicador"} />
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
            </Stack>
            <Stack direction={"row"} mt={"40px"}>
              <Button variant="contained" onClick={handleCreate}>
                Gerar relatório
              </Button>
            </Stack>
          </Stack>
        </ContentArea>
      </ContentCard>
    );
  }
  
  ActionEscalationReportView.getLayout = getLayout("private");
  