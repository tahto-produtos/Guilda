import { createContext } from "react";
import { CartDataModel } from "src/typings/models/cart-data.model";

export interface ShoppingCartContextData {
    cartData: CartDataModel[];
    setCartData: (input: CartDataModel[]) => void;
    updateCart: (input: { stockProductId: number; amount: number }) => void;
    removeItemFromCart: (input: number) => void;
    clearCart: () => void;
    shoppingCartIsLoading: boolean;
    updatingShoppingCart: boolean;
    createOrder: (input: string) => void;
    getShoppingCartData: () => void;
}

export const ShoppingCartContext = createContext<ShoppingCartContextData>(
    {} as ShoppingCartContextData
);
