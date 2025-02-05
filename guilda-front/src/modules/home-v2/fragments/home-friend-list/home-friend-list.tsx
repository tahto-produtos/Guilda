import { Box, Button, Stack, Typography, useTheme } from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { UserPersonaContext } from "src/contexts/user-persona/user-persona.context";
import { useLoadingState } from "src/hooks";
import { ListFollowSugestionUseCase } from "src/modules/follow/use-cases/list-follow-sugestion.use-case";
import { ListFollowUseCase } from "src/modules/follow/use-cases/list-follow.use-case";
import { Friend } from "src/typings/models/friend.model";
import { FriendSuggestionItem } from "../friend-suggestion-item/friend-suggestion-item";
import { FriendItem } from "../friend-item/friend-item";
import { ListFriendUseCase } from "src/modules/follow/use-cases/list-friend-list.use-case";

export function HomeFriendList() {
  const { myUser } = useContext(UserInfoContext);
  const { personaInfo, personaShowUser } = useContext(UserPersonaContext);
  const theme = useTheme();
  const [tab, setTab] = useState<number>(0);
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [suggestions, setSuggestions] = useState<Friend[]>([]);
  const [friends, setFriends] = useState<Friend[]>([]);

  function getFriends() {
    if (!personaShowUser) return;
    startLoading();

    new ListFriendUseCase()
      .handle({
        friend: "",
        tierList: true,
        personaId: personaShowUser?.ID_PERSON_ACCOUNT,
      })
      .then((data) => {
        setFriends(data);
      })
      .catch(() => {
        //
      })
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    personaShowUser && getFriends();
  }, [personaShowUser]);
  function getSuggestions() {
    if (!personaInfo) return;

    startLoading();

    new ListFollowSugestionUseCase()
      .handle({ sectors: personaInfo.sector })
      .then((data) => {
        setSuggestions(data);
      })
      .catch(() => {
        //
      })
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    personaInfo && getSuggestions();
  }, [personaInfo]);

  return (
    <ContentCard sx={{ flexDirection: "column", gap: "28px" }}>
      <Stack direction={"row"} gap={"37px"}>
        <Box
          sx={{
            background:
              tab == 0 ? theme.palette.secondary.main : theme.palette.grey[300],
            color:
              tab == 0
                ? theme.palette.background.default
                : theme.palette.grey[900],
            width: "100%",
            height: "38px",
            borderRadius: "8px",
            justifyContent: "center",
            alignItems: "center",
            display: "flex",
            cursor: "pointer",
          }}
          onClick={() => setTab(0)}
        >
          <Typography
            variant="body1"
            fontSize={"14px"}
            fontWeight={tab == 0 ? "600" : "400"}
            lineHeight={"14px"}
          >
            Amigos
          </Typography>
        </Box>
        <Box
          sx={{
            background:
              tab == 1 ? theme.palette.secondary.main : theme.palette.grey[300],
            color:
              tab == 1
                ? theme.palette.background.default
                : theme.palette.grey[900],
            width: "100%",
            height: "38px",
            borderRadius: "8px",
            justifyContent: "center",
            alignItems: "center",
            display: "flex",
            cursor: "pointer",
          }}
          onClick={() => setTab(1)}
        >
          <Typography
            variant="body1"
            fontSize={"14px"}
            fontWeight={tab == 1 ? "600" : "400"}
            lineHeight={"14px"}
          >
            Sugestões
          </Typography>
        </Box>
      </Stack>
      <Stack gap={"40px"}>
        {tab == 0
          ? friends.map((item, index) => (
              <FriendItem data={item} key={index} nameIsLink={false} />
            ))
          : suggestions.map((item, index) => (
              <FriendSuggestionItem data={item} key={index} />
            ))}
      </Stack>
    </ContentCard>
  );
}
