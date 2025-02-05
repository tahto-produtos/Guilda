import { useEffect, useState, useImperativeHandle, forwardRef, ForwardRefRenderFunction } from "react";
import { ListFeedBackUseCase } from "../use-cases/list-feedback.use-case";
import { useLoadingState } from "src/hooks";
import { Feedback } from "src/typings/models/feedback.model";
import { usePagination } from "src/hooks/use-pagination/use-pagination";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { LinearProgress, Pagination, Stack, Typography } from "@mui/material";
import { ProfileImage } from "src/components/data-display/profile-image/profile-image";
import { capitalizeText } from "src/utils/capitalizeText";
import { uuid } from "uuidv4";
import { ListInfractionsUseCase } from "../use-cases/list-infractions.use-case";
import { Infraction } from "src/typings/models/infraction.model";
import { InfractionDetailsDrawer } from "./infraction-details-drawer";
import InfoOutlined from "@mui/icons-material/InfoOutlined";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { SitePersonaResponse } from "src/modules/personas/use-cases/site-personas.use-case";
import { SectorAndSubsector } from "src/typings";
import { GroupNew } from "src/typings/models/group-new.model";

interface ListInfractionsdBackProps {
  Hierarchy: Hierarchie[];
  Site: SitePersonaResponse[];
  Sector: SectorAndSubsector[];
  SubSector: SectorAndSubsector[];
  Userid: {
      id: number;
      name: string;
      registry: string;
    }[] | null;
  Groups: GroupNew[];
  NameBc: string;
}

export interface ChildComponentHandle {
  childFunction: () => void;
}

export function InfractionsTable() {
  const [infractions, setInfractions] = useState<Infraction[]>([]);
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const { handleChange, page, setPage, setTotalPages, totalPages } =
    usePagination();
  const [selectedDetails, setSelectedDetails] = useState<Infraction | null>(
    null
  );

  async function getFeedbacks() {
    startLoading();

    await new ListInfractionsUseCase()
      .handle({
        limit: 10,
        page: 1,
      })
      .then((data) => {
        setInfractions(data.ListFeedBack);
        setTotalPages(data.totalpages);
      })
      .catch(() => {})
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    getFeedbacks();
  }, []);

  return (
    <>
      <DataGrid
        columns={columns}
        rows={infractions?.length > 0 ? infractions : []}
        hideFooter
        disableColumnFilter
        disableRowSelectionOnClick
        autoHeight
        onRowClick={(params) => {
          setSelectedDetails(params.row);
        }}
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
      />
      {selectedDetails && (
        <InfractionDetailsDrawer
          isOpen={Boolean(selectedDetails)}
          infraction={selectedDetails}
          refreshList={getFeedbacks}
          onClose={() => setSelectedDetails(null)}
        />
      )}
    </>
  );
}

const columns: GridColDef[] = [
  {
    field: "NAME",
    headerName: "Nome da infração",
    flex: 2,
    minWidth: 200,
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
    field: "CREATED_AT",
    headerName: "Criado em",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    minWidth: 150,
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
    field: "TYPE",
    headerName: "Tipo",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    minWidth: 100,
    renderCell: (params) => {
      return (
        <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
          <Typography fontSize={"16px"} fontWeight={"700"} color="primary">
            {params.value}
          </Typography>
        </Stack>
      );
    },
  },
  {
    field: "AMOUNT_COLLABORATORS",
    headerName: "Colaboradores associados",
    flex: 2,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    minWidth: 250,
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
    field: "TYPE_INFRACTION",
    headerName: "Tipo Infração",
    flex: 2,
    minWidth: 150,
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
    field: "ORDER",
    headerName: "Ordem",
    flex: 2,
    minWidth: 100,
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
    field: "CODE",
    headerName: "Cod.",
    flex: 2,
    minWidth: 70,
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
    field: "PERIOD",
    headerName: "Período",
    flex: 2,
    minWidth: 150,
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
    field: "id",
    headerName: "",
    flex: 0.5,
    disableColumnMenu: true,
    filterable: false,
    sortable: false,
    renderCell: (params) => {
      return (
        <Stack
          direction={"row"}
          alignItems={"center"}
          justifyContent={"flex-end"}
          width={"100%"}
          gap={"10px"}
          pr={"18px"}
        >
          <InfoOutlined />
        </Stack>
      );
    },
  },
];
