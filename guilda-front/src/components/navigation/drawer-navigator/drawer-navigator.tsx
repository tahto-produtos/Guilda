import {
    Box,
    Button,
    List,
    Stack,
    SwipeableDrawer,
    Typography,
    useTheme,
} from "@mui/material";
import { DrawerNavigatorItem, DrawerNavigatorItemProps } from "./fragments";
import ArrowForward from "@mui/icons-material/ArrowForward";
import Logout from "@mui/icons-material/Logout";
import { LogoutUseCase } from "../../../modules/auth/use-cases";
import { useRouter } from "next/router";
import { useContext } from "react";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

interface DrawerNavigatorProps {
    items: DrawerNavigatorItemProps[];
    open: boolean;
    onClose: () => void;
    onOpen: () => void;
}

export function DrawerNavigator({
    items,
    open,
    onClose,
    onOpen,
}: DrawerNavigatorProps) {
    const theme = useTheme();
    const router = useRouter();
    const { setMyPermissions } = useContext(PermissionsContext);

    const handleLogout = async () => {
        const logout = new LogoutUseCase();
        await logout.handle();
        await router.push("/login");
        await setMyPermissions([]);
    };

    return (
        <SwipeableDrawer
            anchor={"left"}
            open={open}
            onOpen={onOpen}
            onClose={onClose}
        >
            <Stack
                p={2}
                width={"320x"}
                justifyContent={"space-between"}
                height={"100%"}
            >
                <Box>
                    <Typography
                        color={theme.palette.primary.main}
                        variant={"h2"}
                        pl={2}
                    >
                        Guilda
                    </Typography>

                    <Box mt={2}>
                        <List>
                            {items.map((item, index) => (
                                <DrawerNavigatorItem
                                    onClick={onClose}
                                    key={index}
                                    {...item}
                                />
                            ))}
                        </List>
                    </Box>
                </Box>
                <Box>
                    <Button
                        onClick={handleLogout}
                        fullWidth
                        variant={"contained"}
                        startIcon={<Logout />}
                    >
                        Sair
                    </Button>
                </Box>
            </Stack>
        </SwipeableDrawer>
    );
}
