import React, { forwardRef } from "react";
import Paper from "@mui/material/Paper";
import Typography from "@mui/material/Typography";
import { useEffect, useState, useRef } from "react";
import CardMedia from "@mui/material/CardMedia";
import { formatCurrency } from "src/utils/format-currency";


interface ProductDetailsProps {
    code?: string;
    description: string | null | undefined;
    comercialName: string | null | undefined;
    price?: string | number | null | undefined;
    amount?: number;
    qrCode?: string;
    isVisible: boolean;
    //ref: React.RefObject<any> | null;
}

export const TagToPrintProduct = forwardRef<HTMLDivElement, ProductDetailsProps>((props, ref) => {
    const { code, description, comercialName, price, qrCode, isVisible } = props;
    
    return (
        <div style={{ display: "none" }}>
            <Paper ref={ref} style={{ padding: '20px', marginTop: '20px', width: '100%' }}>
                <CardMedia
                    component="img"
                    image={qrCode}
                    sx={{
                        width: "300px",
                        objectFit: "contain",
                    }}
                />
                <Typography>Código: </Typography>{code}
                <Typography>Nome: </Typography>{comercialName}
                <Typography>Valor: </Typography>{price && formatCurrency(parseInt(price.toString()))} moedas
                {/*  <Typography>Descrição: </Typography>{description} */}
            </Paper>
        </div>
    );
});
