export function isNumber(str: string): boolean {
  const num = parseFloat(str);
  return !isNaN(num) && isFinite(num);
}
