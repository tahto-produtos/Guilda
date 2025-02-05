import Stars from "@mui/icons-material/Stars";

import {
  Autocomplete,
  Box,
  Button,
  FormControl,
  InputLabel,
  LinearProgress,
  MenuItem,
  Select,
  Stack,
  TextField,
  Typography,
  lighten,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format } from "date-fns";
import { useContext, useEffect, useState } from "react";
import { Card, PageHeader } from "src/components";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListGroupsUseCase } from "src/modules/groups";
import { ListHierarchiesUseCase } from "src/modules/hierarchies/use-cases/list-hierarchies.use-case";
import { ListIndicatorsUseCase } from "src/modules/indicators/use-cases";
import { ListSectorsUseCase } from "src/modules/sectors";
import { Group, Indicator, Sector } from "src/typings";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { ListOperationRankingUseCase } from "../../use-cases/list-operation-ranking.use-case";
import { LoadingButton } from "@mui/lab";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { NoResultsOverlay } from "src/components/data-display/table/fragments/no-results-overlay";
import { ListHierarchyByIdUseCase } from "../../use-cases/list-hierarchy-by-id";
import { SectorsByHierachyUseCase } from "../../use-cases/SectorsByHierarchy/SectorsByHierarchy.use-case";
import { IndicatorsBySectorsUseCase } from "../../use-cases/IndicatorsBySectors/IndicatorsBySectors.use-case";
import { toast } from "react-toastify";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { formatCurrency } from "src/utils/format-currency";

interface OperationRankingItem {
  Cargo: string;
  CodigoGIP: string;
  CodigoIndicador: string;
  Data: string;
  Grupo: string;
  MatriculaDoColaborador: string;
  Meta: string;
  NomeAgente: string;
  NomeIndicador: string;
  PercentualDeAtingimento: number;
  Reincidencia: boolean;
  Resultado: number;
  Setor: string;
}

