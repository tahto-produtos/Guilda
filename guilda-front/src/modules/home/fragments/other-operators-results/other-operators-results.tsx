import ArrowForwardIos from "@mui/icons-material/ArrowForwardIos";
import InsertChart from "@mui/icons-material/InsertChart";
import PeopleAlt from "@mui/icons-material/PeopleAlt";
import {
  Box,
  Button,
  ButtonGroup,
  CircularProgress,
  LinearProgress,
  Stack,
  Typography,
  useTheme,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import {
  Dispatch,
  SetStateAction,
  useContext,
  useEffect,
  useState,
} from "react";
import { Card, PageHeader } from "src/components";
import { useLoadingState } from "src/hooks";
import { ListIndicatorsResults } from "../../use-cases/list-indicators-results/list-indicators-results";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { DateUtils } from "src/utils";
import { CollaboratorHierarchy } from "src/modules/collaborators/use-cases/collaborator-hierarchy";
import IndicatorsResults from "../indicators-results/indicators-results";
import IndicatorsSectorsResults from "../indicators-sectors-results/indicators-sectors-results";
import { format } from "date-fns";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { HomeFloor, Period } from "src/typings";
import { Site } from "src/typings/models/site.model";

interface CollaboratorItemProps {
  collaboratorName: string;
  collaboratorId: number;
  selectedOperator: number | null;
  setSelectedOperator: Dispatch<SetStateAction<number | null>>;
  searchStartDate: dateFns | Date | null;
  searchEndDate: dateFns | Date | null;
  period: Period[];
  homeFloor: HomeFloor[];
  site: Site[];
  sectorId: number;
  hierarchyId: number;
}

const CollaboratorItem = (props: CollaboratorItemProps) => {
  const {
    collaboratorId,
    collaboratorName,
    setSelectedOperator,
    selectedOperator,
    searchStartDate,
    sectorId,
    searchEndDate,
    period,
    homeFloor,
    site,
    hierarchyId,
  } = props;
  const [isHover, setIsHover] = useState<boolean>(false);
  const theme = useTheme();

  const isActive = selectedOperator === collaboratorId;


  function handleExpand() {
    return isActive
      ? setSelectedOperator(null)
      : setSelectedOperator(collaboratorId);
  }

  return (
    <Box
      display={"flex"}
      flexDirection={"column"}
      gap={1}
      p={"10px"}
      border={`solid 1px ${grey[100]}`}
      borderRadius={2}
    >
      <Button
        onMouseEnter={() => setIsHover(true)}
        onMouseLeave={() => setIsHover(false)}
        onClick={handleExpand}
      >
        <Box
          justifyContent={"space-between"}
          display={"flex"}
          flexDirection={"row"}
          alignItems={"center"}
          width={"100%"}
          sx={{ cursor: "pointer" }}
        >
          <Typography
            fontWeight={"400"}
            fontSize={"14px"}
            variant="body2"
            sx={{
              color: isHover || isActive ? theme.palette.primary.main : "#000",
            }}
          >
            {collaboratorName}
          </Typography>
          <ArrowForwardIos sx={{ fontSize: "16px", color: "#000" }} />
        </Box>
      </Button>
      {isActive && (
        <Card width={"100%"}>
          <IndicatorsSectorsResults
            userId={collaboratorId}
            hierarchyId={hierarchyId}
            hideIndicatorBasket={true}
            searchStartDate={searchStartDate}
            searchEndDate={searchEndDate}
            sectorId={sectorId}
            period={period}
            homeFloor={homeFloor}
            site={site}
            isOtherOperators={true}
          />
        </Card>
      )}
    </Box>
  );
};

interface IndicatorResultModel {
  indicator: {
    name: string;
  };
  result: number;
  goal: number;
}

interface OtherCollaboratorsModel {
  children: any;
  key: number;
  value: {
    id: number;
    name: string;
    hierarchyId: number;
  };
}

interface OtherOperatorsResultsProps {
  userId: number;
  searchStartDate: dateFns | Date | null;
  searchEndDate: dateFns | Date | null;
  sectorId: number;
  period: Period[];
  homeFloor: HomeFloor[];
  site: Site[];
  sectorIds?: number[];
}

export default function OtherOperatorsResults(
  props: OtherOperatorsResultsProps
) {
  const [indicatorsResults, setIndicatorsResults] = useState<
    IndicatorResultModel[]
  >([]);
  const { userId, searchEndDate, searchStartDate, sectorId, period, homeFloor, site, sectorIds } = props;
  const { myUser } = useContext(UserInfoContext);
  const { finishLoading, isLoading, startLoading } = useLoadingState();

  const [dateFilter, setDateFilter] = useState<"day" | "week" | "month">(
    "month"
  );
  const [othersCollaboratorsList, setOthersCollaboratorsList] = useState<
    OtherCollaboratorsModel[]
  >([]);
  const [selectedOperator, setSelectedOperator] = useState<number | null>(null);

  const todayEndOfDay = DateUtils.formatDateIsoEndOfDay(new Date());

  const getCollaboratorHierarchy = async () => {
    startLoading();

    if (!searchStartDate || !searchEndDate) {
      return;
    }

    const payload = {
      userId: userId as number,
      startDate: format(new Date(searchStartDate.toString()), "yyyy-MM-dd"),
      endDate: format(new Date(searchEndDate.toString()), "yyyy-MM-dd"),
      sectorId: sectorId,
      period: period,
      homeFloor: homeFloor,
      site: site,
      sectorIds: sectorIds,
    };

    new CollaboratorHierarchy()
      .handle(payload)
      .then((data) => {
        setOthersCollaboratorsList(data.tree.children);
      })
      .catch(() => {
        toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    userId && getCollaboratorHierarchy();
  }, [userId]);

console.log(userId);

  return (
    <Card
      width={"100%"}
      display={"flex"}
      flexDirection={"column"}
      justifyContent={"space-between"}
    >
      <PageHeader
        title="Resultado de outros operadores"
        headerIcon={<PeopleAlt />}
      />
      <Stack px={2} py={3} width={"100%"} gap={2}>
        {isLoading && <CircularProgress />}
        {!isLoading && othersCollaboratorsList.length > 0 ? (
          othersCollaboratorsList.map((item, index) => {
            console.log(item);
            return (
            <CollaboratorItem
              collaboratorName={item.value.name}
              collaboratorId={item.value.id}
              selectedOperator={selectedOperator}
              setSelectedOperator={setSelectedOperator}
              key={index}
              searchStartDate={searchStartDate}
              searchEndDate={searchEndDate}
              period={period}
              homeFloor={homeFloor}
              site={site}
              sectorId={sectorId}
              hierarchyId={item.value.hierarchyId}
            />
          )})
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
              Não foi encontrado outros operadores
            </Typography>
          </Box>
        )}
      </Stack>
    </Card>
  );
}
