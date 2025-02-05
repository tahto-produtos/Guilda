import * as yup from "yup";
import { errorLabels } from "../../../../constants";
import { Indicator, Sector } from "../../../../typings";

export const copyMetricsValidationSchema = yup.object().shape({
  sector: yup.mixed<Sector>().required(errorLabels.REQUIRED_FIELD("Setor")),

  indicators: yup
    .array()
    .of(yup.mixed<Indicator>().required())
    .min(1, errorLabels.ALMOST_ONE_ITEM()),
});

export type CopyMetricsSchema = yup.InferType<
  typeof copyMetricsValidationSchema
>;

export function getCopyMetricsFormInitialValues(
  initialValues?: Partial<CopyMetricsSchema>
): CopyMetricsSchema {
  return {
    sector: initialValues?.sector!,
    indicators: initialValues?.indicators || [],
  };
}
