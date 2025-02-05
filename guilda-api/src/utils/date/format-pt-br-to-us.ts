export function formatDatePtBrToUs(input: string): Date {
  const date = input.split('/');
  return new Date(parseInt(date[2]), parseInt(date[1]) - 1, parseInt(date[0]));
}
