import { Collapse, Stack, Typography, useTheme } from "@mui/material";
import { ExpandIcon } from "src/components/icons/expand.icon";
import { SideBarItem } from "./sideBarItem/sideBarItem";
import { useContext, useState } from "react";
import { MainNavigation } from "./mainNavigation/mainNavigation";
import { LeaveIcon } from "src/components/icons/leave.icon";
import { LogoutUseCase } from "src/modules/auth/use-cases";
import { useRouter } from "next/router";
import styles from "./sidebar.module.css";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export function Sidebar() {
    const { palette } = useTheme();
    const router = useRouter();
    const [isExpanded, setIsExpanded] = useState<boolean>(true);
    const { setMyPermissions } = useContext(PermissionsContext);

    const handleLogout = async () => {
        const logout = new LogoutUseCase();
        await logout.handle();
        await router.push("/login");
        await setMyPermissions([]);
    };

    return (
        <Stack
            className={`${styles.sideBarHeight}`}
            sx={{
                minWidth: "fit-content",
                backgroundColor: palette.background.paper,
                borderRadius: "16px",
                px: "16px",
                py: "32px",
                position: "sticky",
                overflow: "auto",
                top: 144,
            }}
            direction={"column"}
            justifyContent={"space-between"}
            gap={"30px"}
        >
            <Stack direction={"column"} gap={"16px"} position={"relative"}>
                <SideBarItem
                    icon={<ExpandIcon width={18} height={12} />}
                    title="Expandir"
                    onClick={() => setIsExpanded(!isExpanded)}
                    isExpanded={isExpanded}
                />
                <Stack
                    direction={"column"}
                    gap={"16px"}
                    sx={{ overflow: "auto" }}
                    maxHeight={"100%"}
                    position={"relative"}
                >
                    <MainNavigation isExpanded={isExpanded} />
                </Stack>
            </Stack>
            <SideBarItem
                icon={
                    <LeaveIcon
                        width={18}
                        height={18}
                        color={palette.error.main}
                    />
                }
                customColor={palette.error.main}
                title="Sair"
                onClick={handleLogout}
                isExpanded={isExpanded}
            />
        </Stack>
    );
}
