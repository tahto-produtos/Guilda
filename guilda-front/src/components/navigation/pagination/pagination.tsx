import { Pagination } from "@mui/material";
import { Box } from "@mui/system";

export default function PaginationComponent(props: {
    totalItems: number;
    limit: number;
    page: number;
    getPage: any;
}) {
    const { totalItems, limit, page, getPage } = props;
    const handleChange = (event: React.ChangeEvent<unknown>, value: number) => {
        getPage(value);
    };

    return (
        <Box
            sx={{
                width: "100%",
                justifyContent: "flex-end",
                alignItems: "center",
                display: "flex",
            }}
        >
            <Pagination
                count={Math.ceil(totalItems / limit)}
                page={page}
                onChange={handleChange}
                size="small"
            />
        </Box>
    );
}
