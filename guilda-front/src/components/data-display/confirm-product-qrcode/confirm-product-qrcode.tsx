import { Box, CardMedia } from "@mui/material";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { BaseModal } from "src/components/feedback";
import { GenerateConfirmProductQrCodeUseCase } from "src/modules/marketing-place/use-cases/generate-confirm-product-qrcode.use-case";

interface IProps {
    isOpen: boolean;
    onClose: () => void;
    productId: number;
    orderId: number;
}

export function ConfirmProductQrCodeModal(props: IProps) {
    const { isOpen, onClose, productId, orderId } = props;
    const [qrCode, setQrCode] = useState<string | null>(null);

    async function generateQrCode() {
        await new GenerateConfirmProductQrCodeUseCase()
            .handle({
                productId,
                orderId,
            })
            .then((data) => {
                setQrCode(data.qrCode);
            })
            .catch((e) => {
                toast.error("Erro ao carregar o QrCode");
            });
    }

    useEffect(() => {
        generateQrCode();
    }, []);

    return (
        <BaseModal
            width={"540px"}
            open={isOpen}
            title={`QrCode`}
            onClose={onClose}
        >
            <Box
                width={"100%"}
                display={"flex"}
                flexDirection={"column"}
                gap={1}
            >
                {qrCode && (
                    <CardMedia
                        component="img"
                        image={qrCode}
                        sx={{
                            width: "300px",
                            objectFit: "contain",
                        }}
                    />
                )}
            </Box>
        </BaseModal>
    );
}
