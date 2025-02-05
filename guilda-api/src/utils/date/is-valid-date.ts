export function isValidDate(input: string | Date): boolean {
  const date = new Date(input);
  return !isNaN(date.getTime());
}
