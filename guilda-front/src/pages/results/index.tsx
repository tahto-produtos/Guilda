import { HomeOutlined, Stars, Close as CloseIcon } from "@mui/icons-material";
import {
    Autocomplete,
    Box,
    Button,
    Checkbox,
    Stack,
    TextField,
    Typography,
    createFilterOptions,
    Chip,
    Popover,
    IconButton,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { DatePicker, LocalizationProvider, deDE } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import {
    format,
    formatISO,
    startOfDay,
    subDays,
    subMonths,
    subWeeks,
} from "date-fns";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { useLoadingState } from "src/hooks";
import { CollaboratorHierarchy } from "src/modules/collaborators/use-cases/collaborator-hierarchy";
import { ConsolidatedResult } from "src/modules/home/fragments/consolidated-result/consolidated-result";
import { IndicatorBasketCard } from "src/modules/home/fragments/indicator-basket/indicator-basket";
import { IndicatorsResultsDate } from "src/modules/home/fragments/indicators-results-date/indicators-results-date";
import IndicatorsResults from "src/modules/home/fragments/indicators-results/indicators-results";
import IndicatorsSectorsResults from "src/modules/home/fragments/indicators-sectors-results/indicators-sectors-results";
import { ModalExportResults } from "src/modules/home/fragments/modal-export-results/modal-export-results";
import { OperationRanking } from "src/modules/home/fragments/operation-ranking/operation-ranking";
import OtherOperatorsResults from "src/modules/home/fragments/other-operators-results/other-operators-results";
import Ranking from "src/modules/home/fragments/ranking/ranking";
import { Sector, Period, HomeFloor } from "src/typings";
import { getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";
import { ListPeriodUseCase } from "src/modules/period";
import { ListHomeFloorUseCase } from "src/modules/home-floor";
import { Site } from "src/typings/models/site.model";
import { SitePersonaUseCase } from "src/modules/personas/use-cases/site-personas.use-case";

export default function ResultsView() {
    const { myPermissions } = useContext(PermissionsContext);
    const { myUser } = useContext(UserInfoContext);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [totalUsersUnder, setTotalUsersUnder] = useState<number>(0);
    const [startDatePicker, setStartDatePicker] = useState<
        dateFns | Date | null
    >(null);
    const [endDatePicker, setEndDatePicker] = useState<dateFns | Date | null>(
        null
    );
    const [modalExportResultOpen, setModalExportResultOpen] =
        useState<boolean>(false);
    const [sectorsFilter, setSectorsFilter] = useState<Sector[]>([]);
    const [sectorsFilterSelected, setSectorsFilterSelected] = useState<
        Sector[]
    >([]);
    const [sectorsGroupsFilter, setSectorsGroupsFilter] = useState<Sector[]>([]);
    const [sectorsGroupsFilterSelected, setSectorsGroupsFilterSelected] = useState<
        Sector[]
    >([]);
    const [searchSector, setSearchSector] = useState("");
    const [selectedPeriods, setSelectedPeriods] = useState<Period[]>([]);
    const [periods, setPeriods] = useState<Period[]>([]);
    const [selectedHomeFloor, setSelectedHomeFloor] = useState<HomeFloor[]>([]);
    const [homesFloors, setHomesFloors] = useState<HomeFloor[]>([]);
    const [selectedSite, setSelectedSite] = useState<Site[]>([]);
    const [sites, setSites] = useState<Site[]>([]);
    //Set default date
    useEffect(() => {
        const today = new Date();
        const thisMonth = today.getMonth();
        const thisYear = today.getFullYear();
        const defaultStartDate = formatISO(
            new Date(startOfDay(new Date(thisYear, thisMonth, 1)))
        );
        const defaultEndDate = today;

        myUser && setStartDatePicker(new Date(defaultStartDate));
        myUser && setEndDatePicker(defaultEndDate);
    }, [myUser]);

    const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);

    const handlePopoverOpen = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handlePopoverClose = () => {
        setAnchorEl(null);
    };

    const open = Boolean(anchorEl);

    const getPeriods = async (codCollaborator: number) => {
        startLoading();

        await new ListPeriodUseCase()
            .handle({
                codCollaborator,
            })
            .then((data) => {
                setPeriods(data);
            })
            .catch(() => {
                toast.error("Falha ao carregar turnos.");
            })
            .finally(() => {
                finishLoading();
            });
    };
    useEffect(() => {
        if (myUser && myUser?.id) {
            getPeriods(myUser?.id);
        }
    }, []);
    const getHomeOrFloor = async (codCollaborator: number) => {
        startLoading();

        await new ListHomeFloorUseCase()
            .handle({
                codCollaborator,
            })
            .then((data) => {
                setHomesFloors(data);
            })
            .catch(() => {
                toast.error("Falha ao carregar home ou piso.");
            })
            .finally(() => {
                finishLoading();
            });
    };

    useEffect(() => {
        if (myUser && myUser?.id) {
            getHomeOrFloor(myUser?.id);
        }
    }, []);

    /*       async function ListSitePersona() {
            await new SitePersonaUseCase()
              .handle()
              .then((data) => {
                setSites(data);
              })
              .catch(() => {
                toast.error("Erro ao listar Sites.");
              })
              .finally(() => {});
          } */

    async function ListSitePersona() {
        try {
            const data = await new SitePersonaUseCase().handle();
            const filteredData = data.filter(item => item.name !== '-'); // Filtra os itens que não são '-'
            setSites(filteredData);
        } catch (error) {
            toast.error("Erro ao listar Sites.");
        }
    }

    useEffect(() => {
        ListSitePersona();
    }, []);
    // useEffect(() => {
    //     myUser && myUser.id && getCollaboratorHierarchy();
    // }, [myUser]);

    // const getCollaboratorHierarchy = async () => {
    //     startLoading();

    //     const payload = {
    //         id: myUser.id,
    //     };

    //     new CollaboratorHierarchy()
    //         .handle(payload)
    //         .then((data) => {
    //             setTotalUsersUnder(data.totalItems);
    //         })
    //         .catch(() => {
    //             toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
    //         })
    //         .finally(() => {
    //             finishLoading();
    //         });
    // };

    return (
        <Box
            display={"flex"}
            width={"100%"}
            flexDirection={"column"}
            gap={"40px"}
            pb={"100px"}
        >
            <ContentCard
                sx={{
                    position: "sticky",
                    top: 144,
                    zIndex: 9,
                    border: `solid 1px #e1e1e1`,
                }}
            >
                <Box
                    display={"flex"}
                    justifyContent={"space-between"}
                    gap={"20px"}
                    alignItems={"center"}
                    borderRadius={"8px"}
                    p={"24px"}
                >
                    <Box>
                        <Typography fontSize={"16px"} fontWeight={"600"}>
                            Indicadores
                        </Typography>
                        <Typography
                            fontSize={"14px"}
                            fontWeight={"300"}
                            sx={{ color: "#676767" }}
                        >
                            Selecione uma data para ver os indicadores
                        </Typography>
                    </Box>
                    <Box>
                        {myUser &&
                            (myUser.hierarchy !== "AGENTE" ||
                                myUser.isAdmin) && (
                                    <Box
                                    display={"flex"}
                                    flexDirection={"row"}
                                    gap={"10px"}
                                    width={"100%"}
                                    paddingBottom={"10px"}
                                  >
                                    <Autocomplete
                                      value={sectorsGroupsFilterSelected}
                                      placeholder={"Agrupamentos"}
                                      multiple
                                      disableClearable={false}
                                      onChange={(e, sector) => {
                                        setSectorsGroupsFilterSelected(sector);
                                        setSectorsFilterSelected([]);
                                      }}
                                      isOptionEqualToValue={(option, value) => option.name == value.name}
                                      disableCloseOnSelect
                                      renderInput={(props) => (
                                        <TextField {...props} size={"small"} label={"Agrupamentos"} />
                                      )} 
                                      renderTags={(value, getTagProps) => {
                                        // Limita a renderização ao primeiro setor
                                        const firstTag = value.slice(0, 1).map((option, index) => {
                                          const { key, ...rest } = getTagProps({ index }); // Captura o resultado de getTagProps separadamente
                                      
                                          return (
                                            <Chip
                                              key={key} // Definindo a chave única manualmente
                                              sx={{
                                                width: '70%',
                                                whiteSpace: 'nowrap',
                                                overflow: 'hidden',
                                                textOverflow: 'ellipsis',
                                              }}
                                              label={option.name}
                                              {...rest} // Aplica o restante das props de getTagProps
                                              onClick={handlePopoverOpen}
                                            />
                                          );
                                        });
                                  
                                        // Chip para mostrar setores adicionais
                                        const additionalTag = value.length > 1 ? (
                                          <Chip
                                            key="additional" // Define uma chave única para o chip adicional
                                            label={`+${value.length - 1}`}
                                            onClick={handlePopoverOpen} // Abre o popover ao clicar no chip extra

                                          />
                                        ) : null;
                                  
                                        // Retorna os chips concatenados
                                        return firstTag.concat(additionalTag ? [additionalTag] : []);
                                      }}
                                      filterOptions={(options, { inputValue }) =>
                                        options.filter(
                                          (item) =>
                                            item.name
                                              .toString()
                                              .toLowerCase()
                                              .includes(inputValue.toString().toLowerCase())
                                        )
                                      }
                                      getOptionLabel={(option) => option.name}
                                      options={sectorsGroupsFilter}
                                      sx={{
                                        mb: 0,
                                        minWidth: "330px",
                                        width: "200px",
                                      }}
                                      renderOption={(props, option) => (
                                        <li {...props} key={option.name}>
                                          {option.name}
                                        </li>
                                      )}
                                    />
                                    <Popover
                                      open={open}
                                      anchorEl={anchorEl}
                                      onClose={handlePopoverClose}
                                      anchorOrigin={{
                                        vertical: 'bottom',
                                        horizontal: 'left',
                                      }}
                                    >
                                      <Box sx={{ p: 2, maxWidth: 300 }}>
                                        {sectorsGroupsFilterSelected.map((option, index) => (
                                          <Chip
                                            key={option.name}
                                            label={option.name}
                                            onDelete={() => {
                                              setSectorsGroupsFilterSelected((prev) =>
                                                prev.filter((sector) => sector.name !== option.name)
                                              );
                                            }}
                                            deleteIcon={<IconButton size="small"><CloseIcon /></IconButton>}
                                            sx={{ m: 0.5 }}
                                          />
                                        ))}
                                      </Box>
                                    </Popover>
                                  </Box>
                                  
                            )}
                        <Box
                            display={"flex"}
                            flexDirection={"row"}
                            gap={"10px"}
                            width={"100%"}
                        >

                            <Autocomplete
                                value={sectorsFilterSelected}
                                placeholder={"Setores"}
                                multiple
                                disableClearable={false}
                                onChange={(e, sector) => {
                                    setSectorsFilterSelected(sector);
                                    setSectorsGroupsFilterSelected([]);
                                }}
                                isOptionEqualToValue={(option, value) =>
                                    option.id == value.id
                                }
                                disableCloseOnSelect
                                renderInput={(props) => (
                                    <TextField
                                        {...props}
                                        size={"small"}
                                        label={"Setores"}
                                    />
                                )}
                                filterOptions={(options, { inputValue }) =>
                                    options.filter(
                                        (item) =>
                                            item.id
                                                .toString()
                                                .includes(
                                                    inputValue.toString()
                                                ) ||
                                            item.name
                                                .toString()
                                                .toLowerCase()
                                                .includes(
                                                    inputValue
                                                        .toString()
                                                        .toLowerCase()
                                                )
                                    )
                                }
                                getOptionLabel={(option) => option.name}
                                options={sectorsFilter}
                                sx={{
                                    mb: 0,
                                    minWidth: "330px",
                                    width: "200px",
                                }}
                                renderOption={(props, option) => {
                                    return (
                                        <li {...props} key={option.id}>
                                            {option.id} - {option.name}
                                        </li>
                                    );
                                }}
                            />
                            <Autocomplete
                                multiple
                                placeholder={"Turno"}
                                value={selectedPeriods}
                                options={periods}

                                getOptionLabel={(option) => option.name}
                                sx={{
                                    mb: 0,
                                    minWidth: "100",
                                    width: "100px",
                                }}
                                onChange={(event, value) => {
                                    setSelectedPeriods(value);
                                }}
                                filterSelectedOptions
                                renderInput={(props) => (
                                    <TextField {...props} size={"small"} label={"Turno"} />
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

                            />
                            <Autocomplete
                                multiple
                                placeholder={"Home/Piso"}
                                value={selectedHomeFloor}
                                options={homesFloors}
                                sx={{
                                    mb: 0,
                                    minWidth: "100",
                                    width: "100px",
                                }}
                                getOptionLabel={(option) => option.name}
                                onChange={(event, value) => {
                                    setSelectedHomeFloor(value);
                                }}
                                filterSelectedOptions
                                renderInput={(props) => (
                                    <TextField {...props} size={"small"} label={"Home/Piso"} />
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
                            />
                            <Autocomplete
                                multiple
                                placeholder={"Site"}
                                value={selectedSite}
                                options={sites}
                                sx={{
                                    mb: 0,
                                    minWidth: "100",
                                    width: "100px",
                                }}
                                getOptionLabel={(option) => option.name}
                                onChange={(event, value) => {
                                    setSelectedSite(value);
                                }}
                                filterSelectedOptions
                                renderInput={(props) => (
                                    <TextField {...props} size={"small"} label={"Sites"} />
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
                            />
                            <LocalizationProvider dateAdapter={AdapterDateFns}>
                                <DatePicker
                                    label="De"
                                    value={startDatePicker}
                                    onChange={(newValue) =>
                                        setStartDatePicker(newValue)
                                    }
                                    slotProps={{
                                        textField: {
                                            size: "small",
                                            sx: {
                                                minWidth: "150px",
                                                maxWidth: "150px",
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
                                    value={endDatePicker}
                                    onChange={(newValue) =>
                                        setEndDatePicker(newValue)
                                    }
                                    slotProps={{
                                        textField: {
                                            size: "small",
                                            sx: {
                                                minWidth: "150px",
                                                maxWidth: "150px",
                                                svg: {
                                                    color: grey[500],
                                                },
                                                width: "100%",
                                            },
                                        },
                                    }}
                                />
                            </LocalizationProvider>
                        </Box>
                    </Box>
                </Box>
            </ContentCard>
            <ContentCard>
                {startDatePicker && endDatePicker && (
                    <Box display={"flex"} flexDirection={"column"} gap={"16px"}>
                        <Box
                            display={"flex"}
                            justifyContent={"space-between"}
                            alignItems={"center"}
                        >
                            <Box>
                                <Typography
                                    fontSize={"16px"}
                                    fontWeight={"600"}
                                >
                                    Resultado dos indicadores
                                </Typography>
                                {startDatePicker && endDatePicker && (
                                    <Typography
                                        fontSize={"14px"}
                                        fontWeight={"300"}
                                        sx={{ color: "#676767" }}
                                    >
                                        Entre{" "}
                                        {format(
                                            new Date(
                                                startDatePicker.toString()
                                            ),
                                            "dd-MM-yyyy"
                                        )}{" "}
                                        e{" "}
                                        {format(
                                            new Date(endDatePicker.toString()),
                                            "dd-MM-yyyy"
                                        )}
                                    </Typography>
                                )}
                            </Box>
                        </Box>
                        {myUser &&
                            (myUser.hierarchy !== "AGENTE" ||
                                myUser.isAdmin) && sectorsGroupsFilterSelected.length == 0 && (
                                <ConsolidatedResult
                                    endDate={endDatePicker}
                                    startDate={startDatePicker}
                                    period={selectedPeriods}
                                    homeFloor={selectedHomeFloor}
                                    site={selectedSite}
                                    selectedSectors={sectorsFilterSelected}
                                    selectedSectorsGroups={sectorsGroupsFilterSelected}
                                    hierarchyId={myUser.hierarchyId}
                                />
                            )}
                        {myUser && (
                            <IndicatorsSectorsResults
                                userId={myUser?.id}
                                searchStartDate={startDatePicker}
                                searchEndDate={endDatePicker}
                                period={selectedPeriods}
                                homeFloor={selectedHomeFloor}
                                site={selectedSite}
                                hierarchyId={myUser.hierarchyId}
                                getSectorsList={(data) =>
                                    setSectorsFilter(data)
                                }
                                getSectorsList2={(data) =>
                                    setSectorsGroupsFilter(data)
                                }
                                filterSelected={sectorsFilterSelected}
                                selectedSectorsGroups={sectorsGroupsFilterSelected}
                            />
                        )}
                    </Box>
                )}
            </ContentCard>
            {/* {totalUsersUnder > 0 && (
                <OtherOperatorsResults
                    userId={myUser?.id}
                    searchStartDate={startDatePicker}
                    searchEndDate={endDatePicker}
                />
            )} */}
            {/* abilityFor(myPermissions).can("Ver Ranking", "Home") && (
                
                    <>
                    <OperationRanking />
                </>
                
            ) */}
            
        </Box>
    );
}

ResultsView.getLayout = getLayout("private");
