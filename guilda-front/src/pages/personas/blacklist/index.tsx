import DeleteOutline from "@mui/icons-material/DeleteOutline";
import {
    Box,
    Button,
    Divider,
    IconButton,
    InputAdornment,
    Stack,
    TextField,
    Typography,
    useTheme,
} from "@mui/material";
import Download from "@mui/icons-material/Download";
import FileOpen from "@mui/icons-material/FileOpen";
import { SheetBuilder } from "src/utils";
import PermDataSetting from "@mui/icons-material/PermDataSetting";
import { LoadingButton } from "@mui/lab";
import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { EmptyState } from "src/components/feedback/empty-state/empty-state";
import { SearchIcon } from "src/components/icons/search.icon";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { useDebounce, useLoadingState } from "src/hooks";
import { DeleteBlackList } from "src/modules/blacklist/use-cases/delete-blacklist.use-case";
import { ListBlacklistUseCase } from "src/modules/blacklist/use-cases/list-blacklist.use-case";
import { DeleteHobbyUseCase } from "src/modules/personas/use-cases/delete-hobby.use-case";
import { ListHobbyUseCase } from "src/modules/personas/use-cases/list-hobby.use-case";
import { BlackList } from "src/typings/models/blacklist.model";
import { Hobby } from "src/typings/models/hobby.model";
import { getLayout } from "src/utils";
import CheckCircle from "@mui/icons-material/CheckCircle";
import InsertDriveFile from "@mui/icons-material/InsertDriveFile";
import { grey } from "@mui/material/colors";
import { Card,PageHeader } from "src/components";

import { InputMassiveBlacklistUseCase } from "src/modules/personas/use-cases/input-massive-blacklist.use-case"

