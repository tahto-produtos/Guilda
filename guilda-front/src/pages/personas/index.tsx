import EditOutlined from "@mui/icons-material/EditOutlined";
import {
    Button,
    Divider,
    IconButton,
    InputAdornment,
    Pagination,
    Stack,
    TextField,
    Typography,
    useTheme,
} from "@mui/material";
import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ProfileImage } from "src/components/data-display/profile-image/profile-image";
import { EmptyState } from "src/components/feedback/empty-state/empty-state";
import { SearchIcon } from "src/components/icons/search.icon";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { useDebounce, useLoadingState } from "src/hooks";
import {
    ListPersonasUseCase,
    ListedPersona,
} from "src/modules/personas/use-cases/list-personas.use-case";
import { getLayout } from "src/utils";
import { capitalizeText } from "src/utils/capitalizeText";

export default function PersonasView() {
    const [personas, setPersonas] = useState<ListedPersona[] | []>([]);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [searchText, setSearchText] = useState<string>("");
    const debouncedSearchText: string = useDebounce<string>(searchText, 400);
    const [totalPages, setTotalPages] = useState<number | null>(null);
    const theme = useTheme();
    const router = useRouter();
    const [page, setPage] = useState(1);

    const handleChangePagination = (
        event: React.ChangeEvent<unknown>,
        value: number
    ) => {
        setPage(value);
    };

    useEffect(() => {
        setPage(1);
    }, [searchText]);

    async function ListPersonas() {
        startLoading();

        new ListPersonasUseCase()
            .handle({
                accountPersona: searchText,
                limit: 10,
                page: page,
            })
            .then((data) => {
                setPersonas(data.ACCOUNTS || []);
                setTotalPages(data.TOTALPAGES);
            })
            .catch(() => {
                toast.error("Erro ao listar as personas.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    useEffect(() => {
        ListPersonas();
    }, [debouncedSearchText, page]);

    return (
        <ContentCard>
            <ContentArea>
                <PageTitle title="Personas" loading={isLoading}>
                    <Stack
                        direction={"row"}
                        alignItems={"center"}
                        gap={"16px"}
                        width={"100%"}
                        justifyContent={"flex-end"}
                    >
                        <TextField
                            label="Busque por uma persona"
                            size="small"
                            sx={{
                                maxWidth: "250px",
                                width: "100%",
                            }}
                            value={searchText}
                            onChange={(e) => setSearchText(e.target.value)}
                            InputProps={{
                                endAdornment: (
                                    <InputAdornment position="end">
                                        <SearchIcon
                                            width={12}
                                            height={12}
                                            color={theme.palette.text.primary}
                                        />
                                    </InputAdornment>
                                ),
                            }}
                        />
                    </Stack>
                </PageTitle>
                <Divider />
                <Stack direction={"column"} gap={"20px"} mt={"20px"}>
                    {personas.length > 0 || isLoading ? (
                        personas.map((persona, index) => (
                            <Stack
                                key={index}
                                direction={"row"}
                                gap={"16px"}
                                alignItems={"center"}
                                justifyContent={"space-between"}
                            >
                                <Stack
                                    direction={"row"}
                                    gap={"16px"}
                                    alignItems={"center"}
                                >
                                    <ProfileImage
                                        image={persona.FOTO}
                                        name={persona.NOME}
                                        width="50px"
                                        height="50px"
                                    />
                                    <Stack direction={"column"} gap={"8px"}>
                                        <Typography variant="h5">
                                            {capitalizeText(persona.NOME || "")}
                                        </Typography>
                                        <Typography variant="body1">
                                            COD: {persona.IDGDA_PERSONA_USER} -
                                            TIPO:{" "}
                                            {persona.TIPO == "Personal"
                                                ? "Pessoal"
                                                : "Comercial"}
                                        </Typography>
                                    </Stack>
                                </Stack>
                                <Button
                                    variant="contained"
                                    sx={{
                                        alignItems: "center",
                                        display: "flex",
                                        gap: "10px",
                                    }}
                                    onClick={() =>
                                        router.push(
                                            `/personas/edit-persona/${persona.IDGDA_PERSONA_USER}`
                                        )
                                    }
                                >
                                    <EditOutlined
                                        sx={{
                                            color: theme.palette.background
                                                .paper,
                                        }}
                                        fontSize="small"
                                    />
                                    Editar
                                </Button>
                            </Stack>
                        ))
                    ) : (
                        <EmptyState
                            title={
                                searchText
                                    ? `Não encontramos personas com o nome "${searchText}"`
                                    : `Nenhuma persona encontrada`
                            }
                        />
                    )}
                </Stack>
                <Stack justifyContent={"flex-end"} direction={"row"}>
                    <Pagination
                        sx={{ mt: "20px" }}
                        count={totalPages || 0}
                        page={page}
                        onChange={handleChangePagination}
                        disabled={isLoading}
                    />
                </Stack>
            </ContentArea>
        </ContentCard>
    );
}

PersonasView.getLayout = getLayout("private");
