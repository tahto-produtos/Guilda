export function truncateDecimals(number: number, decimals: number) {
    const multiplier = Math.pow(10, decimals);
    return Math.trunc(number * multiplier) / multiplier;
}
