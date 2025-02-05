import AddBoxOutlined from "@mui/icons-material/AddBoxOutlined";
import { Button, Stack, Typography } from "@mui/material";
import router from "next/router";
import { useState } from "react";
import { toast } from "react-toastify";
import { ProfileImage } from "src/components/data-display/profile-image/profile-image";
import { useLoadingState } from "src/hooks";
import { CreateFollowUseCase } from "src/modules/follow/use-cases/create-follow.use-case";
import { Friend } from "src/typings/models/friend.model";
import { capitalizeText } from "src/utils/capitalizeText";

interface IProps {
    data: Friend;
}

export function FriendSuggestionItem(props: IProps) {
    const { data } = props;
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [following, setFollowing] = useState<boolean>(false);

    async function follow() {
        startLoading();

        await new CreateFollowUseCase()
            .handle({
                follow: true,
                idFollowed: data.id,
            })
            .then(() => {
                toast.success(
                    `Agora você está seguindo ${capitalizeText(data.name)}`
                );
                setFollowing(true);
            })
            .catch(() => {
                toast.error("Falha ao seguir o usuário");
            })
            .finally(() => {
                finishLoading();
            });
    }

    async function unFollow() {
        startLoading();

        await new CreateFollowUseCase()
            .handle({
                follow: false,
                idFollowed: data.id,
            })
            .then(() => {
                toast.success(
                    `Você deixou de seguir ${capitalizeText(data.name)}`
                );
                setFollowing(false);
            })
            .catch(() => {
                toast.error("Falha ao seguir o usuário");
            })
            .finally(() => {
                finishLoading();
            });
    }

    if (following) {
        return null;
    }

    return (
        <Stack
            direction={"row"}
            alignItems={"center"}
            gap={"15px"}
            justifyContent={"space-between"}
            sx={{ cursor: "pointer" }}
            onClick={() => router.push(`/profile/view-profile/${data.id}`)}
        >
            <Stack direction={"row"} alignItems={"center"} gap={"16px"}>
                <ProfileImage
                    name={data.name}
                    image={data.url}
                    width="50px"
                    height="50px"
                />
                <Stack gap={"8px"}>
                    <Typography variant="body1" fontWeight={"600"}>
                        {capitalizeText(data.name)}
                    </Typography>
                    <Typography variant="body1">
                        {capitalizeText(data.hierarchy)}
                    </Typography>
                </Stack>
            </Stack>
            {following ? (
                <Button
                    variant="text"
                    startIcon={<AddBoxOutlined />}
                    color="inherit"
                    onClick={follow}
                >
                    Deseguir
                </Button>
            ) : (
                <Button
                    variant="text"
                    startIcon={<AddBoxOutlined />}
                    color="inherit"
                    onClick={follow}
                >
                    Seguir
                </Button>
            )}
        </Stack>
    );
}
