import ManageAccounts from "@mui/icons-material/ManageAccounts";
import Settings from "@mui/icons-material/Settings";
import { IconButton, List, ListItem, ListItemText } from "@mui/material";
import { grey } from "@mui/material/colors";
import { Stack } from "@mui/system";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { useLoadingState } from "src/hooks";
import { ListProfilesUseCase } from "src/modules/profiles/use-cases/list-profiles.use-case";
import { PaginationModel } from "src/typings/models/pagination.model";
import { ProfileModel } from "src/typings/models/profile.model";
import { getLayout } from "src/utils";

export default function ProfileSettings() {
    const router = useRouter();
    const [profiles, setProfiles] = useState<ProfileModel[]>([]);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const { myUser } = useContext(UserInfoContext);

    const handleRedirectToDetailPage = (id: number) =>
        router.push(`/profile/profile-settings/${id}`);

    const GetProfiles = async () => {
        startLoading();

        const pagination: PaginationModel = {
            limit: 99,
            offset: 0,
        };

        await new ListProfilesUseCase()
            .handle(pagination)
            .then((data) => setProfiles(data.items))
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    useEffect(() => {
        GetProfiles();
    }, []);

    const ListItemComponent = (props: {
        item: ProfileModel;
        index: number;
    }) => {
        const { item } = props;
        const [isHover, setIsHover] = useState<boolean>(false);

        const isTheLastItem = props.index === profiles.length - 1;

        return (
            <ListItem
                secondaryAction={
                    <IconButton
                        edge="end"
                        aria-label="settings"
                        onClick={() => handleRedirectToDetailPage(item.id)}
                        size="small"
                    >
                        <Settings fontSize="small" />
                    </IconButton>
                }
                sx={{
                    borderBottom: isTheLastItem
                        ? "none"
                        : `solid 1px ${grey[100]}`,
                    backgroundColor: isHover ? grey[100] : "#fff",
                }}
                onMouseEnter={() => setIsHover(true)}
                onMouseLeave={() => setIsHover(false)}
            >
                <ListItemText primary={item.profile} />
            </ListItem>
        );
    };

    return (
        <Card
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"space-between"}
        >
            <PageHeader
                title={`Configurações de perfis`}
                headerIcon={<ManageAccounts />}
            />
            <Stack width={"100%"}>
                <List
                    dense={false}
                    sx={{
                        display: "flex",
                        flexDirection: "column",
                        border: `solid 1px ${grey[100]}`,
                        padding: 0,
                    }}
                >
                    {profiles.map((item, index) => (
                        <ListItemComponent
                            item={item}
                            key={index}
                            index={index}
                        />
                    ))}
                </List>
            </Stack>
        </Card>
    );
}

ProfileSettings.getLayout = getLayout("private");
