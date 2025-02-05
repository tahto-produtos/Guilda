import { createTheme } from "@mui/material";
import { lightTheme } from "./themes";
import { createBreakpoints } from "@mui/system";
import { grey } from "@mui/material/colors";
import "@mui/x-data-grid/themeAugmentation";
import type {} from "@mui/x-date-pickers/themeAugmentation";
import { ptBR } from "date-fns/locale";

const breakpoints = createBreakpoints({});

export const muiTheme = createTheme(
  {
    breakpoints: breakpoints,
    components: {
      MuiCssBaseline: {
        styleOverrides: {
          ["*"]: {
            scrollBehavior: "smooth",
          },
          margin: 0,
          padding: 0,
          a: {
            textDecoration: "none",
          },
          "a:visited": {
            textDecoration: "none",
          },
          "a:link": {
            textDecoration: "none",
          },
          "a:hover": {
            textDecoration: "none",
          },
          "a:active": {
            textDecoration: "none",
          },
        },
      },

      MuiDivider: {
        styleOverrides: {
          root: {
            borderColor: lightTheme.palette.grey[300],
          },
        },
      },

      MuiAutocomplete: {
        styleOverrides: {
          root: {
            marginBottom: "20px",
          },
        },
        defaultProps: {
          noOptionsText: "Opções indisponíveis",
          disableClearable: true,
        },
      },
      MuiInputLabel: {
        styleOverrides: {
          root: {
            color: lightTheme.palette.text.disabled,
            fontFamily: "Montserrat",
            fontWeight: "500",
            fontSize: "16px",
            width: "fit-content",
            background: lightTheme.palette.background.paper,
            paddingRight: "5px",
            lineHeight: "20px",
          },
          sizeSmall: {
            lineHeight: "15px",
            fontSize: "13px",
            fontWeight: "500",
          },
        },
      },
      MuiTypography: {
        styleOverrides: {
          h1: {
            fontFamily: "Montserrat",
            fontSize: "24px",
            fontWeight: "500",
          },
          h2: {
            fontFamily: "Open Sans, Sans Serif",
            fontSize: "20px",
            fontWeight: "600",
          },
          h3: {
            fontFamily: "Montserrat",
            fontSize: "16px",
            fontWeight: "600",
          },
          h5: {
            fontFamily: "Open Sans, Sans Serif",
            fontSize: "14px",
            lineHeight: "14px",
            fontWeight: "600",
          },
          h6: {
            fontFamily: "Open Sans, Sans Serif",
            fontSize: "12px",
            lineHeight: "12px",
            fontWeight: "600",
          },
          body1: {
            fontFamily: "Open Sans, Sans Serif",
            fontSize: "14px",
            fontWeight: "400",
          },

          subtitle1: {
            fontFamily: "Open Sans, Sans Serif",
            fontSize: "1.25rem",
            fontWeight: "200",
            lineHeight: "1.5",
          },
          subtitle2: {
            fontFamily: "Open Sans, Sans Serif",
            fontSize: "1rem",
            fontWeight: "500",
            lineHeight: "1.5",
            marginBottom: "20px",
          },
          caption: {
            fontFamily: "Open Sans, Sans Serif",
            fontSize: "14px",
            fontWeight: "500",
            lineHeight: "1.5",
          },
        },
      },
      MuiButton: {
        defaultProps: {
          disableElevation: true,
          color: "secondary",
        },
        styleOverrides: {
          sizeSmall: {
            fontSize: "14px",
            fontFamily: "Montserrat",
            height: "33px",
            fontWeight: "400",
            paddingLeft: "16px",
            paddingRight: "16px",
          },
          root: {
            borderRadius: "8px",
            fontWeight: "500",
            textTransform: "none",
            fontFamily: "Montserrat",
            fontSize: "0.85rem",
            color: "#fff",
          },
          sizeMedium: {
            height: "38px",
          },
          outlined: {
            color: lightTheme.palette.text.primary,
            borderColor: lightTheme.palette.grey[300],
            lineHeight: "38px",
          },
          text: {
            color: lightTheme.palette.text.primary,
            minWidth: "fit-content",
          },
        },
      },
      MuiPaper: {
        styleOverrides: {
          elevation1: {
            boxShadow: "0 10px 30px rgb(0 0 0 / 10%)",
          },
        },
      },
      // MuiTextField: {
      //     defaultProps: {
      //         inputProps: {
      //             style: {
      //                 height: "52px",
      //                 paddingTop: 0,
      //                 paddingBottom: 0,
      //             },
      //         },
      //     },
      // },
      MuiInputBase: {
        styleOverrides: {
          root: {
            height: "52px",
          },
          sizeSmall: {
            height: "33px",
          },
          multiline: {
            height: "auto",
          },
        },
      },
      MuiOutlinedInput: {
        styleOverrides: {
          root: {
            "&:hover .MuiOutlinedInput-notchedOutline": {
              borderColor: "#e0e0e0",
            },
            fontSize: "16px",
            fontFamily: "Montserrat",
            fontWeight: "500",
          },
          sizeSmall: {
            fontSize: "13px",
            fontFamily: "Montserrat",
            fontWeight: "500",
          },
          notchedOutline: {
            borderColor: lightTheme.palette.grey[700],
            borderRadius: "8px",
          },
        },
      },
      MuiStack: {},
      MuiFormHelperText: {
        styleOverrides: {
          root: {
            marginLeft: "0",
            marginTop: "0",
            marginBottom: "4px",
          },
        },
      },
      MuiDatePicker: {
        defaultProps: {
          format: "dd/MM/yyyy",
        },
      },
      MuiLocalizationProvider: {
        defaultProps: {
          adapterLocale: ptBR,
        },
      },
      MuiDataGrid: {
        defaultProps: {
          rowHeight: 76,
        },
        styleOverrides: {
          root: {
            borderColor: "#eee",
          },
          cell: {
            border: "none",
          },
          columnHeader: {
            backgroundColor: "#FAFAFA",
            fontFamily: "Open Sans",
          },
          columnHeaderTitle: {
            fontSize: "16px",
            color: "#000",
            fontWeight: "700",
            fontFamily: "Open Sans",
          },
        },
      },
    },
  },
  lightTheme
);
