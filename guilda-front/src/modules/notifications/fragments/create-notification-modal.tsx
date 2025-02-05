import { Delete, FolderOpen } from "@mui/icons-material";
import {
  Autocomplete,
  Box,
  Button,
  Checkbox,
  IconButton,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  MenuItem,
  Select,
  Stack,
  TextField,
  Typography,
  useTheme,
} from "@mui/material";
import {
  DatePicker,
  LocalizationProvider,
  TimePicker,
} from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { useContext, useEffect, useState } from "react";
import { BaseModal } from "src/components/feedback";
import { CreateNotificationUseCase } from "../use-cases/create-notification.use-case";
import { toast } from "react-toastify";
import { format, set } from "date-fns";
import { LoadingButton } from "@mui/lab";
import { useLoadingState } from "src/hooks";
import { ActionButton } from "src/components/inputs/action-button/action-button";
import { Client, HomeFloor, Period, SectorAndSubsector } from "src/typings";
import { Site } from "src/typings/models/site.model";
import { GroupNew } from "src/typings/models/group-new.model";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { ListCollaboratorsAllUseCase } from "src/modules/collaborators/use-cases/list-collaborators.use-case";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { ListSectorsAndSubsectrosUseCase } from "src/modules/sectors";
import { ListPeriodUseCase } from "src/modules/period";
import { ListGroupsNewUseCase } from "src/modules/groups/use-cases/list-groups-new";
import { ListHierarchiesUseCase } from "src/modules/hierarchies/use-cases/list-hierarchies.use-case";
import { ListClientUseCase } from "src/modules/client";
import { ListHomeFloorUseCase } from "src/modules/home-floor";
import { SearchAccountsUseCase } from "src/modules/personas/use-cases/search-accounts.use-case";
import { SitePersonaUseCase } from "src/modules/personas/use-cases/site-personas.use-case";

interface CreateNotificationModalProps {
  open: boolean;
  onClose: () => void;
  refresh?: () => void;
}

