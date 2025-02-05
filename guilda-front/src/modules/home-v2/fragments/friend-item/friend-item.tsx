import { Stack, Typography } from "@mui/material";
import Link from "@mui/material/Link";
import { ProfileImage } from "src/components/data-display/profile-image/profile-image";
import { Friend } from "src/typings/models/friend.model";
import { capitalizeText } from "src/utils/capitalizeText";
import { useRouter } from "next/router";
import { BASE_URL } from "../../../../constants/environment-variable.constants";

interface IProps {
  data: Friend;
  nameIsLink: boolean;
}

export function FriendItem(props: IProps) {
  const { data, nameIsLink = false } = props;
  const router = useRouter();

  return (
    <Stack
      direction={"row"}
      alignItems={"center"}
      gap={"15px"}
      justifyContent={"space-between"}
    >
      <Stack
        direction={"row"}
        alignItems={"center"}
        gap={"16px"}
        onClick={() => router.push(`/profile/view-profile/${data.id}`)}
        sx={{ cursor: "pointer" }}
      >
        <ProfileImage
          name={data.name}
          image={data.url}
          width="50px"
          height="50px"
        />
        <Stack gap={"8px"}>
          {nameIsLink ? (
            <Stack>{capitalizeText(data.name)}</Stack>
          ) : (
            <Typography variant="body1" fontWeight={"600"}>
              {capitalizeText(data.name)}
            </Typography>
          )}

          <Typography variant="body1">
            {capitalizeText(data.hierarchy)}
          </Typography>
        </Stack>
      </Stack>
    </Stack>
  );
}
