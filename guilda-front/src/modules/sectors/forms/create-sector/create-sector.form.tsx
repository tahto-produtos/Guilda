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

import { BaseForm, Indicator } from "../../../../typings";
import {
    CreateSectorFormData,
    createSectorValidationSchema,
    getCreateSectorInitialValues,
} from "./create-sector.schema";
import { createFormikValidationHelper } from "../../../../utils";
import { useEffect, useState } from "react";
import { ListIndicatorsUseCase } from "src/modules/indicators/use-cases";
import { toast } from "react-toastify";
import { PaginationModel } from "src/typings/models/pagination.model";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";

export type CreateSectorFormProps = BaseForm<CreateSectorFormData>;
export function CreateSectorForm({
    id,
    onSubmit,
    initialValues,
}: CreateSectorFormProps) {
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
        initialValues: getCreateSectorInitialValues(initialValues),
        validationSchema: createSectorValidationSchema,
    });

    const { hasError, getHelperText } = createFormikValidationHelper({
        touched,
        errors,
    });

    const [indicators, setIndicators] = useState<Indicator[]>([]);
    const [searchText, setSearchText] = useState<string>("");
    const [isLoadingList, setIsLoadingList] = useState<boolean>(false);

    useEffect(() => {
        const getIndicatorList = async () => {
            setIsLoadingList(true);
            const pagination: PaginationModel = {
                limit: 10,
                offset: 0,
                searchText: searchText,
            };

            new ListIndicatorsUseCase()
                .handle(pagination)
                .then((data) => {
                    setIndicators(data.items);
                    setIsLoadingList(false);
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                });
        };
        getIndicatorList();
    }, [searchText]);

    const handleIndicatorSelect = (value: Indicator[]) => {
        const getItemId = value.map((item) => {
            return item.id;
        });
        setFieldValue("indicatorsIds", getItemId);
    };

    return (
        <form id={id} onSubmit={handleSubmit} style={{ width: "100%" }}>
            <Stack>
                <TextField
                    name={"name"}
                    label={"Nome do Setor"}
                    value={values.name}
                    error={hasError("name")}
                    helperText={getHelperText("name")}
                    onBlur={handleBlur("name")}
                    onChange={handleChange("name")}
                    size={"small"}
                />
                <TextField
                    name={"code"}
                    label={"Código"}
                    value={values.code}
                    error={hasError("code")}
                    helperText={getHelperText("code")}
                    onBlur={handleBlur("code")}
                    onChange={handleChange("code")}
                    size={"small"}
                    type="number"
                />
                <Autocomplete
                    multiple
                    options={indicators}
                    getOptionLabel={(option) => option.name}
                    onChange={(event, value) => handleIndicatorSelect(value)}
                    onInputChange={(e, text) => setSearchText(text)}
                    filterOptions={(x) => x}
                    filterSelectedOptions
                    renderInput={(params) => (
                        <TextField
                            {...params}
                            variant="standard"
                            label="Indicadores"
                            placeholder="Buscar Indicadores"
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
            </Stack>
        </form>
    );
}
