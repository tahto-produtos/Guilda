import {
  DangerousOutlined,
  FeedbackOutlined,
  FilterAlt,
  FilterAltOutlined,
  HomeOutlined,
  LinearScale,
  SendTimeExtensionOutlined,
} from "@mui/icons-material";
import {
  Autocomplete,
  Box,
  Breadcrumbs,
  Button,
  Divider,
  Drawer,
  Link,
  Stack,
  TextField,
  Typography,
  lighten,
  useTheme,
} from "@mui/material";
import { format } from "date-fns";
import { useRouter } from "next/router";
import { useContext, useEffect, useState, useRef } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { QuizContext } from "src/contexts/quiz-provider/quiz.context";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListSectorsAndSubsectrosUseCase } from "src/modules";
import { CreateInfractionButton } from "src/modules/feedback/fragments/create-infraction-button";
import FeedbackTable, { ChildComponentHandle } from "src/modules/feedback/fragments/feedback-table";
import { InfractionsTable } from "src/modules/feedback/fragments/infractions-table";
import { ListGroupsNewUseCase } from "src/modules/groups/use-cases/list-groups-new";
import { ListHierarchiesUseCase } from "src/modules/hierarchies/use-cases/list-hierarchies.use-case";
import { SearchAccountsUseCase } from "src/modules/personas/use-cases/search-accounts.use-case";
import { SitePersonaResponse, SitePersonaUseCase } from "src/modules/personas/use-cases/site-personas.use-case";
import { SectorAndSubsector } from "src/typings";
import { GroupNew } from "src/typings/models/group-new.model";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { getLayout } from "src/utils";
import { capitalizeText } from "src/utils/capitalizeText";

