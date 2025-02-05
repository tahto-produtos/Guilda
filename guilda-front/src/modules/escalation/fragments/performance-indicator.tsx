import { useContext, useEffect, useState } from "react";
import { useDebounce, useLoadingState } from "src/hooks";
import { GridColDef } from "@mui/x-data-grid";
import {
  Autocomplete,
  Button,
  Stack,
  TextField,
  Typography,
  useTheme,
} from "@mui/material";
import { capitalizeText } from "src/utils/capitalizeText";
import { format } from "date-fns";
import {
  IndicatorPerformanceActionEscalation,
  IndicatorPerformanceActionEscalationUseCase,
} from "../use-cases/indicator-performance-action-escalation";
import { Gauge, gaugeClasses } from "@mui/x-charts";
import { Person } from "@mui/icons-material";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { grey } from "@mui/material/colors";
import { IndicatorsBySectorsUseCase } from "src/modules/home/use-cases/IndicatorsBySectors/IndicatorsBySectors.use-case";
import { ListSectorsAndSubsectrosUseCase } from "src/modules/sectors";
import { Indicator, SectorAndSubsector } from "src/typings";
import { GroupNew } from "src/typings/models/group-new.model";
import { ListGroupsNewUseCase } from "src/modules/groups/use-cases/list-groups-new";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { Site } from "src/typings/models/site.model";
import { SitePersonaUseCase } from "src/modules/personas/use-cases/site-personas.use-case";
import { LoadingButton } from "@mui/lab";
import { toast } from "react-toastify";

import { CircularProgressbarWithChildren, buildStyles } from 'react-circular-progressbar';
import 'react-circular-progressbar/dist/styles.css';
/* import { originalPrototype } from 'src/pages/_app'; */

interface ReformulatedGaugeProps {
  value: number;
  valueMax?: number;
}

 function ReformulatedGauge({ value, valueMax = 100 }: ReformulatedGaugeProps) {
  const theme = useTheme();

  return (
    <div style={{ width: 120, height: 120 }}>
      <CircularProgressbarWithChildren
        value={value}
        maxValue={valueMax}
        styles={buildStyles({
          rotation: 0.25, // Rotação para simular o ângulo inicial (-130 a 130 graus)
          strokeLinecap: 'round',
          pathColor: theme.palette.secondary.main, // Cor do arco principal
          trailColor: '#E9EDF0', // Cor do arco de fundo
          pathTransitionDuration: 0.5,
          textColor: theme.palette.text.primary,
        })}
      >

        <div style={{ fontSize: 40, transform: "translate(0px, 0px)", color: theme.palette.text.primary }}>
          {value}
        </div>
      </CircularProgressbarWithChildren>
    </div>
  );
} 

