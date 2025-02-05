import { useFormik } from "formik";
import {
    Box,
    Button,
    Card,
    CardMedia,
    Stack,
    TextField,
    Typography,
    useTheme,
} from "@mui/material";

import { BaseForm } from "../../../../typings";
import {
    CreateGroupFormSchema,
    createGroupValidationSchema,
    getCreateGroupInitialValues,
} from "./create-group.schema";
import { createFormikValidationHelper } from "../../../../utils";

export type CreateGroupFormProps = BaseForm<CreateGroupFormSchema>;
export function CreateGroupForm({
    id,
    onSubmit,
    initialValues,
}: CreateGroupFormProps) {
    const theme = useTheme();
    const {
        values,
        errors,
        touched,
        handleSubmit,
        handleBlur,
        handleChange,
        setFieldValue,
    } = useFormik({
        onSubmit,
        initialValues: getCreateGroupInitialValues(initialValues),
        validationSchema: createGroupValidationSchema,
    });

    const { hasError, getHelperText } = createFormikValidationHelper({
        touched,
        errors,
    });

    const handleUpload = (event: any) => {
        const file = event.target.files[0];
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onloadend = () => {
            setFieldValue("image", file);
        };
    };

    return (
        <form id={id} onSubmit={handleSubmit} style={{ width: "100%" }}>
            <Stack>
                <TextField
                    name={"name"}
                    label={"Nome"}
                    value={values.name}
                    error={hasError("name")}
                    helperText={getHelperText("name")}
                    onBlur={handleBlur("name")}
                    onChange={handleChange("name")}
                    size={"small"}
                />

                <TextField
                    name={"alias"}
                    label={"Nome de exibição"}
                    value={values.alias}
                    error={hasError("alias")}
                    helperText={getHelperText("alias")}
                    onBlur={handleBlur("alias")}
                    onChange={handleChange("alias")}
                    size={"small"}
                />

                <TextField
                    name={"description"}
                    label={"Descrição"}
                    value={values.description}
                    error={hasError("description")}
                    helperText={getHelperText("description")}
                    onBlur={handleBlur("description")}
                    onChange={handleChange("description")}
                    multiline
                    rows={3}
                    size={"small"}
                />

                <Box>
                    <input
                        accept="image/*"
                        style={{ display: "none" }}
                        id="image-upload"
                        type="file"
                        onChange={handleUpload}
                    />
                    <label htmlFor="image-upload">
                        <Button
                            variant="outlined"
                            color="primary"
                            component="span"
                        >
                            Selecione um ícone de exibição
                        </Button>
                    </label>

                    {hasError("image") && (
                        <Typography
                            color={theme.palette.error.main}
                            fontSize={"0.75rem"}
                            fontWeight={"400"}
                        >
                            {getHelperText("image")}
                        </Typography>
                    )}

                    {values.image && (
                        <Card
                            sx={{
                                width: "280px",
                                height: "280px",
                                p: 5,
                                mt: 2,
                            }}
                        >
                            <CardMedia
                                component="img"
                                alt="Uploaded image"
                                image={URL.createObjectURL(
                                    values.image as File
                                )}
                            />
                        </Card>
                    )}
                </Box>
            </Stack>
        </form>
    );
}
