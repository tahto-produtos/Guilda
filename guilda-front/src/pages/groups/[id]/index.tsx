import { useRouter } from "next/router";
import {
    Autocomplete,
    Box,
    Stack,
    Typography,
    TextField,
    List,
    ListItem,
    ListItemIcon,
    ListItemText,
    Divider,
    Button,
    CardMedia,
} from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { Group, Indicator, Sector } from "src/typings";
import { Card, PageHeader } from "src/components";
import { getLayout } from "src/utils";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { GoBack } from "src/components/navigation/go-back";
import { DetailsIndicatorUseCase } from "src/modules/indicators/use-cases";
import { DeleteIndicatorUseCase } from "src/modules/indicators/use-cases/delete-indicator.use-case";
import { DateUtils } from "src/utils";
import { LoadingButton } from "@mui/lab";
import { useLoadingState } from "src/hooks";
import CalendarMonth from "@mui/icons-material/CalendarMonth";
import DriveFileRenameOutline from "@mui/icons-material/DriveFileRenameOutline";
import FormatQuote from "@mui/icons-material/FormatQuote";
import ImportExport from "@mui/icons-material/ImportExport";
import Link from "@mui/icons-material/Link";
import Pin from "@mui/icons-material/Pin";
import { BaseModal, ConfirmModal } from "src/components/feedback";
import { CreateGroupUseCase, ListGroupsUseCase } from "src/modules";
import { DeleteGroup } from "src/modules/groups/use-cases/delete-group";
import { UpdateGroupUseCase } from "src/modules/groups/use-cases/update-group";
import abilityFor from "src/utils/ability-for";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export default function GroupDetails() {
    const { isLoading, startLoading, finishLoading } = useLoadingState();
    const { myPermissions } = useContext(PermissionsContext);

    const router = useRouter();
    const { id } = router.query;
    const [group, setGroup] = useState<Group | null>(null);
    const [modalConfirmVisible, setModalConfirmVisible] =
        useState<boolean>(false);
    const [modalChangeImage, setModalChangeImage] = useState<boolean>(false);
    const [selectedImage, setSelectedImage] = useState<File | null>(null);

    useEffect(() => {
        if (id !== undefined) {
            const payload = {
                limit: 999,
                offset: 0,
            };

            new ListGroupsUseCase()
                .handle(payload)
                .then((data) => {
                    const thisGroup = data.items.find(
                        (item: Group) => item.id.toString() == id
                    );
                    console.log(thisGroup);
                    setGroup(thisGroup);
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                });
        }
    }, [id]);

    const updateGroup = async () => {
        if (
            !group ||
            !group.name ||
            !group.alias ||
            !group.description ||
            !selectedImage ||
            !id
        ) {
            return toast.warning("Selecione uma imagem");
        }

        startLoading();

        const payload = {
            name: group?.name,
            alias: group?.alias,
            description: group?.description,
            image: selectedImage,
            id: id.toString(),
        };

        await new UpdateGroupUseCase()
            .handle(payload)
            .then((data) => {
                toast.success(`Grupo deletado com sucesso`);
                router.push(`/indicators`);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                finishLoading();
            });
    };

    const handleDeleteGroup = async (id: number) => {
        startLoading();

        const payload = {
            id: id.toString(),
        };

        await new DeleteGroup()
            .handle(payload)
            .then((data) => {
                toast.success(`Grupo deletado com sucesso`);
                router.push(`/indicators`);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                finishLoading();
            });
    };

    const handleUpload = (event: any) => {
        const file = event.target.files[0];
        const reader = new FileReader();
        if (file) {
            reader.readAsDataURL(file);
            reader.onloadend = () => {
                setSelectedImage(file);
            };
        }
    };

    if (abilityFor(myPermissions).cannot("Ver Grupos", "Grupos")) {
        return null;
    }

    return (
        <Card
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"space-between"}
        >
            <Stack px={2} py={3} width={"100%"}>
                <ConfirmModal
                    open={modalConfirmVisible}
                    onClose={() => setModalConfirmVisible(false)}
                    onConfirm={() => group && handleDeleteGroup(group.id)}
                    text={"Você deseja apagar o grupo?"}
                />
                <GoBack pushTo={"/groups"} />
                <PageHeader title={`Detalhes do Grupo`} />
                {group && (
                    <Box
                        sx={{
                            width: "100%",
                            flexDirection: "column",
                            display: "flex",
                            gap: "10px",
                        }}
                    >
                        <List dense={true}>
                            <ListItem>
                                <ListItemIcon>
                                    <DriveFileRenameOutline />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Nome do grupo"
                                    secondary={group.name}
                                />
                            </ListItem>
                            <Divider />
                            <ListItem>
                                <ListItemIcon>
                                    <Pin />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Código do grupo"
                                    secondary={group.id}
                                />
                            </ListItem>
                            <Divider />
                            <ListItem>
                                <ListItemIcon>
                                    <FormatQuote />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Descrição"
                                    secondary={group.description}
                                />
                            </ListItem>
                            <Divider />
                            <ListItem>
                                <ListItemIcon>
                                    <CalendarMonth />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Criado em"
                                    secondary={
                                        group.createdAt
                                            ? DateUtils.formatDatePtBRWithTime(
                                                  group.createdAt
                                              )
                                            : ""
                                    }
                                />
                            </ListItem>
                        </List>
                        {abilityFor(myPermissions).can(
                            "Editar Grupos",
                            "Grupos"
                        ) && (
                            <Box
                                display={"flex"}
                                justifyContent={"flex-end"}
                                px={2}
                                py={3}
                                gap={1}
                            >
                                <LoadingButton
                                    loading={isLoading}
                                    variant={"outlined"}
                                    color={"primary"}
                                    onClick={() => setModalChangeImage(true)}
                                >
                                    Alterar imagem do grupo
                                </LoadingButton>
                                {/* <LoadingButton
                                    loading={isLoading}
                                    variant={"contained"}
                                    color={"error"}
                                    onClick={() => setModalConfirmVisible(true)}
                                >
                                    Apagar grupo
                                </LoadingButton> */}
                            </Box>
                        )}
                        <BaseModal
                            width={"540px"}
                            open={modalChangeImage}
                            title={"Alterar imagem do grupo"}
                            onClose={() => setModalChangeImage(false)}
                        >
                            <Box width={"100%"}>
                                <Box
                                    display={"flex"}
                                    gap={2}
                                    width={"100%"}
                                    alignItems={"center"}
                                >
                                    <input
                                        accept="image/*"
                                        style={{ display: "none" }}
                                        id="image-upload"
                                        type="file"
                                        onChange={handleUpload}
                                    />
                                    <label htmlFor="image-upload">
                                        <Button
                                            variant="outlined"
                                            color="primary"
                                            component="span"
                                        >
                                            Selecione um ícone de exibição
                                        </Button>
                                    </label>
                                    {selectedImage && (
                                        <Card
                                            sx={{
                                                width: "80px",
                                                height: "80px",
                                                p: 1,
                                                mt: 2,
                                            }}
                                        >
                                            <CardMedia
                                                component="img"
                                                alt="Uploaded image"
                                                image={URL.createObjectURL(
                                                    selectedImage as File
                                                )}
                                            />
                                        </Card>
                                    )}
                                </Box>
                                <Button
                                    variant="contained"
                                    onClick={updateGroup}
                                    fullWidth
                                    sx={{ mt: 2 }}
                                    disabled={!selectedImage}
                                >
                                    Alterar Imagem
                                </Button>
                            </Box>
                        </BaseModal>
                    </Box>
                )}
            </Stack>
        </Card>
    );
}

GroupDetails.getLayout = getLayout("private");
