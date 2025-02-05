import { Avatar, Box, Stack, Typography, useTheme, Checkbox } from "@mui/material";
import { useContext, useState } from "react";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { capitalizeText } from "src/utils/capitalizeText";
import { BaseModal } from "src/components/feedback";
import { ProfileImage } from "../../profile-image/profile-image";
import { UserPersonaContext } from "src/contexts/user-persona/user-persona.context";
import { UpdatePersonalVision } from "src/modules/collaborators/use-cases/update-personal-vision.use-case"
import { toast } from "react-toastify";

export function UserCard() {
    const { myUser, userGroupName } = useContext(UserInfoContext);
    const { persona, personaShowUser } = useContext(UserPersonaContext);
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const theme = useTheme();
    const [checked, setChecked] = useState(personaShowUser?.PERSONAVISION === "1");

    const ListItem = (props: { title: string; value: string }) => {
        const { title, value } = props;

        return (
            <Box display={"flex"} justifyContent={"space-between"}>
                <Typography>{title}</Typography>
                <Typography>{value}</Typography>
            </Box>
        );
    };



    //async (msg: string) => {
    const handleCheck = async (event: any) => {
        setChecked(event.target.checked);

        await new UpdatePersonalVision()
            .handle({
                checked: event.target.checked,
            });

        window.location.reload();
    };

    return (
        <Stack direction={"row"} alignItems={"center"} gap={"17px"}>
            <Stack gap={"11px"} alignItems={"flex-end"}>
                <Typography variant="h5">
                    {myUser && capitalizeText(myUser.name)}
                </Typography>
                {personaShowUser && (
                    <Typography variant="body1" fontSize={"12px"}>
                        Perfil:
                        <Typography
                            component={"span"}
                            fontSize={"12px"}
                            color={"secondary.400"}
                            fontWeight={"600"}
                            ml={"2px"}
                        >
                            {personaShowUser.CARGO}
                        </Typography>
                    </Typography>
                )}
                {personaShowUser && personaShowUser.ISADM == "1" && (
                    <Typography>
                        <Checkbox checked={checked} onChange={handleCheck} />
                        <Typography component="span">Visão Pessoal</Typography>
                    </Typography>
                )}
            </Stack>
            <Stack
                onClick={() => setIsOpen(true)}
                sx={{
                    cursor: "pointer",
                }}
            >
                <ProfileImage
                    name={myUser?.name || ""}
                    width="50px"
                    height="50px"
                />
            </Stack>
            <BaseModal
                width={"540px"}
                open={isOpen}
                title={`Suas informações`}
                onClose={() => setIsOpen(false)}
            >
                <Stack width={"100%"} direction={"column"}>
                    <ListItem title="Nome" value={myUser?.name || ""} />
                    <ListItem
                        title="Código"
                        value={myUser?.id.toString() || ""}
                    />
                    <ListItem
                        title="Identificação"
                        value={myUser?.identification || ""}
                    />
                    <ListItem title="Email" value={myUser?.email || ""} />
                    <ListItem title="Grupo" value={userGroupName || ""} />
                </Stack>
            </BaseModal>
        </Stack>
    );
}
