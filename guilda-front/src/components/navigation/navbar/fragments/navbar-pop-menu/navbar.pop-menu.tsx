import {
    Box,
    MenuItem,
    MenuList,
    Popper,
    Typography,
    useTheme,
} from "@mui/material";
import { ClickAwayListener } from "@mui/base";
import Link from "next/link";
import React from "react";
import { NavbarItemProps } from "../navbar-item";

interface NavbarPopMenu {
    id?: string;
    items?: Omit<NavbarItemProps, "subMenuItems">[];
    anchor: HTMLElement | null;
    onClose: () => void;
    isOpen: boolean;
}
export function NavbarPopMenu({
    id,
    isOpen,
    anchor,
    items,
    onClose,
}: NavbarPopMenu) {
    const theme = useTheme();

    return (
        <Popper
            id={id}
            open={isOpen}
            anchorEl={anchor}
            placement={"bottom-start"}
            sx={{ position: "relative", zIndex: 2 }}
        >
            <Box
                sx={{
                    bgcolor: "white",
                    borderRadius: ".15rem",
                    boxShadow: "0 10px 30px rgb(0 0 0 / 10%)",
                }}
            >
                <ClickAwayListener onClickAway={onClose}>
                    <MenuList sx={{ py: 0 }}>
                        {items?.map((item, index) => {
                            if (item.visible == false) {
                                return null;
                            }

                            return (
                                <Link href={item.href} key={index}>
                                    <MenuItem
                                        sx={{
                                            "&:first-child": {
                                                borderRadius:
                                                    ".15rem .15rem 0 0",
                                            },
                                        }}
                                    >
                                        <Box
                                            display={"flex"}
                                            color={theme.palette.grey["700"]}
                                            alignItems={"center"}
                                        >
                                            {item.icon}

                                            <Typography
                                                ml={item.icon ? 1 : 0}
                                                fontFamily={"Poppins"}
                                                fontWeight={"500"}
                                                fontSize={".80rem"}
                                            >
                                                {item.title}
                                            </Typography>
                                        </Box>
                                    </MenuItem>
                                </Link>
                            );
                        })}
                    </MenuList>
                </ClickAwayListener>
            </Box>
        </Popper>
    );
}
