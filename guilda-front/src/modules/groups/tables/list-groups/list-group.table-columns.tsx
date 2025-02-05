import { GridColDef } from "@mui/x-data-grid";
import { useRouter } from "next/router";
import { DateUtils } from "../../../../utils";
import { Box } from "@mui/material";
import Description from "@mui/icons-material/Description";

const DetailsCard = (params: any) => {
    const router = useRouter();

    const handleRedirectToDetailPage = (id: number) =>
        router.push(`/groups/${id}`);

    return (
        <Box
            sx={{
                width: "100%",
                height: "100%",
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
                cursor: "pointer",
            }}
            onClick={() => handleRedirectToDetailPage(params.params.row.id)}
        >
            <Description color="primary" />
        </Box>
    );
};

export const listGroupTableColumns: GridColDef[] = [
    {
        field: "name",
        headerName: "Nome",
        flex: 2,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "alias",
        headerName: "Nome de Exibição",
        flex: 2,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "description",
        headerName: "Descrição",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "createdAt",
        headerName: "Criado em",
        flex: 2,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
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
        renderCell: (params) => <DetailsCard params={params} />,
    },
];
