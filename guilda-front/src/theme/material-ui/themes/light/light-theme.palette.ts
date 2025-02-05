import { createTheme } from "@mui/material";

export const lightThemePalette = createTheme({
    palette: {
        primary: {
            main: "#692FAE",
            light: "#9a7dc5",
        },
        background: {
            paper: "#fff",
        },
        text: {
            primary: "#000",
            disabled: "#A6A6A6",
        },
        secondary: {
            main: "#2FAC9F",
            dark: "#3B93A2",
            "400": "#2FAC9F",
        },
        error: {
            main: "#FE4A4A",
        },
        info: {
            main: "#06CEFC",
            contrastText: "white",
            dark: "#175CFFFF",
        },
        warning: {
            main: "#ffb116",
            contrastText: "#000",
        },
        grey: {
            900: "#404040",
            700: "#8C8C8C",
            600: "#A6A6A6",
            400: "#E5E5E5",
            300: "#BFBFBF",
            200: "#D9D9D9",
            100: "#FAFAFA",
        },
    },
});
