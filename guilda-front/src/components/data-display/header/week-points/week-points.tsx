import { useContext } from "react";
import { Stack, Typography, useTheme } from "@mui/material";
import {UserInfoContext} from "../../../../contexts/user-context/user.context";

export function WeekPoints() {
    const theme = useTheme();
    const { daysLogged } = useContext(UserInfoContext);

    if(!daysLogged) return null;

    function Item(props: { letter: string; active: boolean }) {
        const { active, letter } = props;

        return (
            <Stack direction={"column"} gap={"2px"} alignItems={"center"}>
                <Typography
                    variant="body1"
                    fontWeight={"700"}
                    fontSize={"16px"}
                    color={active ? "secondary" : theme.palette.text.primary}
                >
                    {letter}
                </Typography>
                <Stack
                    width={"8px"}
                    height={"8px"}
                    bgcolor={
                        active
                            ? theme.palette.secondary.main
                            : theme.palette.grey[200]
                    }
                    borderRadius={"10px"}
                />
            </Stack>
        );
    }

    return (
        <Stack direction={"column"} gap={"10px"}>
            <Stack direction={"row"} gap={"12px"}>
                { daysLogged.map((dayLogged, index) => (
                    <Item key={index} active={dayLogged.LOGIN == 1} letter={`${dayLogged.ACRONYM}`} />
                ))
                }
            </Stack>
            <Typography
                variant="body1"
                fontSize={"12px"}
                fontWeight={"700"}
                color={"secondary"}
                width={"100%"}
                textAlign={"center"}
            >
                Saiba mais
            </Typography>
        </Stack>
    );
}
