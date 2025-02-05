import { useEffect, useState } from "react";
import { ListFeedBackUseCase } from "../use-cases/list-feedback.use-case";
import { useLoadingState } from "src/hooks";
import { Feedback } from "src/typings/models/feedback.model";
import { usePagination } from "src/hooks/use-pagination/use-pagination";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { LinearProgress, Pagination, Stack, Typography } from "@mui/material";
import { ProfileImage } from "src/components/data-display/profile-image/profile-image";
import { capitalizeText } from "src/utils/capitalizeText";
import { uuid } from "uuidv4";
import { ListMyFeedBackUseCase } from "../use-cases/list-my-feedback.use-case";
import { useRouter } from "next/router";

export function MyFeedbackTable() {
    const [feedbacks, setFeedbacks] = useState<Feedback[]>([]);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const { handleChange, page, setPage, setTotalPages, totalPages } =
        usePagination();
    const router = useRouter();

    async function getFeedbacks() {
        startLoading();

        await new ListMyFeedBackUseCase()
            .handle({
                limit: 10,
                page: page,
            })
            .then((data) => {
                setFeedbacks(data.ListMyFeedBack);
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

    return (
        <>
            <DataGrid
                columns={columns}
                rows={feedbacks?.length > 0 ? feedbacks : []}
                hideFooter
                disableColumnFilter
                disableRowSelectionOnClick
                autoHeight
                onRowClick={(params) =>
                    router.push(
                        `/feedback/feedback-details?id=${params.row.IDGDA_FEEDBACK_USER}&sign=1`
                    )
                }
                // rowCount={20}
                localeText={{
                    noResultsOverlayLabel: "Nenhum resultado encontrado",
                }}
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
        </>
    );
}

const columns: GridColDef[] = [
    {
        field: "NAME",
        headerName: "Nome do usuário",
        flex: 2,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        minWidth: 300,
        renderCell: (params) => {
            return (
                <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
                    <ProfileImage
                        height="32px"
                        width="32px"
                        name={params.value}
                    />
                    <Typography fontSize={"14px"} fontWeight={"600"}>
                        {capitalizeText(params.value || "")}
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
        renderCell: (params) => {
            return (
                <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
                    <Typography
                        fontSize={"16px"}
                        fontWeight={"700"}
                        color={"secondary"}
                    >
                        {capitalizeText(params.value || "")}
                    </Typography>
                </Stack>
            );
        },
    },
    {
        field: "DATA_RESPOSTA",
        headerName: "Data da resposta",
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
        field: "PROTOCOLO",
        headerName: "Protocolo",
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
        field: "PENALIDADE",
        headerName: "Penalidade",
        flex: 2,
        disableColumnMenu: true,
        filterable: false,
        sortable: false,
        renderCell: (params) => {
            return (
                <Stack direction={"row"} alignItems={"center"} gap={"10px"}>
                    <Typography
                        fontSize={"16px"}
                        fontWeight={"700"}
                        color={
                            params.value == "Cumprida" ? "primary" : "secondary"
                        }
                    >
                        {params.value}
                    </Typography>
                </Stack>
            );
        },
    },
];