export function CreateNotificationModal(props: CreateNotificationModalProps) {
  const { onClose, open, refresh } = props;

  const theme = useTheme();
  const { myUser } = useContext(UserInfoContext);
  const [fromHour, setFromHour] = useState<Date | null>(null);
  const [toHour, setToHour] = useState<Date | null>(null);

  const [title, setTitle] = useState<string>("");
  const [msg, setMsg] = useState<string>("");
  const [startDate, setStartDate] = useState<Date | null>(null);
  const [endDate, setEndDate] = useState<Date | null>(null);
  const [type, setType] = useState<number>(5);
  const [files, setFiles] = useState<File[]>([]);

  const [isActive, setIsActive] = useState<boolean>(false);

  const { finishLoading, isLoading, startLoading } = useLoadingState();

  const [isOpenModalPostVisibility, setIsOpenModalPostVisibility] =
    useState<boolean>(false);
  const [sectors, setSectors] = useState<SectorAndSubsector[]>([]);
  const [selectedSectors, setSelectedSectors] = useState<SectorAndSubsector[]>(
    []
  );
  const [sectorSearch, setSectorSearch] = useState<string>("");
  const [subSectors, setSubSectors] = useState<SectorAndSubsector[]>([]);
  const [selectedSubSector, setSelectedSubSector] = useState<
    SectorAndSubsector[]
  >([]);
  const [subSectorSearch, setSubSectorSearch] = useState<string>("");
  const [periods, setPeriods] = useState<Period[]>([]);
  const [selectedPeriods, setSelectedPeriods] = useState<Period[]>([]);
  const [groups, setGroups] = useState<GroupNew[]>([]);
  const [selectedGroups, setSelectedGroups] = useState<GroupNew[]>([]);
  const [clients, setClients] = useState<Client[]>([]);
  
  const [selectedClient, setSelectedClient] = useState<Client[]>([]);
  const [sites, setSites] = useState<Site[]>([]);
  const [selectedSite, setSelectedSite] = useState<Site[]>([]);
  const [homesFloors, setHomesFloors] = useState<HomeFloor[]>([]);
  const [selectedHomeFloor, setSelectedHomeFloor] = useState<HomeFloor[]>([]);
  const [collaborators, setCollaborators] = useState<
    {
      id: number;
      name: string;
      registry: string;
    }[]
  >([]);
  const [selectedCollaborator, setSelectedCollaborator] = useState<
    {
      id: number;
      name: string;
      registry: string;
    }[]
  >([]);
  const [collaboratorSearch, setCollaboratorSearch] = useState<string>("");
  const [hierarchies, setHierarchies] = useState<Hierarchie[]>([]);
  const [selectedHierarchies, setSelectedHierachies] = useState<Hierarchie[]>(
    []
  );

  const getSectorsAndSubSectors = async (isSubsector = false, sector = "") => {
    startLoading();

    await new ListSectorsAndSubsectrosUseCase()
      .handle({
        isSubsector,
        sector,
      })
      .then((data) => {
        if (isSubsector) {
          setSubSectors(data);
        } else {
          setSectors(data);
        }
      })
      .catch(() => {
        toast.error(
          `Falha ao carregar ${isSubsector ? "subsetores" : "setores"}.`
        );
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    getSectorsAndSubSectors(false, sectorSearch);
  }, [sectorSearch]);

  useEffect(() => {
    getSectorsAndSubSectors(true, subSectorSearch);
  }, [subSectorSearch]);

  const getPeriods = async (codCollaborator: number) => {
    startLoading();

    await new ListPeriodUseCase()
      .handle({
        codCollaborator,
      })
      .then((data) => {
        setPeriods(data);
      })
      .catch(() => {
        toast.error("Falha ao carregar turnos.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    if (myUser && myUser?.id) {
      getPeriods(myUser?.id);
    }
  }, []);

  const getHierarchies = async () => {
    startLoading();

    await new ListHierarchiesUseCase()
      .handle({
        limit: 100,
        offset: 0,
      })
      .then((data) => {
        const hierarchiesResponse: Hierarchie[] = data.items;
        setHierarchies(hierarchiesResponse);
      })
      .catch(() => {
        toast.error("Falha ao carregar hierarquias.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    getHierarchies();
  }, []);

  const getGroups = async (codCollaborator: number) => {
    startLoading();

    await new ListGroupsNewUseCase()
      .handle({
        codCollaborator,
      })
      .then((data) => {
        setGroups(data);
      })
      .catch(() => {
        toast.error("Falha ao carregar grupos.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    if (myUser && myUser?.id) {
      getGroups(myUser?.id);
    }
  }, []);

  const getClients = async (codCollaborator: number) => {
    startLoading();

    await new ListClientUseCase()
      .handle({
        codCollaborator,
      })
      .then((data) => {
        setClients(data);
      })
      .catch(() => {
        toast.error("Falha ao carregar clients.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  async function getSites() {
    await new SitePersonaUseCase()
      .handle()
      .then((data) => {
        setSites(data);
      })
      .catch(() => { })
      .finally(() => { });
  }

  

  useEffect(() => {
    if (myUser && myUser?.id) {
      getClients(myUser?.id);
      getSites();
    }
  }, []);

  const getHomeOrFloor = async (codCollaborator: number) => {
    startLoading();

    await new ListHomeFloorUseCase()
      .handle({
        codCollaborator,
      })
      .then((data) => {
        setHomesFloors(data);
      })
      .catch(() => {
        toast.error("Falha ao carregar home ou piso.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    if (myUser && myUser?.id) {
      getHomeOrFloor(myUser?.id);
    }
  }, []);

  const getCollaborators = async (searchText: string) => {
    startLoading();

    await new SearchAccountsUseCase()
      .handle({
        limit: 10,
        page: 1,
        Collaborator: searchText,
      })
      .then((data) => {
        setCollaborators(data[0].account);
      })
      .catch(() => {
        toast.error("Falha ao carregar colaboradores.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    getCollaborators(collaboratorSearch);
  }, [collaboratorSearch]);

  const handleClickModalVisibility = () => {
    setIsOpenModalPostVisibility(!isOpenModalPostVisibility);
  };

  const handleUploadFiles = (event: any) => {
    const files = event.target.files;

    if (files.length > 0) {
      const imagesArray = Array.from(files);

      imagesArray.forEach((file, index) => {
        const reader = new FileReader();
        reader.readAsDataURL(file as File);
        reader.onloadend = () => {};
      });

      setFiles(imagesArray as File[]);
    }
  };

  const handleRemoveImage = (imageRemove: any) => {
    const updatedImages = files.filter((img) => img.name !== imageRemove.name);
    setFiles(updatedImages);
  };

  const handleCreate = async () => {
    startLoading();

    const idsSectors = selectedSectors.map((sector) => Number(sector.id));
    const idsSubsectors = selectedSubSector.map((subsector) =>
      Number(subsector.id)
    );
    const idsPeriods = selectedPeriods.map((period) => Number(period.id));
    const idsGroups = selectedGroups.map((group) => Number(group.id));
    const idsHierarchies = selectedHierarchies.map((hierarchie) =>
      Number(hierarchie.id)
    );
    const idsCollaborators = selectedCollaborator.map((collaborator) =>
      Number(collaborator.id)
    );
    const idsClients = selectedClient.map((client) => Number(client.id));
    const idsSites = selectedSite.map((site) => Number(site.id));
    const idsHomeOrFloors = selectedHomeFloor.map((homeFloor) =>
      Number(homeFloor.id)
    );

    const fromHourDateTime =
      fromHour &&
      startDate &&
      set(startDate, {
        hours: fromHour.getHours(),
        minutes: fromHour.getMinutes(),
      });

    const toHourDateTime =
      toHour &&
      endDate &&
      set(endDate, {
        hours: toHour.getHours(),
        minutes: toHour.getMinutes(),
      });

    await new CreateNotificationUseCase()
      .handle({
        ACTIVE: isActive,
        ENDED_AT: toHourDateTime
          ? format(toHourDateTime, "yyyy-MM-dd HH:mm")
          : "",
        files: files,
        IDGDA_NOTIFICATION_TYPE: type,
        NOTIFICATION: msg,
        STARTED_AT: fromHourDateTime
          ? format(fromHourDateTime, "yyyy-MM-dd HH:mm")
          : "",
        TITLE: title,
        visibility: {
          sector: idsSectors || [],
          subSector: idsSubsectors || [],
          period: idsPeriods || [],
          hierarchy: idsHierarchies || [],
          group: idsGroups || [],
          userId: idsCollaborators || [],
          client: idsClients || [],
          site: idsSites || [],
          homeOrFloor: idsHomeOrFloors || [],
        },
      })
      .then((data) => {
        toast.success("Criado com sucesso!");
        refresh && refresh();
      })
      .catch(() => {
        toast.error("Falha ao criar.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  return (
    <BaseModal
      width={"540px"}
      open={open}
      title={`Criar notificação`}
      onClose={onClose}
    >
      <Stack direction={"column"} gap={"16px"}>
        <TextField
          fullWidth
          label="Título"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
        />
        <TextField
          fullWidth
          label="Mensagem"
          value={msg}
          onChange={(e) => setMsg(e.target.value)}
        />
        <Stack direction={"row"} gap={"16px"}>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Inicia em"
              value={startDate}
              onChange={(newValue) => setStartDate(newValue)}
              slotProps={{
                textField: {
                  sx: {
                    fontSize: "12px",
                    width: "230px",
                    height: "38px",
                    "& .MuiInputBase-input": {
                      fontSize: "12px",
                    },
                    svg: {
                      color: theme.palette.grey["500"],
                    },
                  },
                },
              }}
            />
          </LocalizationProvider>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <TimePicker
              label="dás"
              value={fromHour}
              onChange={(newValue) => setFromHour(newValue)}
            />
          </LocalizationProvider>
        </Stack>
        <Stack direction={"row"} gap={"16px"}>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Finaliza em"
              value={endDate}
              onChange={(newValue) => setEndDate(newValue)}
              slotProps={{
                textField: {
                  sx: {
                    fontSize: "12px",
                    width: "230px",
                    height: "38px",
                    "& .MuiInputBase-input": {
                      fontSize: "12px",
                    },
                    svg: {
                      color: theme.palette.grey["500"],
                    },
                  },
                },
              }}
            />
          </LocalizationProvider>

          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <TimePicker
              label="ás"
              value={toHour}
              onChange={(newValue) => setToHour(newValue)}
            />
          </LocalizationProvider>
        </Stack>
        <Select
          value={type}
          sx={{ mt: "20px" }}
          fullWidth
          onChange={(e) => setType(parseInt(e.target.value.toString()))}
        >
          <MenuItem value={5}>Informação</MenuItem>
          <MenuItem value={6}>Evento</MenuItem>
          <MenuItem value={3}>Campanha</MenuItem>
          <MenuItem value={10}>Novidade</MenuItem>
          <MenuItem value={11}>Comunicados internos</MenuItem>
          <MenuItem value={12}>Atualização de status</MenuItem>
          <MenuItem value={13}>Lembretes e alertas</MenuItem>
        </Select>
        <Stack direction={"row"} alignItems={"center"} gap={"20px"}>
          <Typography>Ativar notificação?</Typography>
          <Checkbox
            checked={isActive}
            onChange={(e) => setIsActive(e.target.checked)}
          />
        </Stack>
        <Box mt={3} display={"flex"} flexDirection={"column"} gap={1}>
          <input
            accept=""
            style={{ display: "none" }}
            id="image-upload"
            type="file"
            onChange={handleUploadFiles}
            multiple
          />
          {files.map((img, index) => (
            <List dense={true} key={index}>
              <ListItem
                secondaryAction={
                  <IconButton
                    edge="end"
                    aria-label="delete"
                    onClick={() => handleRemoveImage(img)}
                  >
                    <Delete />
                  </IconButton>
                }
              >
                <ListItemIcon>
                  <FolderOpen />
                </ListItemIcon>
                <ListItemText
                  primary={img.name}
                  primaryTypographyProps={{
                    fontFamily: "Open Sans",
                  }}
                />
              </ListItem>
            </List>
          ))}
          <ActionButton
            title={"Configurar visibilidade"}
            isActive={true}
            loading={false}
            onClick={() => handleClickModalVisibility()}
          />
          <BaseModal
            title={"Configurar visibilidade"}
            open={isOpenModalPostVisibility}
            onClose={handleClickModalVisibility}
            fullWidth={true}
          >
            <Box mt={3} display={"flex"} flexDirection={"column"} gap={1}>
              <Stack direction={"row"} alignItems={"center"} gap={"30px"}>
                <Autocomplete
                  multiple
                  fullWidth
                  value={selectedSectors}
                  options={sectors}
                  getOptionLabel={(option) => option.name}
                  onChange={(event, value) => {
                    setSelectedSectors(value);
                  }}
                  onInputChange={(e, text) => setSectorSearch(text)}
                  filterOptions={(x) => x}
                  filterSelectedOptions
                  renderInput={(props) => (
                    <TextField {...props} label={"Sector"} />
                  )}
                  renderOption={(props, option) => {
                    return (
                      <li {...props} key={option.id}>
                        {option.id} - {option.name}
                      </li>
                    );
                  }}
                  isOptionEqualToValue={(option, value) =>
                    option.name === value.name &&
                    option.id === value.id
                  }
                  sx={{ m: 0 }}
                />
              </Stack>
              <Stack direction={"row"} alignItems={"center"} gap={"30px"}>
                <Autocomplete
                  multiple
                  fullWidth
                  value={selectedSubSector}
                  options={subSectors}
                  getOptionLabel={(option) => option.name}
                  onChange={(event, value) => {
                    setSelectedSubSector(value);
                  }}
                  onInputChange={(e, text) => setSubSectorSearch(text)}
                  filterOptions={(x) => x}
                  filterSelectedOptions
                  renderInput={(props) => (
                    <TextField {...props} label={"Subsetor"} />
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
                  sx={{ m: 0 }}
                />
              </Stack>
              <Stack direction={"row"} alignItems={"center"} gap={"30px"}>
                <Autocomplete
                  multiple
                  fullWidth
                  value={selectedPeriods}
                  options={periods}
                  getOptionLabel={(option) => option.name}
                  onChange={(event, value) => {
                    setSelectedPeriods(value);
                  }}
                  filterSelectedOptions
                  renderInput={(props) => (
                    <TextField {...props} label={"Turno"} />
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
                  sx={{ m: 0 }}
                />
              </Stack>
              <Stack direction={"row"} alignItems={"center"} gap={"30px"}>
                <Autocomplete
                  multiple
                  fullWidth
                  value={selectedGroups}
                  options={groups}
                  getOptionLabel={(option) => option.name}
                  onChange={(event, value) => {
                    setSelectedGroups(value);
                  }} 
                  filterSelectedOptions
                  renderInput={(props) => (
                    <TextField {...props} label={"Grupos"} />
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
                  sx={{ m: 0 }}
                />
              </Stack>
              <Stack direction={"row"} alignItems={"center"} gap={"30px"}>
                <Autocomplete
                  multiple
                  fullWidth
                  value={selectedCollaborator}
                  options={collaborators}
                  onInputChange={(e, text) => setCollaboratorSearch(text)}
                  getOptionLabel={(option) => option.name}
                  onChange={(event, value) => {
                    setSelectedCollaborator(value);
                  }}
                  filterSelectedOptions
                  renderInput={(props) => (
                    <TextField {...props} label={"Colaboradores"} />
                  )}
                  renderOption={(props, option) => {
                    return (
                      <li {...props} key={option?.id}>
                        {option?.id} - {option?.name}
                      </li>
                    );
                  }}
                  isOptionEqualToValue={(option, value) =>
                    option.name === value.name
                  }
                  sx={{ m: 0 }}
                />
              </Stack>
              <Stack direction={"row"} alignItems={"center"} gap={"30px"}>
                <Autocomplete
                  multiple
                  fullWidth
                  value={selectedHierarchies}
                  options={hierarchies}
                  getOptionLabel={(option) => option.levelName}
                  onChange={(event, value) => {
                    setSelectedHierachies(value);
                  }}
                  filterSelectedOptions
                  renderInput={(props) => (
                    <TextField {...props} label={"Hierarquias"} />
                  )}
                  renderOption={(props, option) => {
                    return (
                      <li {...props} key={option.id}>
                        {option.levelName}
                      </li>
                    );
                  }}
                  isOptionEqualToValue={(option, value) =>
                    option.levelName === value.levelName
                  }
                  sx={{ m: 0 }}
                />
              </Stack>
              <Stack direction={"row"} alignItems={"center"} gap={"30px"}>
                <Autocomplete
                  multiple
                  fullWidth
                  value={selectedClient}
                  options={clients}
                  getOptionLabel={(option) => option.name}
                  onChange={(event, value) => {
                    setSelectedClient(value);
                  }}
                  filterSelectedOptions
                  renderInput={(props) => (
                    <TextField {...props} label={"Cliente"} />
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
                  sx={{ m: 0 }}
                />
              </Stack>
              <Stack direction={"row"} alignItems={"center"} gap={"30px"}>
                <Autocomplete
                  multiple
                  fullWidth
                  value={selectedHomeFloor}
                  options={homesFloors}
                  getOptionLabel={(option) => option.name}
                  onChange={(event, value) => {
                    setSelectedHomeFloor(value);
                  }}
                  filterSelectedOptions
                  renderInput={(props) => (
                    <TextField {...props} label={"Home/Piso"} />
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
                  sx={{ m: 0 }}
                />
              </Stack>
              <Stack direction={"row"} alignItems={"center"} gap={"30px"}>
                <Autocomplete
                  multiple
                  fullWidth
                  value={selectedSite}
                  options={sites}
                  getOptionLabel={(option) => option.name}
                  onChange={(event, value) => {
                    setSelectedSite(value);
                  }}
                  filterSelectedOptions
                  renderInput={(props) => (
                    <TextField {...props} label={"Cidade"} />
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
                  sx={{ m: 0 }}
                />
              </Stack>
            </Box>
            <Button
              variant="contained"
              fullWidth
              onClick={handleClickModalVisibility}
              sx={{ mt: "20px" }}
            >
              Concluir
            </Button>
          </BaseModal>
          <label htmlFor="image-upload">
            <Button
              variant="outlined"
              color="primary"
              component="span"
              fullWidth
            >
              Selecionar arquivos
            </Button>
          </label>
        </Box>
        <LoadingButton
          loading={isLoading}
          variant="contained"
          onClick={handleCreate}
        >
          Confirmar
        </LoadingButton>
      </Stack>
    </BaseModal>
  );
}
