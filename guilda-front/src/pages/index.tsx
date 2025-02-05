import { Feed } from "src/modules/home-v2/fragments/feed/feed";
import { HomeFriendList } from "src/modules/home-v2/fragments/home-friend-list/home-friend-list";
import { HomeMonetizationCard } from "src/modules/home-v2/fragments/home-monetization-card/home-monetization-card";
import { HomeUserCard } from "src/modules/home-v2/fragments/home-user-card/home-user-card";
import { Stack } from "@mui/material";
import { getLayout } from "src/utils";
import { HomePostCreate } from "../modules/home-v2/fragments/home-post-create/home-post-create";
import styles from "../components/navigation/sidebar/sidebar.module.css";

export default function Home() {
  return (
    <Stack direction={"row"} gap={"34px"}>
      <Stack gap={"34px"} width={"100%"}>
        <HomeUserCard />
        <HomePostCreate />
        <Feed />
      </Stack>
      <Stack gap={"24px"} minWidth={"345px"}>
        <Stack
          className={`${styles.sideBarHeight}`}
          gap={"24px"}
          minWidth={"345px"}
          sx={{
            position: "sticky",
            top: 144,
            zIndex: 9,
            overflow: "auto",
          }}
        >
          <HomeMonetizationCard />
          <HomeFriendList />
        </Stack>
      </Stack>
    </Stack>
  );
}

Home.getLayout = getLayout("private");
