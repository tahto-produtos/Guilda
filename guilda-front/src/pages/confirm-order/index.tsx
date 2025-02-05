import Add from "@mui/icons-material/Add";
import { Box, Container, Stack, Typography } from "@mui/material";
import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useLoadingState } from "src/hooks";
import { ConfirmQrCodeUseCase } from "src/modules/marketing-place/use-cases/confirm-qr-code.use-case";
import { getLayout } from "src/utils";

export default function ConfirmOrder() {
    const router = useRouter();
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [success, setSuccess] = useState<boolean>(false);
    const [err, setErr] = useState<boolean>(false);
    const query = router.query;

    const collaboratorId = query.collaboratorId as string | undefined;
    const orderId = query.orderId as string | undefined;

    async function confirmQrCode() {
        if (!orderId || !collaboratorId) {
            return;
        }
        startLoading();

        const collaboratorIdNum = parseInt(collaboratorId);

        new ConfirmQrCodeUseCase()
            .handle(orderId, collaboratorIdNum)
            .then((data) => {
                setSuccess(true);
            })
            .catch(() => {
                setErr(true);
            })
            .finally(() => {
                finishLoading();
            });
    }

    useEffect(() => {
        confirmQrCode();
    }, [query]);

    return (
        <Box
            minHeight={"100vh"}
            sx={{
                background:
                    "linear-gradient(90deg, rgba(60,174,161,0.10) 0%, rgba(108,76,156,0.10) 100%)",
            }}
        >
            <Container>
                <Box
                    display={"flex"}
                    width={"100%"}
                    height={"100vh"}
                    justifyContent={"center"}
                    alignItems={"center"}
                >
                    <Card
                        width={"100%"}
                        maxWidth={"400px"}
                        display={"flex"}
                        flexDirection={"column"}
                    >
                        <Stack px={2} py={4} width={"100%"} gap={2}>
                            {isLoading && (
                                <Typography>
                                    {isLoading &&
                                        "Aguarde, estamos confirmando o pedido..."}
                                </Typography>
                            )}
                            {success && (
                                <Typography color="success">
                                    {success &&
                                        "Pedido confirmado com sucesso!"}
                                </Typography>
                            )}
                            {err && (
                                <Typography color={"error"}>
                                    {err &&
                                        "Ocorreu um erro ao tentar confirmar o pedido."}
                                </Typography>
                            )}
                        </Stack>
                    </Card>
                </Box>
            </Container>
        </Box>
    );
}
