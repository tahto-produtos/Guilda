import {
    Autocomplete,
    Button,
    Checkbox,
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    Stack,
    TextField,
    Typography,
    useTheme,
} from "@mui/material";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { StatePersonaUseCase } from "../../use-cases/state-personas.use-case";
import { UF } from "src/typings/models/uf.model";
import { SitePersonaUseCase } from "../../use-cases/site-personas.use-case";
import { Site } from "src/typings/models/site.model";
import { CityPersonaUseCase } from "../../use-cases/city-personas.use-case";
import { City } from "src/typings/models/city.model";
import { Persona } from "src/typings/models/persona.model";
import { EditPersonasUseCase } from "../../use-cases/edit-personas.use-case";
import { capitalizeText } from "src/utils/capitalizeText";
import { format } from "date-fns";
import { removeEmojis } from "src/utils/removeEmojis";

interface EditPersonaComponentProps {
    initialState?: Persona;
    idPersona: string;
}

export function EditPersonaComponent(props: EditPersonaComponentProps) {
    const { initialState, idPersona } = props;

    const theme = useTheme();

    const [birthDate, setBirthDate] = useState<dateFns | Date | null>(
        initialState?.DATA_NASCIMENTO
            ? new Date(
                  new Date(initialState?.DATA_NASCIMENTO).getFullYear(),
                  new Date(initialState?.DATA_NASCIMENTO).getMonth(),
                  new Date(initialState?.DATA_NASCIMENTO).getDate()
              )
            : null
    );

    const [name, setName] = useState<string>(initialState?.NOME || "");
    const [socialName, setSocialName] = useState<string>(
        initialState?.NOME_SOCIAL || ""
    );
    const [displayAge, setDisplayAge] = useState<0 | 1>(
        initialState?.IDADE || 0
    );
    const [email, setEmail] = useState<string>(initialState?.EMAIL || "");
    const [phone, setPhone] = useState<string>(initialState?.TELEFONE || "");
    const [whoIs, setWhoIs] = useState<string>(initialState?.QUEM_E || "");

    const [uf, setUf] = useState<UF | null>(
        initialState?.ID_UF && initialState?.UF
            ? { id: parseInt(initialState.ID_UF), name: initialState.UF }
            : null
    );
    const [ufs, setUfs] = useState<UF[]>([]);

    const [site, setSite] = useState<Site | null>(
        initialState?.ID_SITE && initialState?.SITE
            ? { id: parseInt(initialState.ID_SITE), name: initialState.SITE }
            : null
    );
    const [sites, setSites] = useState<Site[]>([]);

    const [city, setCity] = useState<City | null>(
        initialState?.ID_CIDADE && initialState?.CIDADE
            ? {
                  id: parseInt(initialState.ID_CIDADE),
                  name: initialState.CIDADE,
              }
            : null
    );
    const [cities, setCities] = useState<City[]>([]);
    const [citySearch, setCitySearch] = useState<string>("");

    async function ListStatePersona() {
        await new StatePersonaUseCase()
            .handle()
            .then((data) => {
                setUfs(data);
            })
            .catch(() => {
                toast.error("Erro ao listar UF's.");
            })
            .finally(() => {});
    }

    useEffect(() => {
        ListStatePersona();
    }, []);

    async function ListSitePersona() {
        await new SitePersonaUseCase()
            .handle()
            .then((data) => {
                setSites(data);
            })
            .catch(() => {
                toast.error("Erro ao listar Sites.");
            })
            .finally(() => {});
    }

    useEffect(() => {
        ListSitePersona();
    }, []);

    async function ListCityPersona() {
        if (!uf) return;

        await new CityPersonaUseCase()
            .handle({ city: citySearch, state: uf.id })
            .then((data) => {
                setCities(data);
            })
            .catch(() => {
                toast.error("Erro ao listar Cidades.");
            })
            .finally(() => {});
    }

    useEffect(() => {
        ListCityPersona();
    }, [uf, citySearch]);

    async function handleSave() {
        await new EditPersonasUseCase()
            .handle({
                BC: initialState?.BC || "",
                CIDADE: city?.id,
                NOME_SOCIAL: socialName,
                DATA_NASCIMENTO: birthDate
                    ? format(new Date(birthDate.toString()), "yyyy-MM-dd")
                    : "",
                EMAIL: email,
                FOTO: initialState?.FOTO,
                HOBBIES: initialState?.HOBBIES,
                IDADE: displayAge,
                IDPERSONAUSER: idPersona,
                MOTIVACOES: initialState?.MOTIVACOES,
                NOME: name,
                OBJETIVO: initialState?.OBJETIVO,
                SITE: site?.id.toString(),
                TELEFONE: phone,
                UF: uf?.id,
                WHO_IS: whoIs,
            })
            .then((data) => {
                toast.success("Salvo com sucesso!");
            })
            .catch((e) => {
                const msg = e?.response?.data?.Message;

                toast.error(msg ? msg : "Erro ao salvar.");
            })
            .finally(() => {});
    }

    return (
        <Stack width={"100%"} mt={"25px"}>
            <Stack direction={"column"} gap={"30px"}>
                <TextField
                    value={`${
                        initialState?.BC && `${initialState?.BC} - `
                    } ${capitalizeText(name)}`}
                    label="Nome"
                    InputProps={{ readOnly: true }}
                />
                <TextField
                    value={socialName}
                    onChange={(e) => setSocialName(e.target.value)}
                    label="Nome social"
                />
                <TextField
                    value={email}
                    onChange={(e) => setEmail(removeEmojis(e.target.value))}
                    label="E-mail"
                />
                <TextField
                    value={phone}
                    onChange={(e) => setPhone(removeEmojis(e.target.value))}
                    label="Telefone/Celular"
                    type="number"
                />

                <Stack direction={"row"} alignItems={"center"} gap={"30px"}>
                    <LocalizationProvider dateAdapter={AdapterDateFns}>
                        <DatePicker
                            label="Data de nascimento"
                            value={birthDate}
                            onChange={(newValue) => setBirthDate(newValue)}
                            slotProps={{
                                textField: {
                                    sx: {
                                        width: "100%",
                                        svg: {
                                            color: theme.palette.grey["700"],
                                        },
                                    },
                                },
                            }}
                        />
                    </LocalizationProvider>
                    <Stack
                        direction={"row"}
                        alignItems={"center"}
                        gap={"0px"}
                        onClick={() => {
                            if (displayAge == 1) {
                                setDisplayAge(0);
                            } else {
                                setDisplayAge(1);
                            }
                        }}
                    >
                        <Checkbox checked={displayAge == 1} />
                        <Typography
                            variant="caption"
                            sx={{ textWrap: "nowrap" }}
                        >
                            Esta informação ficará visível?
                        </Typography>
                    </Stack>
                </Stack>

                <Stack direction={"row"} alignItems={"center"} gap={"30px"}>
                    <Autocomplete
                        value={uf}
                        placeholder={"UF"}
                        disableClearable={false}
                        onChange={(e, value) => {
                            setUf(value);
                        }}
                        isOptionEqualToValue={(option, value) =>
                            option.id == value.id
                        }
                        disableCloseOnSelect
                        renderInput={(props) => (
                            <TextField {...props} label={"UF"} />
                        )}
                        getOptionLabel={(option) => option.name}
                        options={ufs}
                        fullWidth
                        renderOption={(props, option) => {
                            return (
                                <li {...props} key={option.id}>
                                    {option.name}
                                </li>
                            );
                        }}
                        sx={{ m: 0 }}
                    />
                    <Autocomplete
                        value={site}
                        placeholder={"Site"}
                        disableClearable={false}
                        onChange={(e, value) => {
                            setSite(value);
                        }}
                        isOptionEqualToValue={(option, value) =>
                            option.id == value.id
                        }
                        disableCloseOnSelect
                        renderInput={(props) => (
                            <TextField {...props} label={"Site"} />
                        )}
                        getOptionLabel={(option) => option.name}
                        options={sites}
                        fullWidth
                        renderOption={(props, option) => {
                            return (
                                <li {...props} key={option.id}>
                                    {option.name}
                                </li>
                            );
                        }}
                        sx={{ m: 0 }}
                    />
                    <Autocomplete
                        value={city}
                        placeholder={"Cidade"}
                        disableClearable={false}
                        onChange={(e, value) => {
                            setCity(value);
                        }}
                        isOptionEqualToValue={(option, value) =>
                            option.id == value.id
                        }
                        onInputChange={(e, text) => setCitySearch(text)}
                        disableCloseOnSelect
                        renderInput={(props) => (
                            <TextField {...props} label={"Cidade"} />
                        )}
                        getOptionLabel={(option) => option.name}
                        options={cities}
                        fullWidth
                        renderOption={(props, option) => {
                            return (
                                <li {...props} key={option.id}>
                                    {option.name}
                                </li>
                            );
                        }}
                        sx={{ m: 0 }}
                    />
                </Stack>
                <TextField
                    label="Quem é"
                    value={whoIs}
                    onChange={(e) => setWhoIs(e.target.value)}
                />
            </Stack>
            <Stack direction={"row"} justifyContent={"flex-end"} mt={"30px"}>
                <Button variant="contained" onClick={handleSave}>
                    Salvar alterações
                </Button>
            </Stack>
        </Stack>
    );
}
