import { useFormik } from "formik";
import {
    Autocomplete,
    Box,
    Chip,
    FormControl,
    InputLabel,
    MenuItem,
    OutlinedInput,
    Select,
    SelectChangeEvent,
    Stack,
    TextField,
    Theme,
    useTheme,
} from "@mui/material";

import { BaseForm, Indicator, Sector } from "../../../../typings";
import { createFormikValidationHelper } from "../../../../utils";
import { useEffect, useState } from "react";
import {
    CreateIndicatorFormData,
    createIndicatorValidationSchema,
    getCreateIndicatorInitialValues,
} from "./create-indicator.schema";
import { ListSectorsUseCase } from "src/modules/sectors";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { PaginationModel } from "src/typings/models/pagination.model";
import { IndicatorMathematicalExpressionsUseCase } from "../../use-cases/indicator-mathematical-expressions.use-case";
import { MathExpression } from "src/typings/models/math-expression.model";

export type CreateIndicatorFormProps = BaseForm<CreateIndicatorFormData>;
export function CreateIndicatorForm({
    id,
    onSubmit,
    initialValues,
}: CreateIndicatorFormProps) {
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
        initialValues: getCreateIndicatorInitialValues(initialValues),
        validationSchema: createIndicatorValidationSchema,
    });

    const { hasError, getHelperText } = createFormikValidationHelper({
        touched,
        errors,
    });
    const [mathExpressions, setMathExpressions] = useState<MathExpression[]>(
        []
    );
    const [selectedExpression, setSelectedExpression] =
        useState<MathExpression | null>(null);
    const [sectorsEdit, setSectorsEdit] = useState<Sector[]>([]);
    const [sectors, setSectors] = useState<Sector[]>([]);
    const [sectorSearchValue, setSectorsSearchValue] = useState<string>("");

    useEffect(() => {
        setFieldValue("selectedSectors", sectorsEdit);
    }, [sectorsEdit]);

    async function getMathExpressions() {
        new IndicatorMathematicalExpressionsUseCase()
            .handle()
            .then((data) => {
                setMathExpressions(data);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }

    useEffect(() => {
        getMathExpressions();
    }, []);

    const getSectorsList = async () => {
        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: sectorSearchValue,
        };

        new ListSectorsUseCase()
            .handle(pagination)
            .then((data) => {
                setSectors(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {});
    };

    useEffect(() => {
        getSectorsList();
    }, [sectorSearchValue]);

    return (
        <form id={id} onSubmit={handleSubmit} style={{ width: "100%" }}>
            <Stack>
                <TextField
                    name={"name"}
                    label={"Nome do Indicador"}
                    value={values.name}
                    error={hasError("name")}
                    helperText={getHelperText("name")}
                    onBlur={handleBlur("name")}
                    onChange={handleChange("name")}
                    size={"small"}
                />
                <TextField
                    name={"id"}
                    label={"Código do Indicador"}
                    value={values.id}
                    type="number"
                    error={hasError("id")}
                    helperText={getHelperText("id")}
                    onBlur={handleBlur("id")}
                    onChange={handleChange("id")}
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
                <Autocomplete
                    multiple
                    size={"small"}
                    value={sectorsEdit}
                    options={sectors}
                    getOptionLabel={(option) => option.name}
                    onChange={(event, value) => {
                        setSectorsEdit(value);
                    }}
                    onInputChange={(e, text) => setSectorsSearchValue(text)}
                    filterOptions={(x) => x}
                    filterSelectedOptions
                    renderInput={(params) => (
                        <TextField
                            {...params}
                            variant="outlined"
                            label="Selecione um ou mais setores "
                            placeholder="Buscar"
                        />
                    )}
                    renderOption={(props, option) => {
                        return (
                            <li {...props} key={option.id}>
                                {option.name}
                            </li>
                        );
                    }}
                    isOptionEqualToValue={(option, value) =>
                        option.name === value.name
                    }
                />
                <TextField
                    name={"weight"}
                    label={"Peso"}
                    value={values.weight}
                    type="number"
                    error={hasError("weight")}
                    helperText={getHelperText("weight")}
                    onBlur={handleBlur("weight")}
                    onChange={handleChange("weight")}
                    size={"small"}
                />
                <FormControl sx={{ width: "100%" }}>
                    <InputLabel sx={{ backgroundColor: "#fff" }} size="small">
                        Tipo de cálculo
                    </InputLabel>
                    <Select
                        onChange={(e) =>
                            setFieldValue("calculationType", e.target.value)
                        }
                        value={values.calculationType}
                        size="small"
                    >
                        <MenuItem value={""}>Nenhum</MenuItem>
                        <MenuItem value={"BIGGER_BETTER"}>
                            Maior Melhor
                        </MenuItem>
                        <MenuItem value={"LESS_BETTER"}>Menor Melhor</MenuItem>
                    </Select>
                </FormControl>
                <FormControl sx={{ width: "100%", mt: 2 }}>
                    <InputLabel sx={{ backgroundColor: "#fff" }} size="small">
                        Status
                    </InputLabel>
                    <Select
                        onChange={(e) =>
                            setFieldValue("isActive", e.target.value)
                        }
                        value={values.isActive}
                        defaultValue={values.isActive}
                        size="small"
                    >
                        <MenuItem value={"ativado"}>Ativado</MenuItem>
                        <MenuItem value={"inativado"}>Inativado</MenuItem>
                    </Select>
                </FormControl>
                <Box display={"flex"} gap={2} alignItems={"center"}>
                    <Autocomplete
                        value={selectedExpression}
                        placeholder={"Copiar expressão"}
                        disableClearable={false}
                        onChange={(_, expression) => {
                            setSelectedExpression(expression);
                            setFieldValue(
                                "expression",
                                expression?.expression || ""
                            );
                        }}
                        onInputChange={(e, text) => setSectorsSearchValue(text)}
                        isOptionEqualToValue={(option, value) =>
                            option.expression === value.expression
                        }
                        renderInput={(props) => (
                            <TextField
                                {...props}
                                size={"small"}
                                label={"Copiar expressão"}
                            />
                        )}
                        renderTags={() => null}
                        getOptionLabel={(option) => option.expression}
                        options={mathExpressions}
                        sx={{ mb: 0, width: "100%" }}
                    />
                    <TextField
                        value={values.expression}
                        label={"Métrica do Indicador"}
                        onChange={(e) => handleChange("expression")}
                        fullWidth
                        error={hasError("expression")}
                        helperText={getHelperText("expression")}
                        onBlur={handleBlur("expression")}
                        name={"expression"}
                    />
                </Box>
                {/* <TextField
                    name={"expression"}
                    label={"Métrica do Indicador"}
                    value={values.expression}
                    error={hasError("expression")}
                    helperText={getHelperText("expression")}
                    onBlur={handleBlur("expression")}
                    onChange={handleChange("expression")}
                    multiline
                    rows={3}
                    size={"small"}
                    sx={{ mt: 3 }}
                /> */}
            </Stack>
        </form>
    );
}
