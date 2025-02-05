import { useTheme } from "@emotion/react";
import {
  Autocomplete,
  Box,
  Button,
  Checkbox,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { BaseModal } from "src/components/feedback";
import { City } from "src/typings/models/city.model";
import { Site } from "src/typings/models/site.model";
import { UF } from "src/typings/models/uf.model";
import { StatePersonaUseCase } from "../../use-cases/state-personas.use-case";
import { SitePersonaUseCase } from "../../use-cases/site-personas.use-case";
import { CityPersonaUseCase } from "../../use-cases/city-personas.use-case";
import { toast } from "react-toastify";
import { capitalizeText } from "src/utils/capitalizeText";
import { removeEmojis } from "src/utils/removeEmojis";
import { Hobby } from "src/typings/models/hobby.model";
import { ListHobbyUseCase } from "../../use-cases/list-hobby.use-case";
import { CreateAccountPersonasUseCase } from "../../use-cases/create-account-persona.use-case";
import { UserPersonaContext } from "src/contexts/user-persona/user-persona.context";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
}

export function CreateAccountPersonaModal(props: IProps) {
  const { isOpen, onClose } = props;
  const { LoggedUserAccounts } = useContext(UserPersonaContext);
  const theme = useTheme();

  const [name, setName] = useState<string>("");
  const [socialName, setSocialName] = useState<string>("");
  const [email, setEmail] = useState<string>("");
  const [phone, setPhone] = useState<string>("");
  const [whoIs, setWhoIs] = useState<string>("");

  const [uf, setUf] = useState<UF | null>(null);
  const [ufs, setUfs] = useState<UF[]>([]);

  const [site, setSite] = useState<Site | null>(null);
  const [sites, setSites] = useState<Site[]>([]);

  const [city, setCity] = useState<City | null>(null);
  const [cities, setCities] = useState<City[]>([]);
  const [citySearch, setCitySearch] = useState<string>("");

  const [motivations, setMotivations] = useState<string>("");
  const [targets, setTargets] = useState<string>("");
  const [hobbies, setHobbies] = useState<Hobby[]>([]);
  const [hobbiesShow, setHobbiesShow] = useState<Hobby[]>([]);
  const [selectedHobbies, setSelectedHobbies] = useState<Hobby[]>([]);

  const [isPublic, setIsPublic] = useState(false);
  const [isPersonal, setIsPersonal] = useState(true);

  const [isTahto, setIsTahto] = useState<boolean>(false);

  async function listHobbies() {
    new ListHobbyUseCase()
      .handle({
        hobby: "",
      })
      .then((data) => {
        setHobbies(data);
        const notSelectedHobbiesList = data.filter(
          (hobby) =>
            !selectedHobbies.some(
              (selectedHobby) => selectedHobby.HOBBY == hobby.HOBBY
            )
        );
        setHobbiesShow(notSelectedHobbiesList);
      })
      .catch(() => {
        toast.error("Erro ao carregar os hobbies.");
      })
      .finally(() => {});
  }

  useEffect(() => {
    listHobbies();
  }, []);

  useEffect(() => {
    const notSelectedHobbiesList = hobbies.filter(
      (hobby) =>
        !selectedHobbies.some(
          (selectedHobby) => selectedHobby.HOBBY == hobby.HOBBY
        )
    );
    setHobbiesShow(notSelectedHobbiesList);
  }, [selectedHobbies]);

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

  async function handleCreate() {
    await new CreateAccountPersonasUseCase()
      .handle({
        TYPE: isPersonal ? 1 : 2,
        VISIBILITY: isPublic ? 1 : 2,
        CIDADE: city?.id,
        HOBBIES: selectedHobbies,
        MOTIVACOES: motivations,
        NOME: name,
        NOME_SOCIAL: socialName,
        OBJETIVO: targets,
        SITE: site?.id,
        TELEFONE: phone,
        UF: uf?.id,
        WHO_IS: whoIs,
        PERSONATAHTO: isTahto ? 1 : 0,
      })
      .then((data) => {
        toast.success("Conta criada com sucesso");
        LoggedUserAccounts();
        onClose();
      })
      .catch((e) => {
        toast.error("Erro ao criar conta");
      });
  }

  return (
    <BaseModal
      width={"540px"}
      open={isOpen}
      title={`Nova conta`}
      onClose={onClose}
    >
      <Box width={"100%"} display={"flex"} flexDirection={"column"} gap={1}>
        <Stack direction={"column"} gap={"30px"}>
          <TextField
            value={`${capitalizeText(name)}`}
            label="Nome"
            onChange={(e) => setName(e.target.value)}
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
          <Stack
            direction={"row"}
            gap={"10px"}
            alignItems={"center"}
            onClick={() => setIsPublic(!isPublic)}
          >
            <Checkbox checked={isPublic} />
            <Typography>Conta publica?</Typography>
          </Stack>
          {/* <Stack
            direction={"row"}
            alignItems={"center"}
            gap={"10px"}
            onClick={() => setIsPersonal(!isPersonal)}
          >
            <Checkbox checked={isPersonal} />
            <Typography>Conta Pessoal?</Typography>
          </Stack> */}
          <Stack
            direction={"row"}
            gap={"10px"}
            alignItems={"center"}
            onClick={() => setIsTahto(!isTahto)}
          >
            <Checkbox checked={isTahto} />
            <Typography>Conta Tahto?</Typography>
          </Stack>
          <Stack direction={"row"} alignItems={"center"} gap={"30px"}>
            <Autocomplete
              value={uf}
              placeholder={"UF"}
              disableClearable={false}
              onChange={(e, value) => {
                setUf(value);
              }}
              isOptionEqualToValue={(option, value) => option.id == value.id}
              disableCloseOnSelect
              renderInput={(props) => <TextField {...props} label={"UF"} />}
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
              isOptionEqualToValue={(option, value) => option.id == value.id}
              disableCloseOnSelect
              renderInput={(props) => <TextField {...props} label={"Site"} />}
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
              isOptionEqualToValue={(option, value) => option.id == value.id}
              onInputChange={(e, text) => setCitySearch(text)}
              disableCloseOnSelect
              renderInput={(props) => <TextField {...props} label={"Cidade"} />}
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
          <TextField
            value={motivations}
            onChange={(e) => setMotivations(removeEmojis(e.target.value))}
            label="Quais suas motivações?"
          />
          <TextField
            value={targets}
            onChange={(e) => setTargets(removeEmojis(e.target.value))}
            label="Objetivos?"
          />
          <Autocomplete
            multiple
            id="tags-outlined"
            options={hobbiesShow}
            getOptionLabel={(option) => option.HOBBY}
            defaultValue={selectedHobbies}
            renderInput={(params) => <TextField {...params} label="Hobbies" />}
            onChange={(event, newValue) => {
              setSelectedHobbies(newValue);
            }}
          />
        </Stack>
        <Button onClick={handleCreate} variant="contained">
          Confirmar
        </Button>
      </Box>
    </BaseModal>
  );
}
