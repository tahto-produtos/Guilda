import { BaseForm, Indicator, Sector } from "../../../../typings";
import {
    getSectorAndIndicatorFormInitialValues,
    sectorAndIndicatorSchema,
    SectorAndIndicatorSchema,
} from "./sector-and-indicator.schema";
import { useFormik } from "formik";
import { createFormikValidationHelper } from "../../../../utils";
import { Autocomplete } from "@mui/lab";
import { CircularProgress, Grid, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { PaginationModel } from "src/typings/models/pagination.model";
import { ListIndicatorsUseCase } from "src/modules/indicators/use-cases";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListSectorsUseCase } from "src/modules/sectors";

export type SectorAndIndicatorFormProps = BaseForm<SectorAndIndicatorSchema>;
export function SectorAndIndicatorForm({
    id,
    initialValues,
    onSubmit
}: SectorAndIndicatorFormProps) {
    const { values, errors, touched, handleSubmit, setFieldValue } = useFormik({
        initialValues: getSectorAndIndicatorFormInitialValues(initialValues),
        validationSchema: sectorAndIndicatorSchema,
        onSubmit,
    });

    const { hasError, getHelperText } = createFormikValidationHelper({
        touched,
        errors,
    });

    const { isLoading, startLoading, finishLoading } = useLoadingState();

    const [sectors, setSectors] = useState<Sector[]>([]);
    const [indicators, setIndicators] = useState<Indicator[]>([]);
    const [period, setPeriod] = useState<string[]>([
        "Diurno",
        "Noturno",
        "Madrugada",
    ]);
    const [periodValue, setPeriodValue] = useState<string>("");
    const [indicatorsSearchValue, setIndicatorsSearchValue] =
        useState<string>("");
    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const debouncedIndicatorsSearchValue: string = useDebounce<string>(
        indicatorsSearchValue,
        400
    );
    const debouncedSectorsSearchValue: string = useDebounce<string>(
        sectorsSearchValue,
        400
    );

    const getIndicatorsList = async () => {
        startLoading();

        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: indicatorsSearchValue,
        };

        new ListIndicatorsUseCase()
            .handle(pagination, undefined, values?.sector?.id)
            .then((data) => {
                setIndicators(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    const getSectorsList = async () => {
        startLoading();

        const pagination = {
            limit: 20,
            offset: 0,
            searchText: sectorsSearchValue,
            deleted: false,
        };

        new ListSectorsUseCase()
            .handle(pagination)
            .then((data) => {
                setSectors(data.items);
            })
            .catch(() => {
                // toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    useEffect(() => {
        getIndicatorsList();
    }, [debouncedIndicatorsSearchValue, values.sector]);

    useEffect(() => {
        getSectorsList();
    }, [debouncedSectorsSearchValue]);

    useEffect(() => {
        const newGoal = {
            period: "",
            sector: null,
            indicator: null
        }

        onSubmit(newGoal);
        if (values.indicator?.name == "Cesta de Indicadores") {
            onSubmit(values);
        }
        if (values.indicator && values.sector && values.period) {
            onSubmit(values);
        }
    }, [values]);

    console.log(sectors);

    return (
        <form id={id} onSubmit={handleSubmit} style={{ width: "100%" }}>
            <Grid container spacing={2}>
                <Grid item sm={12} md={6}>
                    <Autocomplete
                        value={values.sector}
                        placeholder={"Setor"}
                        disableClearable={false}
                        onChange={(e, value) => setFieldValue("sector", value)}
                        onInputChange={(e, text) => setSectorsSearchValue(text)}
                        sx={{ mb: 0 }}
                        renderInput={(props) => (
                            <TextField
                                {...props}
                                size={"medium"}
                                label={"Setor"}
                                error={hasError("sector")}
                                helperText={getHelperText("sector")}
                                InputProps={{
                                    ...props.InputProps,
                                    endAdornment: (
                                        <>
                                            {isLoading ? (
                                                <CircularProgress
                                                    color="primary"
                                                    size={20}
                                                />
                                            ) : (
                                                props.InputProps.endAdornment
                                            )}
                                        </>
                                    ),
                                }}
                            />
                        )}
                        isOptionEqualToValue={(option, value) =>
                            option.name === value.name
                        }
                        getOptionLabel={(option) =>
                            `${option.id} - ${option.name}`
                        }
                        options={sectors}
                    />
                </Grid>

                <Grid item sm={12} md={6}>
                    <Autocomplete
                        value={values.indicator}
                        placeholder={"Indicador"}
                        disableClearable={false}
                        onChange={(e, value) =>
                            setFieldValue("indicator", value)
                        }
                        onInputChange={(e, text) =>
                            setIndicatorsSearchValue(text)
                        }
                        sx={{ mb: 0 }}
                        renderInput={(props) => (
                            <TextField
                                {...props}
                                size={"medium"}
                                label={"Indicador"}
                                error={hasError("indicator")}
                                helperText={getHelperText("indicator")}
                                InputProps={{
                                    ...props.InputProps,
                                    endAdornment: (
                                        <>
                                            {isLoading ? (
                                                <CircularProgress
                                                    color="primary"
                                                    size={20}
                                                />
                                            ) : (
                                                props.InputProps.endAdornment
                                            )}
                                        </>
                                    ),
                                }}
                            />
                        )}
                        isOptionEqualToValue={(option, value) =>
                            option.name === value.name
                        }
                        getOptionLabel={(option) => option.name}
                        options={indicators}
                    />
                </Grid>
                <Grid item sm={12} md={6}>
                    <Autocomplete
                        value={values.period}
                        placeholder={"Período"}
                        disableClearable={false}
                        onInputChange={(e, text) => setPeriodValue(text)}
                        onChange={(e, value) => setFieldValue("period", value)}
                        sx={{ mb: 0 }}
                        renderInput={(props) => (
                            <TextField
                                {...props}
                                size={"medium"}
                                label={"Período"}
                                error={hasError("período")}
                                helperText={getHelperText("período")}
                                InputProps={{
                                    ...props.InputProps,
                                    endAdornment: (
                                        <>
                                            {isLoading ? (
                                                <CircularProgress
                                                    color="primary"
                                                    size={20}
                                                />
                                            ) : (
                                                props.InputProps.endAdornment
                                            )}
                                        </>
                                    ),
                                }}
                            />
                        )}
                        isOptionEqualToValue={(option, value) =>
                            option == value
                        }
                        getOptionLabel={(option) => option}
                        options={period}
                    />
                </Grid>
            </Grid>
        </form>
    );
}
