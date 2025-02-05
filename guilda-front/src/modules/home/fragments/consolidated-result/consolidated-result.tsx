import {
  Box,
  Button,
  CardMedia,
  CircularProgress,
  Stack,
  Tooltip,
  Typography,
  useTheme,
} from "@mui/material";
import { ResultConsolidatedUseCase } from "../../use-cases/ResultConsolidated";
import { useContext, useEffect, useState } from "react";
import { format } from "date-fns";
import { grey } from "@mui/material/colors";
import MonetizationOn from "@mui/icons-material/MonetizationOn";
import { Sector, Period, HomeFloor } from "src/typings";
import { Rank, RankConsolidated } from "src/typings/models/rank.model";
import Ranking from "../ranking/ranking";
import DoNotDisturb from "@mui/icons-material/DoNotDisturb";
import MoneyOff from "@mui/icons-material/MoneyOff";
import { ResultProgress } from "src/components/data-display/result-progress/result-progress";
import { IndicatorBasketCard } from "../indicator-basket/indicator-basket";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { SubSectorsBySectorUseCase } from "../../use-cases/subsectors-by-sector.use-case";
import { Site } from "src/typings/models/site.model";

interface IProps {
  startDate: dateFns | Date | null;
  endDate: dateFns | Date | null;
  period: Period[];
  homeFloor: HomeFloor[];
  site: Site[];
  selectedSectors: Sector[];
  selectedSectorsGroups: Sector[];
  hierarchyId: number | null;
}

export interface ConsolidatedDataModel {
  MATRICULA: string;
  CARGO: string;
  IDINDICADOR: string;
  INDICADOR: string;
  META: number;
  META_HORA: string;
  RESULTADO: number;
  RESULTADO_HORA: string;
  PERCENTUAL: number;
  META_MAXIMA_MOEDAS: number;
  MOEDA_GANHA: number;
  GRUPO: string;
  CODGIP: string;
  SETOR: string;
  IMAGEMGRUPO: string;
  TYPE: string;
  IDGRUPO: number;
  MONETIZATION: boolean;
}

