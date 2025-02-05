import CompareArrows from "@mui/icons-material/CompareArrows";
import East from "@mui/icons-material/East";
import West from "@mui/icons-material/West";
import { GridColDef } from "@mui/x-data-grid";

const Bar = (params: any) => {
    if (params.params.row.value > 0) {
        return <West color="error" />;
    } else {
        return <East color="success" />;
    }
};

const CustomHeader = () => {
    return <CompareArrows />;
};

export const listMonetizationExpiredTableColumns: GridColDef[] = [
    {
        field: "id",
        headerName: "",
        flex: 0.5,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        renderCell: (params) => <Bar params={params} />,
        renderHeader: (params) => <CustomHeader />,
    },
    {
        field: "value",
        headerName: "Valor",
        flex: 1,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        renderCell: (params) => params.row.value,
    },
    {
        field: "dataExpiration",
        headerName: "Data Expiração",
        flex: 1,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        align: "right",
        headerAlign: "right",
        valueFormatter({ value }) {
            if (value) {
                const date = value.split("T")[0];
                const dateSplited = date.split("-");
                return `${dateSplited[2]}/${dateSplited[1]}/${dateSplited[0]}`;
            } else {
                return null;
            }
        },
    },
];
