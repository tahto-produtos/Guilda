import React, { useState, useEffect, useContext } from "react";
import {
  Box,
  Button,
  Stack,
  useTheme,
  TextField,
  Checkbox,
  Typography,
  Autocomplete,
} from "@mui/material";
import { ActionButton } from "src/components/inputs/action-button/action-button";
import {
  Client,
  HomeFloor,
  Period,
  PostCreateModel,
  SectorAndSubsector,
} from "../../../typings";
import TextArea from "../../textarea/textarea";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import FileUploadIcon from "@mui/icons-material/FileUpload";
import VisibilityIcon from "@mui/icons-material/Visibility";
import { BaseModal } from "../../feedback";
import {
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  IconButton,
} from "@mui/material";
import FolderIcon from "@mui/icons-material/Folder";
import DeleteIcon from "@mui/icons-material/Delete";
import { CreatePostUseCase } from "../../../modules/post/create-post.use-case";
import { format } from "date-fns";
import { toast } from "react-toastify";
import { useLoadingState } from "../../../hooks";
import {
  ListClientUseCase,
  ListHomeFloorUseCase,
  ListPeriodUseCase,
  ListSectorsAndSubsectrosUseCase,
} from "../../../modules";
import { UserInfoContext } from "../../../contexts/user-context/user.context";
import { ListGroupsNewUseCase } from "../../../modules/groups/use-cases/list-groups-new";
import { GroupNew } from "../../../typings/models/group-new.model";
import abilityFor from "../../../utils/ability-for";
import { ListCollaboratorsAllUseCase } from "../../../modules/collaborators/use-cases/list-collaborators.use-case";
import { ListHierarchiesUseCase } from "../../../modules/hierarchies/use-cases/list-hierarchies.use-case";
import { Hierarchie } from "../../../typings/models/hierarchie.model";
import { ProfileImage } from "../profile-image/profile-image";
import { UserPersonaContext } from "src/contexts/user-persona/user-persona.context";
import { LoadingButton } from "@mui/lab";
import { SearchAccountsUseCase } from "src/modules/personas/use-cases/search-accounts.use-case";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
interface PostCreateProps {
  data: PostCreateModel;
}

