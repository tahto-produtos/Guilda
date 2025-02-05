import { LoadingButton } from "@mui/lab";
import {
    Autocomplete,
    Box,
    Button,
    Divider,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { useEffect, useState } from "react";
import { useLoadingState } from "src/hooks";
import { PermissionModel } from "src/typings/models/permission.model";
import { PermissionsAccordionGroup } from "./permissions-accordion-group";
import { UpdateAdministrationProfilePermissionsUseCase } from "src/modules/profiles/use-cases/update-administration-profile.use-case";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { ListProfileCollaboratorsUseCase } from "../use-cases/list-profile-collaborators.use-case";
import { BaseModal } from "src/components/feedback";
import { DeleteProfileUseCase } from "../use-cases/delete-profile.use-case";
import { PermissionUseCase } from "src/modules/permissions/Permission.use-case";
import { UpdatePermissionUseCase } from "src/modules/permissions/UpdatePermission.use-case";
import { ReturnHierachyProfileUseCase } from "../use-cases/ReturnHierarchyProfile.use-case";
import { ListHierarchyUseCase } from "../use-cases/ListHierarchy.use-case";
import { CustomProfile } from "src/pages/profile/profile-list";
import { UpdateHierarchyProfileUseCase } from "../use-cases/UpdateHierarchyProfile.use-case";

interface EditAdministrationProfileProps {
    allPermissions: PermissionModel[];
    historyProfilePermissions: Array<number>;
    profileName: string;
    collaboratorQuantity: string | number;
    profileId: number;
    onCancel: () => void;
    updateProfiles: () => void;
    onFinish: () => void;
}

export function EditAdministrationProfile(
    props: EditAdministrationProfileProps
) {
    const {
        historyProfilePermissions,
        onCancel,
        profileName,
        updateProfiles,
        onFinish,
        profileId,
        collaboratorQuantity,
    } = props;
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [permissionsGroups, setPermissionsGroups] = useState<Array<string>>(
        []
    );
    const [allPermissions, setAllPermissions] = useState<PermissionModel[]>([]);
    const [profilePermissions, setProfilePermissions] = useState<Array<number>>(
        []
    );
    const [confirmModal, setConfirmModal] = useState<boolean>(false);
    const [associatedCollaborators, setAssociatedCollaborators] = useState<
        { NAME: string; BC: string; SECTOR: string }[]
    >([]);

    const [hierachyList, setHierarchyList] = useState<
        { id: number; name: string }[]
    >([]);

    const [selectedHierarchy, setSelectedHierarchy] = useState<{
        id: number;
        name: string;
    } | null>(null);

    const [hierarchyId, setHierarchyId] = useState<number | null>(null);
    const [confirmUpdateHierarchyModal, setConfirmUpdateHierarchyModal] =
        useState<string>("");

    useEffect(() => {
        if (hierarchyId && hierachyList) {
            const foundItem = hierachyList.find(
                (item) => item.id == hierarchyId
            );
            console.log(foundItem, hierachyList, hierarchyId);
            foundItem && setSelectedHierarchy(foundItem);
        }
    }, [hierachyList, hierarchyId]);

    async function updateHierarchyProfile(CONFIRM: boolean) {
        if (!selectedHierarchy) {
            return;
        }

        await new UpdateHierarchyProfileUseCase()
            .handle({
                BASICAPROFILEHIERARCHY: selectedHierarchy.id,
                CONFIRM: CONFIRM,
                IDGDA_PROFILE: profileId,
            })
            .then((data) => {
                !CONFIRM && data && setConfirmUpdateHierarchyModal(data);
                CONFIRM && toast.success("Atualizado com sucesso!");
                !CONFIRM && !data && toast.success("Atualizado com sucesso!");
            })
            .catch((e) => {});
    }

    async function getListHierarchy() {
        if (!hierarchyId) {
            return;
        }

        await new ListHierarchyUseCase()
            .handle({ IDGDA_HIERARCHY: hierarchyId })
            .then((data) => {
                setHierarchyList(data);
            })
            .catch((e) => {});
    }

    useEffect(() => {
        hierarchyId && getListHierarchy();
    }, [hierarchyId]);

    async function getHierarchyProfile() {
        await new ReturnHierachyProfileUseCase()
            .handle({ IDGDA_PROFILE: profileId })
            .then((data) => {
                setHierarchyId(data[0]["ID HIERARQUIA"] || 0);
            })
            .catch((e) => {});
    }

    useEffect(() => {
        getHierarchyProfile();
    }, []);

    async function getProfileCollaborators() {
        await new ListProfileCollaboratorsUseCase()
            .handle(profileId)
            .then((data) => {
                setAssociatedCollaborators(data);
            })
            .catch((e) => {
                console.log("err", e);
            });
    }

    useEffect(() => {
        getProfileCollaborators();
    }, []);

    async function getProfilePermissions() {
        await new PermissionUseCase()
            .handle({ codProfile: profileId })
            .then((data) => {
                setAllPermissions(data);
                setProfilePermissions(
                    data
                        .filter((item: any) => item.active == true)
                        .map((permission: any) => permission.id)
                );
            })
            .catch(() => {
                toast.error("Erro ao carregar suas permissões");
            })
            .finally(() => {});
    }

    useEffect(() => {
        getProfilePermissions();
    }, []);

    const updatePermissions = () => {
        function Add(permissionId: number) {
            setProfilePermissions((current) => [...current, permissionId]);
        }

        function Remove(permissionId: number) {
            setProfilePermissions(
                profilePermissions.filter((item) => item !== permissionId)
            );
        }

        function RemoveGroup(permissionsIds: Array<number>) {
            setProfilePermissions(
                profilePermissions.filter((el) => !permissionsIds.includes(el))
            );
        }

        return {
            Add,
            Remove,
            RemoveGroup,
        };
    };

    useEffect(() => {
        if (allPermissions) {
            const groups = allPermissions.map((item) => {
                return item.resource;
            });
            const uniqueGroups = groups.filter(function (item, pos) {
                return groups.indexOf(item) == pos;
            });
            setPermissionsGroups(uniqueGroups);
        }
    }, [allPermissions]);

    function handleSaveChanges() {
        startLoading();

        const arrPermit = allPermissions.map((item) => {
            if (profilePermissions.includes(item.id)) {
                return {
                    permissionId: item.id,
                    active: true,
                };
            } else {
                return {
                    permissionId: item.id,
                    active: false,
                };
            }
        });

        new UpdatePermissionUseCase()
            .handle({
                profileId: profileId,
                permit: arrPermit,
            })
            .then((data) => {
                toast.success("Permissões alteradas com sucesso!");
                updateProfiles();
                onFinish();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });

        // new UpdateAdministrationProfilePermissionsUseCase()
        //     .handle(payload)
        //     .then((data) => {
        //         toast.success("Permissões alteradas com sucesso!");
        //         updateProfiles();
        //         onFinish();
        //     })
        //     .catch(() => {
        //         toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
        //     })
        //     .finally(() => {
        //         finishLoading();
        //     });
    }

    function handleDelete() {
        startLoading();

        new DeleteProfileUseCase()
            .handle(profileId)
            .then((data) => {
                toast.success("Perfil apagado com sucesso!");
                updateProfiles();
                onFinish();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    return (
        <Stack width={"100%"}>
            <Box display={"flex"} flexDirection={"column"} gap={2}>
                {permissionsGroups.length > 0 &&
                    permissionsGroups.map((item, index) => (
                        <PermissionsAccordionGroup
                            group={item}
                            key={index}
                            profilePermissions={profilePermissions}
                            updatePermissions={updatePermissions}
                            permissions={allPermissions}
                        />
                    ))}
            </Box>
            <Box
                display={"flex"}
                justifyContent={"space-between"}
                px={2}
                py={3}
                position={"sticky"}
                bgcolor={"#fff"}
                bottom={0}
                borderTop={"solid 1px #efefef"}
                gap={2}
            >
                <Box display={"flex"} gap={2}>
                    {collaboratorQuantity}
                </Box>
                <Box display={"flex"} gap={2}>
                    <LoadingButton
                        loading={isLoading}
                        variant={"outlined"}
                        disabled={isLoading}
                        onClick={onCancel}
                    >
                        Voltar
                    </LoadingButton>
                    <LoadingButton
                        loading={isLoading}
                        variant={"outlined"}
                        disabled={isLoading}
                        color="error"
                        onClick={onCancel}
                    >
                        Cancelar
                    </LoadingButton>
                    <LoadingButton
                        loading={isLoading}
                        variant={"contained"}
                        disabled={isLoading}
                        onClick={handleSaveChanges}
                    >
                        Salvar permissões
                    </LoadingButton>
                </Box>
            </Box>
            <Box
                display={"flex"}
                flexDirection={"column"}
                border={"solid 1px #e1e1e1"}
            >
                <Typography py={1} px={2} fontWeight={"500"}>
                    Usuários associados a este perfil:
                </Typography>
                <Divider />
                {associatedCollaborators.map((item, index) => (
                    <Typography
                        key={index}
                        borderBottom={"solid 1px #e1e1e1"}
                        py={1}
                        px={2}
                    >
                        {item.BC} - {item.NAME} - {item.SECTOR}
                    </Typography>
                ))}
            </Box>
            <Box display={"flex"} gap={2} alignItems={"center"} mt={2}>
                <Autocomplete
                    value={selectedHierarchy}
                    placeholder={"Definir hierarquia padrão"}
                    disableClearable={false}
                    onChange={(_, item) => setSelectedHierarchy(item)}
                    isOptionEqualToValue={(option, value) =>
                        option.name === value.name
                    }
                    renderInput={(props) => (
                        <TextField
                            {...props}
                            size={"small"}
                            label={"Definir hierarquia padrão"}
                        />
                    )}
                    renderTags={() => null}
                    getOptionLabel={(option) => option.name}
                    options={hierachyList}
                    sx={{ mb: 0 }}
                    fullWidth
                />
                <Button
                    onClick={() => updateHierarchyProfile(false)}
                    variant="contained"
                >
                    Salvar
                </Button>
            </Box>
            <Button
                onClick={() => setConfirmModal(true)}
                variant="contained"
                fullWidth
                color="error"
                sx={{ mt: 4 }}
            >
                Apagar este perfil
            </Button>
            <BaseModal
                width={"540px"}
                open={confirmModal}
                title={`Apagar perfil`}
                onClose={() => setConfirmModal(false)}
            >
                <Box width={"100%"} display={"flex"} flexDirection={"column"}>
                    <Typography variant="body2" textAlign={"center"} mt={2}>
                        Este perfil possui{" "}
                        {collaboratorQuantity.toString().split(":")[1]} usuários
                        associados, você tem certeza que deseja apagar?
                    </Typography>
                    <Button
                        color="success"
                        variant="contained"
                        fullWidth
                        sx={{ mt: 4 }}
                        onClick={() => setConfirmModal(false)}
                    >
                        Não
                    </Button>
                    <LoadingButton
                        color="error"
                        variant="contained"
                        fullWidth
                        sx={{ mt: 1 }}
                        onClick={handleDelete}
                        loading={isLoading}
                    >
                        Sim
                    </LoadingButton>
                </Box>
            </BaseModal>
            <BaseModal
                width={"540px"}
                open={Boolean(confirmUpdateHierarchyModal)}
                title={`Confirmar`}
                onClose={() => setConfirmUpdateHierarchyModal("")}
            >
                <Box width={"100%"} display={"flex"} flexDirection={"column"}>
                    <Typography variant="body2" textAlign={"center"} mt={2}>
                        {confirmUpdateHierarchyModal}
                    </Typography>
                    <Button
                        color="success"
                        variant="contained"
                        fullWidth
                        sx={{ mt: 4 }}
                        onClick={() => {
                            setConfirmUpdateHierarchyModal("");
                            updateHierarchyProfile(true);
                        }}
                    >
                        Confirmar
                    </Button>
                </Box>
            </BaseModal>
        </Stack>
    );
}
