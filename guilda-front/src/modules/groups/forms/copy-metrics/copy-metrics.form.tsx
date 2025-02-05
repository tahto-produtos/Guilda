import { Box, Checkbox, Chip, Stack, TextField } from "@mui/material";
import { Autocomplete } from "@mui/lab";
import { BaseForm, Indicator, Sector } from "../../../../typings";
import { useFormik } from "formik";
import {
    CopyMetricsSchema,
    copyMetricsValidationSchema,
    getCopyMetricsFormInitialValues,
} from "./copy-metrics.schema";
import { createFormikValidationHelper } from "../../../../utils";
import CheckBoxOutlineBlankIcon from "@mui/icons-material/CheckBoxOutlineBlank";
import CheckBoxIcon from "@mui/icons-material/CheckBox";
import { useEffect, useState } from "react";
import { DetailsSectorUseCase, ListSectorsUseCase } from "src/modules/sectors";
import { PaginationModel } from "src/typings/models/pagination.model";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useDebounce } from "src/hooks";
import { ListIndicatorsUseCase } from "src/modules/indicators/use-cases";

const icon = <CheckBoxOutlineBlankIcon fontSize="small" />;
const checkedIcon = <CheckBoxIcon fontSize="small" />;

export type CopyMetricsFormProps = BaseForm<CopyMetricsSchema>;
export function CopyMetricsForm({
    id,
    initialValues,
    onSubmit,
}: CopyMetricsFormProps) {
    const { values, errors, touched, handleBlur, handleSubmit, setFieldValue } =
        useFormik({
            validationSchema: copyMetricsValidationSchema,
            initialValues: getCopyMetricsFormInitialValues(initialValues),
            onSubmit,
        });

    const { hasError, getHelperText } = createFormikValidationHelper({
        errors,
        touched,
    });

    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const [sectors, setSectors] = useState<Sector[]>([]);
    const debouncedSectorSearchTerm: string = useDebounce<string>(
        sectorsSearchValue,
        400
    );

    const [selectedSectorDetails, setSelectedSectorDetails] =
        useState<Sector | null>(null);

    const [indicatorsSearchValue, setIndicatorsSearchValue] =
        useState<string>("");
    const [indicators, setIndicators] = useState<Indicator[]>([]);
    const debouncedIndicatorsSearchTerm: string = useDebounce<string>(
        indicatorsSearchValue,
        400
    );

    const [haveUnassociatedIndicators, setHaveUnassociatedIndicators] =
        useState<boolean>(false);

    const handleDeleteIndicator = (indicatorId: number) => {
        setFieldValue(
            "indicators",
            values.indicators?.filter(({ id }) => id !== indicatorId) || []
        );
    };

    useEffect(() => {
        if (values.sector) {
            getSectorDetails();
        }
    }, [values.sector]);

    useEffect(() => {}, [haveUnassociatedIndicators]);

    // useEffect(() => {
    //     if (selectedSectorDetails) {
    //         const selectedSectorIndicatorsIds =
    //             selectedSectorDetails?.indicators?.map(
    //                 (indicator) => indicator.id
    //             );
    //         const indicatorsSelectedIds = values?.indicators?.map(
    //             (item) => item.id
    //         );

    //         console.log(
    //             selectedSectorIndicatorsIds,
    //             indicatorsSelectedIds,
    //             selectedSectorIndicatorsIds == indicatorsSelectedIds
    //         );
    //     }
    // }, [selectedSectorDetails, values.indicators]);

    const getSectorDetails = async () => {
        const payload = {
            id: values.sector.id.toString(),
        };

        new DetailsSectorUseCase()
            .handle(payload)
            .then((data) => {
                setSelectedSectorDetails(data);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    };

    const getSectorsList = async () => {
        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: sectorsSearchValue,
        };

        new ListSectorsUseCase()
            .handle(pagination)
            .then((data) => {
                setSectors(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    };

    const getIndicatorsList = async () => {
        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: indicatorsSearchValue,
        };

        new ListIndicatorsUseCase()
            .handle(pagination)
            .then((data) => {
                setIndicators(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    };

    useEffect(() => {
        getIndicatorsList();
    }, [debouncedIndicatorsSearchTerm]);

    useEffect(() => {
        getSectorsList();
    }, [debouncedSectorSearchTerm]);

    return (
        <form id={id} onSubmit={handleSubmit}>
            <Stack>
                <Box display={"flex"} gap={1}>
                    <Box width={"100%"}>
                        <Autocomplete
                            value={values.sector}
                            placeholder={"Setor"}
                            disableClearable={false}
                            onChange={(_, sector) =>
                                setFieldValue("sector", sector)
                            }
                            onInputChange={(e, text) =>
                                setSectorsSearchValue(text)
                            }
                            isOptionEqualToValue={(option, value) =>
                                option.name === value.name
                            }
                            onBlur={handleBlur("sector")}
                            renderInput={(props) => (
                                <TextField
                                    {...props}
                                    size={"small"}
                                    label={"Setor"}
                                    error={hasError("sector")}
                                    helperText={getHelperText("sector")}
                                />
                            )}
                            renderTags={() => null}
                            getOptionLabel={(option) => option.name}
                            options={sectors}
                            sx={{ mb: 0 }}
                        />
                    </Box>

                    <Box width={"100%"}>
                        <Autocomplete
                            value={values.indicators}
                            placeholder={"Indicadores"}
                            multiple
                            disableClearable={false}
                            onChange={(e, indicators) =>
                                setFieldValue("indicators", indicators)
                            }
                            onInputChange={(e, text) =>
                                setIndicatorsSearchValue(text)
                            }
                            isOptionEqualToValue={(option, value) =>
                                option.name === value.name
                            }
                            disableCloseOnSelect
                            onBlur={handleBlur("indicators")}
                            renderOption={(props, option, { selected }) => (
                                <li
                                    {...props}
                                    style={{ justifyContent: "space-between" }}
                                >
                                    {option.name}
                                    <Checkbox
                                        icon={icon}
                                        checkedIcon={checkedIcon}
                                        style={{ marginRight: 8 }}
                                        checked={selected}
                                    />
                                </li>
                            )}
                            renderInput={(props) => (
                                <TextField
                                    {...props}
                                    size={"small"}
                                    label={"Indicador"}
                                    error={hasError("indicators")}
                                    helperText={getHelperText("indicators")}
                                />
                            )}
                            renderTags={() => null}
                            getOptionLabel={(option) => option.name}
                            options={indicators}
                            sx={{ mb: 0 }}
                        />
                    </Box>
                </Box>

                <Box
                    display={"flex"}
                    flexWrap={"wrap"}
                    width={"100%"}
                    mt={2}
                    gap={1}
                    minHeight={"70px"}
                >
                    {selectedSectorDetails &&
                        values.indicators?.map((indicator, index) => (
                            <Chip
                                key={index}
                                label={indicator.name}
                                color={
                                    selectedSectorDetails.indicators?.find(
                                        (item) => item.id === indicator.id
                                    )
                                        ? "success"
                                        : "warning"
                                }
                                onDelete={() =>
                                    handleDeleteIndicator(indicator.id)
                                }
                            />
                        ))}
                </Box>
            </Stack>
        </form>
    );
}
