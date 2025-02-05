import {
    IconButton,
    InputAdornment,
    Stack,
    TextField,
    Typography,
    useTheme,
} from "@mui/material";
import { useFormik } from "formik";

import { BaseForm } from "../../../../typings";
import {
    getLoginFormInitialValues,
    LoginFormData,
    loginValidationSchema,
} from "./login.schema";
import { createFormikValidationHelper } from "../../../../utils";
import { useState } from "react";
import VisibilityOutlined from "@mui/icons-material/VisibilityOutlined";
import VisibilityOffOutlined from "@mui/icons-material/VisibilityOffOutlined";

export type LoginFormProps = BaseForm<LoginFormData>;
export function LoginForm({ id, onSubmit, initialValues }: LoginFormProps) {
    const { values, errors, touched, handleSubmit, handleChange, handleBlur } =
        useFormik({
            initialValues: getLoginFormInitialValues(),
            onSubmit,
            validationSchema: loginValidationSchema,
        });

    const { hasError, getHelperText } = createFormikValidationHelper({
        errors,
        touched,
    });

    const [isPasswordVisible, setIsPasswordVisible] = useState<boolean>(false);
    const theme = useTheme();

    return (
        <form id={id} onSubmit={handleSubmit} style={{ width: "100%" }}>
            <Stack>
                <Stack gap={"8px"}>
                    <Typography
                        variant="h3"
                        fontSize={"16px"}
                        fontWeight={"500"}
                    >
                        Usuário
                    </Typography>
                    <TextField
                        name={"username"}
                        placeholder={"Insira o usuário"}
                        value={values.username.toUpperCase()}
                        error={hasError("username")}
                        helperText={getHelperText("username")}
                        onChange={handleChange("username")}
                        onBlur={handleBlur("username")}
                    />
                </Stack>
                <Stack gap={"8px"}>
                    <Typography
                        variant="h3"
                        fontSize={"16px"}
                        fontWeight={"500"}
                    >
                        Senha
                    </Typography>
                    <TextField
                        name={"password"}
                        type={isPasswordVisible ? "text" : "password"}
                        value={values.password}
                        error={hasError("password")}
                        helperText={getHelperText("password")}
                        onChange={handleChange("password")}
                        onBlur={handleBlur("password")}
                        InputProps={{
                            endAdornment: (
                                <InputAdornment position="end">
                                    <IconButton
                                        onClick={() =>
                                            setIsPasswordVisible(
                                                !isPasswordVisible
                                            )
                                        }
                                    >
                                        {isPasswordVisible ? (
                                            <VisibilityOffOutlined
                                                sx={{
                                                    color: theme.palette.text
                                                        .primary,
                                                }}
                                            />
                                        ) : (
                                            <VisibilityOutlined
                                                sx={{
                                                    color: theme.palette.text
                                                        .primary,
                                                }}
                                            />
                                        )}
                                    </IconButton>
                                </InputAdornment>
                            ),
                        }}
                    />
                </Stack>
            </Stack>
        </form>
    );
}
