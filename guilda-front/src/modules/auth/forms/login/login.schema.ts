import * as yup from "yup";
import { errorLabels } from "../../../../constants";

export const loginValidationSchema = yup.object().shape({
  username: yup
    .string()
    .required(errorLabels.REQUIRED_FIELD("Nome de usuário")),
  password: yup.string().required(errorLabels.REQUIRED_FIELD("Senha")),
});

export type LoginFormData = yup.InferType<typeof loginValidationSchema>;

export function getLoginFormInitialValues(
  loginFormData?: LoginFormData
): LoginFormData {
  return {
    username: loginFormData?.username || "",
    password: loginFormData?.password || "",
  };
}
