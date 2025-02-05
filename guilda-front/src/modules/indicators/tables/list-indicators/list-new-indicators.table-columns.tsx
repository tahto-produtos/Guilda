import Description from "@mui/icons-material/Description";
import { Box } from "@mui/system";
import { GridColDef } from "@mui/x-data-grid";
import { DateUtils } from "../../../../utils";
import { useRouter } from "next/router";

const DetailsCard = (params: any) => {
    const router = useRouter();

    const handleRedirectToDetailPage = (id: number) =>
        router.push(`/indicators/news/${id}`);

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
            onClick={() => handleRedirectToDetailPage(params.params.row.INDICATORID)}
        >
            <Description color="primary" />
        </Box>
    );
};

export const listGroupTableNewColumns: GridColDef[] = [
    {
        field: "INDICATORID",
        headerName: "Código",
        flex: 1,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "NAME",
        headerName: "Nome",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "DESCRIPTION",
        headerName: "Descrição",
        flex: 4,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
    },
    {
        field: "CREATED_AT",
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
