import { Transform } from 'class-transformer';

const transformNumber = (value: any) =>
  !!Number(value) || value === '0' ? Number(value) : value;

const parseNumberId = (value: any, isArray: boolean) => {
  if (isArray) {
    if (Array.isArray(value)) {
      return value.map((item) => transformNumber(item));
    }

    return [transformNumber(value)];
  }

  return transformNumber(value);
};

export function TransformInteger(isArray = false) {
  return Transform(({ value }) => parseNumberId(value, isArray));
}