export function PerformanceIndicator() {
  // Usa o HOC com o componente que precisa da exceção
/*   const SafeGauge = withRestoredPrototype(Gauge); */

  const { myUser } = useContext(UserInfoContext);

  const [data, setData] = useState<IndicatorPerformanceActionEscalation[]>([]);
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const theme = useTheme();

  const [startDate, setStartDate] = useState<Date | null>(null);
  const [endDate, setEndDate] = useState<Date | null>(null);

  const [sectors, setSectors] = useState<SectorAndSubsector[]>([]);
  const [selectedSectors, setSelectedSectors] =
    useState<SectorAndSubsector | null>(null);
  const [sectorSearch, setSectorSearch] = useState<string>("");
  const [subSectors, setSubSectors] = useState<SectorAndSubsector[]>([]);
  const [selectedSubSector, setSelectedSubSector] =
    useState<SectorAndSubsector | null>(null);
  const [subSectorSearch, setSubSectorSearch] = useState<string>("");

  const [indicator, setIndicator] = useState<Indicator | null>(null);
  const [indicatorSearchValue, setIndicatorSearchValue] = useState<string>("");
  const [indicators, setIndicators] = useState<Indicator[]>([]);
  const debouncedIndicatorsSearchTerm: string = useDebounce<string>(
    indicatorSearchValue,
    400
  );
  const [groups, setGroups] = useState<GroupNew[]>([]);
  const [selectedGroups, setSelectedGroups] = useState<GroupNew | null>(null);

  const [site, setSite] = useState<Site | null>(null);
  const [sites, setSites] = useState<Site[]>([]);

  async function ListSitePersona() {
    await new SitePersonaUseCase()
      .handle()
      .then((data) => {
        setSites(data);
      })
      .catch(() => { })
      .finally(() => { });
  }

  useEffect(() => {
    ListSitePersona();
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
      .catch(() => { })
      .finally(() => {
        finishLoading();
      });
  };

  useEffect(() => {
    if (myUser && myUser?.id) {
      getGroups(myUser?.id);
    }
  }, []);

  const getIndicatorsList = async () => {
    if (!endDate || !startDate || !selectedSectors) {
      return;
    }

    new IndicatorsBySectorsUseCase()
      .handle({
        sectors: [
          {
            id: selectedSectors.id,
          },
        ],
        dtInicial: format(new Date(startDate.toString()), "yyyy-MM-dd"),
        dtfinal: format(new Date(endDate.toString()), "yyyy-MM-dd"),
      })
      .then((data) => {
        setIndicators(data);
      })
      .catch(() => { });
  };

  useEffect(() => {
    selectedSectors && getIndicatorsList();
  }, [debouncedIndicatorsSearchTerm, selectedSectors, startDate, endDate]);

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
      .catch(() => { })
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

  async function getData() {
    if (!startDate || !endDate || !indicator) return;

    startLoading();

    await new IndicatorPerformanceActionEscalationUseCase()
      .handle({
        STARTEDATFROM: format(new Date(startDate.toString()), "yyyy-MM-dd"),
        STARTEDATTO: format(new Date(endDate.toString()), "yyyy-MM-dd"),
        INDICATORSID: indicator.id,
        SECTORSID: selectedSectors?.id,
        SUBSECTORSID: selectedSubSector?.id,
        IDHOMEBASED: 0,
        IDGROUP: selectedGroups?.id,
        IDSITE: site?.id,
      })
      .then((responseData) => {
        if (typeof responseData === "string") {
          return toast.error(responseData);
        }

        setData([
          ...data,
          {
            ...responseData,
            INDICATORNAME: `${indicator.id} - ${indicator.name}`,
          },
        ]);
      })
      .catch((e) => { })
      .finally(() => {
        finishLoading();
      });
  }

  return (
    <Stack>
      <Stack width={"100%"} gap={"16px"}>
        <Stack direction={"row"} gap={"16px"} width={"100%"}>
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
                  fullWidth: true,
                },
              }}
            />
          </LocalizationProvider>
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
                  fullWidth: true,
                },
              }}
            />
          </LocalizationProvider>
        </Stack>
        <Stack direction={"row"} gap={"16px"} width={"100%"}>
          <Autocomplete
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
            renderInput={(props) => <TextField {...props} label={"Setores"} />}
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
            fullWidth
            value={selectedSubSector}
            options={subSectors}
            disableClearable={false}
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
              setSelectedSubSector(value);
            }}
            onInputChange={(e, text) => setSubSectorSearch(text)}
            filterOptions={(x) => x}
            filterSelectedOptions
            renderInput={(props) => <TextField {...props} label={"Subsetor"} />}
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
            fullWidth
            options={indicators}
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
              setIndicator(value);
            }}
            onInputChange={(e, text) => setIndicatorSearchValue(text)}
            disabled={!selectedSectors}
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
            isOptionEqualToValue={(option, value) => option.name === value.name}
            sx={{ p: 0, m: 0 }}
          />
        </Stack>
        <Stack direction={"row"} gap={"16px"} width={"100%"}>
          <Autocomplete
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
            isOptionEqualToValue={(option, value) => option.name === value.name}
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
        </Stack>
        <Stack direction={"row"}>
          <LoadingButton
            loading={isLoading}
            onClick={getData}
            variant="contained"
            color="secondary"
          >
            Visualizar
          </LoadingButton>
        </Stack>
      </Stack>
      <Stack
        direction={"row"}
        style={{
          flexWrap: "wrap",
          gap: "40px",
          marginTop: "40px",
          width: "100%",
        }}
      >
        {data.map((item, index) => (
          <Stack
            key={index}
            px={"20px"}
            direction={"column"}
            position={"relative"}
            justifyContent={"center"}
            alignItems={"center"}
            height={"250px"}
            width={"300px"}
          >
            <Stack flexDirection={"column"} gap={"5px"} alignItems={"center"}>
              <Typography fontWeight={"700"}>{item.INDICATORNAME}</Typography>
              <Typography fontWeight={"700"}>
                {format(new Date(item.STARTEDATFROM), "dd/MM/yyyy")} -{" "}
                {format(new Date(item.STARTEDATTO), "dd/MM/yyyy")}
              </Typography>
            </Stack>
             {/* <Gauge
              value={item?.RESULT}
              valueMax={100}
              startAngle={-130}
              endAngle={130}
              sx={{
                [`& .${gaugeClasses.valueText}`]: {
                  fontSize: 30,
                  transform: "translate(0px, 0px)",
                },
                [`& .${gaugeClasses.valueText}`]: {
                  fontSize: 40,
                },
                [`& .${gaugeClasses.valueArc}`]: {
                  fill: theme.palette.secondary.main,
                },
                [`& .${gaugeClasses.referenceArc}`]: {
                  fill: "#E9EDF0",
                },
              }}
              text={({ value, valueMax }) => ``}
            />  */}
            <ReformulatedGauge value={item?.RESULT} />
            <Stack
              position={"absolute"}
              justifyContent={"center"}
              alignItems={"center"}
              bottom={"55px"}
              gap={"10px"}
            >
              <Stack
              position={"absolute"}
                width={"32px"}
                height={"32px"}
                bgcolor={"#ECEAF8"}
                justifyContent={"center"}
                alignItems={"center"}
                borderRadius={"100px"}
                bottom={"60px"}
              >
                <Person color="secondary" />
              </Stack>
              <Typography fontSize={"21px"} fontWeight={"700"}>
                {item.GOAL !== 0
                  ? `${item.PERCENT.toFixed(1)}% / ${item.GOAL.toFixed(1)}%`
                  : item.RESULT.toFixed(1)}
              </Typography>
            </Stack>
          </Stack>
        ))}
      </Stack>

      {/* <DataGrid
        columns={columns}
        rows={actionEscalations?.length > 0 ? actionEscalations : []}
        hideFooter
        disableColumnFilter
        disableRowSelectionOnClick
        autoHeight
        // rowCount={20}
        localeText={{
          noResultsOverlayLabel: "Nenhum resultado encontrado",
        }}
        paginationMode="server"
        loading={isLoading}
        slots={{
          loadingOverlay: LinearProgress,
        }}
        getRowId={(row) => {
          const uuidv4 = uuid();
          return uuidv4;
        }}
        sx={{ width: "100%" }}
      />
      <Pagination
        count={totalPages || 0}
        page={page}
        onChange={handleChange}
        disabled={isLoading}
      /> */}
    </Stack>
  );
}

