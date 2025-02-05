import Settings from "@mui/icons-material/Settings";
import { LocalizationProvider, DatePicker } from "@mui/x-date-pickers";
import { Box, Button, FormControl, InputLabel, MenuItem, Select, Stack, TextField } from "@mui/material";
import { grey } from "@mui/material/colors";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { CreateHolidayUseCase } from "src/modules/marketing-place/use-cases/create-holiday.use-case";
import { DeleteHolidayUseCase } from "src/modules/marketing-place/use-cases/delete-holiday.use-case";
import { ListCityUseCase } from "src/modules/marketing-place/use-cases/list-city.use-case";
import { ListHolidayUseCase } from "src/modules/marketing-place/use-cases/list-holiday.use-case";
import { UpdateHolidayUseCase } from "src/modules/marketing-place/use-cases/update-holiday.use-case";
import { getLayout } from "src/utils";
import { format } from "date-fns";


interface Holiday {
    holidayDate: string;
    createdAt: string;
    createdByCollaboratorId: number;
    deletedAt: string;
    site: string;
    type: string;
    id: number;
}

export default function HolidayView() {
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [newHolidayName, setNewHolidayName] = useState<string>("");
    const [holidayList, setHolidayList] = useState<Holiday[]>([]);
    const [selected, setSelected] = useState<Holiday | null>(null);
    const [editName, setEditName] = useState<string>("");
    const [holidayDate, setHolidayDate] = useState<
    dateFns | Date | null
    >(null);
    const [cityList, setCityList] = useState<any[]>([]);
    const [typeList, setTypeList] = useState<any[]>(['NACIONAL', 'ESTADUAL', 'MUNICIPAL']);
    const [city, setCity] = useState<string>("");
    const [type, setType] = useState<string>("");

    const [holidayUpdateDate, setHolidayUpdateDate] = useState<
    dateFns | Date | null
    >(null);
    const [cityUpdate, setUpdateCity] = useState<string>("");
    const [typeUpdate, setUpdateType] = useState<string>("");

    function handleCreateHoliday() {
        console.log(type);
        if (!type) {
            return toast.warning("Escolha um tipo.");
        }
        if (!city && type !== "NACIONAL") {
            return toast.warning("Escolha uma cidade.");
        }
        if (!holidayDate) {
            return toast.warning("Insira a data do feriado.");
        }

        new CreateHolidayUseCase()
            .handle({ city, type, holidayDate })
            .then((data) => {
                toast.success("Feriado criado com sucesso!");
                listHoliday();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }

    function listHoliday() {
        new ListHolidayUseCase()
            .handle()
            .then((data) => {
                setHolidayList(data);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }

    useEffect(() => {
        listHoliday();
    }, []);

    function handleDelete(id: number) {
        new DeleteHolidayUseCase()
            .handle({ id })
            .then(() => {
                toast.success("Feriado apagado com sucesso!");
                listHoliday();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }
    async function getCities() {
        const set = new Set();

        await new ListCityUseCase()
            .handle()
            .then((data) => {
                data.cities.forEach((el: any, index: number) => {
                    if (el.VALUE !== "-") {
                        set.add(el.VALUE);
                    }
                    console.log(set, el.value, el.VALUE, el);
                });
                setCityList(Array.from(set));
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {});
    }

    useEffect(() => {
        getCities();
    }, []);

    function handleUpdateHoliday(id: number) {
        new UpdateHolidayUseCase()
            .handle({ id, city: cityUpdate, type: typeUpdate, holidayDate: holidayUpdateDate })
            .then(() => {
                toast.success("Feriado editado com sucesso!");
                setSelected(null);
                listHoliday();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Feriados"}
                headerIcon={<Settings />}
                addButtonAction={() => setIsOpen(true)}
                addButtonTitle="Novo Feriado"
            />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                {holidayList.map((item) => (
                    <Box
                        key={item.id}
                        display={"flex"}
                        flexDirection={"row"}
                        gap={2}
                        bgcolor={grey[100]}
                        py={1}
                        px={2}
                        justifyContent={"space-between"}
                    >
                        Data: {format(new Date(item.holidayDate.toString()), "yyyy-MM-dd")} {" "}
                        Site: {item.site} {" "}
                        Tipo: {item.type}
                        <Box display={"flex"} gap={2} alignItems={"center"}>
                            <Button
                                variant="outlined"
                                onClick={() => {
                                    setHolidayUpdateDate(new Date(item.holidayDate))
                                    setUpdateType(item.type)
                                    setUpdateCity(item.site)
                                    setSelected(item);
                                }}
                            >
                                Editar
                            </Button>
                            <Button
                                variant="contained"
                                color="error"
                                onClick={() => handleDelete(item.id)}
                            >
                                Apagar
                            </Button>
                        </Box>
                    </Box>
                ))}
            </Stack>
            <BaseModal
                width={"540px"}
                open={isOpen}
                title={`Novo Feriado`}
                onClose={() => setIsOpen(false)}
            >
                <Box
                    width={"100%"}
                    display={"flex"}
                    flexDirection={"column"}
                    gap={2}
                >
                <LocalizationProvider dateAdapter={AdapterDateFns}>
                    <DatePicker
                        label="Data do feriado"
                        value={holidayDate}
                        onChange={(newValue) => setHolidayDate(newValue)}
                        slotProps={{
                            textField: {
                                size: "small",
                                sx: {
                                    minWidth: "180px",
                                    svg: {
                                        color: grey[500],
                                    },
                                    width: "100%",
                                },
                            },
                        }}
                    />
                </LocalizationProvider>
                <FormControl sx={{ width: "100%" }}>
                    <InputLabel
                        sx={{ backgroundColor: "#fff", px: 1 }}
                        size="small"
                    >
                        Cidade
                    </InputLabel>
                    <Select
                        onChange={(e) => setCity(e.target.value)}
                        value={city}
                        size="small"
                    >
                        {cityList.map((item, index) => {
                            return (
                                <MenuItem value={item} key={index}>
                                    {item}
                                </MenuItem>
                            );
                        })}
                    </Select>
                </FormControl>
                <FormControl sx={{ width: "100%" }}>
                    <InputLabel
                        sx={{ backgroundColor: "#fff", px: 1 }}
                        size="small"
                    >
                        Tipo
                    </InputLabel>
                    <Select
                        onChange={(e) => setType(e.target.value)}
                        value={type}
                        size="small"
                    >
                        {typeList.map((item, index) => {
                            return (
                                <MenuItem value={item} key={index}>
                                    {item}
                                </MenuItem>
                            );
                        })}
                    </Select>
                </FormControl>
                    <Button
                        fullWidth
                        variant="contained"
                        onClick={handleCreateHoliday}
                        disabled={!type && !city && !holidayDate}
                    >
                        Criar
                    </Button>
                </Box>
            </BaseModal>
            <BaseModal
                width={"540px"}
                open={!!selected}
                title={`Editar Feriado`}
                onClose={() => setSelected(null)}
            >
                <Box
                    width={"100%"}
                    display={"flex"}
                    flexDirection={"column"}
                    gap={2}
                >
                <LocalizationProvider dateAdapter={AdapterDateFns}>
                    <DatePicker
                        label="Data do feriado"
                        value={selected?.holidayDate ? new Date(selected?.holidayDate) : new Date()}
                        onChange={(newValue: any) => setHolidayUpdateDate(newValue)}
                        slotProps={{
                            textField: {
                                size: "small",
                                sx: {
                                    minWidth: "180px",
                                    svg: {
                                        color: grey[500],
                                    },
                                    width: "100%",
                                },
                            },
                        }}
                    />
                </LocalizationProvider>
                <FormControl sx={{ width: "100%" }}>
                    <InputLabel
                        sx={{ backgroundColor: "#fff", px: 1 }}
                        size="small"
                    >
                        Cidade
                    </InputLabel>
                    <Select
                        onChange={(e) => setUpdateCity(e.target.value)}
                        value={selected?.site}
                        size="small"
                    >
                        {cityList.map((item, index) => {
                            return (
                                <MenuItem value={item} key={index}>
                                    {item}
                                </MenuItem>
                            );
                        })}
                    </Select>
                </FormControl>
                <FormControl sx={{ width: "100%" }}>
                    <InputLabel
                        sx={{ backgroundColor: "#fff", px: 1 }}
                        size="small"
                    >
                        Tipo
                    </InputLabel>
                    <Select
                        onChange={(e) => setUpdateType(e.target.value)}
                        value={selected?.type}
                        size="small"
                    >
                        {typeList.map((item, index) => {
                            return (
                                <MenuItem value={item} key={index}>
                                    {item}
                                </MenuItem>
                            );
                        })}
                    </Select>
                </FormControl>
                    <Button
                        fullWidth
                        variant="contained"
                        onClick={() =>
                            selected && handleUpdateHoliday(selected.id)
                        }
                        disabled={!typeUpdate && !cityUpdate && !holidayUpdateDate}
                    >
                        Editar
                    </Button>
                </Box>
            </BaseModal>
        </Card>
    );
}

HolidayView.getLayout = getLayout("private");
