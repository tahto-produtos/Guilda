import { HomeOutlined, Stars, Close as CloseIcon } from "@mui/icons-material";
import {
  Autocomplete,
  Breadcrumbs,
  Button,
  CardMedia,
  Chip,
  Divider,
  Link,
  MenuItem,
  Select,
  Stack,
  TextField,
  Typography,
  useTheme,
  Popover,
  Box,
  IconButton,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format } from "date-fns";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { TextFieldTitle } from "src/components/inputs/title-text-field/title-text-field";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { useLoadingState } from "src/hooks";
import {
  ListHomeFloorUseCase,
  ListPeriodUseCase,
  ListSectorsAndSubsectrosUseCase,
  ListVeteranoNovatoUseCase,
} from "src/modules";
import { ProductGeneralUseCase } from "src/modules/campaign/use-cases/ProductGeneral.use-case";
import { UpdateImageCreatedOperationalCampaignUseCase } from "src/modules/campaign/use-cases/UpdateImageCreatedOperationalCampaign.use-case";
import { CreateOperationalCampaignUseCase } from "src/modules/campaign/use-cases/create-campaign.use-case";
import {
  ListAllOperationalCampaignAvailableUseCase,
  OperationalCampaignAll,
} from "src/modules/campaign/use-cases/list-all-operational-campaign.use-case";
import {
  ListOtherOperationalCampaignAvailableUseCase,
  OperationalCampaignOther,
} from "src/modules/campaign/use-cases/list-other-operational-campaign.use-case copy";
import { ListGroupsNewUseCase } from "src/modules/groups/use-cases/list-groups-new";
import { ListHierarchiesUseCase } from "src/modules/hierarchies/use-cases/list-hierarchies.use-case";
import { ListIndicatorsUseCase } from "src/modules/indicators/use-cases";
import { HomeFloor, Indicator, Period, SectorAndSubsector, VeteranoNovato } from "src/typings";
import { GroupNew } from "src/typings/models/group-new.model";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { getLayout } from "src/utils";

export interface CampaignPontuationCriteria {
  ID_INDICATOR: number;
  INDICATOR_INCREASE: number;
  PERCENT: string;
  REWARD_POINTS: number;
  NAME_INDICATOR: string;
}

export interface CampaignRankingItem {
  ID_PRODUCT: number;
  NAME_PRODUCT: string;
  POSITION: number;
  MIN_PONTUATION: number;
  VALUE_COINS: number;
  QUANTITY_PRODUCT: number;
}

export interface CampaignEliminationCriteria {
  NAME_INDICATOR: string;
  ID_INDICATOR: number;
  INDICATOR_INCREASE: number;
  PERCENT: string;
}

