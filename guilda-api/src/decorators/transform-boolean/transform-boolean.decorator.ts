import { Transform } from 'class-transformer';

const transformBoolean = (value: any) =>
  value === 'true' ? true : value === 'false' ? false : undefined;

const parseBoolean = (value: any) => {
  return transformBoolean(value);
};

export function TransformBoolean() {
  return Transform(({ value }) => parseBoolean(value));
}
