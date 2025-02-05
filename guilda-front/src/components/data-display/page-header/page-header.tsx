import {
    Box,
    Button,
    SvgIconTypeMap,
    Tooltip,
    Typography,
} from "@mui/material";
import Add from "@mui/icons-material/Add";
import MoreVert from "@mui/icons-material/MoreVert";
import { ReactNode } from "react";
import { OverridableComponent } from "@mui/material/OverridableComponent";
import { grey } from "@mui/material/colors";

interface PageHeaderProps {
    title?: string;
    addButtonTitle?: string;
    addButtonAction?: () => void;
    addButtonIsDisabled?: boolean;
    secondayButtonAction?: () => void;
    secondaryButtonTitle?: string;
    secondaryButtonIsDisable?: boolean;
    secondaryButtonIcon?: any;
    thirdButtonAction?: () => void;
    thirdButtonTitle?: string;
    thirdButtonIsDisable?: boolean;
    thirdButtonIcon?: any;
    headerIcon?: any;
}

export function PageHeader({
    title,
    addButtonTitle,
    addButtonAction,
    addButtonIsDisabled,
    secondaryButtonTitle,
    secondayButtonAction,
    secondaryButtonIsDisable,
    secondaryButtonIcon,
    thirdButtonAction,
    thirdButtonTitle,
    thirdButtonIsDisable,
    thirdButtonIcon,
    headerIcon,
}: PageHeaderProps) {
    return (
        <Box
            display={"flex"}
            justifyContent={"space-between"}
            alignItems={"center"}
            borderBottom={`solid 1px ${grey[100]} `}
            py={"12px"}
            px={2}
        >
            <Box
                display={"flex"}
                alignItems={"center"}
                gap={1}
                sx={{ color: grey[600] }}
            >
                {headerIcon && headerIcon}

                <Typography variant={"h3"} fontSize={15}>
                    {title}
                </Typography>
            </Box>

            <Box gap={2} display={"flex"}>
                {secondaryButtonTitle && secondayButtonAction && (
                    <Button
                        startIcon={
                            secondaryButtonIcon ? (
                                secondaryButtonIcon
                            ) : (
                                <MoreVert />
                            )
                        }
                        variant={"outlined"}
                        onClick={secondayButtonAction}
                        disabled={secondaryButtonIsDisable}
                        size={"small"}
                    >
                        {secondaryButtonTitle}
                    </Button>
                )}
                {thirdButtonTitle && thirdButtonAction && (
                    <Button
                        startIcon={
                            thirdButtonIcon ? (
                                thirdButtonIcon
                            ) : (
                                <MoreVert />
                            )
                        }
                        variant={"outlined"}
                        onClick={thirdButtonAction}
                        disabled={thirdButtonIsDisable}
                        size={"small"}
                    >
                        {thirdButtonTitle}
                    </Button>
                )}
                {addButtonTitle && addButtonAction && (
                    <Tooltip
                        title="Você não tem permissão para realizar esta ação"
                        disableHoverListener={!addButtonIsDisabled}
                        arrow
                    >
                        <span>
                            <Button
                                startIcon={<Add />}
                                size={"small"}
                                variant={"contained"}
                                onClick={addButtonAction}
                                disabled={addButtonIsDisabled}
                            >
                                {addButtonTitle}
                            </Button>
                        </span>
                    </Tooltip>
                )}
            </Box>
        </Box>
    );
}
