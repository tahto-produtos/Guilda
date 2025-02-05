import {
    Button,
    InputAdornment,
    Popover,
    Stack,
    TextField,
    Typography,
    useTheme,
    useThemeProps,
} from "@mui/material";
import { SearchIcon } from "src/components/icons/search.icon";
import { UserCard } from "./user-card/user-card";
import { WeekPoints } from "./week-points/week-points";
import ShoppingCartButton from "../shopping-cart/shopping-cart";
import NotificationsButton from "../notifications-button/notifications-button";
import { MouseEvent, useContext, useEffect, useState } from "react";
import { useDebounce, useLoadingState } from "src/hooks";
import {
    ListPersonasUseCase,
    ListedPersona,
} from "src/modules/personas/use-cases/list-personas.use-case";
import { toast } from "react-toastify";
import { ProfileImage } from "../profile-image/profile-image";
import { capitalizeText } from "src/utils/capitalizeText";
import { CreateFollowUseCase } from "src/modules/follow/use-cases/create-follow.use-case";
import { useRouter } from "next/router";
import { UserPersonaContext } from "src/contexts/user-persona/user-persona.context";

export function Header() {
    const { palette } = useTheme();

    const [anchorEl, setAnchorEl] = useState<HTMLDivElement | null>(null);

    const open = Boolean(anchorEl);

    const handleClick = (
        event: MouseEvent<HTMLDivElement, globalThis.MouseEvent>
    ) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    return (
        <Stack
            direction={"row"}
            alignItems={"center"}
            justifyContent={"space-between"}
        >
            <WeekPoints />
            <Stack>
                <TextField
                    placeholder="Digite aqui para pesquisar"
                    sx={{ minWidth: "345px" }}
                    InputProps={{
                        endAdornment: (
                            <InputAdornment position="end">
                                <SearchIcon
                                    width={18}
                                    height={17}
                                    color={palette.text.primary}
                                />
                            </InputAdornment>
                        ),
                        readOnly: true,
                    }}
                    onClick={(e) => handleClick(e)}
                />
                <SearchPopover
                    anchorEl={anchorEl}
                    handleClose={handleClose}
                    open={open}
                />
            </Stack>
            <Stack direction={"row"} alignItems={"center"} gap={"28px"}>
                <Stack direction={"row"} alignItems={"center"} gap={"0px"}>
                    <ShoppingCartButton />
                    <NotificationsButton />
                </Stack>
                <UserCard />
            </Stack>
        </Stack>
    );
}

interface SearchPopoverProps {
    open: boolean;
    anchorEl: HTMLDivElement | null;
    handleClose: () => void;
}

function SearchPopover(props: SearchPopoverProps) {
    const { anchorEl, handleClose, open } = props;
    const { idPersonAccount } = useContext(UserPersonaContext);
    const theme = useTheme();
    const router = useRouter();
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [searchText, setSearchText] = useState<string>("");
    const debounced = useDebounce(searchText, 600);
    const [personas, setPersonas] = useState<ListedPersona[] | []>([]);

    const getPersonas = async () => {
        startLoading();

        new ListPersonasUseCase()
            .handle({
                accountPersona: searchText,
                limit: 5,
                page: 1,
            })
            .then((data) => {
                setPersonas(data.ACCOUNTS);
            })
            .catch(() => {})
            .finally(() => {
                finishLoading();
            });
    };

    useEffect(() => {
        getPersonas();
    }, [debounced]);

    async function follow(id: number) {
        startLoading();

        await new CreateFollowUseCase()
            .handle({
                follow: true,
                idFollowed: id,
            })
            .then(() => {
                toast.success(`Seguindo com sucesso!`);
                getPersonas();
            })
            .catch(() => {
                toast.error("Falha ao seguir o usuário");
            })
            .finally(() => {
                finishLoading();
            });
    }

    return (
        <Popover
            open={open}
            anchorEl={anchorEl}
            onClose={handleClose}
            anchorOrigin={{
                vertical: "top",
                horizontal: "center",
            }}
            transformOrigin={{
                vertical: "top",
                horizontal: "center",
            }}
        >
            <Stack width={"380px"} minHeight={"300px"} gap={"20px"} p={"20px"}>
                <TextField
                    placeholder="Digite aqui para pesquisar"
                    fullWidth
                    value={searchText}
                    onChange={(e) => setSearchText(e.target.value)}
                    InputProps={{
                        endAdornment: (
                            <InputAdornment position="end">
                                <SearchIcon
                                    width={18}
                                    height={17}
                                    color={theme.palette.text.primary}
                                />
                            </InputAdornment>
                        ),
                    }}
                />
                {personas && personas.length > 0 ? (
                    personas.map((persona) => (
                        <Stack
                            direction={"row"}
                            alignItems={"center"}
                            gap={"16px"}
                            key={persona.IDGDA_PERSONA_USER}
                            justifyContent={"space-between"}
                            sx={{ cursor: "pointer" }}
                        >
                            <Stack
                                direction={"row"}
                                alignItems={"center"}
                                gap={"16px"}
                                onClick={() =>
                                    router.push(
                                        `/profile/view-profile/${persona.IDGDA_PERSONA_USER}`
                                    )
                                }
                            >
                                <ProfileImage
                                    name={persona.NOME}
                                    image={persona.FOTO}
                                    width="50px"
                                    height="50px"
                                />
                                <Stack gap={"8px"}>
                                    <Typography
                                        variant="body1"
                                        fontWeight={"600"}
                                    >
                                        {capitalizeText(persona.NOME)}
                                    </Typography>
                                </Stack>
                            </Stack>
                            {persona.FOLLOWED_BY_ME ? (
                                <Typography
                                    variant="body1"
                                    fontSize={"12px"}
                                    color={"text.disabled"}
                                >
                                    Seguindo
                                </Typography>
                            ) : idPersonAccount ==
                              persona.IDGDA_PERSONA_USER ? (
                                <></>
                            ) : (
                                <Button
                                    variant="contained"
                                    size="small"
                                    sx={{ minWidth: "fit-content" }}
                                    onClick={() =>
                                        follow(persona.IDGDA_PERSONA_USER)
                                    }
                                >
                                    Seguir
                                </Button>
                            )}
                        </Stack>
                    ))
                ) : (
                    <Typography>Sem resultados</Typography>
                )}
            </Stack>
        </Popover>
    );
}
