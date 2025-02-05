import { EXCEPTION_CODES } from '../enums';

export interface Exception {
  message: string;
  code: EXCEPTION_CODES;
  keys?: Record<string, string | number | boolean>;
}
