import { Box, Button, Card, CardMedia, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { BaseModal } from "src/components/feedback";
import { CreateGalleryUseCase } from "../use-cases/create-gallery.use-case";
import { useLoadingState } from "src/hooks";
import { toast } from "react-toastify";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
  refresh: () => void;
}

export function CreateImageModal(props: IProps) {
  const { onClose, isOpen, refresh } = props;
  const [image, setImage] = useState<File[]>([]);
  const { finishLoading, isLoading, startLoading } = useLoadingState();

  // const handleUpload = (event: any) => {
  //     const files = event.target.files;
  //     // files && reader.readAsDataURL(files);
  //     for (let i = 0; i < files?.length; i++) {
  //         const reader = new FileReader();

  //         reader.readAsDataURL(files[i]);
  //         reader.onloadend = () => {
  //             const x = image;
  //             files && setImage(files[i]);
  //         };
  //     }
  // };

  const handleUpload = (event: any) => {
    const files = event.target.files;

    if (files.length > 0) {
      const imagesArray = Array.from(files);

      imagesArray.forEach((file, index) => {
        const reader = new FileReader();
        reader.readAsDataURL(file as File);
        reader.onloadend = () => {
          // files && setImage(files[index]);
        };
      });

      // Atualizar o estado ou fazer o que for necessário com a lista de imagens
      setImage(imagesArray as File[]);
    }
  };

  async function handleUploadNewImage() {
    if (!image) {
      return;
    }

    startLoading();

    await new CreateGalleryUseCase()
      .handle(image)
      .then((data) => {
        toast.success("Imagem adicionada com sucesso!");
        refresh();
        onClose();
      })
      .catch((e) => {
        toast.error("Erro ao adicionar a nova imagem");
      })
      .finally(() => {
        finishLoading();
      });
  }

  return (
    <BaseModal
      sx={{ width: "100%" }}
      fullWidth
      open={isOpen}
      title={`Nova imagem`}
      onClose={onClose}
    >
      <Box>
        <Box
          width={"100%"}
          display={"flex"}
          flexDirection={"row"}
          gap={1}
          justifyContent={"center"}
          alignItems={"center"}
          flexWrap={"wrap"}
        >
          {image.map((img, index) => (
            <Card
              sx={{
                width: "180px",
                height: "180px",
                p: 5,
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
                flexDirection: "column",
                gap: "30px",
              }}
              key={index}
            >
              <Box>
                <CardMedia
                  component="img"
                  alt="Uploaded image"
                  image={URL.createObjectURL(img as File)}
                />
              </Box>
              <Button
                variant="contained"
                color="error"
                onClick={() => {
                  const updatedImagesArray = image.filter(
                    (i) => i.name !== img.name
                  );
                  setImage(updatedImagesArray);
                }}
                size="small"
              >
                Remover
              </Button>
            </Card>
          ))}
        </Box>

        <Box mt={3} display={"flex"} flexDirection={"column"} gap={1}>
          <input
            accept="image/*"
            style={{ display: "none" }}
            id="image-upload-gallery"
            type="file"
            onChange={handleUpload}
            multiple
          />
          <label htmlFor="image-upload-gallery">
            <Button
              variant="outlined"
              color="primary"
              component="span"
              fullWidth
            >
              Selecionar arquivos
            </Button>
          </label>
          <Button
            variant="contained"
            color="primary"
            component="span"
            fullWidth
            onClick={handleUploadNewImage}
          >
            Salvar imagem
          </Button>
        </Box>
      </Box>
    </BaseModal>
  );
}
