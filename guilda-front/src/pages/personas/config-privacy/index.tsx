import { LoadingButton } from "@mui/lab";
import {
  Autocomplete,
  Button,
  Checkbox,
  Stack,
  TextField,
  Typography,
  useTheme,
} from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { useLoadingState } from "src/hooks";
import {
  ListClientUseCase,
  ListHomeFloorUseCase,
  ListPeriodUseCase,
  ListSectorsAndSubsectrosUseCase,
} from "src/modules";
import { CreateBlackList } from "src/modules/blacklist/use-cases/create-blacklist.use-case";
import { ListCollaboratorsAllUseCase } from "src/modules/collaborators/use-cases/list-collaborators.use-case";
import { CreateConfigPrivacyUseCase } from "src/modules/config-privacy/create-config-privacy.use-case";
import { ListGroupsNewUseCase } from "src/modules/groups/use-cases/list-groups-new";
import { ListHierarchiesUseCase } from "src/modules/hierarchies/use-cases/list-hierarchies.use-case";
import { SearchAccountsUseCase } from "src/modules/personas/use-cases/search-accounts.use-case";
import { SitePersonaUseCase } from "src/modules/personas/use-cases/site-personas.use-case";
import { Client, HomeFloor, Period, SectorAndSubsector } from "src/typings";
import { GroupNew } from "src/typings/models/group-new.model";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { Site } from "src/typings/models/site.model";
import { getLayout } from "src/utils";

export default function CreateBackListView() {
  const { myUser } = useContext(UserInfoContext);
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [name, setName] = useState<string>("");

  const theme = useTheme();

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
  const [site, setSite] = useState<Site[]>([]);
  const [sites, setSites] = useState<Site[]>([]);

  const [isPublic, setIsPublic] = useState<boolean>(false);

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

  async function handleCreate() {
    if (!myUser) return;

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

    new CreateConfigPrivacyUseCase()
      .handle({
        sector: idsSectors || [],
        subsector: idsSubsectors || [],
        period: idsPeriods || [],
        hierarchy: idsHierarchies || [],
        group: idsGroups || [],
        client: idsClients || [],
        homeOrFloor: idsHomeOrFloors || [],
        idPublicPrivate: isPublic ? 1 : 2,
        site: site?.map((item) => item.id) || [],
        userId: idsCollaborators || [],
      })
      .then(() => {
        toast.success("Configurado com sucesso!");
      })
      .catch((e) => {
        const msg = e?.response?.data?.Message;
        toast.error(msg ? msg : "Erro ao criar a palavra.");
      })
      .finally(() => {
        finishLoading();
      });
  }

  return (
    <ContentCard>
      <ContentArea>
        <PageTitle title="Configurar publico/privado" loading={isLoading} />
        <Stack direction={"column"} gap={"20px"} mt={"10px"}>
          <Stack
            direction={"row"}
            onClick={() => setIsPublic(!isPublic)}
            alignItems={"center"}
            gap={"10px"}
          >
            <Typography fontWeight={"700"}>Publico?</Typography>
            <Checkbox checked={isPublic} />
          </Stack>
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
              renderInput={(props) => <TextField {...props} label={"Turno"} />}
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
          <Autocomplete
            value={site}
            multiple
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
              renderInput={(props) => <TextField {...props} label={"Grupos"} />}
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
          <Stack direction={"row"} justifyContent={"flex-end"}>
            <LoadingButton
              variant="contained"
              onClick={handleCreate}
              disabled={!myUser}
              loading={isLoading}
            >
              Salvar
            </LoadingButton>
          </Stack>
        </Stack>
      </ContentArea>
    </ContentCard>
  );
}

CreateBackListView.getLayout = getLayout("private");
