import {
    Box,
    Menu,
    MenuItem,
    Stack,
    Typography,
    alpha,
    useTheme,
} from "@mui/material";
import Link from "next/link";
import { useRouter } from "next/router";
import {
    MouseEvent,
    ReactElement,
    ReactNode,
    cloneElement,
    useState,
} from "react";
import { ExpandIcon } from "src/components/icons/expand.icon";

interface SideBarItemProps {
    icon: ReactElement;
    title: string;
    onClick?: () => void;
    isExpanded?: boolean;
    subMenuItems?: SideBarItemProps[];
    visible?: boolean;
    href?: string;
    customColor?: string;
}

export function SideBarItem(props: SideBarItemProps) {
    const {
        icon,
        title,
        onClick,
        isExpanded,
        href,
        subMenuItems,
        customColor,
    } = props;
    const theme = useTheme();
    const router = useRouter();
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const open = Boolean(anchorEl);
    const handleClick = (event: MouseEvent<HTMLDivElement>) => {
        setAnchorEl(event.currentTarget);
    };
    const handleClose = () => {
        setAnchorEl(null);
    };

    const isHomePage = href === "/";

    const isActive = !href
    ? false
    : isHomePage
    ? router.pathname === "/"
    : href.match(/\/\d+$/) // Verifica se o `href` termina com um número (ID)
    ? router.pathname.startsWith(href.split("/").slice(0, -1).join("/")) // Ignora o último segmento (ID)
    : router.pathname === href; // Verifica a URL completa se não for um ID

console.log(router.pathname + " | " + href + " | " + isActive);
    const color = isActive ? alpha(theme.palette.primary.main, 0.2) : "";
    const textColor = isActive ? theme.palette.primary.main : customColor || "";

    return (
        <>
            <Stack
                direction={"row"}
                onClick={(e) => {
                    if (href && !subMenuItems) {
                        return router.push(href);
                    }
                    if (subMenuItems) {
                        return handleClick(e);
                    }
                    onClick && onClick();
                }}
                py={"6px"}
                gap={"8px"}
                alignItems={"center"}
                px={isExpanded ? "16px" : "0px"}
                bgcolor={isExpanded ? color : "default"}
                borderRadius={"8px"}
                sx={{ cursor: "pointer" }}
            >
                <Stack
                    width={"24px"}
                    height={"24px"}
                    justifyContent={"center"}
                    alignItems={"center"}
                >
                    {href
                        ? cloneElement(icon, {
                              color: textColor,
                              sx: { color: textColor },
                          })
                        : icon}
                </Stack>
                {isExpanded && (
                    <Typography
                        variant="h6"
                        mt={"2px"}
                        sx={{ color: textColor }}
                    >
                        {title}
                    </Typography>
                )}
            </Stack>
            {subMenuItems && (
                <Menu
                    anchorEl={anchorEl}
                    open={open}
                    PaperProps={{
                        sx: { borderRadius: "16px", py: "10px", px: "5px" },
                    }}
                    onClose={handleClose}
                    anchorOrigin={{
                        vertical: "center",
                        horizontal: "center",
                    }}
                    transformOrigin={{
                        vertical: "center",
                        horizontal: "left",
                    }}
                >
                    {subMenuItems?.map((item, index) => {
                        if (item.visible == false) {
                            return null;
                        }

                        return (
                            <Stack
                                onClick={() => {
                                    router.push(item.href || "");
                                    handleClose();
                                }}
                                key={index}
                            >
                                <MenuItem
                                    sx={{
                                        "&:first-child": {
                                            borderRadius: ".15rem .15rem 0 0",
                                        },
                                    }}
                                >
                                    <Stack
                                        alignItems={"center"}
                                        direction={"row"}
                                        gap={"6px"}
                                    >
                                        {item.icon}

                                        <Typography variant="h6">
                                            {item.title}
                                        </Typography>
                                    </Stack>
                                </MenuItem>
                            </Stack>
                        );
                    })}
                </Menu>
            )}
        </>
    );
}
