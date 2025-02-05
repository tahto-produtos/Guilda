import ManageAccounts from "@mui/icons-material/ManageAccounts";
import { LoadingButton } from "@mui/lab";
import { Checkbox, Chip, Typography } from "@mui/material";
import { grey } from "@mui/material/colors";
import { Box, Stack } from "@mui/system";
import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useLoadingState } from "src/hooks";
import { ListPermissionsUseCase } from "src/modules/profiles/use-cases/list-permissions.use-case";
import { ListProfilePermissionsUseCase } from "src/modules/profiles/use-cases/list-profile-permissions.use-case";
import { ListProfilesUseCase } from "src/modules/profiles/use-cases/list-profiles.use-case";
import { UpdateProfilePermissionsUseCase } from "src/modules/profiles/use-cases/update-profile-permissions";
import { PaginationModel } from "src/typings/models/pagination.model";
import { PermissionModel } from "src/typings/models/permission.model";
import { ProfileModel } from "src/typings/models/profile.model";
import { getLayout } from "src/utils";

const AccordionGroup = (props: {
    group: string;
    profilePermissions: Array<Number>;
    updatePermissions: any;
    permissions: Array<any>;
}) => {
    const { group, profilePermissions, updatePermissions, permissions } = props;

    const groupPermissions = permissions.filter(
        (item) => item.resource === group
    );

    const groupPermissionsIds = groupPermissions.map((item) => {
        return item.id;
    });

    const handleCheckboxOnChange = (e: any, id: number) => {
        if (e.target.checked === true) {
            updatePermissions().Add(id);
        } else {
            updatePermissions().Remove(id);
        }
    };

    const handleGroupSelect = (e: any) => {
        if (e.target.checked === true) {
            for (let i = 0; i < groupPermissionsIds.length; i++) {
                if (
                    profilePermissions.includes(groupPermissionsIds[i]) ===
                    false
                ) {
                    updatePermissions().Add(groupPermissionsIds[i]);
                }
            }
        } else {
            updatePermissions().RemoveGroup(groupPermissionsIds);
        }
    };

    function translateAction(action: string) {
        switch (action) {
            case "READ":
                return "Pode ler";
            case "CREATE":
                return "Pode criar";
            case "DELETE":
                return "Pode apagar";
            case "FIND_MANY":
                return "Pode ver todos os";
            case "DELETE_MANY":
                return "Pode apagar vários";
            case "DELET_ONE":
                return "Pode apagar um";
            case "FIND_ONE":
                return "Pode ver um";
            case "UPDATE":
                return "Pode alterar um";
            case "UPDATE_PERMISSION":
                return "Pode alterar as permissões de um";
            case "FIND_CHECKING_ACCOUNT_COLLABORATOR":
                return "Pode checar a conta de";
            case "FIND_OWN":
                return "Pode ver o próprio";
            case "ASSOCIATE_PRODUCT_TO_STOCK":
                return "Pode associar produto ao";
            case "REPORT_STOCK":
                return "Pode gerar relatório de";
            default:
                return action;
        }
    }

    function translateTitle(title: string) {
        switch (title) {
            case "health":
                return "saúde da aplicação";
            case "order":
                return "pedido";
            case "authentication":
                return "autenticação na plataforma";
            case "groups":
                return "grupos";
            case "DELETE_MANY":
                return "Pode apagar vários";
            case "sectors":
                return "setores";
            case "metric-settings":
                return "configurações de métricas";
            case "indicators":
                return "indicadores";
            case "hierarchies":
                return "hierarquias";
            case "collaborators":
                return "colaboradores";
            case "profiles":
                return "perfis";
            case "permissions":
                return "permissões";
            case "me":
                return "meus detalhes";
            case "type":
                return "tipo de estoque";
            case "result":
                return "resultado";
            case "stock":
                return "estoque";
            case "collaborator-voucher":
                return "voucher";
            default:
                return title;
        }
    }

    return (
        <Box
            sx={{ border: `solid 1px ${grey[200]}` }}
            display={"flex"}
            flexDirection={"column"}
            borderRadius={1}
        >
            <Box sx={{ backgroundColor: grey[300] }} py={1} px={1}>
                <Box
                    sx={{
                        display: "flex",
                        alignItem: "center",
                    }}
                >
                    <Checkbox
                        checked={groupPermissionsIds.every((item) =>
                            profilePermissions.includes(item)
                        )}
                        indeterminate={
                            groupPermissionsIds.some(
                                (item) => profilePermissions.indexOf(item) >= 0
                            ) &&
                            groupPermissionsIds.every((item) =>
                                profilePermissions.includes(item)
                            ) === false
                        }
                        onChange={(event) => handleGroupSelect(event)}
                    />
                    <Typography
                        sx={{
                            display: "flex",
                            alignItems: "center",
                        }}
                    >
                        {translateTitle(props.group)}
                    </Typography>
                </Box>
            </Box>
            <Box sx={{}}>
                {groupPermissions.map((item, index) => (
                    <Box
                        key={index}
                        sx={{
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "space-between",
                            borderTop: `solid 1px ${grey[200]}`,
                            py: 1,
                            px: 1,
                            paddingLeft: 5,
                        }}
                    >
                        <Box
                            sx={{
                                display: "flex",
                                alignItems: "center",
                            }}
                        >
                            <Checkbox
                                onChange={(event) =>
                                    handleCheckboxOnChange(event, item.id)
                                }
                                checked={profilePermissions.includes(item.id)}
                                size="small"
                            />
                            <Typography
                                sx={{
                                    display: "flex",
                                    alignItems: "center",
                                }}
                            >
                                {`${translateAction(
                                    item.action
                                )} ${translateTitle(props.group)}`}
                            </Typography>
                        </Box>
                        <Chip
                            label={item.action}
                            size="small"
                            sx={{ paddingX: 0.7 }}
                        />
                    </Box>
                ))}
            </Box>
        </Box>
    );
};

