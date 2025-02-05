import ShoppingCartOutlined from "@mui/icons-material/ShoppingCartOutlined";
import { Badge, Box, IconButton, useTheme } from "@mui/material";
import { useRouter } from "next/router";
import { useContext, useState } from "react";
import { ShoppingCartContext } from "src/contexts/shopping-cart-context/shopping-cart-context";
import { formatCurrency } from "src/utils/format-currency";

export default function ShoppingCartButton() {
    const theme = useTheme();
    const {
        cartData,
        removeItemFromCart,
        clearCart,
        shoppingCartIsLoading,
        createOrder,
        getShoppingCartData,
    } = useContext(ShoppingCartContext);
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [observationModal, setObservationModal] = useState<boolean>(false);
    const [observation, setObservation] = useState<string>("");
    const router = useRouter();

    async function handleCreate() {
        createOrder(observation);
    }

    function calcTotalCartPrice() {
        let total = 0;
        cartData.map((item) => {
            const totalItem =
                item.amount *
                parseInt(item.stockProduct.product.price.toString());
            total += totalItem;
        });

        return formatCurrency(total);
    }

    return (
        <Box
            sx={{
                display: "flex",
                position: "relative",
                alignItems: "center",
                justifyContent: "center",
                mr: "10px",
            }}
        >
            <IconButton
                onClick={async () => {
                    await getShoppingCartData(); // Atualiza o carrinho ao clicar no botão
                    router.push("/cart"); // Redireciona para a página do carrinho
                }}
                color="inherit"
                disabled={shoppingCartIsLoading}
                sx={{
                    width: "40px",
                    height: "40px",
                    border: `solid 1px ${theme.palette.grey[600]}`,
                }}
            >
                <Badge
                    badgeContent={cartData?.length}
                    color="primary"
                    sx={{
                        "& .MuiBadge-badge": {
                            right: -3,
                            top: 1,
                            border: `2px solid ${theme.palette.background.paper}`,
                            padding: "0 4px",
                            fontSize: "10px",
                            display: "flex",
                            justifyContent: "center",
                            alignItems: "center",
                            lineHeight: "15px",
                            fontWeight: "600",
                        },
                    }}
                >
                    <ShoppingCartOutlined
                        fontSize="small"
                        sx={{ color: theme.palette.grey[300] }}
                    />
                </Badge>
            </IconButton>
        </Box>
    );
}
