import React, { useContext, useState } from "react";
import Link from "next/link";
import {
    Avatar,
    Badge,
    Box,
    Button,
    Container,
    Divider,
    IconButton,
    Typography,
    useTheme,
} from "@mui/material";
import Logout from "@mui/icons-material/Logout";
import Menu from "@mui/icons-material/Menu";
import { NavbarItem, NavbarItemProps } from "./fragments";
import { DrawerNavigator } from "../drawer-navigator";
import { NavbarPopMenu } from "./fragments";
import { LogoutUseCase } from "../../../modules/auth/use-cases";
import { useRouter } from "next/router";
import Logo from "../../../assets/logo/logo.png";
import Image from "next/image";
import ShoppingCartButton from "src/components/data-display/shopping-cart/shopping-cart";
import MyInfo from "src/components/data-display/my-info";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

interface NavbarProps {
    items: Omit<NavbarItemProps, "onClick">[];
}

export function Navbar({ items }: NavbarProps) {
    const theme = useTheme();
    const [isDrawerOpen, setIsDrawerOpen] = useState(false);
    const router = useRouter();
    const { myUser } = useContext(UserInfoContext);
    const { setMyPermissions } = useContext(PermissionsContext);

    const [selectedMenuItem, setSelectedMenuItem] =
        useState<null | NavbarItemProps>(null);

    const [menuItemAnchor, setMenuItemAnchor] = useState<null | HTMLElement>(
        null
    );

    const isSubMenuOpen = Boolean(menuItemAnchor);
    const id = selectedMenuItem?.href;

    const handleOpen = (anchor: HTMLElement, selectedMenu: NavbarItemProps) => {
        setMenuItemAnchor(anchor);
        setSelectedMenuItem(selectedMenu);
    };

    const handleClose = () => {
        setMenuItemAnchor(null);
        setSelectedMenuItem(null);
    };

    const handleLogout = async () => {
        const logout = new LogoutUseCase();
        await logout.handle();
        await router.push("/login");
        await setMyPermissions([]);
    };

    return (
        <>
            <Box width={"100%"} bgcolor={"white"}>
                <Box>
                    <Box
                        display={"flex"}
                        alignItems={"center"}
                        justifyContent={"space-between"}
                        width={"100%"}
                    >
                        <Box display={"flex"} alignItems={"center"}>
                            <Link href={"/"}>
                                <Image
                                    src={Logo}
                                    alt="tahto"
                                    width={60}
                                    height={35}
                                />
                            </Link>
                            <Divider
                                sx={{ mx: 2, height: "20px" }}
                                orientation={"vertical"}
                            />

                            <Box
                                ml={0}
                                display={"flex"}
                                sx={{
                                    [theme.breakpoints.down("md")]: {
                                        display: "none",
                                    },
                                }}
                                alignItems={"center"}
                                minHeight={"54px"}
                            >
                                {items.map((item, index) => {
                                    if (item.visible == false) {
                                        return null;
                                    }
                                    return (
                                        <NavbarItem
                                            key={index}
                                            onClick={handleOpen}
                                            {...item}
                                        />
                                    );
                                })}
                            </Box>
                        </Box>

                        <Box display={"flex"} gap={1} alignItems={"center"}>
                            <ShoppingCartButton />
                            <MyInfo myUser={myUser} />
                            <IconButton onClick={handleLogout} color="error">
                                <Logout fontSize="small" />
                            </IconButton>

                            <IconButton
                                sx={{
                                    [theme.breakpoints.up("md")]: {
                                        display: "none",
                                    },
                                }}
                                onClick={() => setIsDrawerOpen(true)}
                            >
                                <Menu />
                            </IconButton>
                        </Box>
                    </Box>
                </Box>
                <DrawerNavigator
                    items={items}
                    open={isDrawerOpen}
                    onClose={() => setIsDrawerOpen(false)}
                    onOpen={() => setIsDrawerOpen(true)}
                />
            </Box>

            <NavbarPopMenu
                id={id}
                anchor={menuItemAnchor}
                items={selectedMenuItem?.subMenuItems}
                isOpen={isSubMenuOpen}
                onClose={handleClose}
            />
        </>
    );
}
