export function formatCurrency(price: number) {
    return new Intl.NumberFormat("pt-BR").format(price);
}
