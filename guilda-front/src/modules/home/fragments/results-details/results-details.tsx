import {
  Box,
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
import Ranking from "../ranking/ranking";
import MonetizationOn from "@mui/icons-material/MonetizationOn";
import { useContext, useEffect, useState } from "react";
import { grey } from "@mui/material/colors";
import { Rank, RankConsolidated } from "src/typings/models/rank.model";
import { Sector, Period, HomeFloor } from "src/typings";
import { toast } from "react-toastify";
import { format } from "date-fns";
import { ResultsIndicatorsSectors } from "../../use-cases/results-indicators-sectors";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import MoneyOff from "@mui/icons-material/MoneyOff";
import DoNotDisturb from "@mui/icons-material/DoNotDisturb";
import { ResultProgress } from "src/components/data-display/result-progress/result-progress";
import { IndicatorBasketCard } from "../indicator-basket/indicator-basket";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { ResultConsolidatedUseCase } from "../../use-cases/ResultConsolidated";
import { ConsolidatedDataModel } from "../consolidated-result/consolidated-result";
import { Site } from "src/typings/models/site.model";

interface ResultDeitalsProps {
  searchStartDate: Date | dateFns | null;
  searchEndDate: Date | dateFns | null;
  period: Period[];
  homeFloor: HomeFloor[];
  site: Site[];
  sector: Sector;
  subSectorId?: number;
  userId: number;
  hierarchyId?: number;
}

export interface IndicatorResultModel {
  monetizationCollaborator: number | undefined;
  indicator: {
    type: string;
    name: string;
  };
  sector: Sector;
  result: number;
  goal: number;
  group: {
    name: string;
    alias: string;
    id: number;
    image: { url: string };
  };
  monetization: boolean;
  maxMonetization: number;
}

export function ResultsDetails(props: ResultDeitalsProps) {
  const {
    searchEndDate,
    searchStartDate,
    period,
    homeFloor,
    site,
    sector,
    userId,
    subSectorId,
    hierarchyId,
  } = props;
  const [loadingResults, setLoadingResults] = useState<boolean>(false);
  const theme = useTheme();
  const { myUser } = useContext(UserInfoContext);

  const [indicatorsResults, setIndicatorsResults] = useState<
    ConsolidatedDataModel[]
  >([]);

  const [rankingResults, setRankingResults] = useState<RankConsolidated[]>([]);

  const filteredResults = indicatorsResults;

  useEffect(() => {
    getIndicatorsResults();
  }, [searchStartDate, searchEndDate]);

  const getIndicatorsResults = async () => {
    if (!myUser) return;

    if (!searchStartDate || !searchEndDate) {
      return toast.warning("Selecione as datas");
    }

    setLoadingResults(true);

    const payload = {
      collaboratorId: userId,
      startDate: format(new Date(searchStartDate.toString()), "yyyy-MM-dd"),
      endDate: format(new Date(searchEndDate.toString()), "yyyy-MM-dd"),
      sectorId: sector.id,
    };

    // new ResultsIndicatorsSectors()
    //   .handle(payload)
    //   .then((data) => {
    //     setRankingResults(data.ranking);
    //     setIndicatorsResults(data.data);
    //   })
    //   .catch(() => {
    //     toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
    //   })
    //   .finally(() => {
    //     setLoadingResults(false);
    //   });

    new ResultConsolidatedUseCase()
      .handle({
        Hierarchies: hierarchyId ? hierarchyId : myUser.hierarchyId,
        codCollaborator: userId,
        dataFinal: format(new Date(searchEndDate.toString()), "yyyy-MM-dd"),
        dataInicial: format(new Date(searchStartDate.toString()), "yyyy-MM-dd"),
        sectors: [{ id: sector.id }],
        periods: period.map((item) => {
          return { id: item.id };
        }),
        homeFloors: homeFloor.map((item) => {
          return { id: item.id };
        }),
        sites: site.map((item) => {
          return { id: item.id };
        }),
        sectorsIds: sector.idsGroup?.map((item) => {
          return { id: item };
        }),
        basket: true,
        subSectors: subSectorId ? [{ id: subSectorId }] : [],
      })
      .then((data) => {
        setRankingResults(data.RANKING);
        setIndicatorsResults(data.RESULTS);
      })
      .catch((e) => {
        const msg = e?.response?.data?.Message;

        if (msg) {
          return toast.error(msg);
        }
        toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
      })
      .finally(() => {
        setLoadingResults(false);
      });
  };

  interface ResultProgressProps {
    indicatorName: string;
    progress: number;
    goal: number;
    type: string;
    groupAlias: string | null;
    groupName: string | null;
    isMonetized: boolean;
    groupId?: number | null;
    groupImage?: string | null;
    maxMonetization: number;
  }

  const indicatorBasket =
    filteredResults.find((item) => item.INDICADOR == "Cesta de Indicadores") ||
    null;

  const indicatorNoBasket = indicatorsResults.filter((item) => item.INDICADOR !== "Cesta de Indicadores");

  return (
    <>
      <Divider />
      {myUser /*&& myUser.hierarchy == "AGENTE"*/ && indicatorBasket && (
        <IndicatorBasketCard indicatorBasket={indicatorBasket} />
      )}
      {searchStartDate && searchEndDate && (
        <Ranking consolidatedRank={rankingResults} />
      )}
      {/* filteredResults &&
                !loadingResults &&
                myUser &&
                myUser.profileCollaboratorAdministrationId !== 19 && (
                    <Box
                        sx={{
                            backgroundColor: lighten(
                                theme.palette.primary.main,
                                0.8
                            ),
                            border: `solid 1px ${theme.palette.primary.main}`,
                        }}
                        p={2}
                        gap={2}
                        display={"flex"}
                        alignItems={"center"}
                    >
                        <MonetizationOn
                            sx={{
                                color: darken(theme.palette.primary.main, 0.2),
                            }}
                        />
                        <Typography
                            variant="body2"
                            color={darken(theme.palette.primary.main, 0.4)}
                            fontWeight={"500"}
                        >
                            No período aplicado no filtro, você pode ganhar até{" "}
                            {filteredResults
                                .map((item) => {
                                    return item.maxMonetization || 0;
                                })
                                .reduce((a, n) => a + n, 0)}{" "}
                            moedas no total.
                        </Typography>
                    </Box>
                ) */}
      <Box
        display={"flex"}
        flexDirection={"row"}
        flexWrap={"wrap"}
        gap={"20px"}
      >
        <Stack
          sx={{
            width: "100%",
            paddingY: "20px",
            background: grey[100],
          }}
          direction={"row"}
          gap={1}
        >
          <MonetizationOn sx={{ color: theme.palette.text.secondary }} />
          <Typography
            variant="body1"
            sx={{ fontSize: "16px", fontWeight: 500 }}
          >
            Indicadores monetizados
          </Typography>
        </Stack>
        {loadingResults ? (
          <Stack
            direction={"row"}
            justifyContent={"center"}
            alignItems={"center"}
            width={"100%"}
          >
            <CircularProgress />
          </Stack>
        ) : indicatorNoBasket.length > 0 ? (
          indicatorNoBasket
            .filter((data) => data.MONETIZATION)
            .map((item, index) => (
              <ResultProgress
                indicatorName={item.INDICADOR}
                progress={item.RESULTADO}
                progress_hour={item.RESULTADO_HORA}
                goal={item.META}
                goal_hour={item.META_HORA}
                key={index}
                groupAlias={item?.GRUPO || null}
                groupName={item?.GRUPO || null}
                type={item.TYPE}
                isMonetized={item.MONETIZATION}
                groupId={item?.IDGRUPO || null}
                groupImage={item?.IMAGEMGRUPO || null}
                maxMonetization={item.META_MAXIMA_MOEDAS}
                monetizationCollaborator={item.MOEDA_GANHA}
              />
            ))
        ) : (
          <Box
            width={"100%"}
            display={"flex"}
            flexDirection={"row"}
            justifyContent={"center"}
            alignItems={"center"}
            p={4}
          >
            <Typography color={grey[400]}>
              Não foi encontrado resultados
            </Typography>
          </Box>
        )}
        <Stack
          sx={{
            width: "100%",
            paddingY: "20px",
            background: grey[100],
          }}
          direction={"row"}
          gap={1}
        >
          <MoneyOff sx={{ color: theme.palette.text.secondary }} />
          <Typography
            variant="body1"
            sx={{ fontSize: "16px", fontWeight: 500 }}
          >
            Indicadores não monetizados
          </Typography>
        </Stack>
        {loadingResults ? (
          <Stack
            direction={"row"}
            justifyContent={"center"}
            alignItems={"center"}
            width={"100%"}
          >
            <CircularProgress />
          </Stack>
        ) : indicatorNoBasket.length > 0 ? (
          indicatorNoBasket
            .filter((data) => !data.MONETIZATION && data.META !== null)
            .map((item, index) => (
              <ResultProgress
                indicatorName={item.INDICADOR}
                progress={item.RESULTADO}
                progress_hour={item.RESULTADO_HORA}
                goal={item.META}
                goal_hour={item.META_HORA}
                key={index}
                groupAlias={item?.GRUPO || null}
                groupName={item?.GRUPO || null}
                type={item.TYPE}
                isMonetized={item.MONETIZATION}
                groupId={item?.IDGRUPO || null}
                groupImage={item?.IMAGEMGRUPO || null}
                maxMonetization={item.META_MAXIMA_MOEDAS}
                monetizationCollaborator={item.MOEDA_GANHA}
              />
            ))
        ) : (
          <Box
            width={"100%"}
            display={"flex"}
            flexDirection={"row"}
            justifyContent={"center"}
            alignItems={"center"}
            p={4}
          >
            <Typography color={grey[400]}>
              Não foi encontrado resultados
            </Typography>
          </Box>
        )}
        <Stack
          sx={{
            width: "100%",
            paddingY: "20px",
            background: grey[100],
          }}
          direction={"row"}
          gap={1}
        >
          <DoNotDisturb sx={{ color: theme.palette.text.secondary }} />
          <Typography
            variant="body1"
            sx={{ fontSize: "16px", fontWeight: 500 }}
          >
            Indicadores sem meta
          </Typography>
        </Stack>
        {loadingResults ? (
          <Stack
            direction={"row"}
            justifyContent={"center"}
            alignItems={"center"}
            width={"100%"}
          >
            <CircularProgress />
          </Stack>
        ) : indicatorNoBasket.length > 0 ? (
          indicatorNoBasket
            .filter((data) => !data.MONETIZATION && data.META == null)
            .map((item, index) => (
              <ResultProgress
                indicatorName={item.INDICADOR}
                progress={item.RESULTADO}
                progress_hour={item.RESULTADO_HORA}
                goal={item.META}
                goal_hour={item.META_HORA}
                key={index}
                groupAlias={item?.GRUPO || null}
                groupName={item?.GRUPO || null}
                type={item.TYPE}
                isMonetized={item.MONETIZATION}
                groupId={item?.IDGRUPO || null}
                groupImage={item?.IMAGEMGRUPO || null}
                maxMonetization={item.META_MAXIMA_MOEDAS}
                monetizationCollaborator={item.MOEDA_GANHA}
              />
            ))
        ) : (
          <Box
            width={"100%"}
            display={"flex"}
            flexDirection={"row"}
            justifyContent={"center"}
            alignItems={"center"}
            p={4}
          >
            <Typography color={grey[400]}>
              Não foi encontrado resultados
            </Typography>
          </Box>
        )}
        {/* {loadingResults ? (
                    <Stack
                        direction={"row"}
                        justifyContent={"center"}
                        alignItems={"center"}
                        width={"100%"}
                    >
                        <CircularProgress />
                    </Stack>
                ) : filteredResults.length > 0 ? (
                    filteredResults.map((item, index) => (
                        <ResultProgress
                            indicatorName={item.indicator.name}
                            progress={item.result}
                            goal={item.goal}
                            key={index}
                            groupAlias={item?.group?.alias || null}
                            groupName={item?.group?.name || null}
                            type={item.indicator.type}
                            isMonetized={item.monetization}
                            groupId={item?.group?.id || null}
                            groupImage={item?.group?.image?.url || null}
                            maxMonetization={item.maxMonetization}
                        />
                    ))
                ) : (
                    <Box
                        width={"100%"}
                        display={"flex"}
                        flexDirection={"row"}
                        justifyContent={"center"}
                        alignItems={"center"}
                        p={4}
                    >
                        <Typography color={grey[400]}>
                            Não foi encontrado resultados
                        </Typography>
                    </Box>
                )} */}
        {/* {!loadingResults && indicatorBasket && (
                    <ResultProgress
                        indicatorName={indicatorBasket.indicator.name}
                        progress={indicatorBasket.result}
                        goal={indicatorBasket.goal}
                        type={indicatorBasket.indicator.type}
                        groupAlias={indicatorBasket?.group?.alias || null}
                        groupName={indicatorBasket?.group?.name || null}
                        isMonetized={indicatorBasket.monetization}
                        groupImage={indicatorBasket?.group?.image?.url || null}
                        maxMonetization={indicatorBasket.maxMonetization}
                    />
                )} */}
      </Box>
    </>
  );
}