export default function BlacklistView() {
    const [words, setWords] = useState<BlackList[]>([]);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [searchText, setSearchText] = useState<string>("");
    const debouncedSearchText: string = useDebounce<string>(searchText, 400);
    const router = useRouter();


    const [success, setSuccess] = useState<boolean>(false);
    const [file, setFile] = useState<any>(null);


    const theme = useTheme();

    const handleUpload = (event: any) => {
        const file = event.target.files[0];
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onloadend = () => {
            setFile(file);
        };
    };

    const sendFile = async () => {
        startLoading();

        new InputMassiveBlacklistUseCase()
            .handle(file)
            .then((data) => {
                toast.success("Importação concluida com sucesso!");
                setSuccess(true);
                setFile(null);
            })
            .catch(() => {
                toast.error("Não foi possivel realizar o input!");
            })
            .finally(() => {
                finishLoading();
            });
    };

    function downloadExampleFile2() {
        try {
            const docRows = [
                [
                    "Palavra 1"
                ],
                [
                    "Palavra 2"
                ]
            ];
            let indicatorSheetBuilder = new SheetBuilder();
            indicatorSheetBuilder
                .setHeader([
                    "PALAVRA",
                ])
                .append(docRows)
                .exportAs(`Exemplo_importação_blacklist`);
            toast.success("Exemplo exportado com sucesso!");
        } catch (error) {
            toast.error("Falha na exportação do exemplo!");
        }
    }

    async function listBlacklist() {
        startLoading();

        new ListBlacklistUseCase()
            .handle({
                word: searchText,
            })
            .then((data) => {
                setWords(data);
            })
            .catch(() => {
                toast.error("Erro ao listar as palavras proibidas.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    useEffect(() => {
        listBlacklist();
    }, [debouncedSearchText]);

    async function deleteBlacklist(id: number) {
        startLoading();

        new DeleteBlackList()
            .handle({
                id,
            })
            .then(() => {
                listBlacklist();
                toast.success("Palavra apagada com sucesso!");
            })
            .catch(() => {
                toast.error("Erro ao apagar a palavra.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    return (
        <ContentCard>
            <ContentArea>
            <Card
                        width={"100%"}
                        display={"flex"}
                        flexDirection={"column"}
                        justifyContent={"space-between"}
                        marginBottom={5}
                    > 
                        <PageHeader
                            title={`Input Massivo de blacklist`}
                            headerIcon={<FileOpen />}
                            secondaryButtonIcon={<Download />}
                            secondaryButtonTitle="Baixar exemplo"
                            secondayButtonAction={() => downloadExampleFile2()}
                        />
                        <Stack px={2} py={3} width={"100%"} gap={1}>
                            {success && (
                                <Box
                                    sx={{
                                        width: "100%",
                                        display: "flex",
                                        justifyContent: "center",
                                        alignItems: "center",
                                        flexDirection: "column",
                                        height: "300px",
                                    }}
                                >
                                    <Box display={"flex"} alignItems={"center"} gap={1}>
                                        <CheckCircle color="success" />
                                        <Typography fontWeight={"600"}>
                                            Importação concluida com sucesso!
                                        </Typography>
                                    </Box>

                                    <Button
                                        sx={{ px: "40px", mt: 2 }}
                                        variant="contained"
                                        onClick={() => setSuccess(false)}
                                    >
                                        Importar outro arquivo
                                    </Button>
                                </Box>
                            )}
                            {file && !success && (
                                <Box
                                    sx={{
                                        display: "flex",
                                        justifyContent: "center",
                                        alignItems: "center",
                                        gap: 2,
                                        px: "20px",
                                        mt: "20px",
                                    }}
                                >
                                    <Box
                                        sx={{
                                            backgroundColor: theme.palette.primary.main,
                                            minWidth: "50px",
                                            height: "50px",
                                            borderRadius: "20px",
                                            alignItems: "center",
                                            justifyContent: "center",
                                            display: "flex",
                                        }}
                                    >
                                        <InsertDriveFile
                                            sx={{ fontSize: "30px", color: "#fff" }}
                                        />
                                    </Box>
                                    <Box>
                                        <Typography fontSize={"12px"} sx={{ color: grey[600] }}>
                                            Arquivo selecionado
                                        </Typography>
                                        <Typography fontSize={"14px"} fontWeight={"500"}>
                                            {file.name}
                                        </Typography>
                                        <Typography fontSize={"12px"}>
                                            Tamanho: {file.size}k
                                        </Typography>
                                    </Box>
                                </Box>
                            )}
                            {!success && (
                                <Box width={"100%"} p={"20px 20px"}>
                                    <input
                                        accept=".xls, .xlsx, .csv"
                                        style={{ display: "none" }}
                                        id="image-upload"
                                        type="file"
                                        onChange={(e) => handleUpload(e)}
                                    />
                                    <label htmlFor="image-upload">
                                        <Button
                                            variant="outlined"
                                            color="primary"
                                            component="span"
                                            fullWidth
                                            disabled={isLoading}
                                        >
                                            Selecione um arquivo
                                        </Button>
                                    </label>
                                    <LoadingButton
                                        onClick={sendFile}
                                        fullWidth
                                        loading={isLoading}
                                        variant="contained"
                                        sx={{ mt: 1 }}
                                        disabled={!file}
                                    >
                                        Enviar arquivo
                                    </LoadingButton>
                                </Box>
                            )}
                        </Stack>
                    </Card>
                <PageTitle title="Blacklist" loading={isLoading}>




                    <Stack
                        direction={"row"}
                        alignItems={"center"}
                        gap={"16px"}
                        width={"100%"}
                        justifyContent={"flex-end"}
                    >
                        <TextField
                            label="Busque por uma palavra"
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
                                router.push("/personas/create-blacklist")
                            }
                        >
                            Adicionar palavra
                        </Button>


                    </Stack>
                </PageTitle>
                <Divider />
                <Stack direction={"column"} gap={"20px"} mt={"30px"}>
                    {words.length > 0 || isLoading ? (
                        words.map((item, index) => (
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
                                    {item.WORD}
                                </Typography>
                                <IconButton
                                    size="small"
                                    onClick={() =>
                                        item.IDGDA_BLACKLIST &&
                                        deleteBlacklist(item.IDGDA_BLACKLIST)
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
                                    ? `Não encontramos palavras com "${searchText}"`
                                    : `Nenhuma palavra encontrada`
                            }
                        />
                    )}
                </Stack>
            </ContentArea>
        </ContentCard>
    );
}

BlacklistView.getLayout = getLayout("private");
