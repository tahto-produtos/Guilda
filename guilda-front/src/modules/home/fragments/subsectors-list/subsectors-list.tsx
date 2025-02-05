import {
  Box,
  Button,
  CircularProgress,
  Stack,
  Typography,
} from "@mui/material";
import List from "@mui/material/List";
import { grey } from "@mui/material/colors";
import { useState } from "react";
import { Card, PageHeader } from "src/components";
import { BaseModal } from "src/components/feedback";
import { HomeFloor, Sector, Period } from "src/typings";
import { ResultsDetails } from "../results-details/results-details";
import { formatStringFirstLetterUppercase } from "src/utils/format-string-first-letter-uppercase";
import KeyboardArrowDown from "@mui/icons-material/KeyboardArrowDown";
import { Site } from "src/typings/models/site.model";

interface IProps {
  isLoading: boolean;
  subSectors: Sector[];
  searchEndDate: Date | dateFns | null;
  searchStartDate: Date | dateFns | null;
  period: Period[];
  homeFloor: HomeFloor[];
  site: Site[];
  sector: Sector;
  userId: number;
  isOtherOperators?: boolean;
  hierarchyId?: number;
}

interface SubSectorItemProps {
  subsectorName: string;
  subSectorId: number;
}

export function SubSectorsList(props: IProps) {
  const {
    isLoading,
    subSectors,
    searchEndDate,
    searchStartDate,
    sector,
    period,
    homeFloor,
    site,
    isOtherOperators,
    userId,
    hierarchyId,
  } = props;

  function SubSectorItem(props: SubSectorItemProps) {
    const { subsectorName, subSectorId } = props;
    const [isResultsOpen, setIsResultsOpen] = useState<boolean>(false);
    const [isOpen, setIsOpen] = useState<boolean>(false);

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
                searchStartDate={searchStartDate}
                sector={sector}
                hierarchyId={hierarchyId}
                period={period}
                homeFloor={homeFloor}
                site={site}
                userId={userId}
                subSectorId={subSectorId}
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
            <Typography
              fontWeight={"500"}
              fontSize={"16px"}
              lineHeight={"15px"}
            >
              {formatStringFirstLetterUppercase(subsectorName)}
            </Typography>
            <Typography
              fontWeight={"500"}
              fontSize={"16px"}
              lineHeight={"15px"}
            >
              -
            </Typography>
            <Typography
              fontWeight={"500"}
              fontSize={"16px"}
              lineHeight={"15px"}
            >
              {subSectorId}
            </Typography>
          </Box>
          <KeyboardArrowDown />
        </Box>
        {isOpen && (
          <Button
            fullWidth
            variant="contained"
            onClick={() => setIsResultsOpen(true)}
          >
            Ver resultados
          </Button>
        )}
        {/* {isOpen && !isOtherOperators && (
          <ResultsDetails
            searchEndDate={searchEndDate}
            searchStartDate={searchStartDate}
            sector={sector}
            userId={userId}
            subSectorId={subSectorId}
          />
        )} */}
      </Box>
    );
  }

  return (
    <>
      {subSectors.length > 0 && (
        <Card
          width={"100%"}
          display={"flex"}
          flexDirection={"column"}
          justifyContent={"space-between"}
        >
          <PageHeader title="Sub Setores" headerIcon={<List />} />
          <Stack px={2} py={3} width={"100%"} gap={2}>
            {isLoading && <CircularProgress />}
            {!isLoading && subSectors.length > 0 ? (
              subSectors.map((item, index) => (
                <SubSectorItem
                  key={index}
                  subsectorName={item.name}
                  subSectorId={item.id}
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
                  Este setor não contém subsetores
                </Typography>
              </Box>
            )}
          </Stack>
        </Card>
      )}
    </>
  );
}
