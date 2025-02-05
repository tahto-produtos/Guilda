import ErrorOutline from "@mui/icons-material/ErrorOutline";
import { Stack, Typography, useTheme } from "@mui/material";

interface EmptyStateProps {
    title: string;
}

export function EmptyState(props: EmptyStateProps) {
    const theme = useTheme();

    return (
        <Stack
            width={"100%"}
            height={"300px"}
            borderRadius={"16px"}
            border={`solid 1px ${theme.palette.grey["200"]}`}
            alignItems={"center"}
            justifyContent={"center"}
            gap={"10px"}
        >
            <ErrorOutline color="primary" />
            <Typography variant="body1">{props.title}</Typography>
        </Stack>
    );
}