export function PostCreate() {
  // const { data } = props;
  const theme = useTheme();
  const { myPermissions } = useContext(PermissionsContext);
  const { myUser } = useContext(UserInfoContext);
  const { persona } = useContext(UserPersonaContext);
  const [postMessage, setPostMessage] = useState<string>("");
  const [expirationDate, setExpirationDate] = useState<dateFns | Date | null>(
    null
  );
  const [isOpenModalPostUploadFiles, setIsOpenModalPostUploadFiles] =
    useState<boolean>(false);
  const [isOpenModalPostVisibility, setIsOpenModalPostVisibility] =
    useState<boolean>(false);
  const [image, setImage] = useState<File[]>([]);
  const [publicationHighlight, setPublicationHighlight] =
    useState<boolean>(false);
  const [publicationAllowComments, setPublicationAllowComments] =
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
  const { isLoading, startLoading, finishLoading } = useLoadingState();

  const handleClickModalUpload = () => {
    setIsOpenModalPostUploadFiles(!isOpenModalPostUploadFiles);
  };

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

      setImage(imagesArray as File[]);
    }
  };

  const handleRemoveImage = (imageRemove: any) => {
    const updatedImages = image.filter((img) => img.name !== imageRemove.name);
    setImage(updatedImages);
  };

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

  useEffect(() => {
    if (myUser && myUser?.id) {
      getClients(myUser?.id);
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

  const handlePublicar = async () => {
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
    const idsHomeOrFloors = selectedHomeFloor.map((homeFloor) =>
      Number(homeFloor.id)
    );
    await new CreatePostUseCase()
      .handle({ 
        message: postMessage,
        expirationDate: expirationDate
          ? format(new Date(expirationDate.toString()), "yyyy-MM-dd")
          : "",
        idsSectors,
        idsSubsectors,
        idsPeriods,
        idsGroups,
        idsHierarchies,
        idsCollaborators,
        idsClients,
        idsHomeOrFloors,
        files: image,
        publicationHighlight: publicationHighlight,
        publicationAllowComments: publicationAllowComments,
      })
      .then((data) => {
        setPostMessage("");
        setPublicationHighlight(false);
        setPublicationAllowComments(false);
        setExpirationDate(null);
        setImage([]);
        setSelectedSectors([]);
        setSelectedSubSector([]);
        setSelectedPeriods([]);
        setSelectedGroups([]);
        setSelectedCollaborator([]);
        setSelectedHierachies([]);
        setSelectedClient([]);
        setSelectedHomeFloor([]);
        toast.success("Post criado com sucesso!");
      })
      .catch((e) => {

        toast.error(`${e.response.data.Message}`);
  /*       toast.error("Falha ao criar o post."); */
      })
      .finally(() => {
        finishLoading();
      });
  };

  return (
    <Stack
      sx={{
        flexDirection: "column",
        gap: "5px",
        //p: "40px 32px",
        //border: `solid 1px ${theme.palette.grey[300]}`,
        //borderRadius: "8px",
        //bgcolor: theme.palette.grey[100],
      }}
    >
      <Stack direction={"row"} gap={"20px"} py={"10px"} alignItems={"center"}>
        <ProfileImage
          height="50px"
          width="50px"
          image={persona?.FOTO}
          name={persona?.NOME}
        />
        <TextField
          multiline
          value={postMessage}
          onChange={(e) => setPostMessage(e.target.value)}
          fullWidth
          placeholder="Fazer uma postagem."
        />
      </Stack>
      <Stack direction={"row"} gap={"15px"} pl={"70px"}>
        {Boolean(persona?.FLAGTAHTO) && (
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Data de expiração"
              value={expirationDate}
              onChange={(newValue) => setExpirationDate(newValue)}
              slotProps={{
                textField: {
                  size: "small",
                  sx: {
                    fontSize: "12px",
                    width: "100%",
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
        )}

        <ActionButton
          title={
            image.length > 0
              ? `${image.length} ${
                  image.length == 1
                    ? "arquivo selecionado"
                    : "arquivos selecionados"
                }`
              : "Selecionar imagens"
          }
          size="small"
          isActive={true}
          icon={<FileUploadIcon />}
          loading={false}
          onClick={() => handleClickModalUpload()}
        />
        <BaseModal
          title={"Upload de arquivos"}
          open={isOpenModalPostUploadFiles}
          onClose={handleClickModalUpload}
          fullWidth={true}
        >
          <Box mt={3} display={"flex"} flexDirection={"column"} gap={1}>
            <input
              accept="image/*"
              style={{ display: "none" }}
              id="image-upload"
              type="file"
              onChange={handleUploadFiles}
              multiple
            />
            {image.map((img, index) => (
              <List dense={true} key={index}>
                <ListItem
                  secondaryAction={
                    <IconButton
                      edge="end"
                      aria-label="delete"
                      onClick={() => handleRemoveImage(img)}
                    >
                      <DeleteIcon />
                    </IconButton>
                  }
                >
                  <ListItemIcon>
                    <FolderIcon />
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
            <Button variant="contained" onClick={handleClickModalUpload}>
              Concluir
            </Button>
          </Box>
        </BaseModal>
        
        {abilityFor(myPermissions).can("Visibilidade Publicação", "Persona") && (
          <ActionButton
            title={"Configurar visibilidade"}
            size="small"
            isActive={true}
            icon={<VisibilityIcon />}
            loading={false}
            onClick={() => handleClickModalVisibility()}
          />
        )}
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
                  <TextField {...props} label={"Setores"} />
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
                    <li {...props} key={option.id}>
                      {option.id} - {option.name}
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
          </Box>
          <Button
            fullWidth
            sx={{ mt: "20px" }}
            variant="contained"
            onClick={handleClickModalVisibility}
          >
            Concluir
          </Button>
        </BaseModal>
      </Stack>
      <Stack
        direction={"row"}
        justifyContent={"flex-end"}
        gap={"20px"}
        alignItems={"center"}
        mt={"30px"}
      >
        {Boolean(persona?.FLAGTAHTO) && (
          <Stack direction={"row"} alignItems={"center"} gap={"0px"}>
            <Checkbox
              checked={publicationHighlight}
              onChange={(e) => setPublicationHighlight(e.target.checked)}
            />
            <Typography
              variant="caption"
              sx={{ textWrap: "nowrap" }}
              fontSize={"12px"}
              lineHeight={"9px"}
            >
              Esta publicação será destaque?
            </Typography>
          </Stack>
        )}

        <Stack direction={"row"} alignItems={"center"} gap={"0px"}>
          <Checkbox
            checked={publicationAllowComments}
            onChange={(e) => setPublicationAllowComments(e.target.checked)}
          />
          <Typography
            variant="caption"
            sx={{ textWrap: "nowrap" }}
            fontSize={"12px"}
            lineHeight={"9px"}
          >
            Esta publicação poderá ter comentários?
          </Typography>
        </Stack>
        <LoadingButton
          loading={isLoading}
          onClick={() => handlePublicar()}
          variant="contained"
          color="secondary"
          sx={{
            lineHeight: "20px",
            fontWeight: "700",
            fontSize: "14px",
          }}
        >
          Postar
        </LoadingButton>
      </Stack>

      {/*<Divider/>*/}
    </Stack>
  );
}
