import * as yup from "yup";

export const sectorAndIndicatorSchema = yup.object().shape({
  sector: yup
    .object()
    .shape({
      id: yup.number().integer().required(),
      name: yup.string().required(),
    })
    .nullable(),

  indicator: yup
    .object()
    .shape({
      id: yup.number().integer().required(),
      name: yup.string().required(),
    })
    .nullable(),
  period: yup.string(),
});

export type SectorAndIndicatorSchema = yup.InferType<
  typeof sectorAndIndicatorSchema
>;

export function getSectorAndIndicatorFormInitialValues(
  initialValues?: Partial<SectorAndIndicatorSchema>
): SectorAndIndicatorSchema {
  return {
    indicator: initialValues?.indicator || null,
    sector: initialValues?.sector || null,
    period: initialValues?.period,
  };
}
