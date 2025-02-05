import { Box, TextField, Typography, useTheme } from "@mui/material";

import { BaseForm, Group } from "../../../../typings";
import {
    getMetricMinMaxInitialValues,
    MetricMinMaxSchema,
    metricMinMaxValidationSchema,
} from "./metric-min-max.schema";
import { useFormik } from "formik";
import { createFormikValidationHelper } from "../../../../utils";

export type MetricMinMaxFormProps = BaseForm<MetricMinMaxSchema> & {
    group: Group;
    disabled: boolean;
};
export function MetricMinMaxForm({
    id,
    onSubmit,
    initialValues,
    group,
    disabled,
}: MetricMinMaxFormProps) {
    const theme = useTheme();

    const { values, errors, touched, handleBlur, handleChange, handleSubmit } =
        useFormik({
            onSubmit,
            initialValues: getMetricMinMaxInitialValues(initialValues),
            validationSchema: metricMinMaxValidationSchema,
            enableReinitialize: true,
        });

    const { hasError, getHelperText } = createFormikValidationHelper({
        touched,
        errors,
    });

    return (
        <form id={id} onSubmit={handleSubmit} style={{ width: "100%" }}>
            <Box display={"flex"} justifyContent={"space-between"} gap={1}>
                <Typography
                    color={disabled ? theme.palette.grey["500"] : undefined}
                    variant={"h3"}
                    mt={1}
                    width={"120px"}
                >
                    {group.name}
                </Typography>
                <TextField
                    disabled={disabled}
                    type={"number"}
                    name={"metricMin"}
                    label={"Meta mínima (%)"}
                    value={values.min}
                    error={hasError("min")}
                    helperText={getHelperText("min")}
                    onBlur={handleBlur("min")}
                    onChange={(event) => {
                        handleChange("min")(event);
                        handleSubmit();
                    }}
                    size={"small"}
                    fullWidth
                    InputProps={{
                        value: values.min < 0 ? "" : values.min,
                        placeholder: "Métrica não definida",
                    }}
                />

                {/* <TextField
          disabled={disabled}
          type={"number"}
          name={"metricMax"}
          label={"Meta máxima (%)"}
          value={values.max}
          error={hasError("max")}
          helperText={getHelperText("max")}
          onBlur={handleBlur("max")}
          onChange={(event) => {
            handleChange("max")(event);
            handleSubmit();
          }}
          size={"small"}
          fullWidth
          InputProps={{
            value: values.max < 0 ? "" : values.max,
            placeholder: "Métrica não definida",
          }}
        /> */}
            </Box>
        </form>
    );
}
