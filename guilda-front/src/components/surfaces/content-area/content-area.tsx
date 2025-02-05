import { Box, BoxProps, styled } from "@mui/material";

export const ContentArea = styled(Box)<BoxProps>(({ theme }) => ({
    width: "100%",
    paddingLeft: "44px",
    paddingRight: "44px",
    paddingTop: "8px",
    paddingBottom: "8px",
    display: "flex",
    flexDirection: "column",
}));
