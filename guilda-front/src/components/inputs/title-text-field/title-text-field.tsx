import { Stack, Typography } from "@mui/material";
import { ReactNode } from "react";

interface TextFieldTitleProps {
    children: ReactNode;
    title: string;
}

export function TextFieldTitle(props: TextFieldTitleProps) {
    return (
        <Stack direction={"column"} gap={"8px"} width={"100%"}>
            <Typography
                fontFamily={"Montserrat"}
                fontSize={"16px"}
                fontWeight={"500"}
            >
                {props.title}
            </Typography>
            <Stack>{props.children}</Stack>
        </Stack>
    );
}
