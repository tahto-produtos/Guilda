import {
  Box,
  CardMedia,
  CircularProgress,
  Stack,
  Typography,
  useTheme,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { format } from "date-fns";
import { useContext, useEffect, useState } from "react";
import { BasketGeneralUser } from "src/modules/indicators/use-cases/basket-general-user.use-case";
import { IndicatorResultModel } from "../results-details/results-details";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { ConsolidatedDataModel } from "../consolidated-result/consolidated-result";

export function IndicatorBasketCard(props: {
  startDate?: dateFns | Date | null;
  endDate?: dateFns | Date | null;
  indicatorBasket?: ConsolidatedDataModel | null;
}) {
  const { myUser } = useContext(UserInfoContext);
  const theme = useTheme();
  const [basketData, setBasketData] = useState<any>(null);
  const { endDate, startDate, indicatorBasket } = props;

  async function getBasketGeneralUser() {
    if (!endDate || !startDate || !myUser) {
      return;
    }

    await new BasketGeneralUser()
      .handle({
        codCollaborator: myUser.id,
        dtinicial: format(new Date(startDate.toString()), "yyyy-MM-dd"),
        dtfinal: format(new Date(endDate.toString()), "yyyy-MM-dd"),
      })
      .then((data) => {
        setBasketData(data);
      })
      .catch(() => {});
  }

  useEffect(() => {
    myUser && endDate && startDate && getBasketGeneralUser();
  }, [myUser, endDate, startDate]);

  const progress = indicatorBasket
    ? indicatorBasket.RESULTADO
    : basketData?.coinsEarned;
  const goal = indicatorBasket
    ? indicatorBasket.META
    : basketData?.coinsPossible;
  const type: string = "INTEGER";
  const groupId = indicatorBasket
    ? indicatorBasket.IDGRUPO
    : basketData?.idGroup;
  const groupImage = indicatorBasket
    ? indicatorBasket.IMAGEMGRUPO
    : basketData?.groupImage;
  const groupAlias = indicatorBasket
    ? indicatorBasket.GRUPO
    : basketData?.groupAlias;

  const isCompleted = progress >= goal;
  const isPercent = type == "PERCENT" ? "%" : "";
  const toFixedNum = type == "INTEGER" ? 0 : 1;

  function getColorByGroupIdProgress(group: number) {
    if (group == 1) {
      return "#4d9be6";
    }
    if (group == 2) {
      return "#f9c22b";
    }
    if (group == 3) {
      return "#9babb2";
    }
    if (group == 4) {
      return "#ea4f36";
    }
    return "#000";
  }

  if (progress == null && !indicatorBasket) {
    return null;
  }

  return (
    <Box
      display={"flex"}
      justifyContent={"space-between"}
      gap={"20px"}
      alignItems={"center"}
      borderRadius={"8px"}
      p={"24px"}
      border={`solid 1px ${grey[200]}`}
    >
      <Box
        display={"flex"}
        flexDirection={"row"}
        gap={"15px"}
        alignItems={"center"}
      >
        <Typography fontSize={"16px"} fontWeight={"600"}>
          Cesta de indicadores
        </Typography>
        <Box
          display={"flex"}
          flexDirection={"row"}
          alignItems={"center"}
          gap={"10px"}
          border={"solid 1px #e1e1e1"}
          justifyContent={"center"}
          borderRadius={"200px"}
          px={"20px"}
          py={"5px"}
        >
          <Typography variant="body1" fontWeight={"600"}>
            {groupAlias}
          </Typography>
          <CardMedia
            component="img"
            image={groupImage}
            sx={{
              width: "30px",
              objectFit: "contain",
            }}
          />
        </Box>
      </Box>
      <Box
        position={"relative"}
        display={"flex"}
        justifyContent={"center"}
        alignItems={"center"}
      >
        <CircularProgress
          variant="determinate"
          value={100}
          sx={{
            borderRadius: "20px",
            color: grey[100],
            position: "absolute",
          }}
          size={"130px"}
        />
        <CircularProgress
          variant="determinate"
          value={isCompleted ? 100 : (progress / goal) * 100}
          sx={{
            borderRadius: "20px",
            color: groupId
              ? getColorByGroupIdProgress(groupId)
              : theme.palette.primary.main,
          }}
          size={"130px"}
        />
        <Stack
          position={"absolute"}
          direction={"column"}
          justifyContent={"center"}
          alignItems={"center"}
        >
          <Typography fontSize={"15px"} fontWeight={"700"}>
            <Typography
              fontSize={"13px"}
              component={"span"}
              sx={{ color: "#999" }}
              fontWeight={"600"}
            >
              {typeof goal === "number" &&
                !isNaN(goal) &&
                `${parseInt(goal.toFixed(toFixedNum)).toLocaleString("pt-BR")}`}
              {typeof goal === "number" && !isNaN(goal) && isPercent}
              {typeof goal === "number" && !isNaN(goal) && "/"}
            </Typography>
            render {
              console.log("Resultado: " + progress)
            }
            {progress &&
              parseInt(progress.toFixed(toFixedNum)).toLocaleString("pt-BR")}
            {isPercent}
          </Typography>
        </Stack>
      </Box>
    </Box>
  );
}
