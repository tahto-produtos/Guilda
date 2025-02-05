export function mapBy<T>(items: T[], property: keyof T): Map<any, T> {
  const map = new Map();

  items.forEach((item) => {
    const key = item[property];
    map.set(key, item);
  });

  return map;
}
