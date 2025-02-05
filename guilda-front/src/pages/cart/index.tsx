import ShoppingCartOutlined from "@mui/icons-material/ShoppingCartOutlined";
import Feedback from "@mui/icons-material/Feedback";
import {
    Box,
    Button,
    Checkbox,
    FormControlLabel,
    FormGroup,
    TextField,
    Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { Stack } from "@mui/system";
import { useRouter } from "next/router";
import { useContext, useState } from "react";
import { Card, PageHeader } from "src/components";
import { ShoppingCartContext } from "src/contexts/shopping-cart-context/shopping-cart-context";
import { CartItem } from "src/modules/cart/components/cart-item";
import { getLayout } from "src/utils";
import { formatCurrency } from "src/utils/format-currency";
import { UserInfoContext } from "src/contexts/user-context/user.context";

export default function CartView() {
    const { cartData, createOrder, getShoppingCartData } = useContext(ShoppingCartContext);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const handleClick = async () => {
        if (isSubmitting) return;
        setIsSubmitting(true);
        await getShoppingCartData;

        try {
            console.log("Entrou Order");
            await createOrder(observation);
        } finally {
            setIsSubmitting(false); // ou manter desabilitado, dependendo da lógica
        }
    };

    const { myUser } = useContext(UserInfoContext);
    const router = useRouter();
    const [observation, setObservation] = useState("");
    const [isPsOpen, setIsPsOpen] = useState(false);
    const [isFlagParent, setIsFlagParent] = useState(false);

    // Função para verificar se algum item tem status "inativo"
    const hasInactiveItem = cartData.some(
        (item) => item?.stockProduct?.product?.productsStatus?.status === "Inativo" || item?.amount > item?.stockProduct?.amount
    );

    console.log("a: " + hasInactiveItem);

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

    const handleFlagParent = (
        e: React.ChangeEvent<HTMLInputElement>,
        bc: string
    ) => {
        if (e.target.checked) {
            setObservation("Superior Imediato: " + bc);
        } else {
            setObservation("");
        }
        setIsFlagParent(e.target.checked);
    };

    return (
        <Box display={"flex"} gap={"30px"} width={"100%"} position={"relative"}>
            <Box
                display={"flex"}
                flexDirection={"column"}
                gap={"16px"}
                width={"100%"}
            >
                {cartData.map((item, index) => (
                    <CartItem
                        key={index}
                        name={item?.stockProduct?.product?.comercialName}
                        quantity={item.amount}
                        price={item?.stockProduct?.product?.price}
                        status={item?.stockProduct?.product?.productsStatus?.status}
                        image={
                            item?.stockProduct?.product?.productImages[0]
                                ?.upload?.url
                        }
                        limit={item?.stockProduct?.product?.saleLimit}
                        stockProductId={item?.stockProductId}
                        amountStock={item?.stockProduct?.amount}
                    />
                ))}
                {cartData.length <= 0 && (
                    <Box
                        width={"100%"}
                        height={"300px"}
                        borderRadius={"7px"}
                        bgcolor={"#fff"}
                        justifyContent={"center"}
                        alignItems={"center"}
                        display={"flex"}
                        flexDirection={"column"}
                        gap={"10px"}
                    >
                        <ShoppingCartOutlined
                            sx={{ fontSize: "60px", color: grey[500] }}
                        />
                        <Typography
                            variant="body1"
                            fontSize={"14px"}
                            fontWeight={"500"}
                            sx={{ color: grey[600] }}
                        >
                            Seu carrinho está vázio!
                        </Typography>
                        <Button
                            variant="contained"
                            onClick={() => router.push("/marketing-place")}
                        >
                            Ir para a loja
                        </Button>
                    </Box>
                )}
            </Box>
            <Box
                display={"flex"}
                flexDirection={"column"}
                gap={"16px"}
                width={"500px"}
                height={"100%"}
                position={"relative"}
            >
                <Box
                    width={"100%"}
                    bgcolor={"#fff"}
                    position={"sticky"}
                    top={20}
                >
                    <Box p={"15px 20px"} borderBottom={"solid 1px #e1e1e1"}>
                        <Typography
                            variant="body1"
                            fontSize={"14px"}
                            fontWeight={"500"}
                        >
                            Resumo da compra
                        </Typography>
                    </Box>
                    <Box
                        p={"15px 20px"}
                        gap={"10px"}
                        display={"flex"}
                        flexDirection={"column"}
                    >
                        <Box display={"flex"} justifyContent={"space-between"}>
                            <Typography
                                variant="body2"
                                fontSize={"12px"}
                                sx={{ color: "#666" }}
                            >
                                Produtos ({cartData.length})
                            </Typography>
                            <Typography
                                variant="body1"
                                fontSize={"12px"}
                                sx={{ color: "#666" }}
                            >
                                {calcTotalCartPrice()} moedas
                            </Typography>
                        </Box>
                        <Box
                            display={"flex"}
                            justifyContent={"space-between"}
                            mt={"10px"}
                        >
                            <Typography
                                variant="body2"
                                fontSize={"14px"}
                                fontWeight={"600"}
                            >
                                Total
                            </Typography>
                            <Typography
                                variant="body1"
                                fontSize={"14px"}
                                fontWeight={"600"}
                            >
                                {calcTotalCartPrice()} moedas
                            </Typography>
                        </Box>
                        {isPsOpen || observation ? (
                            <>
                                <TextField
                                    sx={{ mt: "10px" }}
                                    size="small"
                                    label="Quem pode receber o produto?"
                                    value={observation}
                                    onChange={(e) =>
                                        setObservation(e.target.value)
                                    }
                                    disabled={isFlagParent}
                                />
                                {myUser && myUser?.parent !== null ? (
                                    <>
                                        <FormGroup>
                                            <FormControlLabel
                                                control={
                                                    <Checkbox
                                                        checked={isFlagParent}
                                                        onChange={(event) =>
                                                            handleFlagParent(
                                                                event,
                                                                myUser.parent
                                                                    ?.registry ||
                                                                ""
                                                            )
                                                        }
                                                    />
                                                }
                                                label={
                                                    <Typography
                                                        style={{
                                                            fontSize: "12px",
                                                        }}
                                                    >
                                                        {"Autorizo meu superior imediato " +
                                                            myUser.parent.name +
                                                            ", " +
                                                            myUser.parent
                                                                .registry +
                                                            " a retirar meus produtos."}
                                                    </Typography>
                                                }
                                            />
                                        </FormGroup>
                                    </>
                                ) : (
                                    ""
                                )}
                            </>
                        ) : (
                            <Typography
                                variant="body2"
                                fontSize={"12px"}
                                fontWeight={"600"}
                                color={"primary"}
                                display={"flex"}
                                alignItems={"center"}
                                gap={"5px"}
                                width={"100%"}
                                justifyContent={"center"}
                                sx={{ cursor: "pointer" }}
                                onClick={() => setIsPsOpen(true)}
                            >
                                <Feedback sx={{ fontSize: "16px" }} />
                                Quem pode receber o produto?
                            </Typography>
                        )}
                        <Button
                            variant="contained"
                            sx={{ mt: "10px" }}
                            /*  onClick={() => createOrder(observation)} */
                            onClick={handleClick}
                            disabled={isSubmitting || hasInactiveItem}
                        >
                            Finalizar compra
                        </Button>
                        <Button onClick={() => router.push("/marketing-place")}>
                            Continuar comprando
                        </Button>
                    </Box>
                </Box>
            </Box>
        </Box>
    );
}

CartView.getLayout = getLayout("private");
