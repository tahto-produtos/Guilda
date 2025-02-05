import {
    Box,
    Button,
    CardMedia,
    CircularProgress,
    Divider,
    Stack,
    Tooltip,
    Typography,
    darken,
    lighten,
    useTheme,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { useContext, useEffect, useState } from "react";
import { useLoadingState } from "src/hooks";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { DateUtils } from "src/utils";
import {
    endOfToday,
    format,
    formatISO,
    startOfDay,
    startOfToday,
} from "date-fns";
import { ResultsIndicatorsSectors } from "../../use-cases/results-indicators-sectors";
import { ListIndicatorBasketUseCase } from "../../use-cases/get-indicator-basket/get-indicator-basket.use-case";
import { Sector, Period, HomeFloor } from "src/typings";
import KeyboardArrowDown from "@mui/icons-material/KeyboardArrowDown";
import Ranking from "../ranking/ranking";
import { formatStringFirstLetterUppercase } from "src/utils/format-string-first-letter-uppercase";
import Image from "next/image";
import skyLogo from "../../../../assets/sky.png";
import { SectorIndicators } from "../../use-cases/get-sectors-indicators";
import MonetizationOn from "@mui/icons-material/MonetizationOn";
import { Rank } from "src/typings/models/rank.model";
import OtherOperatorsResults from "../other-operators-results/other-operators-results";
import { ResultsDetails } from "../results-details/results-details";
import { BaseModal } from "src/components/feedback";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { SubSectorsBySectorUseCase } from "../../use-cases/subsectors-by-sector.use-case";
import { SubSectorsList } from "../subsectors-list/subsectors-list";
import { SectorIndicatorsV2 } from "../../use-cases/get-sectors-indicators-v2";
import { SectorGroupsIndicatorsV2 } from "../../use-cases/get-sectors-groups-indicators-v2";

import { Site } from "src/typings/models/site.model";
import { Console } from "console";

function getColorByGroupName(group: string) {
    if (group == "G1") {
        return "primary.main";
    }
    if (group == "G2") {
        return "success.main";
    }
    if (group == "G3") {
        return "warning.main";
    }
    if (group == "G4") {
        return "error.main";
    }
    return "inherit";
}

function getColorByGroupNameProgress(group: string) {
    if (group == "G1") {
        return "primary";
    }
    if (group == "G2") {
        return "success";
    }
    if (group == "G3") {
        return "warning";
    }
    if (group == "G4") {
        return "error";
    }
    return "primary";
}

export default function IndicatorsSectorsResults(props: {
    userId: number;
    hierarchyId?: number;
    hideIndicatorBasket?: boolean;
    searchStartDate: dateFns | Date | null;
    searchEndDate: dateFns | Date | null;
    period: Period[];
    homeFloor: HomeFloor[];
    site: Site[];
    sectorId?: number;
    sectorName?: string;
    isOtherOperators?: boolean;
    getSectorsList?: (input: Sector[]) => void;
    getSectorsList2?: (input: Sector[]) => void;
    filterSelected?: Sector[];
    selectedSectorsGroups?: Sector[];
}) {
    const {
        userId,
        hideIndicatorBasket,
        searchStartDate,
        searchEndDate,
        period,
        homeFloor,
        site,
        sectorId,
        sectorName,
        isOtherOperators,
        getSectorsList,
        getSectorsList2,
        filterSelected,
        selectedSectorsGroups,
        hierarchyId,
    } = props;

    console.log("Hierarchy: " + hierarchyId);
    const { myUser } = useContext(UserInfoContext);
    const { finishLoading, isLoading, startLoading } = useLoadingState();

    const [dateFilter, setDateFilter] = useState<"day" | "week" | "month">(
        "month"
    );

    const todayEndOfDay = DateUtils.formatDateIsoEndOfDay(new Date());

    const [collaboratorSectors, setCollaboratorsSectors] = useState<Sector[]>(
        []
    );

    const [collaboratorSectorsGroups, setCollaboratorsSectorsGroups] = useState<Sector[]>(
        []
    );

    const [collaboratorSectorsFiltered, setCollaboratorsSectorsFiltered] =
        useState<Sector[]>([]);

    useEffect(() => {
        let filteredResults;

        // Filtrar com base no `filterSelected` quando `selectedSectorsGroups` estiver vazio
        if (filterSelected && selectedSectorsGroups?.length === 0 && filterSelected?.length > 0) {
            filteredResults = collaboratorSectors.filter((item) =>
                filterSelected.find((f) => f.id === item.id)
            );
        } else if (selectedSectorsGroups && selectedSectorsGroups.length > 0) {
            // Agrupar setores com base no nome quando `selectedSectorsGroups` tiver valores
            filteredResults = collaboratorSectors.reduce((acc: Sector[], item) => {
                const group = selectedSectorsGroups.find((f) => f.name === item.name);
                let existingSector = acc.find((sector) => sector.name === item.name);

                if (group) {
                    // Verificar se já existe um setor agrupado com este nome

                    if (existingSector) {
                        // Adicionar o ID ao idsGroup do setor existente
                        existingSector.idsGroup?.push(item.id);
                    } else {
                        // Adicionar um novo setor com a propriedade idsGroup inicializada
                        acc.push({ ...item, idsGroup: [item.id] });
                    }
                }
                return acc;
            }, []);

            // Adicionar novo filtro ao resultado existente
            let filteredResults2: Sector[] = [];

            collaboratorSectors.forEach((item) => {
                const group = selectedSectorsGroups.find((f) => f.name === item.name);

                if (group) {
                    filteredResults2.push({ ...item, idsGroup: [item.id] });
                }
            });


            /*             let filteredResults2 = collaboratorSectors.reduce((acc: Sector[], item) => {
                            const group = selectedSectorsGroups.find((f) => f.name === item.name);
                            if (group) {
                                // Para cada ID, adicionar um novo item no resultado
                                group.idsGroup?.forEach((id) => {
                                    acc.push({
                                        ...item,
                                        id, // Adicionar cada ID como um novo item
                                    });
                                });
                            }
                            return acc;
                        }, []); */


            // Combinar os resultados existentes com os novos
            filteredResults = [
                ...filteredResults,
                ...filteredResults2
            ];
        } else {
            // Caso padrão: manter os setores sem agrupamento
            filteredResults = collaboratorSectors;
        }
        console.log(filteredResults);
        // Atualizar o estado com os resultados filtrados
        setCollaboratorsSectorsFiltered(filteredResults);
    }, [searchStartDate, searchEndDate, collaboratorSectors, filterSelected]);



    useEffect(() => {
        collaboratorSectors &&
            getSectorsList &&
            getSectorsList(collaboratorSectors);
    }, [collaboratorSectors]);

    useEffect(() => {
        collaboratorSectorsGroups &&
            getSectorsList2 &&
            getSectorsList2(collaboratorSectorsGroups);
    }, [collaboratorSectorsGroups]);

    function dayRules() {
        const startDate = new Date();
        startDate.setDate(startDate.getDate() - 1);

        return {
            startDate: DateUtils.formatDateIsoEndOfDay(startDate),
            endDate: todayEndOfDay,
        };
    }

    function weekRules() {
        const startDate = new Date();
        startDate.setDate(startDate.getDate() - 7);

        return {
            startDate: DateUtils.formatDateIsoEndOfDay(startDate),
            endDate: todayEndOfDay,
        };
    }

    function monthRules() {
        const today = new Date();
        const thisMonth = today.getMonth();
        const thisYear = today.getFullYear();
        const startDate = formatISO(
            new Date(startOfDay(new Date(thisYear, thisMonth, 1)))
        );

        return {
            startDate: startDate,
            endDate: todayEndOfDay,
        };
    }

    const dateFilterRules =
        dateFilter == "day"
            ? dayRules()
            : dateFilter == "week"
                ? weekRules()
                : monthRules();

    const getIndicatorsResults = async () => {
        if (!searchStartDate || !searchEndDate) {
            return toast.warning("Selecione as datas");
        }

        startLoading();

        const payload = {
            collaboratorId: userId,
            startDate: format(
                new Date(searchStartDate.toString()),
                "yyyy-MM-dd"
            ),
            endDate: format(new Date(searchEndDate.toString()), "yyyy-MM-dd"),
        };

        new SectorIndicatorsV2()
            .handle(payload)
            .then((data) => {
                if (sectorId) {
                    const filteredDataBySectorId = data?.filter(
                        (item: { id: number }) => item.id == sectorId
                    );

                    setCollaboratorsSectors(filteredDataBySectorId);
                } else {


                    data && data[0] && setCollaboratorsSectors(data);
                }
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });

        new SectorGroupsIndicatorsV2()
            .handle(payload)
            .then((data) => {



                data && data[0] && setCollaboratorsSectorsGroups(data);

            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    useEffect(() => {
        searchStartDate && searchEndDate && getIndicatorsResults();
    }, [searchStartDate, searchEndDate]);

    const SectorItem = (props: { sector: Sector; isDefaultOpen?: boolean }) => {
        const { sector, isDefaultOpen } = props;
        const theme = useTheme();

        const [loadingSubSectors, setLoadingSubSectors] =
            useState<boolean>(false);

        const [isOpen, setIsOpen] = useState<boolean>(
            isDefaultOpen ? true : false
        );

        const [isResultsOpen, setIsResultsOpen] = useState<boolean>(false);

        const [subSectors, setSubSectors] = useState<Sector[]>([]);

        async function getSubSectors() {
            if (!myUser || !searchStartDate || !searchEndDate) {
                return;
            }

            setLoadingSubSectors(true);

            await new SubSectorsBySectorUseCase()
                .handle({
                    dtInicial: format(
                        new Date(searchStartDate.toString()),
                        "yyyy-MM-dd"
                    ),
                    dtFinal: format(
                        new Date(searchEndDate.toString()),
                        "yyyy-MM-dd"
                    ),
                    sector: sector.id,
                    sectorIds: sector.idsGroup,
                })
                .then((data) => {
                    setSubSectors(data);
                })
                .catch((e) => { })
                .finally(() => setLoadingSubSectors(false));
        }

        useEffect(() => {
            isOpen && getSubSectors();
        }, [isOpen]);

        return (
            <Box
                display={"flex"}
                flexDirection={"column"}
                border={`solid 1px ${grey[100]}`}
                borderRadius={"8px"}
                p={"26px 24px"}
                bgcolor={"#fff"}
                gap={"18px"}
            >
                {isResultsOpen && (
                    <BaseModal
                        sx={{ width: "100%" }}
                        open={isOpen}
                        title={`Resultados`}
                        fullWidth
                        onClose={() => setIsResultsOpen(false)}
                    >
                        <Box
                            width={"100%"}
                            display={"flex"}
                            flexDirection={"column"}
                            gap={"20px"}
                        >
                            <ResultsDetails
                                searchEndDate={searchEndDate}
                                hierarchyId={hierarchyId}
                                searchStartDate={searchStartDate}
                                period={period}
                                homeFloor={homeFloor}
                                site={site}
                                sector={sector}
                                userId={userId}
                            />
                        </Box>
                    </BaseModal>
                )}
                <Box
                    onClick={() => setIsOpen(!isOpen)}
                    sx={{ cursor: "pointer" }}
                    display={"flex"}
                    justifyContent={"space-between"}
                >
                    <Box display={"flex"} gap={"10px"} alignItems={"center"}>
                        {sector.name.split(" ")[0] == "SKY" && (
                            <Image src={skyLogo} width={30} alt={"sky"} />
                        )}
                        <Typography
                            fontWeight={"500"}
                            fontSize={"16px"}
                            lineHeight={"15px"}
                        >
                            {formatStringFirstLetterUppercase(sector.name)}
                        </Typography>
                        <Typography
                            fontWeight={"500"}
                            fontSize={"16px"}
                            lineHeight={"15px"}
                        >
                            -
                        </Typography>

                        {sector.idsGroup && sector.idsGroup?.length > 0 ? (
                            <Typography fontWeight={"500"} fontSize={"16px"} lineHeight={"15px"}>
                                {sector.idsGroup.join(", ")}
                            </Typography>
                        ) : (
                            <Typography fontWeight={"500"} fontSize={"16px"} lineHeight={"15px"}>
                                {sector.id}
                            </Typography>
                        )}




                    </Box>
                    <KeyboardArrowDown />
                </Box>
                {isOpen && isOtherOperators && (
                    <Button
                        fullWidth
                        variant="contained"
                        onClick={() => setIsResultsOpen(true)}
                    >
                        Ver resultados
                    </Button>
                )}
                {isOpen && !isOtherOperators && (
                    <ResultsDetails
                        searchEndDate={searchEndDate}
                        hierarchyId={hierarchyId}
                        searchStartDate={searchStartDate}
                        period={period}
                        homeFloor={homeFloor}
                        site={site}
                        sector={sector}
                        userId={userId}
                    />
                )}
                {isOpen && (
                    <SubSectorsList
                        searchEndDate={searchEndDate}
                        isOtherOperators={isOtherOperators}
                        searchStartDate={searchStartDate}
                        period={period}
                        hierarchyId={hierarchyId}
                        homeFloor={homeFloor}
                        site={site}
                        sector={sector}
                        userId={userId}
                        isLoading={loadingSubSectors}
                        subSectors={subSectors}
                    />
                )}
                {isOpen && hierarchyId != 1 && (
                    <OtherOperatorsResults
                        userId={userId}
                        searchStartDate={searchStartDate}
                        searchEndDate={searchEndDate}
                        period={period}
                        homeFloor={homeFloor}
                        site={site}
                        sectorId={sector.id}
                        sectorIds={sector.idsGroup}
                    />
                )}
            </Box>
        );
    };

    return (
        <Box display={"flex"} flexDirection={"column"} gap={"10px"}>
            {isLoading ? (
                <CircularProgress />
            ) : (
                collaboratorSectorsFiltered.map((item, index) => (
                    <SectorItem
                        sector={item}
                        key={index}
                        isDefaultOpen={false}
                    />
                ))
            )}
        </Box>
    );
}
