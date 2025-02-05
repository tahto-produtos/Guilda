export function groupBy<T>(
  array: T[],
  predicate: (value: T, index: number, array: T[]) => string | number,
) {
  return array.reduce((acc, value, index, array) => {
    (acc[predicate(value, index, array)] ||= []).push(value);
    return acc;
  }, {} as { [key: string]: T[] });
}
