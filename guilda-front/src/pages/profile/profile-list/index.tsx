import ManageAccounts from "@mui/icons-material/ManageAccounts";
import Settings from "@mui/icons-material/Settings";
import {
  Box,
  FormControl,
  IconButton,
  InputLabel,
  List,
  ListItem,
  ListItemText,
  MenuItem,
  Select,
  TextField,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { Stack } from "@mui/system";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useLoadingState } from "src/hooks";
import { ListAdministrationProfilesUseCase } from "src/modules/profiles/use-cases/list-adminitration-profiles.use-case";
import { ListProfilesUseCase } from "src/modules/profiles/use-cases/list-profiles.use-case";
import { PaginationModel } from "src/typings/models/pagination.model";
import { PermissionModel } from "src/typings/models/permission.model";
import { ProfileModel } from "src/typings/models/profile.model";
import { getLayout } from "src/utils";
import { EditAdministrationProfile } from "../../../modules/profiles/components/edit-profilte";
import { ListPermissionsUseCase } from "src/modules/profiles/use-cases/list-permissions.use-case";
import { BaseModal } from "src/components/feedback";
import { LoadingButton } from "@mui/lab";
import { AssociateUserToAdmProfileUseCase } from "src/modules/profiles/use-cases/associate-user-to-adm-profile.use-case";
import { ListProfileQuantityUseCase } from "src/modules/profiles/use-cases/list-profile-quantity.use-case";
import Add from "@mui/icons-material/Add";
import { CreateProfileUseCase } from "src/modules/profiles/use-cases/create-profile.use-case";
import abilityFor from "src/utils/ability-for";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { formatCurrency } from "src/utils/format-currency";

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

