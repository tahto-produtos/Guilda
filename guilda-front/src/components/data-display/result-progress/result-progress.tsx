import MonetizationOn from "@mui/icons-material/MonetizationOn";
import {
  Box,
  CardMedia,
  CircularProgress,
  LinearProgress,
  Popover,
  Stack,
  Tooltip,
  Typography,
  alpha,
  useTheme,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { useContext, useState } from "react";
import { UserInfoContext } from "src/contexts/user-context/user.context";

interface ResultProgressProps {
  indicatorName: string;
  progress: number;
  progress_hour: string;
  goal: number;
  goal_hour: string;
  type: string;
  groupAlias: string | null;
  groupName: string | null;
  isMonetized: boolean;
  groupId?: number | null;
  groupImage?: string | null;
  maxMonetization: number;
  monetizationCollaborator?: number;
}

export const ResultProgress = (props: ResultProgressProps) => {
  const {
    indicatorName,
    progress,
    progress_hour,
    goal,
    goal_hour,
    type,
    groupAlias,
    groupName,
    isMonetized,
    groupId,
    groupImage,
    maxMonetization,
    monetizationCollaborator,
  } = props;
  const theme = useTheme();
  const { myUser } = useContext(UserInfoContext);
  const isCompleted = progress >= goal;
  const isPercent = type == "PERCENT" ? "%" : "";
  const toFixedNum = type == "INTEGER" ? 0 : 1;

  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);

  const handlePopoverOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handlePopoverClose = () => {
    setAnchorEl(null);
  };

  const open = Boolean(anchorEl);

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

  const percentage = monetizationCollaborator
    ? (monetizationCollaborator / maxMonetization) * 100
    : 0;

  return (
    <Box
      display={"flex"}
      flexDirection={"column"}
      gap={1}
      p={"20px"}
      borderRadius={2}
      width={"260px"}
      sx={{ backgroundColor: isMonetized ? grey[100] : "#fff" }}
      border={`solid 1px ${isMonetized ? grey[300] : grey[100]}`}
    >
      <Box
        justifyContent={"center"}
        alignItems={"center"}
        display={"flex"}
        flexDirection={"row"}
        gap={"10px"}
      >
        {isMonetized && <MonetizationOn color="warning" />}
        <Typography fontWeight={"600"} fontSize={"14px"}>
          {indicatorName}
        </Typography>
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
            color: grey[200],
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
              {type === "HOUR" && typeof goal === "number" &&
                !isNaN(goal) &&
                `${goal_hour}`}
              {type !== "HOUR" && typeof goal === "number" &&
                !isNaN(goal) &&
                `${goal.toFixed(toFixedNum)}`}
              {typeof goal === "number" && !isNaN(goal) && isPercent}
              {typeof goal === "number" && !isNaN(goal) && "/"}
            </Typography>
            {type === "HOUR" && progress_hour}
            {type !== "HOUR" && progress && progress.toFixed(toFixedNum)}
            {isPercent}
          </Typography>
        </Stack>
      </Box>
      {groupImage && (
        <Box
          display={"flex"}
          width={"100%"}
          flexDirection={"row"}
          justifyContent={"flex-end"}
        >
          <CardMedia
            component="img"
            image={groupImage}
            sx={{
              width: "30px",
              objectFit: "contain",
            }}
            onMouseEnter={handlePopoverOpen}
            onMouseLeave={handlePopoverClose}
          />
          {isMonetized && (
            <Popover
              sx={{
                pointerEvents: "none",
                maxWidth: "300px",
                minWidth: "300px",
              }}
              open={open}
              anchorEl={anchorEl}
              anchorOrigin={{
                vertical: "bottom",
                horizontal: "center",
              }}
              transformOrigin={{
                vertical: "top",
                horizontal: "center",
              }}
              onClose={handlePopoverClose}
              disableRestoreFocus
            >
              <Stack gap={"0px"} width={"100%"} p={"15px"}>
                <Typography variant="body2" fontWeight={"600"}>
                  Moedas ganhas
                </Typography>
                <Typography variant="body2" fontSize={"12px"}>
                  Você recebeu{" "}
                  {monetizationCollaborator ? monetizationCollaborator : 0} de{" "}
                  {maxMonetization} moedas nesse indicador
                </Typography>
                <Stack
                  direction={"row"}
                  alignItems={"center"}
                  gap={1}
                  justifyContent={"center"}
                  display={"flex"}
                  p={0}
                  height={"30px"}
                >
                  <Typography variant="body2" fontWeight={600}>
                    {monetizationCollaborator ? monetizationCollaborator : 0}
                  </Typography>
                  <Stack
                    direction={"row"}
                    width={"300px"}
                    justifyContent={"space-between"}
                    alignItems={"center"}
                    gap={"5px"}
                  >
                    <Stack
                      width={"100%"}
                      height={"10px"}
                      borderRadius={"50px"}
                      bgcolor={
                        percentage > 0
                          ? getColorByGroupIdProgress(groupId || 0)
                          : alpha(getColorByGroupIdProgress(groupId || 0), 0.35)
                      }
                    />
                    <Stack
                      width={"100%"}
                      height={"10px"}
                      borderRadius={"50px"}
                      bgcolor={
                        percentage >= 25
                          ? getColorByGroupIdProgress(groupId || 0)
                          : alpha(getColorByGroupIdProgress(groupId || 0), 0.35)
                      }
                    />
                    <Stack
                      width={"100%"}
                      height={"10px"}
                      borderRadius={"50px"}
                      bgcolor={
                        percentage >= 50
                          ? getColorByGroupIdProgress(groupId || 0)
                          : alpha(getColorByGroupIdProgress(groupId || 0), 0.35)
                      }
                    />
                    <Stack
                      width={"100%"}
                      height={"10px"}
                      borderRadius={"50px"}
                      bgcolor={
                        percentage >= 75
                          ? getColorByGroupIdProgress(groupId || 0)
                          : alpha(getColorByGroupIdProgress(groupId || 0), 0.35)
                      }
                    />
                    <Stack
                      width={"100%"}
                      height={"10px"}
                      borderRadius={"50px"}
                      bgcolor={
                        percentage >= 100
                          ? getColorByGroupIdProgress(groupId || 0)
                          : alpha(getColorByGroupIdProgress(groupId || 0), 0.35)
                      }
                    />
                  </Stack>
                  {/* <LinearProgress
                                        variant="determinate"
                                        value={
                                            monetizationCollaborator
                                                ? (monetizationCollaborator /
                                                      maxMonetization) *
                                                  100
                                                : 0
                                        }
                                        sx={{
                                            width: 200,
                                            "& .MuiLinearProgress-bar": {
                                                backgroundColor: groupId
                                                    ? getColorByGroupIdProgress(
                                                          groupId
                                                      )
                                                    : theme.palette.primary
                                                          .main,
                                            },
                                            backgroundColor: groupId
                                                ? alpha(
                                                      getColorByGroupIdProgress(
                                                          groupId
                                                      ),
                                                      0.35
                                                  )
                                                : theme.palette.primary.main,
                                        }}
                                    /> */}
                  <Typography variant="body2" fontWeight={600}>
                    {maxMonetization}
                  </Typography>
                </Stack>
                <Stack direction={"row"} justifyContent={"space-between"}>
                  <Typography
                    variant="overline"
                    fontWeight={600}
                    fontSize={"12px"}
                  >
                    Realizado
                  </Typography>
                  <Typography
                    variant="overline"
                    fontWeight={600}
                    fontSize={"12px"}
                  >
                    Meta
                  </Typography>
                </Stack>
              </Stack>
            </Popover>
          )}
        </Box>
      )}
    </Box>
  );
};

// <Tooltip
//     title={
//         maxMonetization &&
//         maxMonetization > 0 &&
//         isMonetized &&
//         myUser.profileCollaboratorAdministrationId !== 19
//             ? `No período aplicado no filtro, você pode ganhar até ${maxMonetization} moedas.`
//             : ""
//     }
//     followCursor
// >
