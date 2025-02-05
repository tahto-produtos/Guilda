import TableRows from "@mui/icons-material/TableRows";
import GridView from "@mui/icons-material/GridView";
import Settings from "@mui/icons-material/Settings";
import {
    Box,
    CardMedia,
    CircularProgress,
    Pagination,
    Stack,
    TextField,
    ToggleButton,
    ToggleButtonGroup,
    Tooltip,
    Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { useLoadingState } from "src/hooks";
import { CreateImageModal } from "src/modules/gallery/fragments/create-image-modal";
import { DetailsImageModal } from "src/modules/gallery/fragments/details-gallery-modal";
import { ListGalleryUseCase } from "src/modules/gallery/use-cases/list-gallery.use-case";
import FileOpen from "@mui/icons-material/FileOpen";
import { SheetBuilder } from "src/utils";

export interface GalleryImage {
    created_at: string;
    id: number;
    key: string;
    originalName: string;
    type: string;
    url: string;
}

function GalleryImageItem(props: {
    data: GalleryImage;
    refresh: () => void;
    getItemOnClick: (input: GalleryImage) => void;
    viewType: string | null;
}) {
    const { data, refresh, getItemOnClick, viewType } = props;
    const [isOpen, setIsOpen] = useState<boolean>(false);

    return (
        <>
            <Box
                flexDirection={"row"}
                display={"flex"}
                alignItems={"center"}
                width={viewType == "list" ? "100%" : "267px"}
                border={`solid 1px ${grey[300]}`}
                borderRadius={"8px"}
                gap={2}
                p={1}
                sx={{ cursor: "pointer" }}
                position={"relative"}
                onClick={() => {
                    setIsOpen(true);
                    getItemOnClick && getItemOnClick(data);
                }}
            >
                <CardMedia
                    component="img"
                    alt={"img"}
                    image={data.url}
                    sx={{ width: "70px", height: "70px" }}
                />
                <Box display={"flex"} flexDirection={"column"}>
                    <Typography
                        variant="body2"
                        fontSize={"12px"}
                        sx={{ color: grey[700] }}
                    >
                        #{data.id}
                    </Typography>
                    <Tooltip title={data.originalName} arrow>
                        <Typography variant="body1" fontSize={"14px"}>
                            {data.originalName.length > 15 &&
                            viewType !== "list"
                                ? `${data.originalName.substring(0, 15)}...`
                                : data.originalName}
                        </Typography>
                    </Tooltip>
                    <Typography
                        variant="body2"
                        fontSize={"12px"}
                        sx={{ color: grey[700] }}
                    >
                        {`${data.created_at.split(" ")[0].split("/")[1]}/${
                            data.created_at.split(" ")[0].split("/")[0]
                        }/${data.created_at.split(" ")[0].split("/")[2]}`}
                    </Typography>
                </Box>
            </Box>
            {isOpen && (
                <DetailsImageModal
                    isOpen={isOpen}
                    onClose={() => {
                        console.log("closing");
                        setIsOpen(false);
                    }}
                    data={data}
                    refresh={refresh}
                />
            )}
        </>
    );
}

interface IProps {
    onClose?: () => void;
    getItemOnClick?: (input: GalleryImage) => void;
}

export function ImageGallery(props: IProps) {
    const { getItemOnClick, onClose } = props;

    const [galleryImages, setGalleryImages] = useState<GalleryImage[]>([]);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [isOpenNewImage, setIsOpenNewImage] = useState<boolean>(false);
    const [page, setPage] = useState(1);
    const [searchText, setSearchText] = useState<string>("");
    const limitPerPage = 20;

    const [viewType, setViewType] = useState<string | null>("grid");

    const handleViewType = (
        event: React.MouseEvent<HTMLElement>,
        newViewType: string | null
    ) => {
        if (newViewType !== null) {
            setViewType(newViewType);
        }
    };

    const paginationGalleryList =
        galleryImages
            .filter((item) =>
                item.originalName
                    .toLowerCase()
                    .includes(searchText.toLowerCase())
            )
            .slice((page - 1) * limitPerPage, limitPerPage * page) || [];

    useEffect(() => {
        startLoading();
        setPage(1);
        setTimeout(() => {
            finishLoading();
        }, 200);
    }, [searchText]);

    const handleChangePagination = (
        event: React.ChangeEvent<unknown>,
        value: number
    ) => {
        startLoading();
        setPage(value);
        setTimeout(() => {
            finishLoading();
        }, 200);
    };

    async function listGallery() {
        startLoading();

        await new ListGalleryUseCase()
            .handle()
            .then((data) => {
                setGalleryImages(data);
            })
            .catch(() => {
                toast.error("Falha ao carregar a galeria de imagens.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    useEffect(() => {
        listGallery();
    }, []);

    function handleExport() {
        if (!galleryImages || galleryImages.length <= 0) {
            toast.warning("Sem dados para exportar.");
        }

        try {
            const docRows = galleryImages.map((item) => {
                return [
                    item.id.toString(),
                    item.originalName,
                    item.type,
                    item.url,
                    item.created_at
                        ? `${item.created_at.split(" ")[0].split("/")[1]}/${
                              item.created_at.split(" ")[0].split("/")[0]
                          }/${item.created_at.split(" ")[0].split("/")[2]}`
                        : "",
                ];
            });
            let indicatorSheetBuilder = new SheetBuilder();
            indicatorSheetBuilder
                .setHeader([
                    "Código",
                    "Nome do arquivo",
                    "Tipo do arquivo",
                    "URL",
                    "Criada em",
                ])
                .append(docRows)
                .exportAs("Relatorio_Galeria_Imagens");
            toast.success("Relatório exportado com sucesso!");
        } catch {
            toast.error("Houve um erro ao extrair o relatório");
        }
    }

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Galeria de imagens"}
                headerIcon={<Settings />}
                addButtonAction={() => setIsOpenNewImage(true)}
                addButtonTitle="Nova imagem"
                secondaryButtonTitle="Exportar relatório"
                secondaryButtonIcon={<FileOpen />}
                secondayButtonAction={handleExport}
            />
            <Box mt={2} px={2} width={"100%"}>
                <TextField
                    label={"Buscar pelo nome da imagem"}
                    fullWidth
                    onChange={(e) => setSearchText(e.target.value)}
                    value={searchText}
                />
            </Box>
            <Box
                sx={{
                    display: "flex",
                    justifyContent: "flex-end",
                    px: 2,
                    mt: 2,
                }}
            >
                <ToggleButtonGroup
                    value={viewType}
                    exclusive
                    onChange={handleViewType}
                    aria-label="text alignment"
                    size="small"
                >
                    <ToggleButton value="grid" aria-label="left aligned">
                        <GridView />
                    </ToggleButton>
                    <ToggleButton value="list" aria-label="left aligned">
                        <TableRows />
                    </ToggleButton>
                </ToggleButtonGroup>
            </Box>
            <Stack
                px={2}
                py={4}
                width={"100%"}
                gap={2}
                flexDirection={"row"}
                flexWrap={"wrap"}
            >
                {isLoading ? (
                    <Stack
                        width={"100%"}
                        height={"400px"}
                        justifyContent={"center"}
                        alignItems={"center"}
                    >
                        <CircularProgress />
                    </Stack>
                ) : (
                    paginationGalleryList.map((item, index) => (
                        <GalleryImageItem
                            key={index}
                            data={item}
                            refresh={listGallery}
                            getItemOnClick={(x) => {
                                getItemOnClick && getItemOnClick(x);
                                onClose && onClose();
                            }}
                            viewType={viewType}
                        />
                    ))
                )}
            </Stack>
            <Box display={"flex"} justifyContent={"flex-end"} pb={"20px"}>
                <Pagination
                    count={Math.ceil(
                        galleryImages.filter((item) =>
                            item.originalName
                                .toLowerCase()
                                .includes(searchText.toLowerCase())
                        ).length / limitPerPage
                    )}
                    page={page}
                    onChange={handleChangePagination}
                    disabled={isLoading}
                />
            </Box>
            <CreateImageModal
                isOpen={isOpenNewImage}
                onClose={() => setIsOpenNewImage(false)}
                refresh={listGallery}
            />
        </Card>
    );
}