export default function FeedbackView() {
  const { myUser } = useContext(UserInfoContext);
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [searchText, setSearchText] = useState<string>("");
  const debouncedSearchText: string = useDebounce<string>(searchText, 400);
  const [selectedTab, setSelectedTab] = useState<
    "status" | "scale" | "infractions"
  >("status");
  const router = useRouter();
  const [isOpenFilter, setIsOpenFilter] = useState(false);

  const [sites, setSites] = useState<SitePersonaResponse[]>([]);
  const [selectedSites, setSelectedSites] = useState<SitePersonaResponse[]>([]);
  const [selectedSitesFilter, setSelectedSitesFilter] = useState<
    SitePersonaResponse[]
  >([]);
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
  const [selectedCollaboratorFilter, setSelectedCollaboratorFilter] = useState<{
    id: number;
    name: string;
    registry: string;
  } | null>(null);
  const [collaboratorSearchFilter, setCollaboratorSearchFilter] =
    useState<string>("");
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
  const [groups, setGroups] = useState<GroupNew[]>([]);
  const [selectedGroups, setSelectedGroups] = useState<GroupNew[]>([]);

  const childRef = useRef<ChildComponentHandle>(null);

  const theme = useTheme();

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

  useEffect(() => {
    getCollaborators(collaboratorSearchFilter);
  }, [collaboratorSearchFilter]);

  const getSites = async (codCollaborator: number) => {
    startLoading();

    await new SitePersonaUseCase()
      .handle()
      .then((data) => {
        setSites(data);
      })
      .catch(() => {
        toast.error("Falha ao carregar sites.");
      })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    if (myUser && myUser?.id) {
      getSites(myUser?.id);
    }
  }, []);

  const getSectorsAndSubSectors = async (isSubsector = false, sector = "") => {
    startLoading();

    new ListSectorsAndSubsectrosUseCase()
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

  const selectedTabContent = {
    status: <FeedbackTable 
      ref={childRef} 
      Hierarchy={selectedHierarchies} 
      Site={selectedSitesFilter}
      Sector={selectedSectors}
      SubSector={selectedSubSector}
      Userid={selectedCollaboratorFilter ? [selectedCollaboratorFilter] : []}
      Groups={selectedGroups}
      NameBc=""
    />,
    infractions: <InfractionsTable />,
    scale: <></>,
  };

  const handleFilter = async () => {
    if (childRef.current) {
      childRef.current.childFunction();
    }
  };

  return (
    <ContentCard sx={{ p: 0 }}>
      <Stack
        width={"100%"}
        height={"80px"}
        sx={{
          borderTopLeftRadius: "16px",
          borderTopRightRadius: "16px",
        }}
        bgcolor={theme.palette.secondary.main}
        pl={"80px"}
        justifyContent={"center"}
      >
        <Breadcrumbs
          aria-label="breadcrumb"
          sx={{
            color: theme.palette.background.default,
          }}
        >
          <Link
            underline="hover"
            sx={{ display: "flex", alignItems: "center" }}
            color={theme.palette.background.default}
            href="/"
          >
            <HomeOutlined
              sx={{
                mr: 0.5,
                color: theme.palette.background.default,
              }}
            />
          </Link>
          <Link
            sx={{
              display: "flex",
              alignItems: "center",

              textDecoration: "none",
            }}
            color={theme.palette.background.default}
          >
            <Typography fontWeight={"700"}>Feedback</Typography>
          </Link>
        </Breadcrumbs>
      </Stack>
      <ContentArea sx={{ py: " 40px" }}>
        <Stack px={"40px"}>
          <PageTitle
            icon={<FeedbackOutlined sx={{ fontSize: "40px" }} />}
            title="Feedback"
            loading={isLoading}
          >
            <Button
              variant="contained"
              onClick={() => {
                router.push("/feedback/create-feedback");
              }}
            >
              Criar formulário de feedback
            </Button>
            <Button variant={"contained"} onClick={() => setIsOpenFilter(true)} sx={{marginLeft: 2}}>
              Filtrar
            </Button>
          </PageTitle>
          <Divider />
          <Stack direction={"column"} gap={"20px"} mt={"20px"}>
            <Stack
              direction={"row"}
              alignItems={"center"}
              gap={"20px"}
              justifyContent={"space-between"}
            >
              <Stack direction={"row"} gap={"20px"} alignItems={"center"}>
                <Button
                  variant={selectedTab == "status" ? "contained" : "text"}
                  sx={{
                    borderRadius: "16px",
                    bgcolor:
                      selectedTab == "status"
                        ? lighten(theme.palette.secondary.main, 0.7)
                        : undefined,
                    color: theme.palette.text.primary,
                  }}
                  startIcon={<SendTimeExtensionOutlined />}
                  onClick={() => setSelectedTab("status")}
                >
                  Status dos feedbacks enviados
                </Button>
                <Button
                  variant={selectedTab == "infractions" ? "contained" : "text"}
                  sx={{
                    borderRadius: "16px",
                    bgcolor:
                      selectedTab == "infractions"
                        ? lighten(theme.palette.secondary.main, 0.7)
                        : undefined,
                    color: theme.palette.text.primary,
                  }}
                  startIcon={<DangerousOutlined />}
                  onClick={() => setSelectedTab("infractions")}
                >
                  Infrações
                </Button>
                {/* <Button
                                    variant={
                                        selectedTab == "scale"
                                            ? "contained"
                                            : "text"
                                    }
                                    sx={{
                                        borderRadius: "16px",
                                        bgcolor:
                                            selectedTab == "scale"
                                                ? lighten(
                                                      theme.palette.secondary
                                                          .main,
                                                      0.7
                                                  )
                                                : undefined,
                                        color: theme.palette.text.primary,
                                    }}
                                    startIcon={<LinearScale />}
                                    onClick={() => setSelectedTab("scale")}
                                >
                                    Escala pedagógica
                                </Button> */}
              </Stack>
              {selectedTab == "infractions" && <CreateInfractionButton />}
            </Stack>

            {selectedTabContent[selectedTab]}
          </Stack>
        </Stack>
      </ContentArea>
      <Drawer
        open={isOpenFilter}
        anchor={"right"}
        onClose={() => setIsOpenFilter(false)}
        PaperProps={{
          sx: {
            borderTopLeftRadius: "16px",
            borderBottomLeftRadius: "16px",
          },
        }}
      >
        <Stack
          width={"100vw"}
          minWidth={"100%"}
          maxWidth={"430px"}
          direction={"column"}
          height={"100vh"}
          gap={"20px"}
          py={"40px"}
          px={"35px"}
        >
          <Typography
            fontSize={"20px"}
            fontWeight={"600"}
            alignItems={"center"}
            gap={"10px"}
            display={"flex"}
            flexDirection={"row"}
          >
            Filtar
          </Typography>
          <Autocomplete
            fullWidth
            value={selectedCollaboratorFilter}
            options={collaborators}
            disableClearable={false}
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
              setSelectedCollaboratorFilter(value);
            }}
            onInputChange={(e, text) => setCollaboratorSearchFilter(text)}
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
            isOptionEqualToValue={(option, value) => option.name === value.name}
            sx={{ m: 0 }}
          />
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
          <Autocomplete
            multiple
            style={{ width: "100%" }}
            disableClearable={false}
            value={selectedSitesFilter}
            options={sites}
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
              setSelectedSitesFilter(value);
            }}
            filterSelectedOptions
            renderInput={(props) => <TextField {...props} label={"Sites"} />}
            renderOption={(props, option) => {
              return (
                <li {...props} key={option.id}>
                  {option.name}
                </li>
              );
            }}
            isOptionEqualToValue={(option, value) => option.name === value.name}
            sx={{ m: 0 }}
          />
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
              <TextField {...props} label={"Setor"} />
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
          <Button variant={"contained"} onClick={() => handleFilter()} >
              Filtrar
            </Button>
        </Stack>
      </Drawer>
    </ContentCard>
  );
}

FeedbackView.getLayout = getLayout("private");
