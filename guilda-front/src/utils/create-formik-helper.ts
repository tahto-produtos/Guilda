import { FormikErrors, FormikTouched } from "formik";
import { getPropertyByString } from "./get-property-by-string";

interface CreateFormValidationHelperParams<T> {
  errors: FormikErrors<T>;
  touched: FormikTouched<T>;
}

export function createFormikValidationHelper<T = unknown>({
  touched,
  errors,
}: CreateFormValidationHelperParams<T>) {
  const hasError = (property: string) => {
    return !!(
      getPropertyByString(touched, property) &&
      getPropertyByString(errors, property)
    );
  };

  const getHelperText = (property: string) => {
    return hasError(property) ? getPropertyByString(errors, property) : " ";
  };

  return {
    hasError,
    getHelperText,
  };
}