export default function ProfilesList() {
  const router = useRouter();
  const [profiles, setProfiles] = useState<CustomProfile[]>([]);
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const { myUser } = useContext(UserInfoContext);
  const [selectedProfile, setSelectedProfile] = useState<CustomProfile | null>(
    null
  );
  const [permissions, setPermissions] = useState<PermissionModel[]>([]);
  const [isOpen, setIsOpen] = useState(false);
  const [modalProfileOption, setModalProfileOption] = useState<string>("");
  const [modalCollaboratorOption, setModaCollaboratorOption] =
    useState<number>(0);
  const [profilesQuantity, setProfilesQuantity] = useState<Array<{
    PROFILENAME: string;
    QUANTITY: number;
  }> | null>(null);
  const [quantityIsFetched, setQuantityIsFetched] = useState<boolean>(false);
  const [isOpenNewProfile, setIsOpenNewProfile] = useState<boolean>(false);
  const [newProfileName, setNewProfileName] = useState<string>("");
  const { myPermissions } = useContext(PermissionsContext);

  async function getProfileQuantity() {
    await new ListProfileQuantityUseCase()
      .handle()
      .then((data) => {
        setProfilesQuantity(data);
        setQuantityIsFetched(true);
      })
      .catch((e) => {
        console.log("err", e);
      });
  }

  useEffect(() => {
    getProfileQuantity();
  }, []);

  const GetProfiles = async () => {
    startLoading();

    const pagination: PaginationModel = {
      limit: 9999,
      offset: 0,
    };

    await new ListAdministrationProfilesUseCase()
      .handle(pagination)
      .then((data) => {
        console.log(data);
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
    const pagination: PaginationModel = {
      limit: 999,
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

  function handleOpenSettings(item: CustomProfile) {
    setSelectedProfile(item);
  }

  const ListItemComponent = (props: { item: CustomProfile; index: number }) => {
    const { item } = props;
    const [isHover, setIsHover] = useState<boolean>(false);

    const isTheLastItem = props.index === profiles.length - 1;

    return (
      <ListItem
        secondaryAction={
          abilityFor(myPermissions).can("Editar Perfis", "Perfis") && (
            <IconButton
              edge="end"
              aria-label="settings"
              onClick={() => handleOpenSettings(item)}
              size="small"
            >
              <Settings fontSize="small" />
            </IconButton>
          )
        }
        sx={{
          borderBottom: isTheLastItem ? "none" : `solid 1px ${grey[100]}`,
          backgroundColor: isHover ? grey[100] : "#fff",
        }}
        onMouseEnter={() => setIsHover(true)}
        onMouseLeave={() => setIsHover(false)}
      >
        <ListItemText
          primary={item.name}
          secondary={
            quantityIsFetched
              ? `Quantidade de colaboradores: ${formatCurrency(
                  profilesQuantity?.find((i) => i.PROFILENAME == item.name)
                    ?.QUANTITY || 0
                )}`
              : "Carregando quantidade..."
          }
        />
      </ListItem>
    );
  };

  function handleConfirmAssociation() {
    startLoading();

    const payload = {
      profileName: modalProfileOption,
      collaboratorId: modalCollaboratorOption,
    };

    new AssociateUserToAdmProfileUseCase()
      .handle(payload)
      .then((data) => {
        toast.success("Associação concluída com sucesso!");
      })
      .catch(() => {
        toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
      })
      .finally(() => {
        finishLoading();
      });
  }

  function handleCreateProfile() {
    startLoading();

    new CreateProfileUseCase()
      .handle(newProfileName)
      .then((data) => {
        toast.success("Perfil criado com sucesso!");
        GetProfiles();
        setIsOpenNewProfile(false);
      })
      .catch(() => {
        toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
      })
      .finally(() => {
        finishLoading();
      });
  }

  if (selectedProfile) {
    return (
      <Card
        width={"100%"}
        display={"flex"}
        flexDirection={"column"}
        justifyContent={"space-between"}
      >
        <PageHeader
          title={`Editar perfil: ${selectedProfile.name}`}
          headerIcon={<ManageAccounts />}
        />
        <Stack width={"100%"}>
          {abilityFor(myPermissions).can("Editar Perfis", "Perfis") && (
            <EditAdministrationProfile
              allPermissions={permissions}
              historyProfilePermissions={selectedProfile.ProfilePermission.map(
                (permission) => permission.permissionId
              )}
              onCancel={() => {
                setSelectedProfile(null);
              }}
              profileName={selectedProfile.name}
              profileId={selectedProfile.id}
              updateProfiles={GetProfiles}
              onFinish={() => setSelectedProfile(null)}
              collaboratorQuantity={
                quantityIsFetched
                  ? `Quantidade de colaboradores: ${
                      profilesQuantity?.find(
                        (i) => i.PROFILENAME == selectedProfile.name
                      )?.QUANTITY || 0
                    }`
                  : "Carregando quantidade..."
              }
            />
          )}
        </Stack>
      </Card>
    );
  }

  return (
    <Card
      width={"100%"}
      display={"flex"}
      flexDirection={"column"}
      justifyContent={"space-between"}
    >
      <PageHeader
        title={`Lista de perfis`}
        headerIcon={<ManageAccounts />}
        addButtonTitle="Associar colaborador"
        addButtonAction={() => setIsOpen(true)}
        addButtonIsDisabled={
          !profiles ||
          abilityFor(myPermissions).cannot("Editar Perfis", "Perfis")
        }
        secondaryButtonTitle="Criar novo perfil"
        secondayButtonAction={() => setIsOpenNewProfile(true)}
        secondaryButtonIcon={<Add />}
        secondaryButtonIsDisable={abilityFor(myPermissions).cannot(
          "Editar Perfis",
          "Perfis"
        )}
      />
      <Stack width={"100%"}>
        <List
          dense={false}
          sx={{
            display: "flex",
            flexDirection: "column",
            border: `solid 1px ${grey[100]}`,
            padding: 0,
          }}
        >
          {profiles.map((item, index) => (
            <ListItemComponent item={item} key={index} index={index} />
          ))}
        </List>
      </Stack>
      <BaseModal
        width={"540px"}
        open={isOpen}
        title={`Associar colaborador`}
        onClose={() => setIsOpen(false)}
      >
        <Box width={"100%"} display={"flex"} flexDirection={"column"} gap={1}>
          <FormControl sx={{ width: "100%" }}>
            <InputLabel sx={{ backgroundColor: "#fff", px: 1 }}>
              Selecionar Perfil
            </InputLabel>
            <Select
              onChange={(e) => setModalProfileOption(e.target.value)}
              value={modalProfileOption}
            >
              {profiles.map((item, index) => {
                return (
                  <MenuItem value={item.name} key={index}>
                    {item.name}
                  </MenuItem>
                );
              })}
            </Select>
          </FormControl>
          <TextField
            label={"ID do Colaborador"}
            type="number"
            value={modalCollaboratorOption}
            onChange={(e) =>
              setModaCollaboratorOption(parseInt(e.target.value.toString()))
            }
          />
          <LoadingButton
            variant="contained"
            loading={isLoading}
            onClick={handleConfirmAssociation}
          >
            Confirmar
          </LoadingButton>
        </Box>
      </BaseModal>
      <BaseModal
        width={"540px"}
        open={isOpenNewProfile}
        title={`Novo perfil`}
        onClose={() => setIsOpenNewProfile(false)}
      >
        <Box width={"100%"} display={"flex"} flexDirection={"column"} gap={1}>
          <TextField
            label={"Nome do perfil"}
            value={newProfileName}
            onChange={(e) => setNewProfileName(e.target.value)}
          />
          <LoadingButton
            variant="contained"
            loading={isLoading}
            onClick={handleCreateProfile}
            disabled={!newProfileName}
          >
            Confirmar
          </LoadingButton>
        </Box>
      </BaseModal>
    </Card>
  );
}

ProfilesList.getLayout = getLayout("private");
