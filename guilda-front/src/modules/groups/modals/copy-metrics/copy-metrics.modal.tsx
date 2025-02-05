import { BaseModal, BaseModalProps } from "../../../../components/feedback";
import { CopyMetricsForm } from "../../forms/copy-metrics/copy-metrics.form";
import { Alert, Box, Button } from "@mui/material";
import { CopyMetricsSchema } from "../../forms/copy-metrics/copy-metrics.schema";

type CopyMetricsModalProps = BaseModalProps & {
    onConfirm: (form: CopyMetricsSchema) => void;
};
export function CopyMetricsModal({
    open,
    onClose,
    onConfirm,
}: CopyMetricsModalProps) {
    return (
        <BaseModal
            width={"540px"}
            open={open}
            title={"Copiar métricas"}
            onClose={onClose}
        >
            <Box mt={1}>
                <CopyMetricsForm
                    id={"copy-metrics-form"}
                    onSubmit={onConfirm}
                />
            </Box>
            <Alert severity="warning">
                Indicadores em amarelo não estão associados.
            </Alert>
            <Box display={"flex"} justifyContent={"space-between"} mt={3}>
                <Button variant={"text"} onClick={onClose}>
                    Cancelar
                </Button>
                <Button
                    type={"submit"}
                    form={"copy-metrics-form"}
                    variant={"contained"}
                    disabled={false}
                >
                    Copiar métricas
                </Button>
            </Box>
        </BaseModal>
    );
}
