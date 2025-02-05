import DeleteForever from "@mui/icons-material/DeleteForever";
import Description from "@mui/icons-material/Description";
import { Box } from "@mui/system";
import { GridColDef } from "@mui/x-data-grid";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { DateUtils } from "../../../../utils";
import { DeleteIndicatorUseCase } from "../../use-cases/delete-indicator.use-case";
import { useRouter } from "next/router";

const DetailsCard = (params: any) => {
    const router = useRouter();

    const handleRedirectToDetailPage = (id: number) =>
        router.push(`/indicators/${id}`);

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
        field: "id",
        headerName: "Código",
        flex: 1,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "name",
        headerName: "Nome",
        flex: 4,
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
