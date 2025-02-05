import * as yup from "yup";
import { errorLabels } from "../../../../constants";

export const createSectorValidationSchema = yup.object().shape({
    name: yup.string().required(errorLabels.REQUIRED_FIELD("Value")),
    code: yup.number().required(errorLabels.REQUIRED_FIELD("Number")),
    indicatorsIds: yup.array().of(yup.number()),
});

export type CreateSectorFormData = yup.InferType<
    typeof createSectorValidationSchema
>;

export function getCreateSectorInitialValues(
    initialValues?: Partial<CreateSectorFormData>
): CreateSectorFormData {
    return {
        name: initialValues?.name || "",
        code: initialValues?.code || 0,
        indicatorsIds: initialValues?.indicatorsIds || [],
    };
}
