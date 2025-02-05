import Description from "@mui/icons-material/Description";
import { Box } from "@mui/system";
import { GridColDef, GridRenderCellParams } from "@mui/x-data-grid";
import { useRouter } from "next/router";
import { Sector } from "src/typings";
import { DateUtils } from "../../../../utils";

const DetailsCard = (params: any) => {
    const router = useRouter();

    const handleRedirectToDetailPage = (id: number) =>
        router.push(`/sectors/${id}`);

    return (
        <Box
            sx={{
                width: "100%",
                height: "100%",
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
            }}
            onClick={() => handleRedirectToDetailPage(params.params.row.id)}
        >
            <Description color="primary" />
        </Box>
    );
};

export const listSectorTableColumns: GridColDef[] = [
    {
        field: "id",
        headerName: "Cod. GIP",
        flex: 1,
        editable: false,
        sortable: false,
        filterable: false,
    },
    {
        field: "name",
        headerName: "Nome do Setor",
        flex: 8,
        editable: false,
        sortable: false,
        filterable: false,
    },
    {
        field: "level",
        headerName: "Level",
        type: "number",
        flex: 1,
        editable: false,
        sortable: false,
        filterable: false,
    },
    {
        field: "createdAt",
        headerName: "Criado em",
        type: "number",
        flex: 3,
        editable: false,
        sortable: false,
        filterable: false,
        valueFormatter({ value }) {
            return DateUtils.formatDatePtBRWithTime(value as Date);
        },
    },
    {
        field: "details",
        headerName: "Detalhes",
        flex: 1,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        editable: false,
        renderCell: (params) => <DetailsCard params={params} />,
    },
];
