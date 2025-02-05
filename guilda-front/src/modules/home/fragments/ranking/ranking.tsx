import Group from "@mui/icons-material/Group";
import Star from "@mui/icons-material/Star";
import {
  Box,
  Button,
  ButtonGroup,
  CardMedia,
  Stack,
  Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { useContext, useEffect, useState } from "react";
import { Card, PageHeader } from "src/components";
import { GetRanking } from "../../use-cases/get-ranking/get-ranking";
import { useLoadingState } from "src/hooks";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { DateUtils } from "src/utils";
import { ListGroupsUseCase } from "src/modules/groups";
import { formatISO, startOfDay } from "date-fns";
import { truncateDecimals } from "src/utils/truncate-decimals";
import { Rank, RankConsolidated } from "src/typings/models/rank.model";
import { UserInfoContext } from "src/contexts/user-context/user.context";

const GroupCard = ({
  value,
  groupName,
  groupColor,
  isLoading,
  image,
}: {
  value: number;
  groupName: string;
  groupColor: string;
  isLoading: boolean;
  image: string;
}) => {
  return (
    <Box
      sx={{
        border: `solid 1px ${grey[200]}`,
        width: "100%",
        borderRadius: 2,
        p: "16px",
        display: "flex",
        flexDirection: "row",
        alignItems: "center",
        gap: "20px",
      }}
    >
      <CardMedia
        component="img"
        image={image}
        sx={{
          width: "40px",
          objectFit: "contain",
        }}
      />
      <Box>
        <Typography
          fontSize={"32px"}
          fontWeight={"600"}
          lineHeight={"40px"}
          color={"#595A6D"}
        >
          {isLoading ? "--" : `${value?.toFixed(1)}%`}
        </Typography>
        <Typography fontWeight={"400"} fontSize={"14px"} color={grey[500]}>
          {groupName}
        </Typography>
      </Box>
    </Box>
  );
};

interface RankingProps {
  rankingResults?: Rank[];
  consolidatedRank?: RankConsolidated[];
}

export default function Ranking(props: RankingProps) {
  const { rankingResults, consolidatedRank } = props;
  const { myUser } = useContext(UserInfoContext);
  const { finishLoading, isLoading, startLoading } = useLoadingState();

  // const ListRank = async () => {
  //     startLoading();

  //     const payload = {
  //         collaboratorId: collaboratorId,
  //         startDate: startDate,
  //         endDate: endDate,
  //         sectorId: sectorId,
  //     };

  //     new GetRanking()
  //         .handle(payload)
  //         .then((data) => {
  //             console.log("RANKING", data);
  //             setRankingResults(data);
  //         })
  //         .catch(() => {
  //             toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
  //         })
  //         .finally(() => {
  //             finishLoading();
  //         });
  // };

  // useEffect(() => {
  //     myUser && ListRank();
  // }, [myUser]);

  return (
    <Box
      width={"100%"}
      display={"flex"}
      flexDirection={"column"}
      justifyContent={"space-between"}
    >
      <Box width={"100%"} gap={2}>
        <Box flexDirection={"row"} display={"flex"} gap={"10px"} px={0} mx={0}>
          {rankingResults?.map((item, index) => (
            <GroupCard
              key={index}
              groupName={item?.group?.alias || "--"}
              value={item?.percent || 0}
              groupColor="primary.main"
              isLoading={isLoading}
              image={item?.group?.image?.url}
            />
          ))}
          {consolidatedRank?.map((item, index) => (
            <GroupCard
              key={index}
              groupName={item.ALIAS}
              value={
                item?.PERCENT && !isNaN(Number(item?.PERCENT))
                  ? item?.PERCENT
                  : 0
              }
              groupColor="primary.main"
              isLoading={isLoading}
              image={item?.IMAGEMGROUP}
            />
          ))}
        </Box>
      </Box>
    </Box>
  );
}