export function OperationRanking() {
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [rowCountState, setRowCountState] = useState(100);
  const { myUser } = useContext(UserInfoContext);

  useEffect(() => {
    setRowCountState((prevRowCountState) => prevRowCountState);
  }, [setRowCountState]);

  const [startDatePicker, setStartDatePicker] = useState<dateFns | Date | null>(
    null
  );
  const [endDatePicker, setEndDatePicker] = useState<dateFns | Date | null>(
    null
  );

  const [sector, setSector] = useState<Sector[]>([]);
  const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
  const [sectors, setSectors] = useState<Sector[]>([]);
  const debouncedSectorSearchTerm: string = useDebounce<string>(
    sectorsSearchValue,
    400
  );

  const [group, setGroup] = useState<Group[]>([]);
  const [groupsSearchValue, setGroupsSearchValue] = useState<string>("");
  const [groups, setGroups] = useState<Group[]>([]);
  const debouncedGroupsSearchTerm: string = useDebounce<string>(
    groupsSearchValue,
    400
  );

  const [indicator, setIndicator] = useState<Indicator[]>([]);
  const [indicatorSearchValue, setIndicatorSearchValue] = useState<string>("");
  const [indicators, setIndicators] = useState<Indicator[]>([]);
  const debouncedIndicatorsSearchTerm: string = useDebounce<string>(
    indicatorSearchValue,
    400
  );

  const [hierarchie, setHierarchie] = useState<{ id: number; name: string }[]>(
    []
  );
  const [hierarchiesSearchValue, setHierarchiesSearchValue] =
    useState<string>("");
  const [hierarchies, setHierarchies] = useState<
    { id: number; name: string }[]
  >([]);
  const debouncedHierarchiesSearchTerm: string = useDebounce<string>(
    hierarchiesSearchValue,
    400
  );

  const [orderBy, setOrderBy] = useState<string>("Melhor");

  const [data, setData] = useState<OperationRankingItem[]>([]);

  const getHierarchiesList = async () => {
    if (!myUser) return;

    new ListHierarchyByIdUseCase()
      .handle({ CollaboratorId: myUser.id })
      .then((data) => {
        setHierarchies(data);
      })
      .catch(() => { });
  };

  useEffect(() => {
    myUser && getHierarchiesList();
  }, [debouncedHierarchiesSearchTerm, myUser]);

  const getIndicatorsList = async () => {
    if (!endDatePicker || !startDatePicker) {
      return;
    }

    new IndicatorsBySectorsUseCase()
      .handle({
        sectors: sector.map((item) => {
          return {
            id: item.id,
          };
        }),
        dtInicial: format(new Date(startDatePicker.toString()), "yyyy-MM-dd"),
        dtfinal: format(new Date(endDatePicker.toString()), "yyyy-MM-dd"),
      })
      .then((data) => {
        setIndicators(data);
      })
      .catch(() => { });
  };

  useEffect(() => {
    sector.length > 0 && getIndicatorsList();
  }, [debouncedIndicatorsSearchTerm, sector]);

  const getGroupsList = async () => {
    const pagination = {
      limit: 20,
      offset: 0,
      searchText: groupsSearchValue,
    };

    new ListGroupsUseCase()
      .handle(pagination)
      .then((data) => {
        setGroups(data.items);
      })
      .catch(() => { });
  };

  useEffect(() => {
    getGroupsList();
  }, [debouncedGroupsSearchTerm]);

  const getSectorsList = async () => {
    if (!endDatePicker || !startDatePicker || !myUser) {
      return;
    }

    new SectorsByHierachyUseCase()
      .handle({
        codCollaborator: myUser.id,
        sector: sectorsSearchValue,
        dtInicial: format(new Date(startDatePicker.toString()), "yyyy-MM-dd"),
        dtfinal: format(new Date(endDatePicker.toString()), "yyyy-MM-dd"),
      })
      .then((data) => {
        setSectors(data);
      })
      .catch(() => { });
  };

  useEffect(() => {
    myUser && endDatePicker && startDatePicker && getSectorsList();
  }, [debouncedSectorSearchTerm, myUser, endDatePicker, startDatePicker]);

  async function getOperationRankingData() {
    if (!myUser) return;

    startLoading();

    setData([]);

    const payload = {
      sectors: sector.map((item) => {
        return { id: item.id };
      }),
      groups: group.map((item) => {
        return { id: item.id };
      }),
      indicators: indicator.map((item) => {
        return { id: item.id };
      }),
      hierarchies: hierarchie.map((item) => {
        return { id: item.id };
      }),
      order: orderBy,
      dataInicial: startDatePicker
        ? format(new Date(startDatePicker.toString()), "yyyy-MM-dd")
        : undefined,
      dataFinal: endDatePicker
        ? format(new Date(endDatePicker.toString()), "yyyy-MM-dd")
        : undefined,
      CollaboratorId: myUser.id,
    };

    await new ListOperationRankingUseCase()
      .handle(payload)
      .then((data) => {
        setData(data);
      })
      .catch(() => { })
      .finally(() => {
        finishLoading();
      });
  }

  return (
    <Card>
      <PageHeader title="Ranking operação" headerIcon={<Stars />} />
      <Stack px={2} py={3} width={"100%"} gap={2}>
        <Box display={"flex"} flexDirection={"row"} gap={"10px"}>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="De"
              value={startDatePicker}
              onChange={(newValue) => setStartDatePicker(newValue)}
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
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Até"
              value={endDatePicker}
              onChange={(newValue) => setEndDatePicker(newValue)}
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
        </Box>
        <Box display={"flex"} flexDirection={"row"} gap={1} width={"100%"}>
          <Autocomplete
            multiple
            size={"small"}
            options={sectors}
            fullWidth
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
              setSector(value);
            }}
            onInputChange={(e, text) => setSectorsSearchValue(text)}
            filterSelectedOptions
            renderInput={(params) => (
              <TextField
                {...params}
                variant="outlined"
                label="Selecione um ou mais setores"
                placeholder="Buscar"
              />
            )}
            filterOptions={(options, { inputValue }) =>
              options.filter(
                (item) =>
                  item.id.toString().includes(inputValue.toString()) ||
                  item.name
                    .toString()
                    .toLowerCase()
                    .includes(inputValue.toString().toLowerCase())
              )
            }
            renderOption={(props, option) => {
              return (
                <li {...props} key={option.id}>
                  {option.id} - {option.name}
                </li>
              );
            }}
            isOptionEqualToValue={(option, value) => option.name === value.name}
            sx={{ p: 0, m: 0 }}
          />
          <Autocomplete
            multiple
            size={"small"}
            options={groups}
            fullWidth
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
              setGroup(value);
            }}
            onInputChange={(e, text) => setGroupsSearchValue(text)}
            filterOptions={(x) => x}
            filterSelectedOptions
            renderInput={(params) => (
              <TextField
                {...params}
                variant="outlined"
                label="Selecione um ou mais grupos"
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
          <Autocomplete
            multiple
            fullWidth
            size={"small"}
            options={indicators}
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
              setIndicator(value);
            }}
            onInputChange={(e, text) => setIndicatorSearchValue(text)}
            disabled={sector.length <= 0}
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
                label="Selecione um ou mais indicadores"
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
        </Box>
        <Box display={"flex"} flexDirection={"row"} gap={1} width={"100%"}>
          <Autocomplete
            multiple
            size={"small"}
            fullWidth
            options={hierarchies}
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
              setHierarchie(value);
            }}
            onInputChange={(e, text) => setHierarchiesSearchValue(text)}
            filterOptions={(x) => x}
            filterSelectedOptions
            renderInput={(params) => (
              <TextField
                {...params}
                variant="outlined"
                label="Selecione uma ou mais hierarquias"
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
          />
          <FormControl fullWidth>
            <InputLabel size="small" sx={{ background: "#fff", px: "5px" }}>
              Agrupar resultados por:
            </InputLabel>
            <Select
              size="small"
              value={orderBy}
              onChange={(e) => setOrderBy(e.target.value)}
            >
              <MenuItem value={"Melhor"}>Melhor</MenuItem>
              <MenuItem value={"Pior"}>Pior</MenuItem>
            </Select>
          </FormControl>
        </Box>
        <LoadingButton
          loading={isLoading}
          variant="contained"
          onClick={getOperationRankingData}
        >
          Confirmar
        </LoadingButton>
        {/* {data.map((item, index) => (
                    <Box key={index} width={"100%"} display={"flex"}>
                        <Typography>{item.NomeAgente}</Typography>
                        <Typography>{item.Meta}</Typography>
                        <Typography>{item.Resultado.toFixed(2)}</Typography>
                        <Typography>{item.NomeIndicador}</Typography>
                        <Typography>{item.PercentualDeAtingimento}</Typography>
                        <Typography>{item.Grupo}</Typography>
                    </Box>
                ))} */}
        {data.length > 0 && (
           <Box
           sx={{
             height: 400,
             width: "100%",
             overflowX: "auto", // Forçar rolagem horizontal
             display: "block", // Garantir que o DataGrid se comporta como bloco
           }}
         >
           <DataGrid
             columns={columns}
             rows={data} // Substituir com seus dados reais
             hideFooter
             disableColumnFilter
             disableRowSelectionOnClick
             autoHeight={false} // Desabilitar autoHeight para permitir rolagem
             rowCount={rowCountState} // Ajustar conforme necessário
             loading={isLoading} // Exemplo de carregamento
             paginationMode="server"
             slots={{
               loadingOverlay: LinearProgress, // Manter o overlay de carregamento
             }}
             getRowId={(row) =>
               `${row.CodigoGip}-${row.Setor}-${row.Cargo}-${row.NomeAgente}-${Math.floor(Math.random() * (1000 - 1 + 1)) + 1}`
             }
             sx={{
               minWidth: 900, // Garantir uma largura mínima para forçar rolagem horizontal
             }}
           />
         </Box>
        )}

      </Stack>
    </Card>
  );
}



