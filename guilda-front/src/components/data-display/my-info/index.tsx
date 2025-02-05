import { Avatar, Box, Typography, useTheme } from "@mui/material";
import { useContext, useState } from "react";
import { BaseModal } from "src/components/feedback";
import { UserInfoContext } from "src/contexts/user-context/user.context";

export default function MyInfo({ myUser }: { myUser: any }) {
    const theme = useTheme();
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const { userGroupName } = useContext(UserInfoContext);

    const ListItem = (props: { title: string; value: string }) => {
        const { title, value } = props;

        return (
            <Box display={"flex"} justifyContent={"space-between"}>
                <Typography>{title}</Typography>
                <Typography>{value}</Typography>
            </Box>
        );
    };

    return (
        <Box>
            <Avatar
                sx={{
                    width: 30,
                    height: 30,
                    fontSize: 14,
                    bgcolor: theme.palette.primary.main,
                    cursor: "pointer",
                }}
                onClick={() => setIsOpen(true)}
            >
                {myUser && myUser.name.charAt(0)}
            </Avatar>
            <BaseModal
                width={"540px"}
                open={isOpen}
                title={`Suas informações`}
                onClose={() => setIsOpen(false)}
            >
                <Box width={"100%"} display={"flex"} flexDirection={"column"}>
                    <ListItem title="Nome" value={myUser?.name} />
                    <ListItem title="Código" value={myUser?.id} />
                    <ListItem
                        title="Identificação"
                        value={myUser?.identification}
                    />
                    <ListItem title="Email" value={myUser?.email} />
                    <ListItem title="Grupo" value={userGroupName || ""} />
                </Box>
            </BaseModal>
        </Box>
    );
}