/* function withRestoredPrototype(Component: React.FC) {
  return function RestoredPrototypeComponent(props: any) {
    // Antes de renderizar, restaura o protótipo original
    Object.setPrototypeOf(Object.prototype, originalPrototype);

    // Rende o componente
    const result = <Component {...props} />;

    // Após renderizar, aplica as regras de segurança novamente
    Object.setPrototypeOf(Object.prototype, null);

    if (typeof window !== 'undefined') {
      Object.freeze(Object.prototype);
    }

    return result;
  };
}
type GaugeProps = {
  value: number;
  valueMax: number;
  startAngle: number;
  endAngle: number;
  sx?: any;
  text?: (params: { value: number; valueMax: number }) => string;
};
 */

const columns: GridColDef[] = [
  {
    field: "NOME",
    headerName: "Nome",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
          <Typography fontSize={"16px"} fontWeight={"400"}>
            {capitalizeText(params.value || "")}
          </Typography>
        </Stack>
      );
    },
  },
  {
    field: "INDICADOR",
    headerName: "Indicador",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
          <Typography fontSize={"16px"} fontWeight={"400"}>
            {params.value}
          </Typography>
        </Stack>
      );
    },
  },
  {
    field: "STATUS",
    headerName: "Status",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    minWidth: 200,
    renderCell: (params) => {
      return (
        <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
          <Typography
            fontSize={"16px"}
            fontWeight={"700"}
            color={params.value !== "Em andamento" ? "primary" : "secondary"}
          >
            {params.value || ""}
          </Typography>
        </Stack>
      );
    },
  },
  {
    field: "startedAt",
    headerName: "Data de inicio",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
          <Typography fontSize={"16px"} fontWeight={"400"}>
            {format(new Date(params.value), "dd/MM/yyyy")}
          </Typography>
        </Stack>
      );
    },
  },
  {
    field: "endedAt",
    headerName: "Data de termino",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
          <Typography fontSize={"16px"} fontWeight={"400"}>
            {format(new Date(params.value), "dd/MM/yyyy")}
          </Typography>
        </Stack>
      );
    },
  },
];