export const columns: GridColDef[] = [
  {
    field: "Reincidencia",
    headerName: "Reincidência",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    minWidth: 150,
    // valueFormatter({ value }) {
    //     return value ? "Sim" : "Não";
    // },
    renderCell: (params) => {
      return (
        <Box
          sx={{
            backgroundColor: params.row.Reincidencia
              ? lighten("#f00", 0.8)
              : "none",
          }}
        >
          {params.row.Reincidencia === true ? "Sim" : "Não"}
        </Box>
      );
    },
    headerClassName: 'header-grid',
  },
  {
    field: "NomeAgente",
    headerName: "Nome",
    flex: 5,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    minWidth: 150,
  },
  {
    field: "NomeSupervisor",
    minWidth: 200,
    headerName: "Nome do supervisor",
    flex: 4,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
  },
  {
    field: "Meta",
    headerName: "Meta",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Box>
          {formatCurrency(params?.row?.Meta)}
          {params?.row?.TipoIndicador == "PERCENT" &&
            params?.row?.Meta !== "-" &&
            "%"}
        </Box>
      );
    },
    minWidth: 120,
  },
  {
    field: "Resultado",
    headerName: "Resultado",
    minWidth: 120,
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Box
          sx={{
            backgroundColor: params.row.value ? lighten("#f00", 0.8) : "none",
          }}
        >
          {params?.row?.TipoIndicador == "INTEGER"
            ? formatCurrency(params.row.Resultado.toFixed(0))
            : formatCurrency(params.row.Resultado.toFixed(2))}
          {params?.row?.TipoIndicador == "PERCENT" && "%"}
        </Box>
      );
    },
  },
  {
    field: "NomeIndicador",
    headerName: "Nome Indicador",
    minWidth: 170,
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
  },
  {
    field: "CodigoGIPSubsetor",
    headerName: "Código GIP Subsetor",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    minWidth: 200,
  },
  {
    field: "Subsetor",
    headerName: "Subsetor",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    minWidth: 120,
  },
  {
    field: "PercentualDeAtingimento",
    headerName: "Percentual De Atingimento",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    valueFormatter({ value }) {
      return `${formatCurrency(value.toFixed(2))}%`;
    },
    minWidth: 250,
  },
  {
    field: "Grupo",
    headerName: "Grupo",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    minWidth: 120,
  },
  {
    field: "Score",
    headerName: "Score",
    flex: 1,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    valueFormatter({ value }) {
      return `${value.toString().replace(".", ",")}%`;
    },
    minWidth: 120,
  },
];
