import * as yup from "yup";
import { errorLabels } from "../../../../constants";

export const createIndicatorValidationSchema = yup.object().shape({
    name: yup.string().required(errorLabels.REQUIRED_FIELD("Nome")),
    description: yup.string().required(errorLabels.REQUIRED_FIELD("Descrição")),
    id: yup
        .number()
        .required(errorLabels.REQUIRED_FIELD("Código do Indicador")),
    weight: yup.number().optional(),
    calculationType: yup.string().optional(),
    expression: yup.string().optional(),
    selectedSectors: yup.array().optional(),
    isActive: yup.string().required(),
});

export type CreateIndicatorFormData = yup.InferType<
    typeof createIndicatorValidationSchema
>;

export function getCreateIndicatorInitialValues(
    initialValues?: Partial<CreateIndicatorFormData>
): CreateIndicatorFormData {
    return {
        name: initialValues?.name || "",
        weight: initialValues?.weight || 0,
        calculationType: initialValues?.calculationType || "",
        expression: initialValues?.expression || "",
        description: initialValues?.description || "",
        id: initialValues?.id || 0,
        selectedSectors: initialValues?.selectedSectors || [],
        isActive: initialValues?.isActive || "ativado",
    };
}