export default function ProfileSettingsDetails() {
    const router = useRouter();
    const { id } = router.query;
    const [permissions, setPermissions] = useState<PermissionModel[]>([]);
    const [permissionsGroups, setPermissionsGroups] = useState<Array<string>>(
        []
    );
    const [historyProfilePermissions, setHistoryProfilePermissions] = useState<
        Array<number>
    >([]);
    const [profilePermissions, setProfilePermissions] = useState<Array<number>>(
        historyProfilePermissions
    );
    const [profile, setProfile] = useState<ProfileModel | null | undefined>(
        null
    );
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [historyIsFetched, setHistoryIsFetched] = useState<boolean>(false);

    useEffect(() => {
        if (id !== undefined) {
            new ListProfilePermissionsUseCase()
                .handle(parseInt(id.toString()))
                .then((data) => {
                    const historyPermissionsArr = data.map((item: any) => {
                        return item.permissionId;
                    });
                    setHistoryProfilePermissions(historyPermissionsArr);
                    setProfilePermissions(historyPermissionsArr);
                    setHistoryIsFetched(true);
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    finishLoading();
                });
        }
    }, [id]);

    useEffect(() => {
        if (id !== undefined) {
            startLoading();

            const pagination: PaginationModel = {
                limit: 99,
                offset: 0,
            };

            new ListProfilesUseCase()
                .handle(pagination)
                .then((data) => {
                    setProfile(
                        data.items.find(
                            (item) => item.id == parseInt(id.toString())
                        )
                    );
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    finishLoading();
                });
        }
    }, [id]);

    useEffect(() => {
        const pagination: PaginationModel = {
            limit: 99,
            offset: 0,
        };

        new ListPermissionsUseCase()
            .handle(pagination)
            .then((data) => {
                setPermissions(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
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
        if (permissions) {
            const groups = permissions.map((item) => {
                return item.resource;
            });
            const uniqueGroups = groups.filter(function (item, pos) {
                return groups.indexOf(item) == pos;
            });
            setPermissionsGroups(uniqueGroups);
        }
    }, [permissions]);

    const handleSaveChanges = () => {
        if (profile) {
            const payload = {
                profileId: profile.id,
                permissionsId: profilePermissions,
            };

            new UpdateProfilePermissionsUseCase()
                .handle(payload)
                .then((data) => {
                    toast.success("Permissões alteradas com sucesso!");
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    finishLoading();
                });
        }
    };

    return (
        <Card
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"space-between"}
        >
            {/* <PageHeader
                title={`Configurações do perfil ${
                    profile ? `: ${profile.profile}` : ""
                }`}
                headerIcon={<ManageAccounts />}
            />
            <Stack width={"100%"}>
                <Box display={"flex"} flexDirection={"column"} gap={2}>
                    {historyIsFetched &&
                        permissionsGroups.length > 0 &&
                        permissionsGroups.map((item, index) => (
                            <AccordionGroup
                                group={item}
                                key={index}
                                profilePermissions={profilePermissions}
                                updatePermissions={updatePermissions}
                                permissions={permissions}
                            />
                        ))}
                </Box>
                <Box
                    display={"flex"}
                    justifyContent={"flex-end"}
                    px={2}
                    py={3}
                    position={"sticky"}
                    bgcolor={"#fff"}
                    bottom={0}
                    borderTop={"solid 1px #efefef"}
                >
                    <LoadingButton
                        loading={isLoading}
                        variant={"contained"}
                        disabled={!profile || isLoading || !historyIsFetched}
                        onClick={handleSaveChanges}
                    >
                        Salvar permissões
                    </LoadingButton>
                </Box>
            </Stack> */}
        </Card>
    );
}
ProfileSettingsDetails.getLayout = getLayout("private");
