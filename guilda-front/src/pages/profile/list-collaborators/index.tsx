import ManageAccounts from "@mui/icons-material/ManageAccounts";
import { LoadingButton } from "@mui/lab";
import {
    Autocomplete,
    Box,
    Button,
    CircularProgress,
    TextField,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { Stack } from "@mui/system";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListSectorsUseCase } from "src/modules";
import { CollaboratorsListAllUseCase } from "src/modules/profiles/use-cases/collaborators-list-all.use-case";
import { CreateCollaboratorUseCase } from "src/modules/profiles/use-cases/create-collaborator.use-case";
import { ListAdministrationProfilesUseCase } from "src/modules/profiles/use-cases/list-adminitration-profiles.use-case";
import { ListCollaboratorsUseCase } from "src/modules/profiles/use-cases/list-collaboratots.use-case";
import { ResetPasswordUsecase } from "src/modules/profiles/use-cases/reset-password.use-case";
import { UpdateCollaboratorUseCase } from "src/modules/profiles/use-cases/update-collaborator.use-case";
import { Sector } from "src/typings";
import { PaginationModel } from "src/typings/models/pagination.model";
import { getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";

export interface CustomProfile {
    name: string;
    id: number;
    deletedAt: string;
    createdAt: string;
    ProfilePermission: {
        createdAt: string;
        id: number;
        permission: { action: string; resource: string };
        permissionId: number;
        profileCollaboratorAdministrationId: number;
        profileId: number | null;
    }[];
}

export interface ICollaborator {
    name: string;
    profileCollaboratorAdministrationId: number;
    profileCollaboratorAdministration: { name: string };
    HistoryCollaboratorSector: { id: number; sectorId: number }[];
    registry: string;
    sectorName: string;
}

export default function ListCollaborators() {
    const router = useRouter();
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const { myUser } = useContext(UserInfoContext);
    const [collaborators, setCollaborators] = useState<ICollaborator[]>([]);
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [createName, setCreateName] = useState<string>("");
    const [createRegistration, setCreateRegistration] = useState<string>("");
    const [createProfile, setCreateProfile] = useState<string>("");
    const [profiles, setProfiles] = useState<CustomProfile[]>([]);
    const [sectors, setSectors] = useState<Sector[]>([]);
    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const debouncedSectorsSearchValue: string = useDebounce<string>(
        sectorsSearchValue,
        400
    );
    const [selectedSector, setSelectedSector] = useState<Sector | null>(null);
    const { myPermissions } = useContext(PermissionsContext);

    const [edit, setEdit] = useState<ICollaborator | null>(null);
    const [editName, setEditName] = useState<string>("");
    const [editRegistration, setEditRegistration] = useState<string>("");
    const [editProfile, setEditProfile] = useState<string>("");
    const [editSector, setEditSector] = useState<Sector | null>(null);

    const [searchValue, setSearchValue] = useState<string>("");
    const debouncedSearchValue = useDebounce<string>(searchValue, 400);

    useEffect(() => {
        if (edit) {
            setEditName(edit.name);
            setEditRegistration(edit.registry);
            setEditProfile(edit?.profileCollaboratorAdministration?.name);

            const pagination = {
                limit: 20,
                offset: 0,
                searchText: `${edit.HistoryCollaboratorSector[0]?.sectorId}`,
                deleted: false,
            };

            new ListSectorsUseCase()
                .handle(pagination)
                .then((data) => {
                    setEditSector(data.items[0] as Sector);
                })
                .catch(() => {
                    // toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    finishLoading();
                });
        } else {
            setEditName("");
            setEditRegistration("");
            setEditProfile("");
            setEditSector(null);
        }
    }, [edit]);

    const getCollaborators = async () => {
        startLoading();

        const pagination = {
            limit: 20,
            offset: 0,
            searchText: searchValue,
        };

        await new CollaboratorsListAllUseCase()
            .handle(pagination)
            .then((data) => {
                setCollaborators(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    const GetProfiles = async () => {
        startLoading();

        const pagination: PaginationModel = {
            limit: 9999,
            offset: 0,
        };

        await new ListAdministrationProfilesUseCase()
            .handle(pagination)
            .then((data) => {
                setProfiles(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    useEffect(() => {
        GetProfiles();
    }, []);

    useEffect(() => {
        getCollaborators();
    }, [debouncedSearchValue]);

    async function handleConfirmCreate() {
        if (
            !createName ||
            !createRegistration ||
            !createProfile ||
            !selectedSector
        ) {
            return toast.warning(
                "Preencha todos os campos para criar o colaborador."
            );
        }

        startLoading();

        const payload = {
            name: createName,
            registration: createRegistration.toUpperCase(),
            profile: createProfile,
            sector: selectedSector.name,
        };

        await new CreateCollaboratorUseCase()
            .handle(payload)
            .then((data) => {
                getCollaborators();
                toast.success("Colaborador criado com sucesso!");
            })
            .catch((e) => {
                if (e.response.data.message) {
                    const message = e.response.data.message;
                    if (message == "collaborator already exists") {
                        return toast.error("Colaborador já existe");
                    }
                    if (message == "sector not found") {
                        return toast.error("Setor não encontrado");
                    }
                }
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    const getSectorsList = async () => {
        startLoading();

        const pagination = {
            limit: 20,
            offset: 0,
            searchText: sectorsSearchValue,
            deleted: false,
        };

        new ListSectorsUseCase()
            .handle(pagination)
            .then((data) => {
                setSectors(data.items);
            })
            .catch(() => {
                // toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    useEffect(() => {
        getSectorsList();
    }, [debouncedSectorsSearchValue]);

    async function handleConfirmEdit() {
        startLoading();

        const payload = {
            name: editName,
            registration: editRegistration,
            profile: editProfile,
            sector: editSector?.name,
        };

        await new UpdateCollaboratorUseCase()
            .handle(payload)
            .then((data) => {
                getCollaborators();
                toast.success("Colaborador editado com sucesso!");
            })
            .catch((e) => {
                if (e.response.data.message) {
                    const message = e.response.data.message;
                    const code = e.response.data.code;
                    if (message == "collaborator already exists") {
                        return toast.error("Colaborador já existe");
                    }
                    if (message == "sector not found") {
                        return toast.error("Setor não encontrado");
                    }
                    if (code == "COLLABORATOR_IS_INACTIVE") {
                        return toast.warning(
                            "Não foi possível realizar as alterações. Colaborador não está ativo."
                        );
                    }
                }
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    async function handleResetPassword() {
        if (!editRegistration) {
            return toast.warning("Colaborador sem registro");
        }

        startLoading();
        await new ResetPasswordUsecase()
            .handle(editRegistration)
            .then((data) => {
                toast.success("Senha resetada com sucesso!");
            })
            .catch((e) => {
                var message = e.response.data;
                if (message) {
                    toast.warning(
                        "Não foi possível realizar as alterações. Colaborador não está ativo."
                    );
                } else {
                    toast.warning(INTERNAL_SERVER_ERROR_MESSAGE);
                }
            })
            .finally(() => {
                finishLoading();
            });
    }

    return (
        <Card
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"space-between"}
        >
            <PageHeader
                title={`Lista de colaboradores`}
                headerIcon={<ManageAccounts />}
                // addButtonTitle="Criar colaborador"
                // addButtonAction={() => setIsOpen(true)}
                // addButtonIsDisabled={abilityFor(myPermissions).cannot(
                //     "Editar Colaboradores",
                //     "Perfis"
                // )}
            />
            <Stack width={"100%"} gap={"10px"}>
                <TextField
                    label="Pesquisar"
                    value={searchValue}
                    onChange={(e) => setSearchValue(e.target.value)}
                />
                {collaborators.map((collaborator, index) => (
                    <Box
                        key={index}
                        sx={{
                            width: "100%",
                            p: "10px",
                            backgroundColor: grey[100],
                        }}
                        display={"flex"}
                        justifyContent={"space-between"}
                        alignItems={"center"}
                    >
                        {collaborator.name}
                        {abilityFor(myPermissions).can(
                            "Editar Colaboradores",
                            "Perfis"
                        ) && (
                            <Button onClick={() => setEdit(collaborator)}>
                                Editar
                            </Button>
                        )}
                    </Box>
                ))}
            </Stack>
            <BaseModal
                width={"540px"}
                open={isOpen}
                title={`Criar colaborador`}
                onClose={() => setIsOpen(false)}
            >
                <Box
                    width={"100%"}
                    display={"flex"}
                    flexDirection={"column"}
                    gap={1}
                >
                    <TextField
                        label={"Nome"}
                        value={createName}
                        onChange={(e) => setCreateName(e.target.value)}
                    />
                    <TextField
                        label={"Registro"}
                        value={createRegistration.toUpperCase()}
                        onChange={(e) => setCreateRegistration(e.target.value)}
                    />
                    <Autocomplete
                        disablePortal
                        options={profiles.map((profile) => profile.name)}
                        onChange={(event: any, newValue: string | null) => {
                            setCreateProfile(newValue || "");
                        }}
                        renderInput={(params) => (
                            <TextField {...params} label="Perfil" />
                        )}
                    />
                    <Autocomplete
                        value={selectedSector}
                        placeholder={"Setor"}
                        disableClearable={false}
                        onChange={(e, value) => setSelectedSector(value)}
                        onInputChange={(e, text) => setSectorsSearchValue(text)}
                        sx={{ mb: 0 }}
                        renderInput={(props) => (
                            <TextField
                                {...props}
                                size={"medium"}
                                label={"Setor"}
                                InputProps={{
                                    ...props.InputProps,
                                    endAdornment: (
                                        <>
                                            {isLoading ? (
                                                <CircularProgress
                                                    color="primary"
                                                    size={20}
                                                />
                                            ) : (
                                                props.InputProps.endAdornment
                                            )}
                                        </>
                                    ),
                                }}
                            />
                        )}
                        isOptionEqualToValue={(option, value) =>
                            option.name === value.name
                        }
                        getOptionLabel={(option) =>
                            `${option.id} - ${option.name}`
                        }
                        options={sectors}
                    />
                    <LoadingButton
                        variant="contained"
                        loading={isLoading}
                        onClick={handleConfirmCreate}
                    >
                        Confirmar
                    </LoadingButton>
                </Box>
            </BaseModal>
            <BaseModal
                width={"540px"}
                open={edit ? true : false}
                title={`Editar colaborador: ${edit?.name}`}
                onClose={() => setEdit(null)}
            >
                <Box
                    width={"100%"}
                    display={"flex"}
                    flexDirection={"column"}
                    gap={1}
                >
                    <TextField
                        label={"Nome"}
                        value={editName}
                        onChange={(e) => setEditName(e.target.value)}
                    />
                    <TextField
                        label={"Login"}
                        value={editRegistration}
                        InputProps={{
                            readOnly: true,
                        }}
                        onChange={(e) => setEditName(e.target.value)}
                    />
                    <Autocomplete
                        disablePortal
                        options={profiles.map((profile) => profile.name)}
                        onChange={(event: any, newValue: string | null) => {
                            setEditProfile(newValue || "");
                        }}
                        renderInput={(params) => (
                            <TextField {...params} label="Perfil" />
                        )}
                        value={editProfile}
                        sx={{ p: 0, m: 0 }}
                    />
                    <Autocomplete
                        value={editSector}
                        placeholder={"Setor"}
                        disableClearable={false}
                        onChange={(e, value) => setEditSector(value)}
                        onInputChange={(e, text) => setSectorsSearchValue(text)}
                        sx={{ mb: 0 }}
                        renderInput={(props) => (
                            <TextField
                                {...props}
                                size={"medium"}
                                label={"Setor"}
                                InputProps={{
                                    ...props.InputProps,
                                    endAdornment: (
                                        <>
                                            {isLoading ? (
                                                <CircularProgress
                                                    color="primary"
                                                    size={20}
                                                />
                                            ) : (
                                                props.InputProps.endAdornment
                                            )}
                                        </>
                                    ),
                                }}
                            />
                        )}
                        isOptionEqualToValue={(option, value) =>
                            option?.name === value?.name
                        }
                        getOptionLabel={(option) =>
                            `${option.id} - ${option?.name}`
                        }
                        options={sectors}
                    />
                    <LoadingButton
                        variant="contained"
                        loading={isLoading}
                        onClick={handleConfirmEdit}
                    >
                        Confirmar
                    </LoadingButton>
                    <LoadingButton
                        variant="contained"
                        loading={isLoading}
                        onClick={handleResetPassword}
                        color="error"
                    >
                        Resetar senha
                    </LoadingButton>
                </Box>
            </BaseModal>
        </Card>
    );
}

ListCollaborators.getLayout = getLayout("private");
