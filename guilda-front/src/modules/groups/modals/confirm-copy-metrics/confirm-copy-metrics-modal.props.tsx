import { BaseModal, BaseModalProps } from "../../../../components/feedback";
import { Box, Button, Typography } from "@mui/material";
import Warning from "@mui/icons-material/Warning";

type ConfirmCopyMetricsModalProps = BaseModalProps & {
    onConfirm: () => void;
};
export function ConfirmCopyMetricsModal({
    open,
    onClose,
    onConfirm,
}: ConfirmCopyMetricsModalProps) {
    return (
        <BaseModal width={"400px"} hideHeader open={open} onClose={onClose}>
            <Box display={"flex"} justifyContent={"center"}>
                <Warning color={"warning"} sx={{ fontSize: "8rem" }} />
            </Box>

            <Typography
                textAlign={"center"}
                fontWeight={200}
                fontSize={"0.85rem"}
            >
                Você deseja confirmar a cópia das métricas?
            </Typography>
            <Typography
                textAlign={"center"}
                fontWeight={200}
                fontSize={"0.85rem"}
            >
                Essa ação não poderá ser revertida
            </Typography>

            <Box display={"flex"} justifyContent={"space-between"} mt={5}>
                <Button onClick={onClose}>Cancelar</Button>
                <Button onClick={onConfirm} variant={"contained"}>
                    Confirmar
                </Button>
            </Box>
        </BaseModal>
    );
}
