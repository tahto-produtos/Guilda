import { ReactNode } from "react";
import {
    Box,
    Dialog,
    Divider,
    IconButton,
    SxProps,
    Typography,
    useTheme,
} from "@mui/material";
import Close from "@mui/icons-material/Close";
import { width } from "@mui/system";

export interface BaseModalProps {
    open: boolean;
    onClose: () => void;
}

interface ControlledModalProps extends BaseModalProps {
    title?: string;
    children: ReactNode;
    disableCloseButton?: boolean;
    sx?: SxProps;
    hideHeader?: boolean;
    width?: string;
    startAdornment?: ReactNode;
    endAdornment?: ReactNode;
    fullWidth?: boolean;
}

export function BaseModal({
    title,
    open,
    onClose,
    children,
    disableCloseButton = false,
    hideHeader = false,
    sx,
    width,
    endAdornment,
    startAdornment,
    fullWidth,
}: ControlledModalProps) {
    const theme = useTheme();

    return (
        <Dialog
            open={open}
            onClose={onClose}
            maxWidth={fullWidth ? "xl" : "sm"}
            PaperProps={{ sx: { borderRadius: "16px", maxWidth: "1000px"} }}
        >
            <Box width={width} sx={sx}>
                {!hideHeader && (
                    <Box
                        display={"flex"}
                        alignItems={"center"}
                        justifyContent={"space-between"}
                        width={"100%"}
                        px={"32px"}
                        pt={"38px"}
                        pb={"26px"}
                    >
                        <Typography variant="h2">{title}</Typography>
                        {!disableCloseButton && (
                            <IconButton
                                onClick={onClose}
                                sx={{ color: theme.palette.text.primary }}
                            >
                                <Close />
                            </IconButton>
                        )}
                    </Box>
                )}
                {children && (
                    <>
                        <Box px={"32px"} pb={"30px"}>
                            {children}
                        </Box>
                    </>
                )}
            </Box>
        </Dialog>
    );
}