export default function CreateCampaignView() {
  const { myUser } = useContext(UserInfoContext);
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const theme = useTheme();
  const router = useRouter();

  const [ID_PRODUCT, setID_PRODUCT] = useState<number | null>(null);
  const [POSITION, setPOSITION] = useState<number>(0);
  const [QUANTITY, setQUANTITY] = useState<number>(0);

  const [startDate, setStartDate] = useState<dateFns | null>(null);
  const [endDate, setEndDate] = useState<dateFns | null>(null);

  const [indicator, setIndicator] = useState<Indicator | null>(null);
  const [indicatorCri, setIndicatorCri] = useState<Indicator | null>(null);
  const [indicatorSearchValue, setIndicatorSearchValue] = useState<string>("");
  const [indicatorSearchValueCri, setIndicatorSearchValueCri] = useState<string>("");
  const [indicators, setIndicators] = useState<Indicator[]>([]);
  const [indicatorsCri, setIndicatorsCri] = useState<Indicator[]>([]);
  
  const [pontuationCriterias, setPontuationCriterias] = useState<
    CampaignPontuationCriteria[]
  >([]);

  const [eliminationCriterias, setEliminationCriterias] = useState<
    CampaignEliminationCriteria[]
  >([]);

  const [rankingItems, setRankingItems] = useState<CampaignRankingItem[]>([]);

  const [indicatorIncrease, setIndicatorIncrease] = useState<number>(0);

  const [indicatorIncreaseCri, setIndicatorIncreaseCri] = useState<number>(0);

  const [percent, setPercent] = useState<string>('0');
  const [percentCri, setPercentCri] = useState<string>('0');
  const [rewardPoints, setRewardPoints] = useState<number>(0);

  const [name, setName] = useState("");
  const [desc, setDesc] = useState("");

  const [file, setFile] = useState<any>(null);

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
  const [hierarchies, setHierarchies] = useState<Hierarchie[]>([]);
  const [selectedHierarchies, setSelectedHierachies] = useState<Hierarchie[]>(
    []
  );

  
  const [homesFloors, setHomesFloors] = useState<HomeFloor[]>([]);
  const [selectedHomeFloor, setSelectedHomeFloor] = useState<HomeFloor[]>([]);
  const [periods, setPeriods] = useState<Period[]>([]);
  const [selectedPeriods, setSelectedPeriods] = useState<Period[]>([]);

  const [veteranoNovatos, setVeteranoNovatos] = useState<VeteranoNovato[]>([]);
  const [selectedVeteranoNovatos, setSelectedVeteranoNovatos] = useState<VeteranoNovato[]>([]);

  const [groups, setGroups] = useState<GroupNew[]>([]);
  const [selectedGroups, setSelectedGroups] = useState<GroupNew[]>([]);

  const [searchOtherCampaign, setSearchOtherCampaign] = useState("");

  const [othersCampaign, setOthersCampaign] = useState<
    OperationalCampaignAll[]
  >([]);
  const [othersCampaignSelect, setOthersCampaignSelect] =
    useState<OperationalCampaignAll | null>(null);
  const [otherCampaignData, setOtherCampaignData] =
    useState<OperationalCampaignOther | null>(null);

  const [products, setProducts] = useState<
    {
      IDGDA_PRODUCT: number;
      DESCRIPTION: string;
      CODE: string;
      REGISTERED_BY: string;
      PRICE: string;
      TYPE: string;
      COMERCIAL_NAME: string;
      VALIDITY_DATE: string;
    }[]
  >([]);
  const [selectedProduct, setSelectedProduct] = useState<{
    IDGDA_PRODUCT: number;
    DESCRIPTION: string;
    CODE: string;
    REGISTERED_BY: string;
    PRICE: string;
    TYPE: string;
    COMERCIAL_NAME: string;
    VALIDITY_DATE: string;
  } | null>(null);
  const [productSearch, setProductSearch] = useState<string>("");

  const handleUpload = (event: any) => {
    const file = event.target.files[0];
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onloadend = () => {
      setFile(file);
    };
  };

  const [ID_RANKING_AWARD_PUBLIC, setID_RANKING_AWARD_PUBLIC] =
    useState<boolean>(false);
  const [ID_RANKING_AWARD_TYPE, setID_RANKING_AWARD_TYPE] =
    useState<boolean>(false);
  const [ID_RANKING_PAY_OPTION, setID_RANKING_PAY_OPTION] =
    useState<boolean>(false);

  const getOtherCampaignsDetails = async () => {
    if (!othersCampaignSelect) return;

    await new ListOtherOperationalCampaignAvailableUseCase()
      .handle({
        codCampanha: othersCampaignSelect?.IDCAMPAIGN,
      })
      .then((data) => {
        setOtherCampaignData(data);
      })
      .catch(() => { })
      .finally(() => { });
  };

  useEffect(() => {
    getOtherCampaignsDetails();
  }, [othersCampaignSelect]);

  const getOtherCampaigns = async () => {
    await new ListAllOperationalCampaignAvailableUseCase()
      .handle({
        campaign: searchOtherCampaign,
      })
      .then((data) => {
        setOthersCampaign(data);
      })
      .catch(() => { })
      .finally(() => { });
  };

  useEffect(() => {
    getOtherCampaigns();
  }, [searchOtherCampaign]);

  const getProducts = async () => {
    await new ProductGeneralUseCase()
      .handle({
        product: productSearch,
      })
      .then((data) => {
        setProducts(data);
      })
      .catch(() => { })
      .finally(() => { });
  };

  useEffect(() => {
    getProducts();
  }, []);

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

  const getVeteranoNovatos = async (codCollaborator: number) => {
    startLoading();

    await new ListVeteranoNovatoUseCase()
      .handle({
        codCollaborator,
      })
      .then((data) => {
        setVeteranoNovatos(data);
      })
      .catch(() => {
        toast.error("Falha ao carregar turnos.");
      })
      .finally(() => {
        finishLoading();
      });
  };


  const handleInputChangeCri = (e: any) => {
    let value = e.target.value;
  
    // Permite apenas números e até 2 casas decimais
    const regex = /^[0-9]*\.?[0-9]*$/;
  
    if (regex.test(value)) {
      // Atualiza apenas o item específico da lista
      
      setPercentCri(value);
    }
  };

  const handleInputChangeCriMap = (e: any, index: any) => {
    let value = e.target.value;
  
    // Permite apenas números e até 2 casas decimais
    const regex = /^[0-9]*\.?[0-9]*$/;
  
    if (regex.test(value)) {
      // Atualiza apenas o item específico da lista
      const updatedCriterias = pontuationCriterias.map((item, i) =>
        i === index ? { ...item, PERCENT: value } : item
      );
      
      setPontuationCriterias(updatedCriterias);
    }
  };

  const handleInputChangeElimMap = (e: any, index: any) => {
    let value = e.target.value;
  
    // Permite apenas números e até 2 casas decimais
    const regex = /^[0-9]*\.?[0-9]*$/;
  
    if (regex.test(value)) {
      // Atualiza apenas o item específico da lista
      const updatedCriterias = pontuationCriterias.map((item, i) =>
        i === index ? { ...item, PERCENT: value } : item
      );
      
      setEliminationCriterias(updatedCriterias);
    }
  };


  const handleInputChangeElim = (e: any) => {
    let value = e.target.value;
  
    // Permite apenas números e até 2 casas decimais
    const regex = /^[0-9]*\.?[0-9]*$/;
  
    if (regex.test(value)) {
      // Atualiza apenas o item específico da lista
      
      setPercent(e.target.value)
    }
  };
  
  useEffect(() => {
    if (myUser && myUser?.id) {
      getPeriods(myUser?.id);
      getVeteranoNovatos(myUser?.id);
    }
  }, []);

  const getIndicators = async (searchText: string) => {
    await new ListIndicatorsUseCase()
      .handle({
        limit: 10,
        offset: 1,
        searchText: searchText,
      })
      .then((data) => {
        setIndicators(data.items);
      })
      .catch(() => { })
      .finally(() => { });
  };

  const getIndicatorsCri = async (searchText: string) => {
    await new ListIndicatorsUseCase()
      .handle({
        limit: 10,
        offset: 1,
        searchText: searchText,
      })
      .then((data) => {
        setIndicatorsCri(data.items);
      })
      .catch(() => { })
      .finally(() => { });
  };

  useEffect(() => {
    getIndicatorsCri(indicatorSearchValueCri);
  }, [indicatorSearchValueCri]);

  useEffect(() => {
    getIndicators(indicatorSearchValue);
  }, [indicatorSearchValue]);


  function handleAddPontuationCriteria() {
    if (!indicatorCri) return;

    if (pontuationCriterias.find((item) => item.ID_INDICATOR == indicatorCri.id))
      return;

    setPontuationCriterias([
      ...pontuationCriterias,
      {
        NAME_INDICATOR: indicatorCri.name,
        ID_INDICATOR: indicatorCri.id,
        INDICATOR_INCREASE: indicatorIncreaseCri,
        PERCENT: percentCri,
        REWARD_POINTS: rewardPoints,
      },
    ]);
  }

  function handleAddRankItem() {
    setRankingItems([
      ...rankingItems,
      {
        ID_PRODUCT: selectedProduct?.IDGDA_PRODUCT || 0,
        POSITION: POSITION,
        MIN_PONTUATION: POSITION,
        QUANTITY_PRODUCT: QUANTITY,
        VALUE_COINS: QUANTITY,
        NAME_PRODUCT: selectedProduct?.COMERCIAL_NAME || "",
      },
    ]);
  }

  function handleAddEliminationCriteria() {
    if (!indicator) return;

    if (eliminationCriterias.find((item) => item.ID_INDICATOR == indicator.id))
      return;

    setEliminationCriterias([
      ...eliminationCriterias,
      {
        NAME_INDICATOR: indicator.name,
        ID_INDICATOR: indicator.id,
        INDICATOR_INCREASE: indicatorIncrease,
        PERCENT: percent,
      },
    ]);
  }

  async function handleCreateCampaign() {
    if (!endDate || !startDate) return;

    await new CreateOperationalCampaignUseCase()
      .handle({
        DESCRIPTION: desc,
        ELIMINATION: eliminationCriterias,
        ENDED_AT: format(new Date(endDate.toString()), "yyyy/MM/dd"),
        STARTED_AT: format(new Date(startDate.toString()), "yyyy/MM/dd"),
        NAME_CAMPAIGN: name,
        PONTUATION: pontuationCriterias,
        RANKING: {
          ID_RANKING_AWARD_PUBLIC: ID_RANKING_AWARD_PUBLIC ? 1 : 2,
          ID_RANKING_AWARD_TYPE: ID_RANKING_AWARD_TYPE ? 1 : 2,
          ID_RANKING_PAY_OPTION: ID_RANKING_PAY_OPTION ? 1 : 2,
          RANKING_ITENS: rankingItems.map((item) => {
            return {
              ID_PRODUCT: item.ID_PRODUCT,
              MIN_PONTUATION: !ID_RANKING_AWARD_PUBLIC ? 0 : item.POSITION,
              POSITION: ID_RANKING_AWARD_PUBLIC ? 0 : item.POSITION,
              QUANTITY_PRODUCT: ID_RANKING_AWARD_TYPE
                ? 0
                : item.QUANTITY_PRODUCT,
              VALUE_COINS: ID_RANKING_AWARD_TYPE ? item.VALUE_COINS : 0,
              NAME_PRODUCT: item.NAME_PRODUCT,
            };
          }),
        },
        VISIBILITY: {
          GROUP: selectedGroups.map((item) => item.id),
          HIERARCHY: selectedHierarchies.map((item) => item.id),
          HOMEORFLOOR: selectedHomeFloor.map((item) => item.id),
          SECTOR: selectedSectors.map((item) => item.id),
          SUBSECTOR: selectedSubSector.map((item) => item.id),
          VETERANONOVADO: selectedVeteranoNovatos.map((item) => item.id),
        },
      })
      .then(async (data) => {
        // setIndicators(data.items);
        if (file) {
          await new UpdateImageCreatedOperationalCampaignUseCase().handle({
            file: file,
            ID_OPERATIONAL_CAMPAIGN: data.ID_OPERATIONAL_CAMPAIGN,
          });
        }
        toast.success("Campanha criada com sucesso");
      })
      .catch(() => {
        toast.error("Error");
      })
      .finally(() => { });
  }

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
  }, [myUser]);

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
  }, [myUser]);

  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);

  const handlePopoverOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handlePopoverClose = () => {
    setAnchorEl(null);
  };

  const open = Boolean(anchorEl);

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
            <Typography fontWeight={"700"}>Campanhas</Typography>
          </Link>
        </Breadcrumbs>
      </Stack>
      <ContentArea sx={{ py: " 40px" }}>
        <Stack px={"40px"}>
          <PageTitle
            icon={<Stars sx={{ fontSize: "40px" }} />}
            title={`Criação de nova campanha`}
            loading={isLoading}
          ></PageTitle>
          <Divider />

          {file && (
            <CardMedia
              component="img"
              image={URL.createObjectURL(file as File)}
              sx={{
                width: "500px",
                height: "350px",
                objectFit: "cover",
                mt: "40px",
                borderRadius: "24px",
              }}
            />
          )}

          <Stack direction={"column"} gap={"20px"} mt={"40px"}>
            <Stack direction={"row"} gap={"10px"} alignItems={"flex-end"}>
              <TextFieldTitle title="Nome da campanha">
                <TextField
                  placeholder="Escreva o nome"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                />
              </TextFieldTitle>
              <TextFieldTitle title="Descrição">
                <TextField
                  placeholder="Faça uma breve descrição da campanha"
                  value={desc}
                  onChange={(e) => setDesc(e.target.value)}
                />
              </TextFieldTitle>
              <TextFieldTitle title="Adicionar imagem a campanha">
                <input
                  accept=".png, .jpg, .jpeg"
                  style={{ display: "none" }}
                  id="image-upload"
                  type="file"
                  onChange={(e) => handleUpload(e)}
                />
                <label htmlFor="image-upload" style={{ width: "100%" }}>
                  <Button
                    variant={file ? "outlined" : "contained"}
                    color="primary"
                    component="span"
                    fullWidth
                    size="large"
                    disabled={isLoading}
                  >
                    {file ? "Alterar arquivo" : "Selecione um arquivo"}
                  </Button>
                </label>{" "}
              </TextFieldTitle>
            </Stack>
            <Stack direction={"row"} gap={"10px"}>
              <TextFieldTitle title="Data de inicio da campanha">
                <LocalizationProvider dateAdapter={AdapterDateFns}>
                  <DatePicker
                    label="De"
                    value={startDate}
                    onChange={(newValue) => setStartDate(newValue)}
                    slotProps={{
                      textField: {
                        sx: {
                          minWidth: "180px",
                          svg: {
                            color: grey[500],
                          },
                        },
                      },
                    }}
                  />
                </LocalizationProvider>
              </TextFieldTitle>
              <TextFieldTitle title="Data final da campanha">
                <LocalizationProvider dateAdapter={AdapterDateFns}>
                  <DatePicker
                    label="Até"
                    value={endDate}
                    onChange={(newValue) => setEndDate(newValue)}
                    slotProps={{
                      textField: {
                        sx: {
                          minWidth: "180px",
                          svg: {
                            color: grey[500],
                          },
                        },
                      },
                    }}
                  />
                </LocalizationProvider>
              </TextFieldTitle>
            </Stack>
            <Divider />
            <Typography variant="body1" fontSize={"20px"}>
              Enviar para: (Eligibilidade)
            </Typography>
            <Stack direction={"row"} gap={"10px"}>
              {/* <Autocomplete
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
              /> */}
              <Autocomplete
                multiple
                fullWidth
                value={selectedSectors}
                options={sectors}
                getOptionLabel={(option) => option.name}
                onChange={(event, value) => {
                  setSelectedSectors(value);
                }}
                renderInput={(props) => (
                  <TextField {...props} label={"Setores"} />
                )}



                



                renderTags={(value, getTagProps) => {
                  // Limita a renderização ao primeiro setor
                  const firstTag = value.slice(0, 1).map((option, index) => {
                    const { key, ...rest } = getTagProps({ index }); // Captura o resultado de getTagProps separadamente
                
                    return (
                      <Chip
                        key={key} // Definindo a chave única manualmente
                        sx={{
                          width: '70%', // Ajusta conforme necessário
                          whiteSpace: 'nowrap',
                          overflow: 'hidden',
                          textOverflow: 'ellipsis',
                        }}
                        label={option.name}
                        {...rest} // Aplica o restante das props de getTagProps
                        onClick={handlePopoverOpen}
                      />
                    );
                  });
                
                  // Chip para mostrar setores adicionais
                  const additionalTag = value.length > 1 ? (
                    <Chip
                      key="additional" // Definindo a chave manualmente para o chip adicional
                      label={`+${value.length - 1}`}
                      onClick={handlePopoverOpen}
                    />
                  ) : null;
                
                  // Retorna os chips concatenados
                  return firstTag.concat(additionalTag ? [additionalTag] : []);
                }}
                renderOption={(props, option) => (
                  <li {...props} key={option.id}>
                    {option.name}
                  </li>
                )}
                isOptionEqualToValue={(option, value) =>
                  option.name === value.name
                }
                sx={{ m: 0 }}
              />

              <Popover
                open={open}
                anchorEl={anchorEl}
                onClose={handlePopoverClose}
                anchorOrigin={{
                  vertical: 'bottom',
                  horizontal: 'left',
                }}
              >
                <Box sx={{ p: 2, maxWidth: 300 }}>
                  {selectedSectors.map((option, index) => (
                    <Chip
                      key={option.id}
                      label={option.name}
                      onDelete={() => {
                        setSelectedSectors((prev) =>
                          prev.filter((sector) => sector.id !== option.id)
                        );
                      }}
                      deleteIcon={<IconButton size="small"><CloseIcon /></IconButton>}
                      sx={{ m: 0.5 }}
                    />
                  ))}
                </Box>
              </Popover>

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
            <Stack direction={"row"} gap={"10px"}>
              {/* -- */}
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
            <Stack direction={"row"} gap={"10px"}>
              {/* -- */}
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
                  <TextField {...props} label={"Home/Work"} />
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
                value={selectedVeteranoNovatos}
                options={veteranoNovatos}
                getOptionLabel={(option) => option.name}
                onChange={(event, value) => {
                  setSelectedVeteranoNovatos(value);
                }}
                filterSelectedOptions
                renderInput={(props) => (
                  <TextField {...props} label={"Veterano/Novato"} />
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
            <Divider />
            <Typography variant="body1" fontSize={"20px"}>
              Critérios de pontuação:
            </Typography>
            {pontuationCriterias.map((item, index) => (
              <Stack
                direction={"row"}
                gap={"10px"}
                key={index}
                alignItems={"flex-end"}
              >
                <TextFieldTitle title="Indicador">
                  <TextField
                    placeholder="Indicador"
                    value={item.NAME_INDICATOR}
                    InputProps={{ readOnly: true }}
                    disabled
                  />
                </TextFieldTitle>
                <TextFieldTitle title="Caso o indicador">
                  <Select value={item.INDICATOR_INCREASE} disabled>
                    <MenuItem value={1}>Aumentar</MenuItem>
                    <MenuItem value={0}>Baixar</MenuItem>
                  </Select>
                </TextFieldTitle>
                <TextFieldTitle title="Em (%)">
                  <TextField
                    value={item.PERCENT}
                    InputProps={{ readOnly: true }}
                    disabled
                    onChange={(e) => handleInputChangeCriMap(e, index)}
                  />
                </TextFieldTitle>
                <TextFieldTitle title="Premio (valor) em pontos">
                  <TextField
                    value={item.REWARD_POINTS}
                    InputProps={{ readOnly: true }}
                    disabled
                  />
                </TextFieldTitle>
                <Button
                  variant="contained"
                  color="error"
                  onClick={() => {
                    const arr = [...pontuationCriterias];

                    setPontuationCriterias(
                      arr.filter((x) => x.ID_INDICATOR !== item.ID_INDICATOR)
                    );
                  }}
                >
                  Remover
                </Button>
              </Stack>
            ))}
            <Stack direction={"row"} gap={"10px"}>
              <TextFieldTitle title="Indicador">
                <Autocomplete
                  fullWidth
                  value={indicatorCri}
                  options={indicatorsCri}
                  getOptionLabel={(option) => option.name}
                  onChange={(event, value) => {
                    setIndicatorCri(value);
                  }}
                  onInputChange={(e, text) => setIndicatorSearchValueCri(text)}
                  filterOptions={(options, { inputValue }) =>
                    options.filter((item) =>
                      item.name
                        .toLocaleLowerCase()
                        .includes(inputValue.toLocaleLowerCase())
                    )
                  }
                  filterSelectedOptions
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      variant="outlined"
                      label="Indicadores"
                      placeholder="Buscar"
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
                  sx={{ p: 0, m: 0 }}
                />
              </TextFieldTitle>
              <TextFieldTitle title="Caso o indicador">
                <Select
                  value={indicatorIncreaseCri}
                  onChange={(e) =>
                    setIndicatorIncreaseCri(e.target.value as number)
                  }
                >
                  <MenuItem value={1}>Aumentar</MenuItem>
                  <MenuItem value={0}>Baixar</MenuItem>
                </Select>
              </TextFieldTitle>
              <TextFieldTitle title="Em (%)">
                <TextField
                  placeholder="Digite um valor"
                  value={percentCri}
                  onChange={(e) => handleInputChangeCri(e)} // Passa o índice do item
                />
              </TextFieldTitle>
              <TextFieldTitle title="Premio (valor) em pontos">
                <TextField
                  placeholder="Digite um valor"
                  value={rewardPoints}
                  onChange={(e) =>
                    setRewardPoints(parseInt(e.target.value) || 0)
                  }
                />
              </TextFieldTitle>
            </Stack>
            <Stack direction={"row"}>
              <Button
                onClick={handleAddPontuationCriteria}
                variant="contained"
                color={"primary"}
              >
                Salvar critério de pontuação
              </Button>
            </Stack>
            <Divider />
            <Typography variant="body1" fontSize={"20px"}>
              Ranking:
            </Typography>
            <Stack direction={"row"} gap={"10px"}>
              <TextFieldTitle title="Tipo de premiação:">
                <Stack direction={"row"} gap={"10px"}>
                  <Chip
                    label="Moedas virtuais"
                    sx={{
                      background: ID_RANKING_AWARD_TYPE
                        ? theme.palette.primary.main
                        : undefined,
                      color: ID_RANKING_AWARD_TYPE
                        ? theme.palette.background.default
                        : undefined,
                    }}
                    onClick={() => setID_RANKING_AWARD_TYPE(true)}
                  />
                  <Chip
                    label="Itens específicos"
                    sx={{
                      background: !ID_RANKING_AWARD_TYPE
                        ? theme.palette.primary.main
                        : undefined,
                      color: !ID_RANKING_AWARD_TYPE
                        ? theme.palette.background.default
                        : undefined,
                    }}
                    onClick={() => setID_RANKING_AWARD_TYPE(false)}
                  />
                </Stack>
              </TextFieldTitle>
              <TextFieldTitle title="Opção de pagamento das campanhas:">
                <Stack direction={"row"} gap={"10px"}>
                  <Chip
                    label="Automático"
                    sx={{
                      background: ID_RANKING_PAY_OPTION
                        ? theme.palette.primary.main
                        : undefined,
                      color: ID_RANKING_PAY_OPTION
                        ? theme.palette.background.default
                        : undefined,
                    }}
                    onClick={() => setID_RANKING_PAY_OPTION(true)}
                  />
                  <Chip
                    label="Manual"
                    sx={{
                      background: !ID_RANKING_PAY_OPTION
                        ? theme.palette.primary.main
                        : undefined,
                      color: !ID_RANKING_PAY_OPTION
                        ? theme.palette.background.default
                        : undefined,
                    }}
                    onClick={() => setID_RANKING_PAY_OPTION(false)}
                  />
                </Stack>
              </TextFieldTitle>
            </Stack>
            <Stack direction={"row"} gap={"10px"}>
              <TextFieldTitle title="Seleção de premiação:">
                <Stack direction={"row"} gap={"10px"}>
                  <Chip
                    label="Primeiras colocações"
                    sx={{
                      background: ID_RANKING_AWARD_PUBLIC
                        ? theme.palette.primary.main
                        : undefined,
                      color: ID_RANKING_AWARD_PUBLIC
                        ? theme.palette.background.default
                        : undefined,
                    }}
                    onClick={() => setID_RANKING_AWARD_PUBLIC(true)}
                  />
                  <Chip
                    label="Pontuação mínima"
                    sx={{
                      background: !ID_RANKING_AWARD_PUBLIC
                        ? theme.palette.primary.main
                        : undefined,
                      color: !ID_RANKING_AWARD_PUBLIC
                        ? theme.palette.background.default
                        : undefined,
                    }}
                    onClick={() => setID_RANKING_AWARD_PUBLIC(false)}
                  />
                </Stack>
              </TextFieldTitle>
            </Stack>
            {rankingItems.map((item, index) => (
              <Stack
                direction={"row"}
                gap={"10px"}
                key={index}
                alignItems={"flex-end"}
              >
                {!ID_RANKING_AWARD_TYPE && (
                  <TextFieldTitle title="Adicione o premio que será enviado">
                    <TextField value={item.NAME_PRODUCT} />
                  </TextFieldTitle>
                )}
                <TextFieldTitle
                  title={
                    ID_RANKING_AWARD_TYPE ? "Quantidade moedas" : "Quantidade"
                  }
                >
                  <TextField
                    placeholder="Digite um valor"
                    value={item.QUANTITY_PRODUCT}
                    InputProps={{ readOnly: true }}
                    disabled
                  />
                </TextFieldTitle>
                <TextFieldTitle
                  title={
                    ID_RANKING_AWARD_PUBLIC ? "Colocação" : "Pontuação mínima"
                  }
                >
                  <TextField
                    placeholder="Digite um valor"
                    value={item.POSITION}
                    InputProps={{ readOnly: true }}
                    disabled
                  />
                </TextFieldTitle>
                <Button
                  variant="contained"
                  color="error"
                  onClick={() => {
                    const arr = [...rankingItems];

                    setRankingItems(arr.filter((x) => x !== item));
                  }}
                >
                  Remover
                </Button>
              </Stack>
            ))}
            <Stack direction={"row"} gap={"10px"}>
              {!ID_RANKING_AWARD_TYPE && (
                <TextFieldTitle title="Adicione o premio que será enviado">
                  <Autocomplete
                    fullWidth
                    value={selectedProduct}
                    options={products}
                    getOptionLabel={(option) => option.COMERCIAL_NAME}
                    onChange={(event, value) => {
                      setSelectedProduct(value);
                    }}
                    onInputChange={(e, text) => setProductSearch(text)}
                    filterSelectedOptions
                    renderInput={(props) => (
                      <TextField {...props} label={"Adicione o premio"} />
                    )}
                    renderOption={(props, option) => {
                      return (
                        <li {...props} key={option.CODE}>
                          {option.COMERCIAL_NAME}
                        </li>
                      );
                    }}
                    isOptionEqualToValue={(option, value) =>
                      option.COMERCIAL_NAME === value.COMERCIAL_NAME
                    }
                    sx={{ m: 0 }}
                  />
                </TextFieldTitle>
              )}
              <TextFieldTitle
                title={
                  ID_RANKING_AWARD_TYPE ? "Quantidade moedas" : "Quantidade"
                }
              >
                <TextField
                  placeholder="Digite um valor"
                  value={QUANTITY}
                  onChange={(e) =>
                    setQUANTITY(parseInt(e.target.value) || 0 || 0)
                  }
                />
              </TextFieldTitle>
              <TextFieldTitle
                title={
                  ID_RANKING_AWARD_PUBLIC ? "Colocação" : "Pontuação mínima"
                }
              >
                <TextField
                  placeholder="Digite um valor"
                  value={POSITION}
                  onChange={(e) => setPOSITION(parseInt(e.target.value) || 0)}
                />
              </TextFieldTitle>
            </Stack>
            <Stack direction={"row"}>
              <Button
                variant="contained"
                color="primary"
                onClick={handleAddRankItem}
              >
                Adicionar prêmio
              </Button>
            </Stack>
            <Divider />
            <Typography variant="body1" fontSize={"20px"}>
              Critérios de eliminação:
            </Typography>
            {eliminationCriterias.map((item, index) => (
              <Stack
                direction={"row"}
                gap={"10px"}
                key={index}
                alignItems={"flex-end"}
              >
                <TextFieldTitle title="Indicador">
                  <TextField
                    placeholder="Indicador"
                    value={item.NAME_INDICATOR}
                    InputProps={{ readOnly: true }}
                    disabled
                  />
                </TextFieldTitle>
                <TextFieldTitle title="Caso o indicador">
                  <Select value={item.INDICATOR_INCREASE} disabled>
                    <MenuItem value={1}>Aumentar</MenuItem>
                    <MenuItem value={0}>Baixar</MenuItem>
                  </Select>
                </TextFieldTitle>
                <TextFieldTitle title="Em (%)">
                  <TextField
                    value={item.PERCENT}
                    InputProps={{ readOnly: true }}
                    disabled
                    onChange={(e) => handleInputChangeElimMap(e, index)}
                  />
                </TextFieldTitle>
                <Button
                  variant="contained"
                  color="error"
                  onClick={() => {
                    const arr = [...eliminationCriterias];

                    setEliminationCriterias(
                      arr.filter((x) => x.ID_INDICATOR !== item.ID_INDICATOR)
                    );
                  }}
                >
                  Remover
                </Button>
              </Stack>
            ))}
            <Stack direction={"row"} gap={"10px"}>
              <TextFieldTitle title="Indicador">
                <Autocomplete
                  fullWidth
                  value={indicator}
                  options={indicators}
                  getOptionLabel={(option) => option.name}
                  onChange={(event, value) => {
                    setIndicator(value);
                  }}
                  onInputChange={(e, text) => setIndicatorSearchValue(text)}
                  filterOptions={(options, { inputValue }) =>
                    options.filter((item) =>
                      item.name
                        .toLocaleLowerCase()
                        .includes(inputValue.toLocaleLowerCase())
                    )
                  }
                  filterSelectedOptions
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      variant="outlined"
                      label="Indicadores"
                      placeholder="Buscar"
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
                  sx={{ p: 0, m: 0 }}
                />
              </TextFieldTitle>
              <TextFieldTitle title="Caso o indicador">
                <Select
                  value={indicatorIncrease}
                  onChange={(e) =>
                    setIndicatorIncrease(e.target.value as number)
                  }
                >
                  <MenuItem value={1}>Aumentar</MenuItem>
                  <MenuItem value={0}>Baixar</MenuItem>
                </Select>
              </TextFieldTitle>
              <TextFieldTitle title="Em (%)">
                <TextField
                  placeholder="Digite um valor"
                  value={percent}
                  onChange={(e) => handleInputChangeElim(e)}
                />
              </TextFieldTitle>
            </Stack>
            <Stack direction={"row"}>
              <Button
                onClick={handleAddEliminationCriteria}
                variant="contained"
                color={"primary"}
              >
                Salvar critério de eliminação
              </Button>
            </Stack>
            <Divider />
            <Typography variant="body1" fontSize={"20px"}>
              Resultados esperados:
            </Typography>{" "}
            <Autocomplete
              fullWidth
              value={othersCampaignSelect}
              options={othersCampaign}
              getOptionLabel={(option) => option.NAME}
              onChange={(event, value) => {
                setOthersCampaignSelect(value);
              }}
              onInputChange={(e, text) => setSearchOtherCampaign(text)}
              filterOptions={(x) => x}
              filterSelectedOptions
              renderInput={(props) => (
                <TextField
                  {...props}
                  label={"Selecione uma campanha para comparar"}
                />
              )}
              renderOption={(props, option) => {
                return (
                  <li {...props} key={option.IDCAMPAIGN}>
                    {option.NAME}
                  </li>
                );
              }}
              isOptionEqualToValue={(option, value) =>
                option.NAME === value.NAME
              }
              sx={{ m: 0 }}
            />
            {otherCampaignData && (
              <Stack direction={"row"} gap={"10px"}>
                <TextFieldTitle title="Custo geral da campanha">
                  <Stack
                    width={"100%"}
                    paddingY={"10px"}
                    border={`solid 1px ${theme.palette.primary.main}`}
                    borderRadius={"16px"}
                    justifyContent={"center"}
                    alignItems={"center"}
                  >
                    <Typography
                      fontWeight={"700"}
                      fontSize={"16px"}
                      color={"primary"}
                    >
                      {otherCampaignData.TOTALCOST}
                    </Typography>
                  </Stack>
                </TextFieldTitle>{" "}
                <TextFieldTitle title="Numero de operadores participantes">
                  <Stack
                    width={"100%"}
                    paddingY={"10px"}
                    border={`solid 1px ${theme.palette.primary.main}`}
                    borderRadius={"16px"}
                    justifyContent={"center"}
                    alignItems={"center"}
                  >
                    <Typography
                      fontWeight={"700"}
                      fontSize={"16px"}
                      color={"primary"}
                    >
                      {otherCampaignData.PARTICIPATINGOPERATORS}
                    </Typography>
                  </Stack>
                </TextFieldTitle>{" "}
                <TextFieldTitle title="Aderência (%)">
                  <Stack
                    width={"100%"}
                    paddingY={"10px"}
                    border={`solid 1px ${theme.palette.primary.main}`}
                    borderRadius={"16px"}
                    justifyContent={"center"}
                    alignItems={"center"}
                  >
                    <Typography
                      fontWeight={"700"}
                      fontSize={"16px"}
                      color={"primary"}
                    >
                      {otherCampaignData.GRIP}
                    </Typography>
                  </Stack>
                </TextFieldTitle>
              </Stack>
            )}
          </Stack>
        </Stack>
        <Stack direction={"row"} justifyContent={"flex-end"} mt={"50px"}>
          <Button variant="contained" onClick={handleCreateCampaign}>
            Criar Campanha
          </Button>
        </Stack>
      </ContentArea>
    </ContentCard>
  );
}

CreateCampaignView.getLayout = getLayout("private");
