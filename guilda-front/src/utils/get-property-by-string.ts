export function getPropertyByString<T = any>(
  object: any,
  path: string
): T | undefined {
  if (!path) return object;
  const properties = path.split(".");
  const currentPath = properties.shift();
  if (currentPath && object[currentPath]) {
    return getPropertyByString(object[currentPath], properties.join("."));
  }
  return undefined;
}
