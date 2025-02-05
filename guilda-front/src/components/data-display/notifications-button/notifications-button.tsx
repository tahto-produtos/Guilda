import NotificationsOutlined from "@mui/icons-material/NotificationsOutlined";
import { Badge, Box, IconButton, useTheme } from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { NotificationsDrawer } from "src/components/feedback/notifications-drawer/notifications-drawer";
import { NotificationsContext } from "src/contexts/notifications-provider/notifications.context";
import { CountNotificationUseCase } from "src/modules/notifications/use-cases/count-notification.use-case";

export default function NotificationsButton() {
    const theme = useTheme();
    const { count } = useContext(NotificationsContext);

    const [isOpen, setIsOpen] = useState<boolean>(false);

    return (
        <Box
            sx={{
                display: "flex",
                position: "relative",
                alignItems: "center",
                justifyContent: "center",
                mr: "10px",
            }}
        >
            <IconButton
                color="inherit"
                sx={{
                    width: "40px",
                    height: "40px",
                    border: `solid 1px ${theme.palette.grey[600]}`,
                }}
                onClick={() => setIsOpen(true)}
            >
                <Badge
                    color="error"
                    sx={{
                        "& .MuiBadge-badge": {
                            right: -3,
                            top: 1,
                            border: `2px solid ${theme.palette.background.paper}`,
                            padding: "0px 4px",
                            fontSize: "11px",
                            display: "flex",
                            justifyContent: "center",
                            alignItems: "center",
                            lineHeight: "15px",
                            fontWeight: "600",
                        },
                    }}
                    badgeContent={count}
                >
                    <NotificationsOutlined
                        sx={{ color: theme.palette.grey[300] }}
                    />
                </Badge>
            </IconButton>
            <NotificationsDrawer
                open={isOpen}
                onClose={() => setIsOpen(false)}
            />
        </Box>
    );
}
