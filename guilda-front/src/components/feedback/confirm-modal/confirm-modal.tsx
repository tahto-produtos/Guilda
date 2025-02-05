import Warning from "@mui/icons-material/Warning";
import { Button, Typography } from "@mui/material";
import { grey } from "@mui/material/colors";
import { Box } from "@mui/system";
import { BaseModal } from "../base-modal";

export interface ConfirmModalProps {
    open?: any;
    onClose?: any;
    onConfirm?: () => void;
    text?: string;
}

export function ConfirmModal({
    open,
    onClose,
    onConfirm,
    text,
}: ConfirmModalProps) {
    const handleConfirm = () => {
        onClose();
        onConfirm?.();
    };

    return (
        <BaseModal width={"400px"} hideHeader open={open} onClose={onClose}>
            <Box sx={{ padding: "20px 20px" }}>
                <Box display={"flex"} justifyContent={"center"} mb={2}>
                    <Warning color={"error"} sx={{ fontSize: "5rem" }} />
                </Box>

                <Typography
                    textAlign={"center"}
                    fontWeight={400}
                    fontSize={"0.85rem"}
                >
                    {text ? text : "Você deseja confirmar a ação?"}
                </Typography>
                <Typography
                    textAlign={"center"}
                    fontWeight={300}
                    fontSize={"0.85rem"}
                    color={grey[600]}
                >
                    Essa ação não poderá ser revertida
                </Typography>

                <Box display={"flex"} justifyContent={"center"} gap={2} mt={3}>
                    <Button
                        onClick={onClose}
                        size={"large"}
                        variant={"outlined"}
                    >
                        Cancelar
                    </Button>
                    <Button
                        onClick={handleConfirm}
                        variant={"contained"}
                        size={"large"}
                    >
                        Confirmar
                    </Button>
                </Box>
            </Box>
        </BaseModal>
    );
}
