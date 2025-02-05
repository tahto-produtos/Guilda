import { Box, BoxProps, styled } from "@mui/material";

export const ContentCard = styled(Box)<BoxProps>(({ theme }) => ({
    width: "100%",
    display: "flex",
    flexDirection: "column",
    backgroundColor: theme.palette.background.paper,
    borderRadius: "16px",
    padding: "24px",
}));
