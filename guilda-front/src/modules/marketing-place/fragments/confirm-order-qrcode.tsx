import { Box, Button, CardMedia, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { BaseModal } from "src/components/feedback";
import { GenerateQrCodeUseCase } from "../use-cases/generate-qrcode.use-case";

interface ConfirmOrderQrCodeProps {
    orderId: number;
    open: boolean;
    onClose: () => void;
}

export function ConfirmOrderQrCodeModal(props: ConfirmOrderQrCodeProps) {
    const { onClose, open, orderId } = props;
    const [isLoadingQrCode, setIsLoadingQrCode] = useState<boolean>(false);
    const [err, setErr] = useState<string | null>(null);
    const [qrCode, setQrCode] = useState<string | null>(null);

    async function generateQrCode() {
        setIsLoadingQrCode(true);
        await new GenerateQrCodeUseCase()
            .handle(orderId)
            .then((data) => {
                setQrCode(data.qrCode);
                setErr(null);
            })
            .catch((e) => {
                console.log(e);
                setErr("Ocorreu um erro ao gerar o QRCode");
            })
            .finally(() => {
                setIsLoadingQrCode(false);
            });
    }

    useEffect(() => {
        !qrCode && generateQrCode();
    }, []);

    return (
        <BaseModal
            width={"540px"}
            open={open}
            title={"Confirmar pedido"}
            onClose={onClose}
        >
            <Box
                width={"100%"}
                display={"flex"}
                flexDirection={"column"}
                justifyContent={"center"}
                alignItems={"center"}
                gap={2}
                py={4}
            >
                <Typography textAlign={"center"} variant="body2">
                    Escaneie o QRCode para confirmar o pedido.
                </Typography>
                {qrCode && (
                    <CardMedia
                        component="img"
                        image={qrCode}
                        sx={{
                            width: "100px",
                            objectFit: "contain",
                        }}
                    />
                )}
                {err && (
                    <Typography variant="overline" color="error">
                        {err}
                    </Typography>
                )}
            </Box>
        </BaseModal>
    );
}
