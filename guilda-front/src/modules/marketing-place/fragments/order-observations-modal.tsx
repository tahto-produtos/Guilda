import Chat from "@mui/icons-material/Chat";
import DoDisturb from "@mui/icons-material/DoDisturb";
import LocalShipping from "@mui/icons-material/LocalShipping";
import Person from "@mui/icons-material/Person";
import Sell from "@mui/icons-material/Sell";
import ShoppingCart from "@mui/icons-material/ShoppingCart";

import { Box, Typography, alpha, useTheme } from "@mui/material";
import { grey } from "@mui/material/colors";
import { BaseModal } from "src/components/feedback";
import { Order } from "src/typings/models/order.model";

interface IProps {
    isOpen: boolean;
    onClose: () => void;
    order: Order;
}

function ObservationItem({
    text,
    title,
    productName,
    icon,
}: {
    title: string;
    text?: string;
    productName?: string;
    icon?: "ORDERED" | "RELEASED" | "DELIVERED" | "PERSON" | "CANCELED";
}) {
    const theme = useTheme();
    const color =
        icon == "ORDERED"
            ? theme.palette.warning.main
            : icon == "RELEASED"
            ? theme.palette.success.main
            : icon == "DELIVERED"
            ? theme.palette.primary.main
            : icon == "PERSON"
            ? theme.palette.info.main
            : icon == "CANCELED"
            ? theme.palette.error.main
            : grey[300];

    if (!text) {
        return null;
    }

    return (
        <Box
            width={"100%"}
            p={"10px"}
            bgcolor={alpha(color, 0.1)}
            display={"flex"}
            flexDirection={"column"}
            gap={"10px"}
            borderRadius={"5px"}
            border={`solid 1px ${color}`}
        >
            <Box
                display={"flex"}
                flexDirection={"row"}
                alignItems={"center"}
                gap={"10px"}
            >
                {icon == "ORDERED" ? (
                    <ShoppingCart sx={{ fontSize: "18px", color: color }} />
                ) : icon == "RELEASED" ? (
                    <Sell sx={{ fontSize: "18px", color: color }} />
                ) : icon == "DELIVERED" ? (
                    <LocalShipping sx={{ fontSize: "18px", color: color }} />
                ) : icon == "PERSON" ? (
                    <Person sx={{ fontSize: "18px", color: color }} />
                ) : icon == "CANCELED" ? (
                    <DoDisturb sx={{ fontSize: "18px", color: color }} />
                ) : (
                    <Chat sx={{ fontSize: "18px", color: color }} />
                )}
                <Box
                    display={"flex"}
                    flexDirection={"column"}
                    justifyContent={"center"}
                    gap={"1px"}
                >
                    <Typography
                        variant="body2"
                        fontSize={"14px"}
                        fontWeight={"600"}
                    >
                        {title}
                    </Typography>
                    {productName && (
                        <Typography
                            variant="body2"
                            fontSize={"12px"}
                            fontWeight={"500"}
                            sx={{ color: grey[600] }}
                        >
                            {productName}
                        </Typography>
                    )}
                </Box>
            </Box>
            <Typography variant="body2" pl={"28px"} sx={{ color: grey[700] }}>
                "{text ? text : "Sem observações"}"
            </Typography>
        </Box>
    );
}

export function OrderObservationsModal(props: IProps) {
    const { isOpen, onClose, order } = props;

    return (
        <BaseModal
            width={"550px"}
            open={isOpen}
            title={"Observações"}
            onClose={onClose}
        >
            <Box
                width={"100%"}
                display={"flex"}
                flexDirection={"column"}
                gap={1}
            >
                <ObservationItem
                    title="Observação do pedido"
                    text={order.observationOrder}
                    icon={"ORDERED"}
                />
                <ObservationItem
                    title="Observação da liberação do pedido"
                    text={order.observationReleased}
                    icon={"RELEASED"}
                />
                {order.GdaOrderProduct.map((item, index) => {
                    if (!item.observationReleased) {
                        return null;
                    }

                    return (
                        <ObservationItem
                            key={index}
                            title="Observação da liberação"
                            text={item.observationReleased}
                            productName={item.product.comercialName}
                            icon={"RELEASED"}
                        />
                    );
                })}
                <ObservationItem
                    title="Recebido por"
                    text={order.observationDelivered}
                    icon={"PERSON"}
                />
                <ObservationItem
                    title="Observação da entrega do pedido"
                    text={order.deliveryNote}
                    icon={"DELIVERED"}
                />
                {order.GdaOrderProduct.map((item, index) => {
                    if (!item.deliveryNote) {
                        return null;
                    }

                    return (
                        <ObservationItem
                            key={index}
                            title="Observação da entrega"
                            text={item.deliveryNote}
                            icon={"DELIVERED"}
                            productName={item.product.comercialName}
                        />
                    );
                })}
                {order.GdaOrderProduct.map((item, index) => {
                    if (!item.observationDelivered) {
                        return null;
                    }

                    return (
                        <ObservationItem
                            key={index}
                            title="Quem retirou"
                            text={item.observationDelivered}
                            icon={"PERSON"}
                            productName={item.product.comercialName}
                        />
                    );
                })}
                <ObservationItem
                    title="Observação do cancelamento do pedido"
                    text={order.observationCanceled}
                    icon={"CANCELED"}
                />
                {order.GdaOrderProduct.map((item, index) => {
                    if (!item.observationCanceled) {
                        return null;
                    }

                    return (
                        <ObservationItem
                            key={index}
                            title="Motivo do cancelamento"
                            text={item.observationCanceled}
                            icon={"CANCELED"}
                            productName={item.product.comercialName}
                        />
                    );
                })}
            </Box>
        </BaseModal>
    );
}
