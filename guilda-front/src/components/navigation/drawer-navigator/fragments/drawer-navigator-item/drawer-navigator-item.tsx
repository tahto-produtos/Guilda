import {
    Box,
    Collapse,
    IconButton,
    ListItemButton,
    Stack,
    Typography,
} from "@mui/material";
import { useRouter } from "next/router";
import Link from "next/link";
import { useEffect, useState } from "react";
import KeyboardArrowDown from "@mui/icons-material/KeyboardArrowDown";
import KeyboardArrowUp from "@mui/icons-material/KeyboardArrowUp";

export interface DrawerNavigatorItemProps {
    title: string;
    href: string;
    subMenuItems?: DrawerNavigatorItemProps[];
    onClick?: () => void;
    isSubItem?: boolean;
}

export function DrawerNavigatorItem({
    title,
    href,
    subMenuItems,
    onClick,
    isSubItem = false,
}: DrawerNavigatorItemProps) {
    const router = useRouter();
    const isHomePage = href === "/";
    const isActive = isHomePage
        ? router.pathname === "/"
        : router.pathname.startsWith(href);

    const [isSubMenuOpen, setIsSubMenuOpen] = useState(isActive);

    useEffect(() => {
        isActive ? setIsSubMenuOpen(true) : setIsSubMenuOpen(false);
    }, [isActive]);

    const toggleSubmenu = () => setIsSubMenuOpen(!isSubMenuOpen);

    const preventRedirect = () => {
        toggleSubmenu();
        onClick?.();
    };

    return (
        <>
            <Box
                display={"flex"}
                width={"100%"}
                justifyContent={"space-between"}
            >
                <Link
                    href={subMenuItems ? {} : href}
                    onClick={() => toggleSubmenu()}
                    style={{ width: "100%" }}
                >
                    <ListItemButton selected={isActive}>
                        <Typography
                            fontFamily={"Poppins"}
                            fontWeight={isSubItem ? "400" : "500"}
                            fontSize={isSubItem ? ".90rem" : ".95rem"}
                            sx={(theme) => ({
                                color: isActive
                                    ? theme.palette.primary.main
                                    : theme.palette.grey["800"],
                                "&:hover": {
                                    color: theme.palette.primary.main,
                                    cursor: "pointer",
                                    transition: "ease-in-out 0.25s",
                                },
                                borderLeft: isSubItem ? "3px solid" : undefined,
                                borderColor: isActive
                                    ? theme.palette.primary.main
                                    : theme.palette.grey["300"],
                                pl: isSubItem ? 1 : 0,
                            })}
                        >
                            {title}
                        </Typography>
                    </ListItemButton>
                </Link>

                {/* <IconButton
                    sx={{ visibility: subMenuItems ? "visible" : "hidden" }}
                    onClick={toggleSubmenu}
                >
                    {isSubMenuOpen ? (
                        <KeyboardArrowUp />
                    ) : (
                        <KeyboardArrowDown />
                    )}
                </IconButton> */}
            </Box>

            <Collapse in={isSubMenuOpen} onClick={toggleSubmenu}>
                <Stack ml={1.5}>
                    {subMenuItems?.map((item, index) => (
                        <DrawerNavigatorItem
                            key={index}
                            isSubItem
                            onClick={onClick}
                            {...item}
                        />
                    ))}
                </Stack>
            </Collapse>
        </>
    );
}
