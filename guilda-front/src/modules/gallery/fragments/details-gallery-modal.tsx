import {
    Box,
    Button,
    Card,
    CardMedia,
    Tooltip,
    Typography,
} from "@mui/material";
import { useEffect, useState } from "react";
import { BaseModal } from "src/components/feedback";
import { CreateGalleryUseCase } from "../use-cases/create-gallery.use-case";
import { useLoadingState } from "src/hooks";
import { toast } from "react-toastify";
import { DetailsGalleryUseCase } from "../use-cases/details-gallery.use-case";
import { DeleteGalleryUseCase } from "../use-cases/delete-gallery.use-case";
import { grey } from "@mui/material/colors";
import { GalleryImage } from "src/components/data-display/image-gallery/image-gallery";

interface IProps {
    isOpen: boolean;
    onClose: () => void;
    refresh?: () => void;
    data: GalleryImage;
}

export function DetailsImageModal(props: IProps) {
    const { onClose, isOpen, refresh, data } = props;
    const { finishLoading, isLoading, startLoading } = useLoadingState();

    async function handleUploadNewImage() {
        startLoading();

        await new DetailsGalleryUseCase()
            .handle()
            .then((data) => {
                console.log(data);
            })
            .catch((e) => {
                toast.error("Erro ao carregar detalhes da imagem");
            })
            .finally(() => {
                finishLoading();
            });
    }

    async function handleDeleteImage() {
        startLoading();

        await new DeleteGalleryUseCase()
            .handle(data.id)
            .then((data) => {
                refresh && refresh();
            })
            .catch((e) => {
                toast.error("Erro ao apagar a imagem");
            })
            .finally(() => {
                finishLoading();
            });
    }

    useEffect(() => {
        handleUploadNewImage();
    }, []);

    return (
        <BaseModal
            width={"540px"}
            open={isOpen}
            title={`Detalhes da imagem`}
            onClose={onClose}
        >
            <Box
                width={"100%"}
                display={"flex"}
                flexDirection={"column"}
                gap={3}
                justifyContent={"center"}
                alignItems={"center"}
            >
                <Box
                    flexDirection={"row"}
                    display={"flex"}
                    alignItems={"center"}
                    width={"100%"}
                    border={`solid 1px ${grey[300]}`}
                    borderRadius={"8px"}
                    gap={2}
                    p={1}
                    position={"relative"}
                >
                    <CardMedia
                        component="img"
                        alt={"img"}
                        image={data.url}
                        sx={{ width: "70px", height: "70px" }}
                    />
                    <Box display={"flex"} flexDirection={"column"}>
                        <Typography variant="body1" fontSize={"14px"}>
                            {data.originalName}
                        </Typography>
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
                <Button
                    onClick={handleDeleteImage}
                    variant="contained"
                    color={"error"}
                >
                    Apagar Imagem
                </Button>
            </Box>
        </BaseModal>
    );
}