export function ConsolidatedResult(props: IProps) {
  const theme = useTheme();

  const { endDate, startDate, selectedSectors, selectedSectorsGroups, period, homeFloor, site } = props;
  const [consolidatedData, setConsolidatedData] = useState<
    ConsolidatedDataModel[]
  >([]);
  const [consolidatedDataRank, setConsolidatedDataRank] = useState<
    RankConsolidated[]
  >([]);
  const [isOpen, setIsOpen] = useState<boolean>(false);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const { myUser } = useContext(UserInfoContext);
  const [error, setError] = useState<string>("");

  async function getConsolidatedResults() {
    if (!myUser || !startDate || !endDate) {
      return;
    }

    setConsolidatedData([]);
    setError("");
    setIsLoading(true);

    await new ResultConsolidatedUseCase()
      .handle({
        codCollaborator: myUser.id,
        Hierarchies:  myUser.hierarchyId,
        dataInicial: format(new Date(startDate.toString()), "yyyy-MM-dd"),
        dataFinal: format(new Date(endDate.toString()), "yyyy-MM-dd"),
        sectors: selectedSectors.map((item) => {
          return { id: item.id };
        }),
        sectorsGroups: selectedSectorsGroups.map((item) => {
          return { name: item.name };
        }),
        periods: period.map((item) => {
          return { id: item.id };
        }),
        homeFloors: homeFloor.map((item) => {
          return { id: item.id };
        }),
        sites: site.map((item) => {
          return { id: item.id };
        }),
        basket: false,
        subSectors: [],
      })
      .then((data) => {
        setConsolidatedData(data.RESULTS);
        setConsolidatedDataRank(data.RANKING);
      })
      .catch((e) => {
        const msg = e?.response?.data?.Message;
        msg && setError(msg);
      })
      .finally(() => setIsLoading(false));
  }

  useEffect(() => {
    endDate && startDate && isOpen && getConsolidatedResults();
  }, [endDate, startDate,  isOpen, selectedSectors, period, homeFloor, site]);

  const indicatorBasket =
  consolidatedData.find((item) => item.INDICADOR == "Cesta de Indicadores") ||
    null;

    const indicatorNoBasket = consolidatedData.filter((item) => item.INDICADOR !== "Cesta de Indicadores");

  return (
    <Box
      display={"flex"}
      justifyContent={"space-between"}
      gap={"20px"}
      alignItems={"center"}
      borderRadius={"8px"}
      p={"24px"}
      bgcolor={"#fff"}
    >
      <Box
        display={"flex"}
        flexDirection={"column"}
        gap={"25px"}
        justifyContent={"center"}
        width={"100%"}
      >
        <Box
          display={"flex"}
          justifyContent={"space-between"}
          alignItems={"center"}
          width={"100%"}
        >
          <Typography fontSize={"16px"} fontWeight={"600"}>
            Resultado consolidado
          </Typography>
          <Button
            variant="outlined"
            onClick={() => setIsOpen(!isOpen)}
            color={isOpen ? "error" : "primary"}
          >
            {isOpen ? "Fechar" : "Ver detalhes"}
          </Button>
        </Box>
        {isOpen && (
          <>
            <IndicatorBasketCard indicatorBasket={indicatorBasket} />
          </>
        )}
        {consolidatedDataRank?.length > 0 &&
          !isLoading &&
          isOpen &&
          indicatorNoBasket && (
            <Ranking consolidatedRank={consolidatedDataRank} />
          )}
        {isOpen && (
          <Box
            display={"flex"}
            flexDirection={"row"}
            flexWrap={"wrap"}
            gap={"20px"}
          >
            {!isLoading && error && (
              <Typography variant="body2" color="error">
                {error}
              </Typography>
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
              <MonetizationOn sx={{ color: theme.palette.text.secondary }} />
              <Typography
                variant="body1"
                sx={{ fontSize: "16px", fontWeight: 500 }}
              >
                Indicadores monetizados
              </Typography>
            </Stack>
            {isLoading ? (
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
                    groupAlias={item.GRUPO || null}
                    groupName={item.GRUPO || null}
                    type={item.TYPE}
                    isMonetized={item.MONETIZATION}
                    groupId={item.IDGRUPO}
                    groupImage={item.IMAGEMGRUPO}
                    maxMonetization={item.META_MAXIMA_MOEDAS}
                    monetizationCollaborator={item.MOEDA_GANHA}
                  />
                  //     <ResultProgress
                  //     indicatorName={item.indicator.name}
                  //     progress={item.result}
                  //     goal={item.goal}
                  //     key={index}
                  //     groupAlias={item?.group?.alias || null}
                  //     groupName={item?.group?.name || null}
                  //     type={item.indicator.type}
                  //     isMonetized={item.monetization}
                  //     groupId={item?.group?.id || null}
                  //     groupImage={item?.group?.image?.url || null}
                  //     maxMonetization={item.maxMonetization}
                  // />
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
            {isLoading ? (
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
                    groupAlias={item.GRUPO || null}
                    groupName={item.GRUPO || null}
                    type={item.TYPE}
                    isMonetized={item.MONETIZATION}
                    groupId={item.IDGRUPO}
                    groupImage={item.IMAGEMGRUPO}
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
            {isLoading ? (
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
                    groupAlias={item.GRUPO || null}
                    groupName={item.GRUPO || null}
                    type={item.TYPE}
                    isMonetized={item.MONETIZATION}
                    groupId={item.IDGRUPO}
                    groupImage={item.IMAGEMGRUPO}
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
            {isLoading && <CircularProgress />}
            {!isLoading && indicatorNoBasket?.length <= 0 && !error && (
              <Typography variant="body2">
                Sem resultados encontrados.
              </Typography>
            )}
          </Box>
        )}
      </Box>
    </Box>
  );
}
