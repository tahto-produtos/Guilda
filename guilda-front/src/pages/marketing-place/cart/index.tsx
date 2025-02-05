import { Box, Button, Stack, Typography } from "@mui/material";
import { grey } from "@mui/material/colors";
import { Card, PageHeader } from "src/components";
import { getLayout } from "src/utils";

const CartItem = () => {
    return (
        <Box
            border={`solid 1px ${grey[200]}`}
            display={"flex"}
            width={"100%"}
            borderRadius={2}
            p={2}
            alignItems={"center"}
            justifyContent={"space-between"}
        >
            <Box display={"flex"} alignItems={"center"} gap={2}>
                <Box
                    sx={{
                        width: "70px",
                        height: "70px",
                        backgroundColor: grey[100],
                        borderRadius: 2,
                    }}
                ></Box>
                <Box>
                    <Typography variant="body2" fontSize={"16px"}>
                        Nome do Produto
                    </Typography>
                    <Typography
                        variant="body2"
                        fontSize={"14px"}
                        color={grey[600]}
                    >
                        400 moedas
                    </Typography>
                </Box>
            </Box>
            <Button color="error">Remover</Button>
        </Box>
    );
};

export default function Cart() {
    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader title={"Seu carrinho"} />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                <Box display={"flex"} flexDirection={"column"} gap={1}>
                    <CartItem />
                    <CartItem />
                    <CartItem />
                    <CartItem />
                </Box>
                <Box
                    justifyContent={"flex-end"}
                    display={"flex"}
                    width={"100%"}
                    gap={2}
                >
                    <Button variant="outlined" color="primary">
                        Continuar comprando
                    </Button>
                    <Button variant="contained" color="success">
                        Finalizar compra
                    </Button>
                </Box>
            </Stack>
        </Card>
    );
}

Cart.getLayout = getLayout("private");
