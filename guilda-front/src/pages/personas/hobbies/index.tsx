import DeleteOutline from "@mui/icons-material/DeleteOutline";
import {
    Button,
    Divider,
    IconButton,
    InputAdornment,
    Stack,
    TextField,
    Typography,
    useTheme,
} from "@mui/material";
import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { EmptyState } from "src/components/feedback/empty-state/empty-state";
import { SearchIcon } from "src/components/icons/search.icon";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { useDebounce, useLoadingState } from "src/hooks";
import { DeleteHobbyUseCase } from "src/modules/personas/use-cases/delete-hobby.use-case";
import { ListHobbyUseCase } from "src/modules/personas/use-cases/list-hobby.use-case";
import { Hobby } from "src/typings/models/hobby.model";
import { getLayout } from "src/utils";

export default function HobbiesView() {
    const [hobbies, setHobbies] = useState<Hobby[]>([]);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [searchText, setSearchText] = useState<string>("");
    const debouncedSearchText: string = useDebounce<string>(searchText, 400);
    const router = useRouter();

    const theme = useTheme();

    async function listHobbies() {
        startLoading();

        new ListHobbyUseCase()
            .handle({
                hobby: searchText,
            })
            .then((data) => {
                setHobbies(data);
            })
            .catch(() => {
                toast.error("Erro ao listar os hobbies.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    useEffect(() => {
        listHobbies();
    }, [debouncedSearchText]);

    async function deleteHobby(id: number) {
        startLoading();

        new DeleteHobbyUseCase()
            .handle({
                id,
            })
            .then(() => {
                listHobbies();
                toast.success("Hobby apagado com sucesso!");
            })
            .catch(() => {
                toast.error("Erro ao apagar o hobby.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    return (
        <ContentCard>
            <ContentArea>
                <PageTitle title="Hobbies" loading={isLoading}>
                    <Stack
                        direction={"row"}
                        alignItems={"center"}
                        gap={"16px"}
                        width={"100%"}
                        justifyContent={"flex-end"}
                    >
                        <TextField
                            label="Busque por um hobbie"
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
                        <Button
                            variant="contained"
                            onClick={() =>
                                router.push("/personas/create-hobby")
                            }
                        >
                            Adicionar hobby
                        </Button>
                    </Stack>
                </PageTitle>
                <Divider />
                <Stack direction={"column"} gap={"20px"} mt={"30px"}>
                    {hobbies.length > 0 || isLoading ? (
                        hobbies.map((hobby, index) => (
                            <Stack
                                key={index}
                                width={"100%"}
                                py={"10px"}
                                px={"20px"}
                                border={`solid 1px ${theme.palette.grey[200]}`}
                                borderRadius={"8px"}
                                justifyContent={"space-between"}
                                direction={"row"}
                                alignItems={"center"}
                            >
                                <Typography variant="body1">
                                    {hobby.HOBBY}
                                </Typography>
                                <IconButton
                                    size="small"
                                    onClick={() =>
                                        hobby.IDGDA_PERSONA_USER_HOBBY &&
                                        deleteHobby(
                                            hobby.IDGDA_PERSONA_USER_HOBBY
                                        )
                                    }
                                >
                                    <DeleteOutline fontSize="small" />
                                </IconButton>
                            </Stack>
                        ))
                    ) : (
                        <EmptyState
                            title={
                                searchText
                                    ? `Não encontramos hobbies com o nome "${searchText}"`
                                    : `Nenhum hobby encontrado`
                            }
                        />
                    )}
                </Stack>
            </ContentArea>
        </ContentCard>
    );
}

HobbiesView.getLayout = getLayout("private");
