import * as yup from "yup";
import { errorLabels } from "../../../../constants";

export const metricMinMaxValidationSchema = yup.object().shape({
    min: yup.number().required(errorLabels.REQUIRED_FIELD("Meta mínima")),
    max: yup.number().required(errorLabels.REQUIRED_FIELD("Meta máxima")),
});

export type MetricMinMaxSchema = yup.InferType<
    typeof metricMinMaxValidationSchema
>;

export function getMetricMinMaxInitialValues(
    initialValues?: Partial<MetricMinMaxSchema>
): MetricMinMaxSchema {
    const { min, max } = initialValues || {};

    return {
        min: min == 0 ? 0 : min || -1,
        max: max == 0 ? 0 : max || -1,
    };

    // return {
    //   min: min || -1,
    //   max: max || -1,
    // };
}
