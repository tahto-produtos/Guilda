import ProductionQuantityLimitsOutlined from "@mui/icons-material/ProductionQuantityLimitsOutlined";
import { Box, CardMedia, LinearProgress, Typography } from "@mui/material";
import { grey } from "@mui/material/colors";
import { Console } from "console";
import { useContext } from "react";
import { ShoppingCartContext } from "src/contexts/shopping-cart-context/shopping-cart-context";
import { formatCurrency } from "src/utils/format-currency";

interface IProps {
    name?: string | number;
    image?: string;
    price?: string | number;
    status?: string;
    quantity?: number;
    limit?: number;
    stockProductId: number;
    amountStock?: number;
}

export function CartItem(props: IProps) {
    const { image, name, price, status, amountStock, quantity, limit, stockProductId } = props;

    const { removeItemFromCart } = useContext(ShoppingCartContext);

    return (
        <Box
            width={"100%"}
            bgcolor={"#fff"}
            borderRadius={"7px"}
            display={"flex"}
            flexDirection={"column"}
            border={`solid 1px ${grey[300]}`}
        >
            <Box
                p={"16px 24px"}
                width={"100%"}
                display={"flex"}
                alignItems={"center"}
                height={"100%"}
                gap={"20px"}
                justifyContent={"space-between"}
            >
                <Box display={"flex"} alignItems={"center"} gap={"20px"}>
                    {image && (
                        <CardMedia
                            component="img"
                            alt="Uploaded image"
                            sx={{ width: "50px" }}
                            image={image}
                        />
                    )}
                    <Box>
                        <Typography
                            variant="body1"
                            fontSize={"12px"}
                            fontWeight={"400"}
                        >
                            Quantidade: {quantity}
                        </Typography>
                        <Typography
                            variant="body1"
                            fontSize={"14px"}
                            fontWeight={"500"}
                        >
                            {name}
                        </Typography>
                        <Typography
                            variant="body1"
                            fontSize={"12px"}
                            fontWeight={"500"}
                            color="error"
                            sx={{ textWrap: "nowrap", cursor: "pointer" }}
                            width={"max-content"}
                            onClick={() => removeItemFromCart(stockProductId)}
                        >
                            Remover
                        </Typography>
                    </Box>
                </Box>
                <Box>

                    <Typography
                        variant="body1"
                        fontSize={"14px"}
                        fontWeight={"500"}
                        sx={{ textWrap: "nowrap" }}
                    >
                        {price &&
                            quantity &&
                            formatCurrency(
                                parseInt(price.toString()) * quantity
                            )}{" "}
                        moedas
                    </Typography>
                    <Typography
                        variant="body1"
                        fontSize={"14px"}
                        fontWeight={"500"}
                        sx={{ textWrap: "nowrap" }}
                        color="error"
                    >
                        {status && status.toString() == "Inativo" && status.toString()}
                    </Typography>
                    <Typography
                        variant="body1"
                        fontSize={"14px"}
                        fontWeight={"500"}
                        sx={{ textWrap: "nowrap" }}
                        color="error"
                    >
                        <>{console.log("amountStock" + amountStock)}</>
                        
                        {amountStock && quantity && amountStock < quantity && "Produto sem estoque!"}
                    </Typography>
                </Box>
            </Box>
            {!!limit && !!quantity && (
                <Box
                    width={"100%"}
                    p={"16px 24px"}
                    bgcolor={grey[200]}
                    borderRadius={"0px 0px 7px 7px"}
                >
                    <Box display={"flex"} alignItems={"center"} gap={"20px"}>
                        <LinearProgress
                            variant="determinate"
                            value={(quantity / limit) * 100}
                            sx={{ width: "100%" }}
                        />
                        <Typography
                            variant="body1"
                            fontSize={"11px"}
                            fontWeight={"400"}
                            sx={{ color: grey[600], textWrap: "nowrap" }}
                            width={"max-content"}
                        >
                            Limite de compra: {limit}
                        </Typography>
                    </Box>
                    {quantity >= limit && (
                        <Typography
                            variant="body1"
                            fontSize={"12px"}
                            fontWeight={"400"}
                            alignItems={"center"}
                            display={"flex"}
                            gap={"5px"}
                        >
                            <ProductionQuantityLimitsOutlined
                                sx={{ fontSize: "14px" }}
                            />
                            {quantity >= limit &&
                                "Você atingiu o limite de compras para este produto"}
                        </Typography>
                    )}
                </Box>
            )}
        </Box>
    );
}
