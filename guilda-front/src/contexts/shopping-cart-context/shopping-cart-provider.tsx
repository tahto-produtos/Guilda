import { ReactNode, useContext, useEffect, useState } from "react";
import { ShoppingCartContext } from "./shopping-cart-context";
import { CartDataModel } from "src/typings/models/cart-data.model";
import { ShoppingCartUseCase } from "src/modules/marketing-place/use-cases/cart.use-case";
import { ShoppingCartRemoveUseCase } from "src/modules/marketing-place/use-cases/cart.use-case-remove";
import { toast } from "react-toastify";
import { CreateOrderUseCase } from "src/modules/marketing-place/use-cases/create-order";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import Cookies from "js-cookie";
import { GenerateQrCodeUseCase } from "src/modules/marketing-place/use-cases/generate-qrcode.use-case";
import { UserInfoContext } from "../user-context/user.context";

interface IProviderProps {
    children: ReactNode;
}

export function ShoppingCartProvider({ children }: IProviderProps) {
    const { myUser, userGroup } = useContext(UserInfoContext);
    const userToken = Cookies.get("jwtToken");
    const [cartData, setCartData] = useState<CartDataModel[]>([]);
    const [shoppingCartIsLoading, setShoppingCartIsLoading] =
        useState<boolean>(false);
    const [updatingShoppingCart, setUpdatingShoppingCart] =
        useState<boolean>(false);
    const [observationOrder, setObservationOrder] = useState<string>("");

    async function getShoppingCartData() {
        setShoppingCartIsLoading(true);

        await new ShoppingCartUseCase()
            .read()
            .then((data) => {
                setCartData(data);
            })
            .catch((e) => {
                console.log(e);
            })
            .finally(() => {
                setShoppingCartIsLoading(false);
            });
    }

    async function createOrder(whoReceived: string) {
        //console.log("Aqui");
        //console.log("Aqui2: " + userGroup);
        let userGG = 4;
        if (userGroup) {
            userGG = userGroup;
        };
        //if (!userGroup) return;
        //console.log("Aqui2: " + userGroup);
        setShoppingCartIsLoading(true);

        const payload = {
            whoReceived: whoReceived,
            userGroup: userGG,
        };

        await new CreateOrderUseCase()
            .handle(payload)
            .then((data) => {
                toast.success("Pedido criado com sucesso!");
                getShoppingCartData();
            })
            .catch((e) => {

                if (e?.response?.data?.Message !== "") {
                    toast.warning(e?.response?.data?.Message);
                } else {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                }
               /*  if (e?.response?.data?.keys?.quantity) {
                    return toast.error(
                        `
                            É possível comprar apenas ${e?.response?.data?.keys?.quantity}
                            itens para o produto: ${e?.response?.data?.keys?.product}
                            e você ja comprou: ${e?.response?.data?.keys?.bought}
                        `
                    );
                }
                if (e?.response?.data?.code == "OUT_OF_RANGE") {
                    toast.warning("Você não tem moedas suficientes");
                }
                else if (e?.response?.data?.Message == "Moedas insuficientes!") {
                    toast.warning("Você não tem moedas suficientes");
                } else if (e?.response?.data?.Message == "Nenhum produto no carrinho!") {
                    toast.warning("Nenhum produto no carrinho!");
                } else {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                } */
            })
            .finally(() => {
                setShoppingCartIsLoading(false);
            });
    }

    async function updateCart(product: {
        stockProductId: number;
        amount: number;
    }) {
        setShoppingCartIsLoading(true);

        let cart = cartData.map((item) => {
            return {
                stockProductId: item.stockProductId,
                amount: item.amount,
            };
        });

        const findProductInCart = cart.find(
            (item) => item.stockProductId === product.stockProductId
        );

        if (findProductInCart) {
            cart = cart.filter((item) => item !== findProductInCart);
            product["amount"] = product.amount + findProductInCart.amount;
            cart.push(product);
        } else {
            cart.push(product);
        }

        const payload = {
            products: cart,
        };

        await new ShoppingCartUseCase()
            .update(payload)
            .then(() => {
                toast.success("Produto adicionado ao carrinho com sucesso!");
            })
            .catch((e) => {
                if (e?.response?.data?.keys?.quantity) {
                    return toast.error(
                        `É possível comprar apenas ${e?.response?.data?.keys?.quantity} itens para esse produto`
                    );
                }
                toast.error(`${e.response.data.message}`);
            })
            .finally(() => {
                getShoppingCartData();
                setShoppingCartIsLoading(false);
            });
    }

    async function removeItemFromCart(stockProductId: number) {
        setShoppingCartIsLoading(true);

        let cart = cartData.map((item) => {
            return {
                stockProductId: item.stockProductId,
                amount: item.amount,
            };
        });

        cart = cart.filter((item) => item.stockProductId !== stockProductId);

        const payload = {
            products: cart,
        };

        await new ShoppingCartRemoveUseCase()
            .handle(payload)
            .then(() => {
                toast.success("Produto removido do carrinho");
            })
            .catch((e) => {
                toast.error("Erro ao remover produto do carrinho");
            })
            .finally(() => {
                getShoppingCartData();
                setShoppingCartIsLoading(false);
            });
    }

    async function clearCart() {
        setShoppingCartIsLoading(true);

        const payload = {
            products: [],
        };

        await new ShoppingCartUseCase()
            .update(payload)
            .then(() => { })
            .catch((e) => {
                console.log(e);
            })
            .finally(() => {
                getShoppingCartData();
                setShoppingCartIsLoading(false);
            });
    }

    useEffect(() => {
        userToken && myUser && getShoppingCartData();
    }, [myUser, userToken]);

    return (
        <ShoppingCartContext.Provider
            value={{
                cartData,
                updateCart,
                setCartData,
                shoppingCartIsLoading,
                updatingShoppingCart,
                removeItemFromCart,
                clearCart,
                createOrder,
                getShoppingCartData,
            }}
        >
            {children}
        </ShoppingCartContext.Provider>
    );
}
