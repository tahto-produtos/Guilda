import { useEffect, useState } from "react";
import { useLoadingState } from "src/hooks";
import { Feedback } from "src/typings/models/feedback.model";
import { usePagination } from "src/hooks/use-pagination/use-pagination";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import {
  Autocomplete,
  Button,
  Checkbox,
  Drawer,
  LinearProgress,
  Pagination,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { ProfileImage } from "src/components/data-display/profile-image/profile-image";
import { capitalizeText } from "src/utils/capitalizeText";
import { uuid } from "uuidv4";
import { ListFeedBackUseCase } from "src/modules/feedback/use-cases/list-feedback.use-case";
import {
  LoadActionEscalation,
  LoadLibraryActionEscalationUseCase,
} from "../use-cases/load-library-action-escalation";
import { format } from "date-fns";
import { useRouter } from "next/router";
import { FilterList } from "@mui/icons-material";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { grey } from "@mui/material/colors";
import { ListIndicatorsUseCase } from "src/modules/indicators/use-cases";
import { Indicator } from "src/typings";

export function ActionPlanTable() {
  const [actionEscalations, setActionEscalations] = useState<
    LoadActionEscalation[]
  >([]);
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const { handleChange, page, setPage, setTotalPages, totalPages } =
    usePagination();
  const router = useRouter();
  const [isOpenFilters, setIsOpenFilters] = useState(false);
  const [startDate, setStartDate] = useState<dateFns | null>(null);
  const [endDate, setEndDate] = useState<dateFns | null>(null);
  const [indicator, setIndicator] = useState<Indicator[]>([]);
  const [indicatorSearchValue, setIndicatorSearchValue] = useState<string>("");
  const [indicators, setIndicators] = useState<Indicator[]>([]);
  const [isAutomatic, setIsAutomatic] = useState(false);
  const [name, setName] = useState("");
  const [desc, setDesc] = useState("");

  async function getFeedbacks(pageForce?: number) {
    startLoading();

    await new LoadLibraryActionEscalationUseCase()
      .handle({
        CREATEDATFROM: "",
        CREATEDATTO: "",
        STARTEDATFROM: startDate
          ? format(new Date(startDate.toString()), "yyyy-MM-dd")
          : "",
        STARTEDATTO: endDate
          ? format(new Date(endDate.toString()), "yyyy-MM-dd")
          : "",
        ENDEDATFROM: "",
        ENDEDATTO: "",
        NAME: name,
        DESCRIPTION: desc,
        INDICATOR: indicator.map((i) => i.id),
        AUTOMATIC: isAutomatic ? 1 : 0,
        LIMIT: 5,
        PAGE: pageForce || page,
      })
      .then((data) => {
        setActionEscalations(data.LoadActionEscalation);
        setTotalPages(data.totalpages);
      })
      .catch(() => {})
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    getFeedbacks();
  }, [page]);

  const getIndicators = async (searchText: string) => {
    //startLoading();

    await new ListIndicatorsUseCase()
      .handle({
        limit: 10,
        offset: 1,
        searchText: searchText,
      })
      .then((data) => {
        setIndicators(data.items);
      })
      .catch(() => {})
      .finally(() => {
        //finishLoading();
      });
  };

  useEffect(() => {
    getIndicators(indicatorSearchValue);
  }, [indicatorSearchValue]);

  return (
    <Stack>
      <Stack
        direction={"row"}
        justifyContent={"flex-end"}
        position={"relative"}
      >
        <Button
          variant="contained"
          color="primary"
          sx={{ position: "absolute", top: "-60px" }}
          onClick={() => setIsOpenFilters(true)}
          startIcon={<FilterList />}
        >
          Filtros
        </Button>
      </Stack>
      <DataGrid
        columns={columns}
        rows={actionEscalations?.length > 0 ? actionEscalations : []}
        hideFooter
        disableColumnFilter
        disableRowSelectionOnClick
        onRowClick={(params) => {
          router.push(
            `/escalation/${params.row.IDGDA_ESCALATION_ACTION}-${params.row.AUTOMATIC}`
          );
        }}
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
        sx={{ width: "100%", cursor: "pointer" }}
      />
      <Pagination
        count={totalPages || 0}
        page={page}
        onChange={handleChange}
        disabled={isLoading}
      />
      <Drawer
        open={isOpenFilters}
        anchor={"right"}
        onClose={() => setIsOpenFilters(false)}
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
            <FilterList />
            Filtrar
          </Typography>
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
                    width: "100%",
                  },
                },
              }}
            />
          </LocalizationProvider>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="De"
              value={endDate}
              onChange={(newValue) => setEndDate(newValue)}
              slotProps={{
                textField: {
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
          <Autocomplete
            fullWidth
            multiple
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
            isOptionEqualToValue={(option, value) => option.name === value.name}
            sx={{ p: 0, m: 0 }}
          />
          <TextField
            value={name}
            label="Nome"
            onChange={(e) => setName(e.target.value)}
          />
          <TextField
            value={desc}
            label="Descrição"
            onChange={(e) => setDesc(e.target.value)}
          />
          <Stack
            direction={"row"}
            alignItems={"center"}
            gap={"10px"}
            onClick={() => setIsAutomatic(!isAutomatic)}
          >
            <Typography>Filtrar por Automático:</Typography>
            <Checkbox checked={isAutomatic} />
          </Stack>
          <Button
            variant="contained"
            onClick={() => {
              getFeedbacks(1);
              setPage(1);
              setIsOpenFilters(false);
            }}
          >
            Filtrar
          </Button>
        </Stack>
      </Drawer>
    </Stack>
  );
}

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
            {params.value ? format(new Date(params.value), "dd/MM/yyyy") : ""}
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
            {params.value ? format(new Date(params.value), "dd/MM/yyyy") : ""}
          </Typography>
        </Stack>
      );
    },
  },
];
