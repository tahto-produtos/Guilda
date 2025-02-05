import { Box, Typography, useTheme } from "@mui/material";
import Link from "next/link";
import { useRouter } from "next/router";
import KeyboardArrowDown from "@mui/icons-material/KeyboardArrowDown";
import React, { ReactNode } from "react";
import { ConditionalWrapper } from "../../../../surfaces";
import { is } from "date-fns/locale";

export interface NavbarItemProps {
    title: string;
    href: string;
    icon?: ReactNode;
    subMenuItems?: Omit<NavbarItemProps, "subMenuItems" | "onClick">[];
    onClick?: (element: HTMLElement, item: NavbarItemProps) => void;
    visible?: boolean;
}

export function NavbarItem(props: NavbarItemProps) {
    const { title, href, subMenuItems, icon, onClick } = props;
    const theme = useTheme();
    const router = useRouter();

    const isHomePage = href === "/";

    const isActive = isHomePage
        ? router.pathname === "/"
        : router.pathname.startsWith(href);

    const color = isActive
        ? theme.palette.primary.main
        : theme.palette.grey["800"];

    const handleClick = (event: React.MouseEvent<HTMLElement>) => {
        const element = event.currentTarget;
        onClick?.(element, props);
    };

    const hasSubMenuItems = subMenuItems?.length! > 0;

    return (
        <Box py={"0.2rem"} px={"0.25rem"}>
            <ConditionalWrapper
                condition={!hasSubMenuItems}
                wrapper={(children) => <Link href={href!}>{children}</Link>}
            >
                <Box
                    onClick={handleClick}
                    display={"flex"}
                    alignItems={"center"}
                    py={0.1}
                    px={1.2}
                    borderRadius={"0.15rem"}
                    minHeight={"30px"}
                    bgcolor={
                        isActive ? theme.palette.grey["100"] : "transparent"
                    }
                    sx={(theme) => ({
                        color: color,
                        "&:hover": {
                            bgcolor: theme.palette.grey["100"],
                            color: theme.palette.primary.main,
                            cursor: "pointer",
                            transition: "ease-in-out 0.25s",
                        },
                    })}
                >
                    <Typography
                        fontFamily={"Poppins"}
                        fontWeight={"500"}
                        fontSize={".77rem"}
                        color={"inherit"}
                    >
                        {title}
                    </Typography>

                    <Box
                        display={subMenuItems ? "flex" : "none"}
                        alignItems={"center"}
                        // mt={0.75}
                    >
                        <KeyboardArrowDown
                            fontSize={"small"}
                            sx={{ ml: 0.6 }}
                        />
                    </Box>
                </Box>
            </ConditionalWrapper>
        </Box>
    );
}
