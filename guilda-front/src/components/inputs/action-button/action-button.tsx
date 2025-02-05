import { LoadingButton } from "@mui/lab";
import { Button, darken, useTheme } from "@mui/material";
import { ReactNode } from "react";

interface ActionButtonProps {
    title: string;
    icon?: ReactNode;
    onClick?: () => void;
    isActive?: boolean;
    loading?: boolean;
    border?: string;
    size?: "small" | "medium" | "large";
    disabled?: boolean;
}

export function ActionButton(props: ActionButtonProps) {
    const theme = useTheme();
    const { icon, title, onClick, isActive, loading, border, size, disabled } = props;

    return (
        <LoadingButton
            variant={isActive ? "outlined" : "contained"}
            size={size || "medium"}
            sx={{
                border: border,
                backgroundColor: isActive
                    ? theme.palette.background.default
                    : theme.palette.grey[400],
                "&:hover": {
                    backgroundColor: isActive
                        ? theme.palette.grey[100]
                        : darken(theme.palette.grey[400], 0.1),
                },
                color: theme.palette.text.primary,
                fontFamily: "Open Sans",
                minWidth: "fit-content",
            }}
            startIcon={icon}
            onClick={onClick}
            loading={loading}
            component={"span"}
            disabled={disabled ?  disabled : false}
        >
            {title}
        </LoadingButton>
    );
}
