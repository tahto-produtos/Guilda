import { Transform } from 'class-transformer';

export function TransformArray() {
  return Transform(({ value }) => (Array.isArray(value) ? value : [value]));
}
