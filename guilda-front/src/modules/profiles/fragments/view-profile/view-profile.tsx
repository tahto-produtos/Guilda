import {
    Stack,
    useTheme,
} from "@mui/material";
import { useEffect, useState } from "react";
import { Persona } from "src/typings/models/persona.model";

interface ViewProfileComponentProps {
    initialState?: Persona;
    idPersona: number;
}

export function ViewProfileComponent(props: ViewProfileComponentProps) {
    const { initialState, idPersona } = props;

    const theme = useTheme();

    return (
        <Stack width={"100%"} mt={"25px"}>
            <p>Meu perfil</p>
        </Stack>
    );
}
