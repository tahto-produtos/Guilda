import * as yup from "yup";
import { errorLabels } from "../../../../constants";

export const createGroupValidationSchema = yup.object().shape({
  name: yup.string().required(errorLabels.REQUIRED_FIELD("Nome")),
  description: yup.string().required(errorLabels.REQUIRED_FIELD("Descrição")),
  alias: yup.string().required(errorLabels.REQUIRED_FIELD("Nome de exibição")),
  image: yup.mixed().required(errorLabels.REQUIRED_FIELD("Ícone de exibição")),
});

export type CreateGroupFormSchema = yup.InferType<
  typeof createGroupValidationSchema
>;

export function getCreateGroupInitialValues(
  initialValues?: Partial<CreateGroupFormSchema>
): CreateGroupFormSchema {
  return {
    name: initialValues?.name || "",
    description: initialValues?.description || "",
    alias: initialValues?.alias || "",
    image: initialValues?.image!,
  };
}
